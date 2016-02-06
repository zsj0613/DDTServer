using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class AssimilateBloodEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public AssimilateBloodEffect(int count, int probability) : base(eEffectType.AssimilateBloodEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AssimilateBloodEffect effect = living.EffectList.GetOfType(eEffectType.AssimilateBloodEffect) as AssimilateBloodEffect;
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
			player.AfterKillingLiving += new KillLivingEventHanlde(this.player_AfterKillingLiving);
			player.BeforePlayerShoot += new PlayerShootEventHandle(this.player_PlayerShoot);
		}
		private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount, int delay)
		{
			if (living.IsLiving)
			{
				if (this.IsTrigger)
				{
					living.AddBlood(damageAmount * this.m_count / 100);
					living.Game.SendGameUpdateHealth(living, 0, damageAmount * this.m_count / 100);
				}
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.AfterKillingLiving -= new KillLivingEventHanlde(this.player_AfterKillingLiving);
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.player_PlayerShoot);
		}
		private void player_PlayerShoot(Player player, int ball)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				this.IsTrigger = true;
				player.AttackEffectTrigger = true;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AssimilateBloodEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
