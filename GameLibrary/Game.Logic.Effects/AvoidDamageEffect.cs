using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class AvoidDamageEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public AvoidDamageEffect(int count, int probability) : base(eEffectType.AvoidDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AvoidDamageEffect effect = living.EffectList.GetOfType(eEffectType.AvoidDamageEffect) as AvoidDamageEffect;
			bool result;
			if (effect != null)
			{
				effect.m_probability = ((this.m_probability > effect.m_probability) ? this.m_probability : effect.m_probability);
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
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				this.IsTrigger = true;
				living.DefenceEffectTrigger = true;
				damageAmount = damageAmount * (100 - this.m_count) / 100;
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AvoidDamageEffect.msg", new object[0]), 9, delay, 1000));
			}
		}
	}
}
