using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class NoHoleEquipEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public NoHoleEquipEffect(int count, int probability) : base(eEffectType.NoHoleEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			NoHoleEquipEffect effect = living.EffectList.GetOfType(eEffectType.NoHoleEquipEffect) as NoHoleEquipEffect;
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
			player.CollidByObject += new PlayerEventHandle(this.player_AfterKilledByLiving);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.CollidByObject -= new PlayerEventHandle(this.player_AfterKilledByLiving);
		}
		private void player_AfterKilledByLiving(Living living, int delay)
		{
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				living.DefenceEffectTrigger = true;
				new NoHoleEffect(1).Start(living);
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("NoHoleEquipEffect.msg", new object[0]), 9, delay, 1000));
			}
		}
	}
}
