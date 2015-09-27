using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;
using Game.Language;
namespace Game.Logic.Effects
{
	public class AddBombEquipEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		public AddBombEquipEffect(int count, int probability) : base(eEffectType.AddBombEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AddBombEquipEffect effect = living.EffectList.GetOfType(eEffectType.AddBombEquipEffect) as AddBombEquipEffect;
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
			player.BeginAttacking += new LivingEventHandle(this.ChangeProperty);
			player.BeforePlayerShoot += new PlayerShootEventHandle(this.playerShot);
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeginAttacking -= new LivingEventHandle(this.ChangeProperty);
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.playerShot);
		}
		private void ChangeProperty(Living player)
		{
			this.IsTrigger = false;
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
			{
				if (player is Player && (player as Player).CurrentBall.ID != 3)
				{
					this.IsTrigger = true;
					(player as Player).ShootCount += this.m_count;
					player.AttackEffectTrigger = true;
				}
			}
		}
		private void playerShot(Player player, int ball)
		{
			if (this.IsTrigger && player.CurrentBall.ID != 1 && player.CurrentBall.ID != 64 && player.CurrentBall.ID != 5)
			{
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddBombEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
