using log4net;
using System;
using System.Reflection;
namespace Game.Logic.Actions
{
	public class CheckPVEGameStateAction : IAction
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private long m_time;
		private bool m_isFinished;
		public CheckPVEGameStateAction(int delay)
		{
			this.m_time = TickHelper.GetTickCount() + (long)delay;
			this.m_isFinished = false;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (this.m_time <= tick && game.GetWaitTimer() < tick)
			{
				PVEGame pve = game as PVEGame;
				if (pve != null)
				{
					switch (pve.GameState)
					{
					case eGameState.Inited:
						pve.Prepare();
						break;
					case eGameState.Prepared:
						pve.PrepareNewSession();
						break;
					case eGameState.Loading:
						if (pve.IsAllComplete())
						{
							pve.StartGame();
						}
						else
						{
							game.WaitTime(1000);
						}
						break;
					case eGameState.GameStart:
						if (game.CurrentActionCount <= 1)
						{
							pve.PrepareFightingLivings();
						}
						break;
					case eGameState.Playing:
						if ((pve.CurrentLiving == null || !pve.CurrentLiving.IsAttacking) && game.CurrentActionCount <= 1)
						{
							if (pve.CanGameOver())
							{
								pve.PrepareGameOver();
							}
							else
							{
								pve.NextTurn();
							}
						}
						break;
					case eGameState.PrepareGameOver:
						if (pve.CurrentActionCount <= 1)
						{
							pve.GameOver();
						}
						break;
					case eGameState.GameOver:
						if (pve.HasNextSession())
						{
							pve.PrepareNewSession();
						}
						else
						{
							pve.GameOverAllSession();
						}
						break;
					case eGameState.SessionPrepared:
						if (pve.CanStartNewSession())
						{
							pve.StartLoading();
						}
						else
						{
							game.WaitTime(1000);
						}
						break;
					case eGameState.ALLSessionStopped:
						if (pve.PlayerCount == 0 || pve.WantTryAgain == 0)
						{
							if (pve.PveInfo.Type == 10 && !pve.isTankCard())
							{
								return;
							}
							pve.Stop();
						}
						else
						{
							if (pve.WantTryAgain == 1)
							{
								pve.SessionId--;
								pve.PrepareNewSession();
							}
							else
							{
								game.WaitTime(1000);
							}
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
