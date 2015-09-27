using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Reflection;
namespace Game.Server.Quests
{
	public class BaseCondition
	{
		protected QuestConditionInfo m_info;
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private int m_value;
		private BaseQuest m_quest;
		public QuestConditionInfo Info
		{
			get
			{
				return this.m_info;
			}
		}
		public int Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				if (this.m_value != value)
				{
					this.m_value = value;
					this.m_quest.Update();
				}
			}
		}
		public BaseCondition(BaseQuest quest, QuestConditionInfo info, int value)
		{
			this.m_quest = quest;
			this.m_info = info;
			this.m_value = value;
		}
		public virtual void Reset(GamePlayer player)
		{
			this.m_value = this.m_info.Para2;
		}
		public virtual void AddTrigger(GamePlayer player)
		{
		}
		public virtual void RemoveTrigger(GamePlayer player)
		{
		}
		public virtual bool IsCompleted(GamePlayer player)
		{
			return false;
		}
		public virtual bool Finish(GamePlayer player)
		{
			return true;
		}
		public virtual bool CancelFinish(GamePlayer player)
		{
			return true;
		}
		public static BaseCondition CreateCondition(BaseQuest quest, QuestConditionInfo info, int value)
		{
			BaseCondition result;
			switch (info.CondictionType)
			{
			case 1:
				result = new OwnGradeCondition(quest, info, value);
				break;
			case 2:
				result = new ItemMountingCondition(quest, info, value);
				break;
			case 3:
				result = new UsingItemCondition(quest, info, value);
				break;
			case 4:
				result = new GameKillByRoomCondition(quest, info, value);
				break;
			case 5:
				result = new GameFightByRoomCondition(quest, info, value);
				break;
			case 6:
				result = new GameOverByRoomCondition(quest, info, value);
				break;
			case 7:
				result = new GameCopyOverCondition(quest, info, value);
				break;
			case 8:
				result = new GameCopyPassCondition(quest, info, value);
				break;
			case 9:
				result = new ItemStrengthenCondition(quest, info, value);
				break;
			case 10:
				result = new ShopCondition(quest, info, value);
				break;
			case 11:
				result = new ItemFusionCondition(quest, info, value);
				break;
			case 12:
				result = new ItemMeltCondition(quest, info, value);
				break;
			case 13:
				result = new GameMonsterCondition(quest, info, value);
				break;
			case 14:
				result = new OwnPropertyCondition(quest, info, value);
				break;
			case 15:
				result = new TurnPropertyCondition(quest, info, value);
				break;
			case 16:
				result = new DirectFinishCondition(quest, info, value);
				break;
			case 17:
				result = new OwnMarryCondition(quest, info, value);
				break;
			case 18:
				result = new OwnConsortiaCondition(quest, info, value);
				break;
			case 19:
				result = new ItemComposeCondition(quest, info, value);
				break;
			case 20:
				result = new ClientModifyCondition(quest, info, value);
				break;
			case 21:
				result = new GameMissionOverCondition(quest, info, value);
				break;
			case 22:
				result = new GameKillByGameCondition(quest, info, value);
				break;
			case 23:
				result = new GameFightByGameCondition(quest, info, value);
				break;
			case 24:
				result = new GameOverByGameCondition(quest, info, value);
				break;
			case 25:
				result = new ItemInsertCondition(quest, info, value);
				break;
			case 26:
				result = new GameFightByCouples(quest, info, value);
				break;
			case 27:
				result = new OwnSpaCondition(quest, info, value);
				break;
			case 28:
				result = new GameFightWinByCouples(quest, info, value);
				break;
			default:
				if (BaseCondition.log.IsErrorEnabled)
				{
					BaseCondition.log.Error(string.Format("Can't find quest condition : {0}", info.CondictionType));
				}
				result = null;
				break;
			}
			return result;
		}
	}
}
