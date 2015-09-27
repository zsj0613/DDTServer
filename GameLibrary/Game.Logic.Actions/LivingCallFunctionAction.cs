using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingCallFunctionAction : BaseAction
	{
		private Living m_living;
		private LivingCallBack m_func;
		public LivingCallFunctionAction(Living living, LivingCallBack func, int delay) : base(delay)
		{
			this.m_living = living;
			this.m_func = func;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			try
			{
				if (this.m_func != null)
				{
					this.m_func();
				}
			}
			finally
			{
				base.Finish(tick);
			}
		}
	}
}
