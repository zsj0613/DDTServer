using Game.Server.GameObjects;
using log4net;
using System;
using System.Collections;
using System.Reflection;
namespace Game.Server.Achievement
{
	public class BaseUserRecord
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected GamePlayer m_player;
		protected int m_type;
		public BaseUserRecord(GamePlayer player, int type)
		{
			this.m_player = player;
			this.m_type = type;
		}
		public virtual void AddTrigger(GamePlayer player)
		{
		}
		public virtual void RemoveTrigger(GamePlayer player)
		{
		}
		public static void CreateCondition(Hashtable ht, GamePlayer m_player)
		{
			foreach (DictionaryEntry de in ht)
			{
				int type = int.Parse(de.Key.ToString());
				switch (type)
				{
				case 1:
					new ChangeAttackCondition(m_player, type);
					break;
				case 2:
					new ChangeDefenceCondition(m_player, type);
					break;
				case 3:
					new ChangeAgilityCondition(m_player, type);
					break;
				case 4:
					new ChangeLuckyCondition(m_player, type);
					break;
				case 9:
					new ChangeFightPowerCondition(m_player, type);
					break;
				case 10:
					new ChangeGradeCondition(m_player, type);
					break;
				case 11:
					new ChangeTotalCondition(m_player, type);
					break;
				case 12:
					new ChangeWinCondition(m_player, type);
					break;
				case 13:
					new ChangeOnlineTimeCondition(m_player, type);
					break;
				case 14:
					new FightByFreeCondition(m_player, type);
					break;
				case 15:
					new FightByGuildCondition(m_player, type);
					break;
				case 16:
					new FightByFreeSpanAreaCondition(m_player, type);
					break;
				case 17:
					new FightByGuildSpanAreaCondition(m_player, type);
					break;
				case 18:
					new MarryApplyReplyCondition(m_player, type);
					break;
				case 19:
					new GameKillByGameCondition(m_player, type);
					break;
				case 20:
					new FightDispatchesCondition(m_player, type);
					break;
				case 21:
					new QuestBlueCondition(m_player, type);
					break;
				case 23:
					new PlayerGoodsPresentCondition(m_player, type);
					break;
				case 24:
					new AddRichesOfferCondition(m_player, type);
					break;
				case 25:
					new AddRichesRobCondition(m_player, type);
					break;
				case 26:
					new Mission1KillCondition(m_player, type);
					break;
				case 27:
					new Mission2KillCondition(m_player, type);
					break;
				case 28:
					new Mission1OverCondition(m_player, type);
					break;
				case 29:
					new Mission2OverCondition(m_player, type);
					break;
				case 36:
					new ChangeColorsCondition(m_player, type);
					break;
				case 38:
					new AddGoldCondition(m_player, type);
					break;
				case 39:
					new AddGiftTokenCondition(m_player, type);
					break;
				case 40:
					new AddMedalCondition(m_player, type);
					break;
				case 43:
					new UsingGEMCondition(m_player, type);
					break;
				case 44:
					new UsingRenameCardCondition(m_player, type);
					break;
				case 45:
					new UsingSalutingGunCondition(m_player, type);
					break;
				case 46:
					new UsingSpanAreaBugleCondition(m_player, type);
					break;
				case 47:
					new UsingBigBugleCondition(m_player, type);
					break;
				case 48:
					new UsingSmallBugleCondition(m_player, type);
					break;
				case 49:
					new UsingEngagementRingCondition(m_player, type);
					break;
				case 52:
					new Mission3OverCondition(m_player, type);
					break;
				case 53:
					new Mission4OverCondition(m_player, type);
					break;
				case 54:
					new Mission5OverCondition(m_player, type);
					break;
				case 55:
					new Mission6OverCondition(m_player, type);
					break;
				case 56:
					new Mission7OverCondition(m_player, type);
					break;
				case 57:
					new ItemStrengthenCondition(m_player, type);
					break;
				case 59:
					new QuestGoodManCardCondition(m_player, type);
					break;
				}
			}
		}
	}
}
