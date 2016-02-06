using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingSayAction : BaseAction
	{
		private Living m_living;
		private string m_msg;
		private int m_type;
		public LivingSayAction(Living living, string msg, int type, int delay, int finishTime) : base(delay, finishTime)
		{
			this.m_living = living;
			this.m_msg = msg;
			this.m_type = type;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendLivingSay(this.m_living, this.m_msg, this.m_type);
			base.Finish(tick);
		}
	}
}
