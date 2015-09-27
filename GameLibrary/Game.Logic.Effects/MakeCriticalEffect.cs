using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class MakeCriticalEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public MakeCriticalEffect(int count, int probability) : base(eEffectType.MakeCriticalEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			MakeCriticalEffect effect = living.EffectList.GetOfType(eEffectType.MakeCriticalEffect) as MakeCriticalEffect;
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
			player.BeforePlayerShoot += new PlayerShootEventHandle(this.player_BeforePlayerShoot);
			player.TakePlayerDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_BeforePlayerShoot(Player player, int ball)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				this.IsTrigger = true;
				player.AttackEffectTrigger = true;
				player.FlyingPartical = 65;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("MakeCriticalEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
		private void player_AfterPlayerShooted(Player player, int delay)
		{
			player.FlyingPartical = 0;
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.player_BeforePlayerShoot);
			player.TakePlayerDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			if (this.IsTrigger)
			{
				criticalAmount = (int)((0.5 + living.Lucky * 0.0003) * (double)damageAmount);
			}
		}
	}
}
