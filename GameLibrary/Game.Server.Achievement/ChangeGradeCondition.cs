using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class ChangeGradeCondition : BaseUserRecord
	{
		public ChangeGradeCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.LevelUp += new GamePlayer.PlayerEventHandle(this.player_LevelUp);
		}
		private void player_LevelUp(GamePlayer player)
		{
			this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, player.Level, 1);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.LevelUp -= new GamePlayer.PlayerEventHandle(this.player_LevelUp);
		}
	}
}
