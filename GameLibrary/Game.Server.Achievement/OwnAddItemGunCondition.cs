using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class OwnAddItemGunCondition : BaseUserRecord
	{
		public OwnAddItemGunCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea)
		{
			int count = 0;
			count += this.m_player.GetItemCount(7015);
			count += this.m_player.GetItemCount(7016);
			count += this.m_player.GetItemCount(7017);
			count += this.m_player.GetItemCount(7018);
			count += this.m_player.GetItemCount(7019);
			count += this.m_player.GetItemCount(7020);
			count += this.m_player.GetItemCount(7021);
			count += this.m_player.GetItemCount(7022);
			count += this.m_player.GetItemCount(7023);
			this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, count);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
	}
}
