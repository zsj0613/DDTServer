using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class Mission5OverCondition : BaseUserRecord
	{
		public Mission5OverCondition(GamePlayer player, int type) : base(player, type)
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
				if (missionId == 103 || missionId == 113 || missionId == 123)
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
