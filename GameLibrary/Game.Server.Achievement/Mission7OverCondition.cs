using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class Mission7OverCondition : BaseUserRecord
	{
		public Mission7OverCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.MissionOver += new GamePlayer.PlayerMissionOverEventHandle(this.player_MissionOver);
		}
		private void player_MissionOver(AbstractGame game, int missionId, bool isWin)
		{
			if (game.GameType == eGameType.FightLab && isWin)
			{
				if (missionId == 105 || missionId == 115 || missionId == 125)
				{
					this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, 1);
				}
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.MissionOver -= new GamePlayer.PlayerMissionOverEventHandle(this.player_MissionOver);
		}
	}
}
