using System;
namespace Game.Logic.Actions
{
	public class FocusAction : BaseAction
	{
		private int m_x;
		private int m_y;
		private int m_type;
		public FocusAction(int x, int y, int type, int delay, int finishTime) : base(delay, finishTime)
		{
			this.m_x = x;
			this.m_y = y;
			this.m_type = type;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendPhysicalObjFocus(this.m_x, this.m_y, this.m_type);
			base.Finish(tick);
		}
	}
}
