using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	internal class PlayerGoodsPresentCondition : BaseUserRecord
	{
		public PlayerGoodsPresentCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.PlayerGoodsPresent += new GamePlayer.PlayerGoodsPresentEventHandel(this.player_PlayerGoodsPresent);
		}
		private void player_PlayerGoodsPresent(int count)
		{
			this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, count);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.PlayerGoodsPresent -= new GamePlayer.PlayerGoodsPresentEventHandel(this.player_PlayerGoodsPresent);
		}
	}
}
