using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class Mission1KillCondition : BaseUserRecord
	{
		public Mission1KillCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea)
		{
			if (game.GameType == eGameType.Treasure)
			{
				if (!isLiving && type == 2)
				{
					if (id == 2003 || id == 2103)
					{
						this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, 1);
					}
				}
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
	}
}
