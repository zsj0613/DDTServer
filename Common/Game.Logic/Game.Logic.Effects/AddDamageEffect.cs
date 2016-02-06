using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class AddDamageEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public AddDamageEffect(int count, int probability) : base(eEffectType.AddDamageEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddDamageEffect effect = living.EffectList.GetOfType(eEffectType.AddDamageEffect) as AddDamageEffect;
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
			player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.BeforePlayerShoot += new PlayerShootEventHandle(this.playerShot);
			player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_AfterPlayerShooted(Player player, int delay)
		{
			player.FlyingPartical = 0;
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.playerShot);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			if (this.IsTrigger)
			{
				damageAmount += this.m_count;
			}
		}
		private void playerShot(Player player, int ball)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				player.FlyingPartical = 65;
				this.IsTrigger = true;
				player.AttackEffectTrigger = true;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddDamageEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
