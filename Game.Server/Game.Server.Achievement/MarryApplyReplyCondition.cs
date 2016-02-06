using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class MarryApplyReplyCondition : BaseUserRecord
	{
		public MarryApplyReplyCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.PlayerMarry += new GamePlayer.PlayerMarryEventHandel(this.player_PlayerMarry);
		}
		private void player_PlayerMarry()
		{
			this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, 1);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.PlayerMarry -= new GamePlayer.PlayerMarryEventHandel(this.player_PlayerMarry);
		}
	}
}
