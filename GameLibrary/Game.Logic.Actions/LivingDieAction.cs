using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingDieAction : BaseAction
	{
		private Living m_living;
		public LivingDieAction(Living living, int delay) : base(delay, 1000)
		{
			this.m_living = living;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			this.m_living.Die();
			base.Finish(tick);
		}
	}
}
