using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Game.Language;
namespace Game.Logic.Phy.Object
{
	public class Living : Physics
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected BaseGame m_game;
		protected int m_maxBlood;
		protected int m_blood;
		private int m_team;
		private string m_name;
		private string m_modelId;
      protected string m_createAction;
		private Rectangle m_demageRect;
		private int m_state;
		private int m_degree;
		public int m_direction;
		private eLivingType m_type;
		public double BaseDamage = 10.0;
		public double BaseGuard = 10.0;
		public double Defence = 10.0;
		public double Attack = 10.0;
		public double Agility = 10.0;
		public double Lucky = 10.0;
		public int Grade = 1;
		public int Experience = 10;
		public float CurrentDamagePlus;
		public float CurrentShootMinus;
		public bool IgnoreArmor;
		public bool ControlBall;
		public bool GemControlBall;
		public bool NoHoleTurn;
		public bool CurrentIsHitTarget;
		public int TurnNum;
		public int TotalHurt;
		public int TotalHitTargetCount;
		public int TotalShootCount;
		public int TotalKill;
		public int MaxBeatDis;
		public int EffectsCount;
		public int ShootMovieDelay;
		private EffectList m_effectList;
		public bool AttackEffectTrigger;
		public bool DefenceEffectTrigger;
		public bool WillIceForonze;
		protected bool m_syncAtTime;
		public int FlyingPartical;
		private bool m_isAttacking;
		protected static int step_X = 1;
		protected static int step_Y = 3;
		protected static int speed = 2;
		protected bool m_isFrost;
		protected bool m_isHide;
      protected bool m_isNoHole;
      protected bool m_isSeal;
		public event LivingEventHandle Died;
		public event LivingTakedDamageEventHandle BeforeTakeDamage;
		public event LivingTakedDamageEventHandle TakePlayerDamage;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.TakePlayerDamage = (LivingTakedDamageEventHandle)Delegate.Combine(this.TakePlayerDamage, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.TakePlayerDamage = (LivingTakedDamageEventHandle)Delegate.Remove(this.TakePlayerDamage, value);
        //    }
        //}
		public event LivingEventHandle PlayerMakeDamage;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerMakeDamage = (LivingEventHandle)Delegate.Combine(this.PlayerMakeDamage, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerMakeDamage = (LivingEventHandle)Delegate.Remove(this.PlayerMakeDamage, value);
        //    }
        //}
		public event LivingEventHandle BeginNextTurn;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.BeginNextTurn = (LivingEventHandle)Delegate.Combine(this.BeginNextTurn, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.BeginNextTurn = (LivingEventHandle)Delegate.Remove(this.BeginNextTurn, value);
        //    }
        //}
		public event LivingEventHandle BeginSelfTurn;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.BeginSelfTurn = (LivingEventHandle)Delegate.Combine(this.BeginSelfTurn, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.BeginSelfTurn = (LivingEventHandle)Delegate.Remove(this.BeginSelfTurn, value);
        //    }
        //}
		public event LivingEventHandle BeginAttacking;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.BeginAttacking = (LivingEventHandle)Delegate.Combine(this.BeginAttacking, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.BeginAttacking = (LivingEventHandle)Delegate.Remove(this.BeginAttacking, value);
        //    }
        //}
		public event LivingEventHandle BeginAttacked;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.BeginAttacked = (LivingEventHandle)Delegate.Combine(this.BeginAttacked, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.BeginAttacked = (LivingEventHandle)Delegate.Remove(this.BeginAttacked, value);
        //    }
        //}
		public event LivingEventHandle EndAttacking;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.EndAttacking = (LivingEventHandle)Delegate.Combine(this.EndAttacking, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.EndAttacking = (LivingEventHandle)Delegate.Remove(this.EndAttacking, value);
        //    }
        //}
		public event KillLivingEventHanlde AfterKillingLiving;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.AfterKillingLiving = (KillLivingEventHanlde)Delegate.Combine(this.AfterKillingLiving, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.AfterKillingLiving = (KillLivingEventHanlde)Delegate.Remove(this.AfterKillingLiving, value);
        //    }
        //}
		public event KillLivingEventHanlde AfterKilledByLiving;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.AfterKilledByLiving = (KillLivingEventHanlde)Delegate.Combine(this.AfterKilledByLiving, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.AfterKilledByLiving = (KillLivingEventHanlde)Delegate.Remove(this.AfterKilledByLiving, value);
        //    }
        //}
		public BaseGame Game
		{
			get
			{
				return this.m_game;
			}
		}
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}
		public string ModelId
		{
			get
			{
				return this.m_modelId;
			}
		}
		public int Team
		{
			get
			{
				return this.m_team;
			}
		}
		public string CaeateAction
		{
			get
			{
				return this.m_createAction;
			}
			set
			{
				this.m_createAction = value;
			}
		}
		public int ChangeMaxBeatDis
		{
			get
			{
				return this.MaxBeatDis;
			}
			set
			{
				this.MaxBeatDis = value;
			}
		}
		public int Degree
		{
			get
			{
				return this.m_degree;
			}
			set
			{
				this.m_degree = value;
			}
		}
		public bool SyncAtTime
		{
			get
			{
				return this.m_syncAtTime;
			}
			set
			{
				this.m_syncAtTime = value;
			}
		}
		public int Direction
		{
			get
			{
				return this.m_direction;
			}
			set
			{
				if (this.m_direction != value)
				{
					this.m_direction = value;
					base.SetRect(-this.m_rect.X - this.m_rect.Width, this.m_rect.Y, this.m_rect.Width, this.m_rect.Height);
					this.SetRelateDemagemRect(-this.m_demageRect.X - this.m_demageRect.Width, this.m_demageRect.Y, this.m_demageRect.Width, this.m_demageRect.Height);
					if (this.m_syncAtTime)
					{
						this.m_game.SendLivingUpdateDirection(this);
					}
				}
			}
		}
		public eLivingType Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}
		public bool CanSay
		{
			get;
			set;
		}
		public int Group
		{
			get;
			set;
		}
		public EffectList EffectList
		{
			get
			{
				return this.m_effectList;
			}
		}
		public bool IsAttacking
		{
			get
			{
				return this.m_isAttacking;
			}
		}
		public virtual bool IsFrost
		{
			get
			{
				return this.m_isFrost;
			}
			set
			{
				if (this.m_isFrost != value)
				{
					this.m_isFrost = value;
					if (this.m_syncAtTime)
					{
						this.m_game.SendGameUpdateFrozenState(this);
					}
				}
			}
		}
		public bool IsNoHole
		{
			get
			{
				return this.m_isNoHole;
			}
			set
			{
				if (this.m_isNoHole != value)
				{
					this.m_isNoHole = value;
					if (this.m_syncAtTime)
					{
						this.m_game.SendGameUpdateNoHoleState(this);
					}
				}
			}
		}
		public bool IsHide
		{
			get
			{
				return this.m_isHide;
			}
			set
			{
				if (this.m_isHide != value)
				{
					this.m_isHide = value;
					if (this.m_syncAtTime)
					{
						this.m_game.SendGameUpdateHideState(this);
					}
				}
			}
		}
		public int State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				if (this.m_state != value)
				{
					this.m_state = value;
					if (this.m_syncAtTime)
					{
						this.m_game.SendLivingUpdateAngryState(this);
					}
				}
			}
		}
		public int MaxBlood
		{
			get
			{
				return this.m_maxBlood;
			}
			set
			{
				this.m_maxBlood = value;
			}
		}
		public virtual int Blood
		{
			get
			{
				return this.m_blood;
			}
            set
            {
                this.m_blood = value;
            }
		}
		public Living(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction, int maxBeatDis) : base(id)
		{
			this.m_game = game;
			this.m_team = team;
			this.m_name = name;
			this.m_modelId = modelId;
			this.m_maxBlood = maxBlood;
			this.m_direction = direction;
			this.m_state = 0;
			this.MaxBeatDis = maxBeatDis;
			this.m_effectList = new EffectList(this, immunity);
			this.m_syncAtTime = true;
			this.m_createAction = null;
			this.m_type = eLivingType.Living;
		}
		public void ChangeDefence(double value)
		{
			this.BaseGuard += value;
			if (this.BaseGuard < 0.0)
			{
				this.BaseGuard = 0.0;
			}
		}
		public void ChangeDamage(double value)
		{
			this.BaseDamage += value;
			if (this.BaseDamage < 0.0)
			{
				this.BaseDamage = 0.0;
			}
		}
		public void ChangeAgility(double value)
		{
			this.Agility += value;
			if (this.Agility < 0.0)
			{
				this.Agility = 0.0;
			}
		}
		public void ChangeLucky(double value)
		{
			this.Lucky += value;
			if (this.Lucky < 0.0)
			{
				this.Lucky = 0.0;
			}
		}
		public void SetRelateDemagemRect(int x, int y, int width, int height)
		{
			this.m_demageRect.X = x;
			this.m_demageRect.Y = y;
			this.m_demageRect.Width = width;
			this.m_demageRect.Height = height;
		}
		public Point GetShootPoint()
		{
			Point result;
			if (this is SimpleBoss)
			{
				result = ((this.m_direction > 0) ? new Point(this.X - ((SimpleBoss)this).NpcInfo.FireX, this.Y + ((SimpleBoss)this).NpcInfo.FireY) : new Point(this.X + ((SimpleBoss)this).NpcInfo.FireX, this.Y + ((SimpleBoss)this).NpcInfo.FireY));
			}
			else
			{
				result = ((this.m_direction > 0) ? new Point(this.X - this.m_rect.X + 5, this.Y + this.m_rect.Y - 5) : new Point(this.X + this.m_rect.X - 5, this.Y + this.m_rect.Y - 5));
			}
			return result;
		}
		public Rectangle GetDirectDemageRect()
		{
			return new Rectangle(this.X + this.m_demageRect.X, this.Y + this.m_demageRect.Y, this.m_demageRect.Width, this.m_demageRect.Height);
		}
		public List<Rectangle> GetDirectBoudRect()
		{
			return new List<Rectangle>
			{
				new Rectangle(this.X + base.Bound.X, this.Y + base.Bound.Y, base.Bound.Width, base.Bound.Height)
			};
		}
		public double Distance(Point p)
		{
			List<double> distances = new List<double>();
			Rectangle rect = this.GetDirectDemageRect();
			for (int x = rect.X; x <= rect.X + rect.Width; x += 10)
			{
				distances.Add(Math.Sqrt((double)((x - p.X) * (x - p.X) + (rect.Y - p.Y) * (rect.Y - p.Y))));
				distances.Add(Math.Sqrt((double)((x - p.X) * (x - p.X) + (rect.Y + rect.Height - p.Y) * (rect.Y + rect.Height - p.Y))));
			}
			for (int y = rect.Y; y <= rect.Y + rect.Height; y += 10)
			{
				distances.Add(Math.Sqrt((double)((rect.X - p.X) * (rect.X - p.X) + (y - p.Y) * (y - p.Y))));
				distances.Add(Math.Sqrt((double)((rect.X + rect.Width - p.X) * (rect.X + rect.Width - p.X) + (y - p.Y) * (y - p.Y))));
			}
			return distances.Min();
		}
		public double BoundDistance(Point p)
		{
			List<double> distances = new List<double>();
			foreach (Rectangle rect in this.GetDirectBoudRect())
			{
				for (int x = rect.X; x <= rect.X + rect.Width; x += 10)
				{
					distances.Add(Math.Sqrt((double)((x - p.X) * (x - p.X) + (rect.Y - p.Y) * (rect.Y - p.Y))));
					distances.Add(Math.Sqrt((double)((x - p.X) * (x - p.X) + (rect.Y + rect.Height - p.Y) * (rect.Y + rect.Height - p.Y))));
				}
				for (int y = rect.Y; y <= rect.Y + rect.Height; y += 10)
				{
					distances.Add(Math.Sqrt((double)((rect.X - p.X) * (rect.X - p.X) + (y - p.Y) * (y - p.Y))));
					distances.Add(Math.Sqrt((double)((rect.X + rect.Width - p.X) * (rect.X + rect.Width - p.X) + (y - p.Y) * (y - p.Y))));
				}
			}
			return distances.Min();
		}
		public virtual void Reset()
		{
			this.m_blood = this.m_maxBlood;
			this.m_isFrost = false;
			this.m_isHide = false;
			this.m_isNoHole = false;
			this.m_isLiving = true;
			this.m_createAction = null;
			this.TurnNum = 0;
			this.TotalHurt = 0;
			this.TotalKill = 0;
			this.TotalShootCount = 0;
			this.TotalHitTargetCount = 0;
			this.FlyingPartical = 0;
		}
		public virtual void PickBox(Box box)
		{
			box.UserID = base.Id;
			box.Die();
			if (this.m_syncAtTime)
			{
				this.m_game.SendGamePickBox(this, box.Id, 0, "");
			}
		}
		public override void PrepareNewTurn()
		{
			this.ShootMovieDelay = 0;
			this.CurrentDamagePlus = 1f;
			this.CurrentShootMinus = 1f;
			this.IgnoreArmor = false;
			this.ControlBall = false;
			this.NoHoleTurn = false;
			this.CurrentIsHitTarget = false;
			this.WillIceForonze = false;
			this.OnBeginNewTurn();
			this.FlyingPartical = 0;
		}
		public virtual void PrepareSelfTurn()
		{
			this.OnBeginSelfTurn();
		}
		public void StartAttacked()
		{
			this.OnStartAttacked();
		}
		public virtual void StartAttacking()
		{
			if (!this.m_isAttacking)
			{
				this.m_isAttacking = true;
				this.OnStartAttacking();
			}
		}
		public virtual void StopAttacking()
		{
			if (this.m_isAttacking)
			{
				this.m_isAttacking = false;
				this.OnStopAttacking();
			}
		}
		public override void CollidedByObject(Physics phy, int delay)
		{
			if (phy is SimpleBomb)
			{
				((SimpleBomb)phy).Bomb();
			}
		}
		public void SetXY(int x, int y, int delay)
		{
			this.m_game.AddAction(new LivingDirectSetXYAction(this, x, y, delay));
		}
		public void AddEffect(AbstractEffect effect, int delay)
		{
			this.m_game.AddAction(new LivingDelayEffectAction(this, effect, delay));
		}
		public void Say(string msg, int type, int delay, int finishTime)
		{
			this.m_game.AddAction(new LivingSayAction(this, msg, type, delay, finishTime));
		}
		public void Say(string msg, int type, int delay)
		{
			this.m_game.AddAction(new LivingSayAction(this, msg, type, delay, 1000));
		}
		public bool MoveTo(int x, int y, string action, int delay)
		{
			return this.MoveTo(x, y, action, delay, null);
		}
		public bool MoveTo(int x, int y, string action, int delay, LivingCallBack callback)
		{
			if (this is SimpleBoss)
			{
				Living.speed = (this as SimpleBoss).NpcInfo.DropRate;
			}
			else
			{
				if (this is SimpleNpc)
				{
					Living.speed = (this as SimpleNpc).NpcInfo.DropRate;
				}
			}
			bool result;
			if (this.m_x == x && this.m_y == y)
			{
				result = false;
			}
			else
			{
				if (x < 0 || x > this.m_map.Bound.Width)
				{
					result = false;
				}
				else
				{
					List<Point> path = new List<Point>();
					int tx = this.m_x;
					int ty = this.m_y;
					int direction = (x > tx) ? 1 : -1;
					while ((x - tx) * direction > 0)
					{
						Point p = this.m_map.FindNextWalkPoint(tx, ty, direction, Living.speed * Living.step_X, Living.speed * Living.step_Y + 1);
						if (!(p != Point.Empty))
						{
							break;
						}
						path.Add(p);
						tx = p.X;
						ty = p.Y;
					}
					if (path.Count > 0)
					{
						this.m_game.AddAction(new LivingMoveToAction(this, path, Living.speed, action, delay, callback));
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}
		public Point StartFalling(bool direct)
		{
			return this.StartFalling(direct, 0, 30);
		}
		public virtual Point StartFalling(bool direct, int delay, int speed)
		{
			Point p = this.m_map.FindYLineNotEmptyPoint(this.X, this.Y);
			if (p == Point.Empty)
			{
				p = new Point(this.X, this.m_game.Map.Bound.Height + 1);
			}
			Point result;
			if (p.Y != this.Y)
			{
				if (direct)
				{
					base.SetXY(p);
					if (this.m_map.IsOutMap(p.X, p.Y))
					{
						this.Die();
						if (this.Game.CurrentLiving != this && this.Game.CurrentLiving is Player && this is Player && this.Team != this.Game.CurrentLiving.Team)
						{
							Player player = this.Game.CurrentLiving as Player;
							player.PlayerDetail.OnKillingLiving(this.m_game, 1, base.Id, base.IsLiving, 0, player.PlayerDetail.IsArea);
							this.Game.CurrentLiving.TotalKill++;
							player.CalculatePlayerOffer(this as Player);
						}
					}
				}
				else
				{
					this.m_game.AddAction(new LivingFallingAction(this, p.X, p.Y, speed, null, delay, 0, null));
				}
				result = p;
			}
			else
			{
				result = Point.Empty;
			}
			return result;
		}
		public Point FallFrom(int x, int y, string action, int delay, int type, int speed)
		{
			return this.FallFrom(x, y, action, delay, type, speed, null);
		}
		public Point FallFrom(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
		{
			Point p = this.m_map.FindYLineNotEmptyPoint(x, y);
			if (p == Point.Empty)
			{
				p = new Point(x, this.m_game.Map.Bound.Height + 1);
			}
			if (this.Y < p.Y)
			{
				this.m_game.AddAction(new LivingFallingAction(this, p.X, p.Y, speed, action, delay, type, callback));
			}
			return p;
		}
		public bool JumpTo(int x, int y, string action, int delay, int type)
		{
			return this.JumpTo(x, y, action, delay, type, 20, null, true);
		}
		public bool JumpTo(int x, int y, string ation, int delay, int type, LivingCallBack callback)
		{
			return this.JumpTo(x, y, ation, delay, type, 20, callback, true);
		}
		public bool JumpTo(int x, int y, string ation, int delay, int type, int speed, LivingCallBack callback)
		{
			return this.JumpTo(x, y, ation, delay, type, 20, callback, true);
		}
		public bool JumpTo(int x, int y, string ation, int delay, int type, LivingCallBack callback, bool isNotEmptyPoint)
		{
			return this.JumpTo(x, y, ation, delay, type, 20, callback, isNotEmptyPoint);
		}
		public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback, bool isNotEmptyPoint)
		{
			Point p = this.m_map.FindYLineNotEmptyPoint(x, y);
			bool result;
			if (isNotEmptyPoint)
			{
				if (p.Y >= this.Y)
				{
					result = false;
					return result;
				}
			}
			this.m_game.AddAction(new LivingJumpAction(this, p.X, p.Y, speed, action, delay, type, callback));
			result = true;
			return result;
		}
		public void ChangeDirection(int direction, int delay)
		{
			if (delay > 0)
			{
				this.m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
			}
			else
			{
				this.Direction = direction;
			}
		}
		protected int MakeDamage(Living target)
		{
			double baseDamage = this.BaseDamage;
			double baseGuard = target.BaseGuard;
			double defence = target.Defence;
			double attack = this.Attack;
			if (this.IgnoreArmor)
			{
				baseGuard = 0.0;
				defence = 0.0;
			}
			float damagePlus = this.CurrentDamagePlus;
			float shootMinus = this.CurrentShootMinus;
			double DR = 0.95 * (baseGuard - (double)(3 * this.Grade)) / (500.0 + baseGuard - (double)(3 * this.Grade));
			double DR2;
			if (defence - this.Lucky < 0.0)
			{
				DR2 = 0.0;
			}
			else
			{
				DR2 = 0.95 * (defence - this.Lucky) / (600.0 + defence - this.Lucky);
			}
			double damage = baseDamage * (1.0 + attack * 0.001) * (1.0 - (DR + DR2 - DR * DR2)) * (double)damagePlus * (double)shootMinus;
			Point p = new Point(this.X, this.Y);
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
		public bool Beat(Living target, string action, int demageAmount, int delay)
		{
			return target != null && target.IsLiving && this.BeatImpl(target, action, delay, demageAmount, false);
		}
		public bool Beat(Living target, string action, int delay)
		{
			return this.Beat(target, action, delay, false);
		}
		public bool Beat(Living target, string action, int delay, bool isdemage)
		{
			bool result;
			if (target == null || !target.IsLiving)
			{
				result = false;
			}
			else
			{
				int demageAmount = this.MakeDamage(target);
				result = this.BeatImpl(target, action, delay, demageAmount, isdemage);
			}
			return result;
		}
		private bool BeatImpl(Living target, string action, int delay, int demageAmount, bool isdemage)
		{
			int dis = (int)target.Distance(this.X, this.Y);
			bool result;
			if (dis <= this.MaxBeatDis)
			{
				if (this.X - target.X > 0)
				{
					this.Direction = -1;
				}
				else
				{
					this.Direction = 1;
				}
				this.m_game.AddAction(new LivingBeatAction(this, target, demageAmount, 0, action, delay, isdemage));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public bool RangeAttacking(int fx, int tx, string action, int delay, List<Living> exceptPlayers)
		{
			return this.RangeAttacking(fx, tx, action, delay, exceptPlayers, 0);
		}
		public bool RangeAttacking(int fx, int tx, string action, int delay, List<Living> exceptPlayers, int type)
		{
			bool result;
			if (base.IsLiving)
			{
				this.m_game.AddAction(new LivingRangeAttackingAction(this, fx, tx, action, delay, exceptPlayers, type));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public void GetShootForceAndAngle(ref int x, ref int y, int bombId, int minTime, int maxTime, int bombCount, float time, ref int force, ref int angle)
		{
			if (minTime < maxTime)
			{
				BallInfo ballInfo = BallMgr.FindBall(bombId);
				if (this.m_game != null && ballInfo != null)
				{
					Map map = this.m_game.Map;
					Point sp = this.GetShootPoint();
					float dx = (float)(x - sp.X);
					float dy = (float)(y - sp.Y);
					float arf = map.airResistance * (float)ballInfo.DragIndex;
					float gf = map.gravity * (float)ballInfo.Weight * (float)ballInfo.Mass;
					float wf = map.wind * (float)ballInfo.Wind;
					float mass = (float)ballInfo.Mass;
					for (float t = time; t <= 4f; t += 0.6f)
					{
						double vx = Living.ComputeVx((double)dx, mass, arf, wf, t);
						double vy = Living.ComputeVy((double)dy, mass, arf, gf, t);
						if (vy < 0.0 && vx * (double)this.m_direction > 0.0)
						{
							double tf = Math.Sqrt(vx * vx + vy * vy);
							if (tf < 2000.0)
							{
								force = (int)tf;
								angle = (int)(Math.Atan(vy / vx) / 3.1415926535897931 * 180.0);
								if (vx < 0.0)
								{
									angle += 180;
								}
								break;
							}
						}
					}
					x = sp.X;
					y = sp.Y;
				}
			}
		}
		public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay)
		{
			this.m_game.AddAction(new LivingShootAction(this, bombId, x, y, 0, 0, bombCount, minTime, maxTime, time, delay));
			return true;
		}
		public bool IsFriendly(Living living)
		{
			return !(living is Player) && living.Team == this.Team;
		}
		public bool Shoot(int bombId, int x, int y, int force, int angle, int bombCount, int delay)
		{
			this.m_game.AddAction(new LivingShootAction(this, bombId, x, y, force, angle, bombCount, delay, 0, 0f, 0));
			return true;
		}
		public static double ComputeVx(double dx, float m, float af, float f, float t)
		{
			return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 0.8;
		}
		public static double ComputeVy(double dx, float m, float af, float f, float t)
		{
			return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 1.3;
		}
		public static double ComputDX(double v, float m, float af, float f, float dt)
		{
			return v * (double)dt + ((double)f - (double)af * v) / (double)m * (double)dt * (double)dt;
		}
		public bool ShootImp(int bombId, int x, int y, int force, int angle, int bombCount)
		{
			BallInfo ballInfo = BallMgr.FindBall(bombId);
			Tile shape = BallMgr.FindTile(bombId);
			BombType ballType = BallMgr.GetBallType(bombId);
			bool result;
			if (ballInfo != null)
			{
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.Parameter1 = base.Id;
				pkg.WriteByte(2);
				pkg.WriteInt(bombCount);
				float lifeTime = 0f;
				for (int i = 0; i < bombCount; i++)
				{
					double reforce = 1.0;
					int reangle = 0;
					if (i == 1)
					{
						reforce = 0.9;
						reangle = -5;
					}
					else
					{
						if (i == 2)
						{
							reforce = 1.1;
							reangle = 5;
						}
					}
					int vx = (int)((double)force * reforce * Math.Cos((double)(angle + reangle) / 180.0 * 3.1415926535897931));
					int vy = (int)((double)force * reforce * Math.Sin((double)(angle + reangle) / 180.0 * 3.1415926535897931));
					SimpleBomb bomb = new SimpleBomb(this.m_game.physicalId++, ballType, this, this.m_game, ballInfo, shape, this.ControlBall || this.GemControlBall);
					bomb.SetXY(x, y);
					bomb.setSpeedXY(vx, vy);
					this.m_map.AddPhysical(bomb);
					bomb.StartMoving();
					pkg.WriteBoolean(bomb.DigMap);
					pkg.WriteInt(bomb.Id);
					pkg.WriteInt(x);
					pkg.WriteInt(y);
					pkg.WriteInt(vx);
					pkg.WriteInt(vy);
					pkg.WriteInt(bomb.BallInfo.ID);
					if (this.FlyingPartical != 0)
					{
						pkg.WriteString(this.FlyingPartical.ToString());
					}
					else
					{
						pkg.WriteString(ballInfo.FlyingPartical);
					}
					pkg.WriteInt(bomb.Actions.Count);
					foreach (BombAction ac in bomb.Actions)
					{
						pkg.WriteInt(ac.TimeInt);
						pkg.WriteInt(ac.Type);
						pkg.WriteInt(ac.Param1);
						pkg.WriteInt(ac.Param2);
						pkg.WriteInt(ac.Param3);
						pkg.WriteInt(ac.Param4);
					}
					lifeTime = Math.Max(lifeTime, bomb.LifeTime);
				}
				this.m_game.SendToAll(pkg);
				this.m_game.WaitTime((int)((lifeTime + 2f + (float)(bombCount / 3)) * 1000f));
				result = true;
			}
			else
			{
				Living.log.Error(string.Format("Living ShootImpl ballInfo is null. bombId : {0}", bombId));
				result = false;
			}
			return result;
		}
		public void PlayMovie(string action, int delay, int MovieTime)
		{
			this.PlayMovie(action, delay, MovieTime, null);
		}
		public void PlayMovie(string action, int delay, int MovieTime, LivingCallBack callBack)
		{
			this.m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime, callBack));
		}
		public void SetSeal(bool state, int type)
		{
			if (this.m_isSeal != state)
			{
				this.m_isSeal = state;
				if (this.m_syncAtTime)
				{
					this.m_game.SendGameUpdateSealState(this, type);
				}
			}
		}
		public bool GetSealState()
		{
			return this.m_isSeal;
		}
		public void Seal(Living target, int type, int delay)
		{
			this.m_game.AddAction(new LivingSealAction(this, target, type, delay));
		}
		public void OffSeal(Living target, int delay)
		{
			this.m_game.AddAction(new LivingOffSealAction(this, target, delay));
		}
		public virtual int AddBlood(int value)
		{
			return this.AddBlood(value, 0);
		}
		public virtual int AddBlood(int value, int type)
		{
			this.m_blood += value;
			if (this.m_blood > this.m_maxBlood)
			{
				this.m_blood = this.m_maxBlood;
			}
			if (this.m_syncAtTime)
			{
				this.m_game.SendGameUpdateHealth(this, type, value);
			}
			return value;
		}
		public virtual bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, int type, int delay)
		{
			bool result = false;
			bool result2;
			if (!this.IsFrost && this.m_blood > 0)
			{
				if (this.m_game is PVEGame)
				{
					if (!((PVEGame)this.m_game).CanCampAttack(source, this))
					{
						result2 = false;
						return result2;
					}
				}
				if (type == 0)
				{
					this.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount, delay);
					this.StartAttacked();
				}
				this.m_blood -= damageAmount + criticalAmount;
				if (source.WillIceForonze && source.IsLiving)
				{
					this.BeginNextTurn = (LivingEventHandle)Delegate.Combine(this.BeginNextTurn, new LivingEventHandle(this.SetIceFronze));
				}
				if (this.m_syncAtTime)
				{
					if (this is SimpleBoss && (((SimpleBoss)this).NpcInfo.ID == 1207 || ((SimpleBoss)this).NpcInfo.ID == 1307))
					{
						this.m_game.SendGameUpdateHealth(this, 6, damageAmount + criticalAmount);
					}
					else
					{
						this.m_game.SendGameUpdateHealth(this, 1, damageAmount + criticalAmount);
					}
				}
				this.OnAfterTakedDamage(source, damageAmount, criticalAmount, delay);
				if (this.m_blood <= 0)
				{
					this.Die();
				}
				source.OnAfterKillingLiving(this, damageAmount, criticalAmount, delay);
				result = true;
			}
			this.EffectList.StopEffect(typeof(IceFronzeEffect));
			this.EffectList.StopEffect(typeof(HideEffect));
			this.EffectList.StopEffect(typeof(NoHoleEffect));
			result2 = result;
			return result2;
		}
		public void SetIceFronze(Living living)
		{
			new IceFronzeEffect(2).Start(this);
			this.BeginNextTurn = (LivingEventHandle)Delegate.Remove(this.BeginNextTurn, new LivingEventHandle(this.SetIceFronze));
		}
		public virtual void Die(int delay)
		{
			if (base.IsLiving && this.m_game != null)
			{
				this.m_game.AddAction(new LivingDieAction(this, delay));
			}
		}
		public override void Die()
		{
			if (this.m_blood > 0)
			{
				this.m_blood = 0;
				if (this.m_syncAtTime)
				{
					this.m_game.SendGameUpdateHealth(this, 6, 0);
				}
			}
			if (base.IsLiving)
			{
				if (this.IsAttacking)
				{
					this.StopAttacking();
				}
				base.Die();
				this.OnDied();
				this.m_game.CheckState(0);
			}
		}
		protected void OnDied()
		{
			if (this.Died != null)
			{
				this.Died(this);
			}
		}
		public void OnMakeDamage(Living living)
		{
			if (this.PlayerMakeDamage != null)
			{
				this.PlayerMakeDamage(living);
			}
		}
		protected void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			if (this.BeforeTakeDamage != null)
			{
				this.BeforeTakeDamage(this, source, ref damageAmount, ref criticalAmount, delay);
			}
		}
		public void OnTakedDamage(Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			if (this.TakePlayerDamage != null)
			{
				this.TakePlayerDamage(source, this, ref damageAmount, ref criticalAmount, delay);
			}
		}
		protected void OnBeginNewTurn()
		{
			if (this.BeginNextTurn != null)
			{
				this.BeginNextTurn(this);
			}
		}
		protected void OnBeginSelfTurn()
		{
			if (this.BeginSelfTurn != null)
			{
				this.BeginSelfTurn(this);
			}
		}
		protected void OnStartAttacked()
		{
			if (this.BeginAttacked != null)
			{
				this.BeginAttacked(this);
			}
		}
		protected void OnStartAttacking()
		{
			if (this.BeginAttacking != null)
			{
				this.BeginAttacking(this);
			}
		}
		protected void OnStopAttacking()
		{
			if (this.EndAttacking != null)
			{
				this.EndAttacking(this);
			}
		}
		public virtual void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount, int delay)
		{
			if (target.Team != this.Team)
			{
				this.CurrentIsHitTarget = true;
				this.TotalHurt += damageAmount + criticalAmount;
				if (!target.IsLiving)
				{
					this.TotalKill++;
				}
				this.m_game.currentTurnTotalDamage = damageAmount + criticalAmount;
				this.m_game.TotalHurt += damageAmount + criticalAmount;
			}
			if (this.AfterKillingLiving != null)
			{
				this.AfterKillingLiving(this, target, damageAmount, criticalAmount, delay);
			}
			if (target.DefenceEffectTrigger && target is Player && (target as Player).DefenceInformation)
			{
				this.Game.SendMessage((target as Player).PlayerDetail, LanguageMgr.GetTranslation("PlayerEquipEffect.Success2", new object[0]), LanguageMgr.GetTranslation("PlayerEquipEffect.Success3", new object[]
				{
					(target as Player).PlayerDetail.PlayerCharacter.NickName
				}), 3);
				(target as Player).DefenceInformation = false;
				target.DefenceEffectTrigger = false;
			}
		}
		public void OnAfterTakedDamage(Living target, int damageAmount, int criticalAmount, int delay)
		{
			if (this.AfterKilledByLiving != null)
			{
				this.AfterKilledByLiving(this, target, damageAmount, criticalAmount, delay);
			}
		}
		public void CallFuction(LivingCallBack func, int delay)
		{
			if (this.m_game != null)
			{
				this.m_game.AddAction(new LivingCallFunctionAction(this, func, delay));
			}
		}
	}
}