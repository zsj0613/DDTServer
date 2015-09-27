using System;
namespace Game.Logic.Actions
{
	public class CheckPVPGameStateAction : IAction
	{
		private long m_tick;
		private bool m_isFinished;
		public CheckPVPGameStateAction(int delay)
		{
			this.m_isFinished = false;
			this.m_tick += TickHelper.GetTickCount() + (long)delay;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (this.m_tick <= tick)
			{
				PVPGame pvp = game as PVPGame;
				if (pvp != null)
				{
					switch (game.GameState)
					{
					case eGameState.Inited:
						pvp.Prepare();
						break;
					case eGameState.Prepared:
						pvp.StartLoading();
						break;
					case eGameState.Loading:
						if (pvp.IsAllComplete())
						{
							pvp.StartGame();
						}
						break;
					case eGameState.Playing:
						if ((pvp.CurrentPlayer == null || !pvp.CurrentPlayer.IsAttacking) && pvp.CurrentActionCount == 1)
						{
							if (pvp.CanGameOver())
							{
								pvp.GameOver();
							}
							else
							{
								pvp.NextTurn();
							}
						}
						break;
					case eGameState.GameOver:
						if (pvp.CurrentActionCount == 1)
						{
							pvp.Stop();
						}
						break;
					}
				}
				this.m_isFinished = true;
			}
		}
		public bool IsFinished(BaseGame game, long tick)
		{
			return this.m_isFinished;
		}
	}
}
