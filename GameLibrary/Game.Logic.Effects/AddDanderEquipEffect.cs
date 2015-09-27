using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class AddDanderEquipEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public AddDanderEquipEffect(int count, int probability) : base(eEffectType.AddDander)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddDanderEquipEffect effect = living.EffectList.GetOfType(eEffectType.AddDander) as AddDanderEquipEffect;
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
			player.BeforeTakeDamage += new LivingTakedDamageEventHandle(this.ChangeProperty);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforeTakeDamage -= new LivingTakedDamageEventHandle(this.ChangeProperty);
		}
		private void ChangeProperty(Living player, Living source, ref int damageAmount, ref int criticalAmount, int delay)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				if (player is Player)
				{
					(player as Player).AddDander(this.m_count);
				}
				this.IsTrigger = true;
				player.DefenceEffectTrigger = true;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddDanderEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
