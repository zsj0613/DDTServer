using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class SealEffect : AbstractEffect
	{
		private int m_count;
		private int m_type;
		public SealEffect(int count, int type) : base(eEffectType.SealEffect)
		{
			this.m_count = count;
			this.m_type = type;
		}
		public override bool Start(Living living)
		{
			SealEffect effect = living.EffectList.GetOfType(eEffectType.SealEffect) as SealEffect;
			bool result;
			if (effect != null)
			{
				effect.m_count = this.m_count;
				result = true;
			}
			else
			{
				result = base.Start(living);
			}
			return result;
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.SetSeal(true, this.m_type);
			this.m_living.Game.SendGameUpdateSealState(living, this.m_type);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.SetSeal(false, this.m_type);
			this.m_living.Game.SendGameUpdateSealState(living, this.m_type);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_count--;
			if (this.m_count < 0)
			{
				this.Stop();
			}
		}
	}
}
