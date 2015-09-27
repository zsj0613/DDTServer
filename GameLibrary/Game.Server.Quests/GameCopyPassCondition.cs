using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameCopyPassCondition : BaseCondition
	{
		public GameCopyPassCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.MissionOver += new GamePlayer.PlayerMissionOverEventHandle(this.player_MissionOver);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.MissionOver -= new GamePlayer.PlayerMissionOverEventHandle(this.player_MissionOver);
		}
		private void player_MissionOver(AbstractGame game, int missionId, bool isWin)
		{
			if (isWin && missionId == this.m_info.Para1 && base.Value > 0)
			{
				base.Value--;
			}
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
