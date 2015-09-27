using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingDirectSetXYAction : BaseAction
	{
		private Living m_living;
		private int m_x;
		private int m_y;
		public LivingDirectSetXYAction(Living living, int x, int y, int delay) : base(delay)
		{
			this.m_living = living;
			this.m_x = x;
			this.m_y = y;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			this.m_living.SetXY(this.m_x, this.m_y);
			base.Finish(tick);
		}
	}
}
