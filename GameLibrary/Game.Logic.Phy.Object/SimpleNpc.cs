using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Logic.LogEnum;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Managers;
namespace Game.Logic.Phy.Object
{
	public class SimpleNpc : Living
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private NpcInfo m_npcInfo;
		private ABrain m_ai;
		private int m_rank;
		public int Rank
		{
			get
			{
				return this.m_rank;
			}
		}
		public NpcInfo NpcInfo
		{
			get
			{
				return this.m_npcInfo;
			}
		}
		public SimpleNpc(int id, BaseGame game, NpcInfo npcInfo, int type, int direction, int rank) : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction, npcInfo.MaxBeatDis)
		{
			if (type == 0)
			{
				base.Type = eLivingType.SimpleNpc;
			}
			else
			{
				if (type == 1)
				{
					base.Type = eLivingType.SimpleNpcNormal;
				}
				else
				{
					if (type == 2)
					{
						base.Type = eLivingType.SimpleNpcDeck;
					}
					else
					{
						if (type == 3)
						{
							base.Type = eLivingType.SimpleWingNpc;
						}
					}
				}
			}
			this.m_npcInfo = npcInfo;
			this.m_ai = (ScriptMgr.CreateInstance(npcInfo.Script) as ABrain);
			if (this.m_ai == null)
			{
				if (this.m_npcInfo.Type != 3)
				{
					SimpleNpc.log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
				}
				this.m_ai = SimpleBrain.Simple;
			}
			this.m_ai.Game = this.m_game;
			this.m_ai.Body = this;
			this.m_rank = rank;
			this.Reset();
			try
			{
				this.m_ai.OnCreated();
			}
			catch (Exception ex)
			{
				SimpleNpc.log.ErrorFormat("SimpleNpc Created error:{0}", ex);
			}
		}
		public override void Reset()
		{
			this.Agility = (double)this.m_npcInfo.Agility;
			this.Attack = (double)this.m_npcInfo.Attack;
			this.BaseDamage = (double)this.m_npcInfo.BaseDamage;
			this.BaseGuard = (double)this.m_npcInfo.BaseGuard;
			this.Lucky = (double)this.m_npcInfo.Lucky;
			this.Grade = this.m_npcInfo.Level;
			this.Experience = this.m_npcInfo.Experience;
			base.SetRect(this.m_npcInfo.X, this.m_npcInfo.Y, this.m_npcInfo.Width, this.m_npcInfo.Height);
			base.Reset();
		}
		public void GetDropItemInfo()
		{
			if (this.m_game.CurrentLiving is Player)
			{
				Player p = this.m_game.CurrentLiving as Player;
				List<ItemInfo> infos = null;
				int gold = 0;
				int money = 0;
				int gifttoken = 0;
				DropInventory.NPCDrop(this.m_npcInfo.DropId, ref infos);
				if (infos != null)
				{
					foreach (ItemInfo info in infos)
					{
						ItemInfo tempInfo = ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref gifttoken);
						if (tempInfo != null)
						{
							if (tempInfo.Template.CategoryID == 10)
							{
								p.PlayerDetail.AddTemplate(tempInfo, eBageType.FightBag, info.Count);
							}
							else
							{
								p.PlayerDetail.AddTemplate(tempInfo, eBageType.TempBag, info.Count);
							}
						}
					}
					p.PlayerDetail.AddGold(gold);
					p.PlayerDetail.AddMoney(money, LogMoneyType.Award, LogMoneyType.Award_Drop);
					p.PlayerDetail.AddGiftToken(gifttoken);
				}
			}
		}
		public override void Die()
		{
			this.GetDropItemInfo();
			base.Die();
		}
		public override void Die(int delay)
		{
			this.GetDropItemInfo();
			base.Die(delay);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc BeginNewTurn error:{0}", ex);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc StartAttacking error:{0}", ex);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc Dispose error:{0}", ex);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc DiedSay error {0}", ex);
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
				SimpleNpc.log.ErrorFormat("SimpleNpc DiedEvent error {0}", ex);
			}
		}
	}
}
