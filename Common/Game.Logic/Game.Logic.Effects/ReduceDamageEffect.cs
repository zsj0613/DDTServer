using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class ReduceDamageEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public ReduceDamageEffect(int count, int probability) : base(eEffectType.ReduceDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			ReduceDamageEffect effect = living.EffectList.GetOfType(eEffectType.ReduceDamageEffect) as ReduceDamageEffect;
			bool result;
			if (effect != null)
			{
				this.m_probability = ((this.m_probability > effect.m_probability) ? this.m_probability : effect.m_probability);
				result = true;
			}
			else
			{
				result = base.Start(living);
			}
			return result;
		}
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			this.IsTrigger = false;
			if (damageAmount > 0)
			{
				if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
				{
					this.IsTrigger = true;
					if ((damageAmount -= this.m_count) <= 0)
					{
						damageAmount = 1;
					}
					living.DefenceEffectTrigger = true;
					living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("ReduceDamageEffect.msg", new object[0]), 9, delay, 1000));
				}
			}
		}
	}
}
