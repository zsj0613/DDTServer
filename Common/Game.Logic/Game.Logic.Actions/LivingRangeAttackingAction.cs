using Bussiness;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
namespace Game.Logic.Actions
{
	public class LivingRangeAttackingAction : BaseAction
	{
		private Living m_living;
		private List<Living> m_Livings;
		private int m_fx;
		private int m_tx;
		private string m_action;
		private int m_type;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public LivingRangeAttackingAction(Living living, int fx, int tx, string action, int delay, List<Living> livings, int type) : base(delay, 1500)
		{
			this.m_living = living;
			this.m_Livings = livings;
			this.m_fx = fx;
			this.m_tx = tx;
			this.m_action = action;
			this.m_type = type;
		}
		private int MakeDamage(Living p)
		{
			double baseDamage = this.m_living.BaseDamage;
			double baseGuard = p.BaseGuard;
			double defence = p.Defence;
			double attack = this.m_living.Attack;
			if (this.m_living.IgnoreArmor)
			{
				baseGuard = 0.0;
				defence = 0.0;
			}
			float damagePlus = this.m_living.CurrentDamagePlus;
			float shootMinus = this.m_living.CurrentShootMinus;
			double DR = 0.95 * (baseGuard - (double)(3 * this.m_living.Grade)) / (500.0 + baseGuard - (double)(3 * this.m_living.Grade));
			double DR2;
			if (defence - this.m_living.Lucky < 0.0)
			{
				DR2 = 0.0;
			}
			else
			{
				DR2 = 0.95 * (defence - this.m_living.Lucky) / (600.0 + defence - this.m_living.Lucky);
			}
			double damage = baseDamage * (1.0 + attack * 0.001) * (1.0 - (DR + DR2 - DR * DR2)) * (double)damagePlus * (double)shootMinus;
			double distance = (double)Math.Abs(p.GetDirectDemageRect().X - this.m_living.X);
			damage *= 1.1 - distance / (double)Math.Abs(this.m_tx - this.m_fx) / 5.0;
			int result;
			if (damage < 0.0)
			{
				result = 1;
			}
			else
			{
				result = (int)damage;
			}
			return result;
		}
		private int MakeCriticalDamage(Living p, int baseDamage)
		{
			double lucky = this.m_living.Lucky;
			bool canHit = lucky * 70.0 / (2000.0 + lucky) > (double)LivingRangeAttackingAction.random.Next(100);
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
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.Parameter1 = this.m_living.Id;
			pkg.WriteByte(61);
			List<Living> tempLivingsAround = game.Map.FindLiving(this.m_fx, this.m_tx, this.m_Livings);
			List<Living> livingsAround = new List<Living>();
			foreach (Living p in tempLivingsAround)
			{
				if (((PVEGame)game).CanCampAttack(this.m_living, p))
				{
					livingsAround.Add(p);
				}
			}
			int count = livingsAround.Count;
			foreach (Living p in livingsAround)
			{
				if (this.m_living.IsFriendly(p))
				{
					count--;
				}
			}
			pkg.WriteInt(count);
			this.m_living.SyncAtTime = false;
			try
			{
				foreach (Living p in livingsAround)
				{
					p.SyncAtTime = false;
					if (!this.m_living.IsFriendly(p))
					{
						int dander = 0;
						p.IsFrost = false;
						game.SendGameUpdateFrozenState(p);
						int damage = this.MakeDamage(p);
						int critical = this.MakeCriticalDamage(p, damage);
						int totalDemageAmount = 0;
						if (p.TakeDamage(this.m_living, ref damage, ref critical, this.m_type, 1000))
						{
							totalDemageAmount = damage + critical;
							if (p is Player)
							{
								Player player = p as Player;
								dander = player.Dander;
							}
						}
						pkg.WriteInt(p.Id);
						pkg.WriteInt(totalDemageAmount);
						pkg.WriteInt(p.Blood);
						pkg.WriteInt(dander);
						pkg.WriteInt((critical != 0) ? 2 : 1);
					}
				}
				game.SendToAll(pkg);
				base.Finish(tick);
			}
			finally
			{
				this.m_living.SyncAtTime = true;
				foreach (Living p in livingsAround)
				{
					p.SyncAtTime = true;
				}
			}
		}
	}
}
