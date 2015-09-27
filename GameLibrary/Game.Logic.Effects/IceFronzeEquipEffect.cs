using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class IceFronzeEquipEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public IceFronzeEquipEffect(int count, int probability) : base(eEffectType.IceFronzeEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			IceFronzeEquipEffect effect = living.EffectList.GetOfType(eEffectType.IceFronzeEquipEffect) as IceFronzeEquipEffect;
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
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
		}
		private void ChangeProperty(Player player, int ball)
		{
			if (player.CurrentBall.ID != 1 && player.CurrentBall.ID != 3 && player.CurrentBall.ID != 5)
			{
				if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
				{
					player.AttackEffectTrigger = true;
					player.WillIceForonze = true;
					player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("IceFronzeEquipEffect.msg", new object[0]), 9, 0, 1000));
				}
			}
		}
	}
}
