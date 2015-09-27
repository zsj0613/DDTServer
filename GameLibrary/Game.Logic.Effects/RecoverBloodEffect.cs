using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class RecoverBloodEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public RecoverBloodEffect(int count, int probability) : base(eEffectType.RecoverBloodEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			RecoverBloodEffect effect = living.EffectList.GetOfType(eEffectType.RecoverBloodEffect) as RecoverBloodEffect;
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
			player.AfterKilledByLiving += new KillLivingEventHanlde(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.AfterKilledByLiving -= new KillLivingEventHanlde(this.ChangeProperty);
		}
		public void ChangeProperty(Living living, Living target, int damageAmount, int criticalAmount, int delay)
		{
			if (living.IsLiving)
			{
				this.IsTrigger = false;
				if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
				{
					this.IsTrigger = true;
					living.DefenceEffectTrigger = true;
					living.SyncAtTime = true;
					living.AddBlood(this.m_count);
					living.SyncAtTime = false;
					living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("RecoverBloodEffect.msg", new object[0]), 9, delay, 1000));
				}
			}
		}
	}
}
