using Bussiness;
using Bussiness.Managers;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using Game.Logic.Spells;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class ArmorPiercerEquipEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		private bool EffectTrigger = false;
		public ArmorPiercerEquipEffect(int count, int probability) : base(eEffectType.ArmorPiercer)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			ArmorPiercerEquipEffect effect = living.EffectList.GetOfType(eEffectType.ArmorPiercer) as ArmorPiercerEquipEffect;
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
			if (this.EffectTrigger)
			{
				player.FlyingPartical = 0;
				player.IgnoreArmor = false;
				this.EffectTrigger = false;
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void ChangeProperty(Player player, int ball)
		{
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000 && !player.AttackEffectTrigger)
			{
				this.EffectTrigger = true;
				player.FlyingPartical = 65;
				SpellMgr.ExecuteSpell(player.Game, player, ItemMgr.FindItemTemplate(10020));
				player.AttackEffectTrigger = true;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("ArmorPiercerEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
