using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class Mission2OverCondition : BaseUserRecord
	{
		public Mission2OverCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.MissionOver += new GamePlayer.PlayerMissionOverEventHandle(this.player_MissionOver);
		}
		private void player_MissionOver(AbstractGame game, int missionId, bool isWin)
		{
			if (game.GameType == eGameType.Treasure && isWin)
			{
				if (missionId == 1073 || missionId == 1176 || missionId == 1277 || missionId == 1378)
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
