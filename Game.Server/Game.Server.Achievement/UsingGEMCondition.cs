using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class UsingGEMCondition : BaseUserRecord
	{
		public UsingGEMCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterUsingItem += new GamePlayer.PlayerItemPropertyEventHandle(this.player_AfterUsingItem);
		}
		private void player_AfterUsingItem(int templateID)
		{
			if (templateID == 311000 || templateID == 311999 || templateID == 312000 || templateID == 312999 || templateID == 313000 || templateID == 313999)
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
