using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingChangeDirectionAction : BaseAction
	{
		private Living m_Living;
		private int m_direction;
		public LivingChangeDirectionAction(Living living, int direction, int delay) : base(delay)
		{
			this.m_Living = living;
			this.m_direction = direction;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			this.m_Living.Direction = this.m_direction;
			base.Finish(tick);
		}
	}
}
