using Game.Server.GameObjects;
using Game.Server.Quests;
using System;
namespace Game.Server.Achievement
{
	internal class QuestGoodManCardCondition : BaseUserRecord
	{
		public QuestGoodManCardCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.PlayerQuestFinish += new GamePlayer.PlayerQuestFinishEventHandel(this.player_PlayerQuestFinish);
		}
		private void player_PlayerQuestFinish(BaseQuest baseQuest)
		{
			if (baseQuest.Info.ID == 86)
			{
				this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, 1);
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.PlayerQuestFinish -= new GamePlayer.PlayerQuestFinishEventHandel(this.player_PlayerQuestFinish);
		}
	}
}
