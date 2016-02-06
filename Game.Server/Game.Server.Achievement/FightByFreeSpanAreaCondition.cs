using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class FightByFreeSpanAreaCondition : BaseUserRecord
	{
		public FightByFreeSpanAreaCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
		}
		private void player_GameOver(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			if (game.GameType == eGameType.Free && isWin && isSpanArea)
			{
				this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, 1);
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
		}
	}
}
