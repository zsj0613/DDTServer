using System;
namespace Game.Logic.Actions
{
	public class BaseAction : IAction
	{
		private long m_tick;
		private long m_finishDelay;
		private long m_finishTick;
		public BaseAction(int delay) : this(delay, 0)
		{
		}
		public BaseAction(int delay, int finishDelay)
		{
			this.m_tick = TickHelper.GetTickCount() + (long)delay;
			this.m_finishDelay = (long)finishDelay;
			this.m_finishTick = 9223372036854775807L;
		}
		public void Execute(BaseGame game, long tick)
		{
			if (this.m_tick <= tick && this.m_finishTick == 9223372036854775807L)
			{
				this.ExecuteImp(game, tick);
			}
		}
		protected virtual void ExecuteImp(BaseGame game, long tick)
		{
			this.Finish(tick);
		}
		public void Finish(long tick)
		{
			this.m_finishTick = tick + this.m_finishDelay;
		}
		public bool IsFinished(BaseGame game, long tick)
		{
			return (game is PVEGame && (game as PVEGame).IsPassDrama) || this.m_finishTick <= tick;
		}
	}
}
