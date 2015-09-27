using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ReduceStrengthEffect : AbstractEffect
	{
		private int m_count;
		private int m_turn = 2;
		public ReduceStrengthEffect(int count) : base(eEffectType.ReduceStrengthEffect)
		{
			this.m_count = count;
		}
		public override bool Start(Living living)
		{
			ReduceStrengthEffect effect = living.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect;
			bool result;
			if (effect != null)
			{
				effect.m_turn = this.m_turn;
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
			living.Game.SendPlayerPicture(living, 1, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 1, false);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_turn--;
			if (this.m_turn < 0)
			{
				this.Stop();
			}
			else
			{
				if (living is Player)
				{
					(living as Player).Energy -= this.m_count;
				}
			}
		}
	}
}
