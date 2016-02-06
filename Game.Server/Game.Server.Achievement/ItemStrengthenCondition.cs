using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class ItemStrengthenCondition : BaseUserRecord
	{
		public ItemStrengthenCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.ItemStrengthen += new GamePlayer.PlayerItemStrengthenEventHandle(this.player_ItemStrengthen);
		}
		private void player_ItemStrengthen(int categoryID, int level)
		{
			this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, level, 1);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.ItemStrengthen -= new GamePlayer.PlayerItemStrengthenEventHandle(this.player_ItemStrengthen);
		}
	}
}
