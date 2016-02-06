using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingBoltMoveAction : BaseAction
	{
		private int m_x;
		private int m_y;
		private string m_action;
		private Living m_living;
		public LivingBoltMoveAction(Living living, int toX, int toY, string action, int delay, int finishTime) : base(delay, finishTime)
		{
			this.m_living = living;
			this.m_x = toX;
			this.m_y = toY;
			this.m_action = action;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			this.m_living.SetXY(this.m_x, this.m_y);
			game.SendLivingBoltMove(this.m_living, this.m_x, this.m_y, this.m_action);
			base.Finish(tick);
		}
	}
}
