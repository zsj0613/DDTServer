using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameKillByRoomCondition : BaseCondition
	{
		public GameKillByRoomCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea)
		{
			if (!isLiving)
			{
			}
			if (!isLiving && type == 1)
			{
				switch (game.RoomType)
				{
				case eRoomType.Match:
					if ((this.m_info.Para1 == 0 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;
				case eRoomType.Freedom:
					if ((this.m_info.Para1 == 1 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;
				case eRoomType.Exploration:
					if ((this.m_info.Para1 == 2 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;
				case eRoomType.Boss:
					if ((this.m_info.Para1 == 3 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;
				case eRoomType.Treasure:
					if ((this.m_info.Para1 == 4 || this.m_info.Para1 == -1) && base.Value > 0)
					{
						base.Value--;
					}
					break;
				}
				if (base.Value < 0)
				{
					base.Value = 0;
				}
			}
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
