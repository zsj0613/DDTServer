using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class AddBloodEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public AddBloodEffect(int count, int probability) : base(eEffectType.AddBloodEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddBloodEffect effect = living.EffectList.GetOfType(eEffectType.AddBloodEffect) as AddBloodEffect;
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
			player.BeforePlayerShoot += new PlayerShootEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
		}
		public void ChangeProperty(Living living, int ball)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				this.IsTrigger = true;
				living.AttackEffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(this.m_count);
				living.SyncAtTime = false;
			}
		}
	}
}
