using Game.Logic.Effects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
	public class SimpleBomb : BombObject
	{
		private Living m_owner;
		private BaseGame m_game;
		protected Tile m_shape;
		protected int m_radius;
		protected double m_power;
		protected List<BombAction> m_actions;
		protected BombType m_type;
		protected bool m_controled;
		private float m_lifeTime;
		private BallInfo m_info;
		private bool m_bombed;
		private bool digMap;
		public bool DigMap
		{
			get
			{
				return this.digMap;
			}
		}
		public BallInfo BallInfo
		{
			get
			{
				return this.m_info;
			}
		}
		public Living Owner
		{
			get
			{
				return this.m_owner;
			}
		}
		public List<BombAction> Actions
		{
			get
			{
				return this.m_actions;
			}
		}
		public float LifeTime
		{
			get
			{
				return this.m_lifeTime;
			}
		}
		public SimpleBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled) : base(id, (float)info.Mass, (float)info.Weight, (float)info.Wind, (float)info.DragIndex)
		{
			this.m_owner = owner;
			this.m_game = game;
			this.m_info = info;
			this.m_shape = shape;
			this.m_type = type;
			this.m_power = info.Power;
			this.m_radius = info.Radii;
			this.m_controled = controled;
			this.m_bombed = false;
			this.m_lifeTime = 0f;
			this.digMap = true;
		}
		public override void StartMoving()
		{
			base.StartMoving();
			this.m_actions = new List<BombAction>();
			int oldLifeTime = this.m_game.LifeTime;
			while (this.m_isMoving && this.m_isLiving && this.m_lifeTime < 80f)
			{
				this.m_lifeTime += 0.04f;
				Point pos = base.CompleteNextMovePoint(0.04f);
				base.MoveTo(pos.X, pos.Y);
				if (this.m_isLiving)
				{
					if (Math.Round((double)(this.m_lifeTime * 100f)) % 40.0 == 0.0 && pos.Y > 0)
					{
						this.m_game.AddTempPoint(pos.X, pos.Y);
					}
					if (this.m_controled && base.vY > 0f)
					{
						Living player = this.m_map.FindNearestEnemy(this.m_x, this.m_y, 150.0, this.m_owner);
						if (player != null)
						{
							Point v;
							if (player is SimpleBoss)
							{
								Rectangle dis = player.GetDirectDemageRect();
								v = new Point(dis.X - this.m_x + 20, dis.Y + dis.Height - this.m_y);
							}
							else
							{
								v = new Point(player.X - this.m_x, player.Y - this.m_y);
							}
							v = v.Normalize(1000);
							base.setSpeedXY(v.X, v.Y);
							base.UpdateForceFactor(0f, 0f, 0f);
							this.m_controled = false;
							this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CHANGE_SPEED, v.X, v.Y, 0, 0));
						}
					}
				}
				if (this.m_bombed)
				{
					this.m_bombed = false;
					this.BombImp();
				}
			}
		}
		protected override void CollideObjects(Physics[] list)
		{
			for (int i = 0; i < list.Length; i++)
			{
				Physics phy = list[i];
				phy.CollidedByObject(this, (int)(this.m_lifeTime * 1000f) + 1000);
				this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.PICK, phy.Id, 0, 0, 0));
			}
		}
		protected override void CollideGround()
		{
			base.CollideGround();
			this.Bomb();
		}
		public void Bomb()
		{
			this.StopMoving();
			this.m_isLiving = false;
			this.m_bombed = true;
		}
		private void BombImp()
		{
			List<Living> playersAround = this.m_map.FindHitByHitPiont(this.GetCollidePoint(), this.m_radius);
			foreach (Living p in playersAround)
			{
				if (p is Player)
				{
					(p as Player).OnBeforeBomb((int)(this.m_lifeTime * 1000f) + 1000);
				}
				if (p.IsNoHole || p.NoHoleTurn)
				{
					p.NoHoleTurn = true;
					this.digMap = false;
				}
				p.SyncAtTime = false;
			}
			this.m_owner.SyncAtTime = false;
			try
			{
				if (this.digMap)
				{
					this.m_map.Dig(this.m_x, this.m_y, this.m_shape, null);
				}
				this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.BOMB, this.m_x, this.m_y, this.digMap ? 1 : 0, 0));
				switch (this.m_type)
				{
				case BombType.FORZEN:
					foreach (Living p in playersAround)
					{
						if (!this.m_owner.IsFriendly(p))
						{
							if (this.m_owner is SimpleBoss && new IceFronzeEffect(100).Start(p))
							{
								this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, p.Id, 0, 0, 0));
							}
							else
							{
								if (p is SimpleBoss)
								{
									double distance = p.Distance(new Point(this.X, this.Y));
									if (distance > (double)this.m_radius)
									{
										return;
									}
								}
								if (new IceFronzeEffect(2).Start(p))
								{
									this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, p.Id, 0, 0, 0));
								}
								else
								{
									this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FORZEN, -1, 0, 0, 0));
									this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.UNANGLE, p.Id, 0, 0, 0));
								}
							}
						}
					}
					break;
				case BombType.TRANFORM:
					if (this.m_y > 10 && this.m_lifeTime > 0.04f)
					{
						Point p2 = this.m_map.FindYLineNotEmptyPoint(this.m_x, this.m_y);
						if (p2 != Point.Empty)
						{
							PointF v = new PointF(-base.vX, -base.vY);
							v = v.Normalize(5f);
							this.m_x += (int)v.X;
							this.m_y += (int)v.Y;
						}
						this.m_owner.SetXY(this.m_x, this.m_y);
						this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.TRANSLATE, this.m_x, this.m_y, 0, 0));
						this.m_owner.StartFalling(true);
						this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.START_MOVE, this.m_owner.Id, this.m_owner.X, this.m_owner.Y, this.m_owner.IsLiving ? 1 : 0));
					}
					break;
				case BombType.CURE:
					foreach (Living p in playersAround)
					{
						List<Living> list = this.m_map.FindLivings(this.GetCollidePoint(), this.m_radius);
						double plus;
						if (this.FindAddBloodLivingCount(list, this.m_owner))
						{
							plus = 0.4;
						}
						else
						{
							plus = 1.0;
						}
						int blood = (int)((double)((Player)this.m_owner).PlayerDetail.SecondWeapon.Template.Property7 * Math.Pow(1.1, (double)((Player)this.m_owner).PlayerDetail.SecondWeapon.StrengthenLevel) * plus);
						if (this.m_game is PVEGame)
						{
							if (((PVEGame)this.m_game).CanAddBlood(this.m_owner, p))
							{
								p.AddBlood(blood);
								if (p is Player)
								{
									((Player)p).TotalCure += blood;
								}
								this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CURE, p.Id, p.Blood, blood, 0));
							}
						}
						else
						{
							p.AddBlood(blood);
							if (p is Player)
							{
								((Player)p).TotalCure += blood;
							}
							this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.CURE, p.Id, p.Blood, blood, 0));
						}
					}
					break;
				default:
					foreach (Living p in playersAround)
					{
						if (!this.m_owner.IsFriendly(p))
						{
							p.OnMakeDamage(p);
							int damage = this.MakeDamage(p);
							int critical = 0;
							if (damage != 0)
							{
								critical = this.MakeCriticalDamage(p, damage);
								this.m_owner.OnTakedDamage(this.m_owner, ref damage, ref critical, (int)(this.m_lifeTime * 1000f + 1000f));
								if (p.TakeDamage(this.m_owner, ref damage, ref critical, 0, (int)(this.m_lifeTime * 1000f + 1000f)))
								{
									this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.KILL_PLAYER, p.Id, damage + critical, (critical != 0) ? 2 : 1, p.Blood));
								}
								else
								{
									this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.UNFORZEN, p.Id, 0, 0, 0));
								}
								if (p is Player)
								{
									this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.DANDER, p.Id, ((Player)p).Dander, 0, 0));
								}
							}
							else
							{
								if (p is SimpleBoss)
								{
									this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.PLAYMOVIE, p.Id, 0, 0, 2));
								}
							}
							if (p.IsLiving)
							{
								p.StartFalling(true);
								this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.START_MOVE, p.Id, p.X, p.Y, p.IsLiving ? 1 : 0));
							}
						}
					}
					break;
				}
				this.Die();
			}
			finally
			{
				this.m_owner.SyncAtTime = true;
				foreach (Living p in playersAround)
				{
					p.SyncAtTime = true;
				}
			}
		}
		protected int MakeDamage(Living target)
		{
			int result;
			if (target is SimpleNpc && (target as SimpleNpc).NpcInfo.Type == 3)
			{
				result = 1;
			}
			else
			{
				double baseDamage = this.m_owner.BaseDamage;
				double baseGuard = target.BaseGuard;
				double defence = target.Defence;
				double attack = this.m_owner.Attack;
				if (this.m_owner.IgnoreArmor)
				{
					baseGuard = 0.0;
					defence = 0.0;
				}
				float damagePlus = this.m_owner.CurrentDamagePlus;
				float shootMinus = this.m_owner.CurrentShootMinus;
				double DR = 0.95 * (baseGuard - (double)(3 * this.m_owner.Grade)) / (500.0 + baseGuard - (double)(3 * this.m_owner.Grade));
				double DR2;
				if (defence - this.m_owner.Lucky < 0.0)
				{
					DR2 = 0.0;
				}
				else
				{
					DR2 = 0.95 * (defence - this.m_owner.Lucky) / (600.0 + defence - this.m_owner.Lucky);
				}
				double damage = baseDamage * (1.0 + attack * 0.001) * (1.0 - (DR + DR2 - DR * DR2)) * (double)damagePlus * (double)shootMinus;
				Point p = new Point(this.X, this.Y);
				double distance = target.Distance(p);
				if (distance <= (double)this.m_radius)
				{
					damage *= 1.0 - distance / (double)this.m_radius / 4.0;
					if (damage < 0.0)
					{
						result = 1;
					}
					else
					{
						result = (int)damage;
					}
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}
		protected int MakeCriticalDamage(Living target, int baseDamage)
		{
			double lucky = this.m_owner.Lucky;
			bool canHit = lucky * 70.0 / (2000.0 + lucky) > (double)this.m_game.Random.Next(100);
			int result;
			if (canHit)
			{
				result = (int)((0.5 + lucky * 0.0003) * (double)baseDamage);
			}
			else
			{
				result = 0;
			}
			return result;
		}
		private bool FindAddBloodLivingCount(List<Living> list, Living living)
		{
			int Count = 0;
			bool result;
			foreach (Living liv in list)
			{
				if (this.m_game is PVEGame)
				{
					if (((PVEGame)this.m_game).CanAddBlood(living, liv))
					{
						Count++;
					}
				}
				else
				{
					Count++;
				}
				if (Count >= 2)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		protected override void FlyoutMap()
		{
			this.m_actions.Add(new BombAction(this.m_lifeTime, ActionType.FLY_OUT, 0, 0, 0, 0));
			base.FlyoutMap();
		}
	}
}
