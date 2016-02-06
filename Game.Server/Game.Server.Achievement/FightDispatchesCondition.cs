using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class FightDispatchesCondition : BaseUserRecord
	{
		public FightDispatchesCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.PlayerDispatches += new GamePlayer.PlayerDispatchesEventHandel(this.player_PlayerDispatches);
		}
		private void player_PlayerDispatches()
		{
			this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, 1);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.PlayerDispatches -= new GamePlayer.PlayerDispatchesEventHandel(this.player_PlayerDispatches);
		}
	}
}
