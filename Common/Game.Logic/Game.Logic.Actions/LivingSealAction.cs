using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingSealAction : BaseAction
	{
		private Living m_Living;
		private Living m_Target;
		private int m_Type;
		public LivingSealAction(Living Living, Living target, int type, int delay) : base(delay, 2000)
		{
			this.m_Living = Living;
			this.m_Target = target;
			this.m_Type = type;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			this.m_Target.AddEffect(new SealEffect(2, this.m_Type), 0);
			base.Finish(tick);
		}
	}
}
