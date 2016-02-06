using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class AtomBombEquipEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		private int m_oldBall = -1;
		public AtomBombEquipEffect(int count, int probability) : base(eEffectType.AtomBomb)
		{
			this.m_count = count;
			this.m_probability = probability;
		}
		public override bool Start(Living living)
		{
			AtomBombEquipEffect effect = living.EffectList.GetOfType(eEffectType.AtomBomb) as AtomBombEquipEffect;
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
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
			player.AfterPlayerShooted -= new PlayerEventHandle(this.player_AfterPlayerShooted);
		}
		private void player_AfterPlayerShooted(Player player, int delay)
		{
			if (this.m_oldBall != -1)
			{
				if (BallMgr.IsExist(this.m_oldBall))
				{
					player.CurrentBall = BallMgr.FindBall(this.m_oldBall);
					player.Game.SendGameUpdateBall(player, false);
					this.m_oldBall = -1;
				}
			}
		}
		private void ChangeProperty(Player player, int ball)
		{
			if (player.CurrentBall.ID != 1 && player.CurrentBall.ID != 3 && player.CurrentBall.ID != 5)
			{
				if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000)
				{
					player.AttackEffectTrigger = true;
					player.CurrentBall = BallMgr.FindBall(4);
					player.Game.SendGameUpdateBall(player, false);
					this.m_oldBall = ball;
					player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AtomBombEquipEffect.msg", new object[0]), 9, 0, 1000));
				}
			}
		}
	}
}
