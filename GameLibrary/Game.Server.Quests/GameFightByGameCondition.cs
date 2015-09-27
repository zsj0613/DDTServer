using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameFightByGameCondition : BaseCondition
	{
		public GameFightByGameCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.player_GameOver);
		}
		private void player_GameOver(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			switch (game.GameType)
			{
			case eGameType.Free:
				if ((this.m_info.Para1 == 0 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			case eGameType.Guild:
				if ((this.m_info.Para1 == 1 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			case eGameType.Training:
				if ((this.m_info.Para1 == 2 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			case eGameType.ALL:
				if ((this.m_info.Para1 == 4 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			case eGameType.Exploration:
				if ((this.m_info.Para1 == 5 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			case eGameType.Boss:
				if ((this.m_info.Para1 == 6 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			case eGameType.Treasure:
				if ((this.m_info.Para1 == 7 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
				break;
			}
			if (base.Value < 0)
			{
				base.Value = 0;
			}
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
