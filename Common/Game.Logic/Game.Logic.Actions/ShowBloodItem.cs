using System;
namespace Game.Logic.Actions
{
	public class ShowBloodItem : BaseAction
	{
		private int m_livingId;
		public ShowBloodItem(int livingId, int delay, int finishTime) : base(delay, finishTime)
		{
			this.m_livingId = livingId;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			game.SendShowBloodItem(this.m_livingId);
			base.Finish(tick);
		}
	}
}
