using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingOffSealAction : BaseAction
	{
		private Living m_Living;
		private Living m_Target;
		public LivingOffSealAction(Living Living, Living target, int delay) : base(delay, 1000)
		{
			this.m_Living = Living;
			this.m_Target = target;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			SealEffect effect = (SealEffect)this.m_Target.EffectList.GetOfType(eEffectType.SealEffect);
			if (effect != null)
			{
				effect.Stop();
			}
			base.Finish(tick);
		}
	}
}
