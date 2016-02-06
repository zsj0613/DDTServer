using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameMissionOverCondition : BaseCondition
	{
		public GameMissionOverCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.MissionTurnOver += new GamePlayer.PlayerMissionTurnOverEventHandle(this.player_MissionOver);
		}
		private void player_MissionOver(AbstractGame game, int missionId, int turnCount)
		{
			if ((missionId == this.m_info.Para1 || this.m_info.Para1 == -1) && turnCount <= this.m_info.Para2 && base.Value > 0)
			{
				base.Value = 0;
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.MissionTurnOver -= new GamePlayer.PlayerMissionTurnOverEventHandle(this.player_MissionOver);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
