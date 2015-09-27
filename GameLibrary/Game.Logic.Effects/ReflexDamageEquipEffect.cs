using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class ReflexDamageEquipEffect : BasePlayerEffect
	{
		private int m_count;
		private int m_probability;
		public ReflexDamageEquipEffect(int count, int probability) : base(eEffectType.ReflexDamageEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			ReflexDamageEquipEffect effect = living.EffectList.GetOfType(eEffectType.ReflexDamageEquipEffect) as ReflexDamageEquipEffect;
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
		}
		private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
		{
			if (this.IsTrigger)
			{
				target.SyncAtTime = true;
				target.AddBlood(-this.m_count, 1);
				target.SyncAtTime = false;
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
		}
		public void ChangeProperty(Living living)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				this.IsTrigger = true;
				living.DefenceEffectTrigger = true;
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("ReflexDamageEquipEffect.msg", new object[0]), 9, 1000, 1000));
			}
		}
	}
}
