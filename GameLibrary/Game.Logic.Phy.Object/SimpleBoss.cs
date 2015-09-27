using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Game.Base.Managers;
namespace Game.Logic.Phy.Object
{
	public class SimpleBoss : TurnedLiving
	{
		public delegate void SimpleBossShootedEventHanld(int delay);
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected NpcInfo m_npcInfo;
		private ABrain m_ai;
		private List<SimpleNpc> m_child = new List<SimpleNpc>();
		private List<SimpleBoss> m_childB = new List<SimpleBoss>();
		private Dictionary<Player, int> m_mostHateful;
		public event SimpleBoss.SimpleBossShootedEventHanld SimpleBossShooted;
		public NpcInfo NpcInfo
		{
			get
			{
				return this.m_npcInfo;
			}
		}
		public List<SimpleNpc> Child
		{
			get
			{
				return this.m_child;
			}
		}
		public List<SimpleBoss> ChildB
		{
			get
			{
				return this.m_childB;
			}
		}
		public int CurrentLivingNpcNum
		{
			get
			{
				int count = 0;
				foreach (SimpleNpc child in this.Child)
				{
					if (child.IsLiving)
					{
						count++;
					}
				}
				foreach (SimpleBoss childb in this.ChildB)
				{
					if (childb.IsLiving)
					{
						count++;
					}
				}
				return count;
			}
		}
		public SimpleBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction, npcInfo.MaxBeatDis)
		{
			if (type == 0)
			{
				base.Type = eLivingType.SimpleBoss;
			}
			if (type == 1)
			{
				base.Type = eLivingType.SimpleBossNormal;
			}
			if (type == 2)
			{
				base.Type = eLivingType.SimpleBossHard;
			}
			if (type == 3)
			{
				base.Type = eLivingType.SimpleLongNpc;
			}
			this.m_mostHateful = new Dictionary<Player, int>();
			this.m_npcInfo = npcInfo;
			this.m_ai = (ScriptMgr.CreateInstance(npcInfo.Script) as ABrain);
			if (this.m_ai == null)
			{
				SimpleBoss.log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
				this.m_ai = SimpleBrain.Simple;
			}
			this.m_ai.Game = this.m_game;
			this.m_ai.Body = this;
			this.SimpleBossShooted = (SimpleBoss.SimpleBossShootedEventHanld)Delegate.Combine(this.SimpleBossShooted, new SimpleBoss.SimpleBossShootedEventHanld(this.ShootedSay));
			try
			{
				this.m_ai.OnCreated();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss Created error:{0}", ex);
			}
		}
		public override void Reset()
		{
			this.m_maxBlood = this.m_npcInfo.Blood;
			this.BaseDamage = (double)this.m_npcInfo.BaseDamage;
			this.BaseGuard = (double)this.m_npcInfo.BaseGuard;
			this.Attack = (double)this.m_npcInfo.Attack;
			this.Defence = (double)this.m_npcInfo.Defence;
			this.Agility = (double)this.m_npcInfo.Agility;
			this.Lucky = (double)this.m_npcInfo.Lucky;
			this.Grade = this.m_npcInfo.Level;
			this.Experience = this.m_npcInfo.Experience;
			base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
			this.m_delay = (int)this.Agility;
			base.Reset();
		}
		public override void Die()
		{
			base.Die();
		}
		public override void Die(int delay)
		{
			base.Die(delay);
		}
		public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, int type, int delay)
		{
			bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, type, delay);
			if (source is Player)
			{
				Player p = source as Player;
				int damage = damageAmount + criticalAmount;
				if (this.m_mostHateful.ContainsKey(p))
				{
					this.m_mostHateful[p] = this.m_mostHateful[p] + damage;
				}
				else
				{
					this.m_mostHateful.Add(p, damage);
				}
			}
			if (result)
			{
				this.OnShooted(delay);
			}
			return result;
		}
		public Player FindMostHatefulPlayer()
		{
			Player result;
			if (this.m_mostHateful.Count > 0)
			{
				KeyValuePair<Player, int> i = this.m_mostHateful.ElementAt(0);
				foreach (KeyValuePair<Player, int> kvp in this.m_mostHateful)
				{
					if (i.Value < kvp.Value)
					{
						i = kvp;
					}
				}
				result = i.Key;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type)
		{
			this.CreateChild(id, brithPoint, maxCount, maxCountForOnce, type, 0);
		}
		public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type, int objtype)
		{
			Point[] tag = new Point[brithPoint.Length];
			if (this.CurrentLivingNpcNum < maxCount)
			{
				int CountForOnce;
				if (maxCount - this.CurrentLivingNpcNum >= maxCountForOnce)
				{
					CountForOnce = maxCountForOnce;
				}
				else
				{
					CountForOnce = maxCount - this.CurrentLivingNpcNum;
				}
				for (int i = 0; i < CountForOnce; i++)
				{
					int index = 0;
					if (CountForOnce <= brithPoint.Length)
					{
						for (int j = 0; j < brithPoint.Length; j++)
						{
							index = base.Game.Random.Next(0, brithPoint.Length);
							bool result = false;
							for (int length = 0; length < tag.Length; length++)
							{
								if (brithPoint[index] == tag[length])
								{
									result = true;
									break;
								}
							}
							if (!result)
							{
								tag[index] = brithPoint[index];
								break;
							}
							j--;
						}
					}
					if (objtype == 0)
					{
						this.Child.Add(((PVEGame)base.Game).CreateNpc(id, brithPoint[index].X, brithPoint[index].Y, type));
					}
					else
					{
						this.ChildB.Add(((PVEGame)base.Game).CreateBoss(id, brithPoint[index].X, brithPoint[index].Y, -1, type));
					}
				}
			}
		}
		public void RandomConSay(string[] msg, int type, int delay, int finishTime)
		{
			int IsSay = base.Game.Random.Next(0, 2);
			if (IsSay == 1)
			{
				int index = base.Game.Random.Next(0, msg.Count<string>());
				string text = msg[index];
				base.Say(text, type, delay, finishTime);
			}
		}
		public void TowardsToPlayer(int playerX, int delay)
		{
			if (playerX > this.X)
			{
				base.ChangeDirection(1, delay);
			}
			else
			{
				base.ChangeDirection(-1, delay);
			}
		}
		public void OnShooted(int delay)
		{
			if (this.SimpleBossShooted != null)
			{
				this.SimpleBossShooted(delay);
			}
		}
		public override void PrepareNewTurn()
		{
			base.PrepareNewTurn();
			try
			{
				this.m_ai.OnBeginNewTurn();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss BeginNewTurn error:{0}", ex);
			}
		}
		public override void PrepareSelfTurn()
		{
			base.PrepareSelfTurn();
			base.AddDelay(this.m_npcInfo.Delay);
			try
			{
				this.m_ai.OnBeginSelfTurn();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss BeginSelfTurn error:{0}", ex);
			}
		}
		public override void StartAttacking()
		{
			base.StartAttacking();
			try
			{
				this.m_ai.OnStartAttacking();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss StartAttacking error:{0}", ex);
			}
			if (base.IsAttacking)
			{
				this.StopAttacking();
			}
		}
		public override void StopAttacking()
		{
			base.StopAttacking();
			try
			{
				this.m_ai.OnStopAttacking();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss StopAttacking error:{0}", ex);
			}
		}
		public override void Dispose()
		{
			base.Dispose();
			try
			{
				this.m_ai.Dispose();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss Dispose error:{0}", ex);
			}
		}
		public void KillPlayerSay()
		{
			try
			{
				this.m_ai.OnKillPlayerSay();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss Say error:{0}", ex);
			}
		}
		public void DiedSay()
		{
			try
			{
				this.m_ai.OnDiedSay();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss DiedSay error {0}", ex);
			}
		}
		public void DiedEvent()
		{
			try
			{
				this.m_ai.OnDiedEvent();
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss DiedEvent error {0}", ex);
			}
		}
		public void ShootedSay(int delay)
		{
			try
			{
				this.m_ai.OnShootedSay(delay);
			}
			catch (Exception ex)
			{
				SimpleBoss.log.ErrorFormat("SimpleBoss ShootedSay error {0}", ex);
			}
		}
	}
}