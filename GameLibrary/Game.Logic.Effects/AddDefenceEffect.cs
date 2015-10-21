using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class AddDefenceEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		private int m_added;
		public AddDefenceEffect(int count, int probability) : base(eEffectType.AddDefenceEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_added = 0;
		}
		public override bool Start(Living living)
		{
			AddDefenceEffect effect = living.EffectList.GetOfType(eEffectType.AddDefenceEffect) as AddDefenceEffect;
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
			player.PlayerMakeDamage += new LivingEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.PlayerMakeDamage -= new LivingEventHandle(this.ChangeProperty);
		}
		public void ChangeProperty(Living living)
		{
			living.Defence -= (double)this.m_added;
			this.m_added = 0;
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				this.IsTrigger = true;
				living.Defence += (double)this.m_count;
				this.m_added = this.m_count;
				living.DefenceEffectTrigger = true;
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AddDefenceEffect.msg", new object[0]), 9, 3000, 1000));
			}
		}
	}
}
