using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class UsingRenameCardCondition : BaseUserRecord
	{
		public UsingRenameCardCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterUsingItem += new GamePlayer.PlayerItemPropertyEventHandle(this.player_AfterUsingItem);
		}
		private void player_AfterUsingItem(int templateID)
		{
			if (templateID == 11994)
			{
				this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, 1);
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterUsingItem -= new GamePlayer.PlayerItemPropertyEventHandle(this.player_AfterUsingItem);
		}
	}
}
