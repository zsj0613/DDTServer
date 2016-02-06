using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class AddLuckyEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		private int m_added = 0;
		public AddLuckyEffect(int count, int probability) : base(eEffectType.AddLuckyEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_added = 0;
		}
		public override bool Start(Living living)
		{
			AddLuckyEffect effect = living.EffectList.GetOfType(eEffectType.AddLuckyEffect) as AddLuckyEffect;
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
			player.AfterPlayerShooted += new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_AfterPlayerShooted(Player player, int delay)
		{
			player.FlyingPartical = 0;
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void ChangeProperty(Player player, int ball)
		{
			player.Lucky -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				player.FlyingPartical = 65;
				this.IsTrigger = true;
				player.Lucky += (double)this.m_count;
				player.AttackEffectTrigger = true;
				this.m_added = this.m_count;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddLuckyEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
