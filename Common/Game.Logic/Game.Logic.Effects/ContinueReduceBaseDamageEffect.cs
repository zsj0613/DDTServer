using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ContinueReduceBaseDamageEffect : AbstractEffect
	{
		private int m_count;
		private int m_effectcount;
		public ContinueReduceBaseDamageEffect(int count, int effectcount) : base(eEffectType.ContinueReduceBaseDamageEffect)
		{
			this.m_count = count;
			this.m_effectcount = effectcount;
		}
		public override bool Start(Living living)
		{
			ContinueReduceBaseDamageEffect effect = living.EffectList.GetOfType(eEffectType.ContinueReduceBaseDamageEffect) as ContinueReduceBaseDamageEffect;
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
			living.BaseDamage = living.BaseDamage * (double)(100 - this.m_effectcount) / 100.0;
			living.Game.SendPlayerPicture(living, 4, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.BaseDamage = living.BaseDamage / (double)(100 - this.m_effectcount) * 100.0;
			living.Game.SendPlayerPicture(living, 4, false);
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
