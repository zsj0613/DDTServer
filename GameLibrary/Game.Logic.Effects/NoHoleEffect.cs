using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class NoHoleEffect : AbstractEffect
	{
		private int m_count;
		public NoHoleEffect(int count) : base(eEffectType.NoHoleEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			NoHoleEffect effect = living.EffectList.GetOfType(eEffectType.NoHoleEffect) as NoHoleEffect;
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
			living.IsNoHole = true;
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.IsNoHole = false;
		}
		private void player_BeginFitting(Living player)
		{
			this.m_count--;
			if (this.m_count <= 0)
			{
				this.Stop();
			}
		}
	}
}
