using System;
namespace Game.Logic.Actions
{
	public class LockFocusAction : BaseAction
	{
		private bool m_isLock;
		public LockFocusAction(bool isLock, int delay, int finishTime) : base(delay, finishTime)
		{
			this.m_isLock = isLock;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendLockFocus(this.m_isLock);
			base.Finish(tick);
		}
	}
}
