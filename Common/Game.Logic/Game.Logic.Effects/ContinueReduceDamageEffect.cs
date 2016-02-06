using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ContinueReduceDamageEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_turn = 0;
		public ContinueReduceDamageEffect(int count, int turn) : base(eEffectType.ContinueReduceBloodEffect)
		{
			this.m_count = count;
			this.m_turn = turn;
		}
		public override bool Start(Living living)
		{
			ContinueReduceDamageEffect effect = living.EffectList.GetOfType(eEffectType.ContinueReduceDamageEffect) as ContinueReduceDamageEffect;
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
		protected override void OnAttachedToPlayer(Player player)
		{
			player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			player.Game.SendPlayerPicture(player, 6, true);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.player_BeforeTakeDamage);
			player.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			player.Game.SendPlayerPicture(player, 6, false);
		}
		public void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			if (damageAmount > 1)
			{
				if ((damageAmount -= this.m_count) <= 0)
				{
					damageAmount = 1;
				}
			}
		}
		public void player_BeginFitting(Living living)
		{
			this.m_turn--;
			if (this.m_turn <= 0)
			{
				this.Stop();
			}
		}
	}
}
