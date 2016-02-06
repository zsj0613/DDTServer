using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class FatalEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public FatalEffect(int count, int probability) : base(eEffectType.FatalEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			FatalEffect effect = living.EffectList.GetOfType(eEffectType.FatalEffect) as FatalEffect;
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
			player.BeforePlayerShoot += new PlayerShootEventHandle(this.ChangeProperty);
			player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_AfterPlayerShooted(Player player, int delay)
		{
			this.IsTrigger = false;
			player.GemControlBall = false;
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			if (this.IsTrigger && living is Player)
			{
				damageAmount = damageAmount * (100 - this.m_count) / 100;
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
			player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void ChangeProperty(Player player, int ball)
		{
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				player.ShootMovieDelay = 50;
				this.IsTrigger = true;
				if (player.CurrentBall.ID != 3)
				{
					player.GemControlBall = true;
				}
				player.AttackEffectTrigger = true;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("FatalEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
