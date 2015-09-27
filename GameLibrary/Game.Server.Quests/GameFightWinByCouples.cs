using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameFightWinByCouples : BaseCondition
	{
		public GameFightWinByCouples(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
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
			if (isCouple && isWin && (game.GameType == eGameType.ALL || game.GameType == eGameType.Guild || game.GameType == eGameType.Free))
			{
				if ((this.m_info.Para1 == 0 || this.m_info.Para1 == -1) && base.Value > 0)
				{
					base.Value--;
				}
			}
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
