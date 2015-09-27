using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class AssimilateDamageEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		private int m_turn = 5;
		public AssimilateDamageEffect(int count, int probability) : base(eEffectType.AssimilateDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_turn = 5;
		}
		public override bool Start(Living living)
		{
			AssimilateDamageEffect effect = living.EffectList.GetOfType(eEffectType.AssimilateDamageEffect) as AssimilateDamageEffect;
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
			player.BeginSelfTurn += new LivingEventHandle(this.player_BeginSelfTurn);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.BeginSelfTurn -= new LivingEventHandle(this.player_BeginSelfTurn);
		}
		protected void player_BeginSelfTurn(Living living)
		{
			this.m_turn++;
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000 && this.m_turn >= 5)
			{
				damageAmount = ((damageAmount > this.m_count) ? this.m_count : damageAmount);
				this.m_turn = 0;
				this.IsTrigger = true;
				living.DefenceEffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(damageAmount);
				living.SyncAtTime = false;
				damageAmount -= damageAmount;
				criticalAmount -= criticalAmount;
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AssimilateDamageEffect.msg", new object[0]), 9, delay, 1000));
			}
		}
	}
}
