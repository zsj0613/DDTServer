using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using System;

namespace Game.Logic.Effects
{
	public class AddTurnEquipEffect : BasePlayerEffect
	{
		private int m_count = 0;
		private int m_probability = 0;
		private int m_turn = 5;
		private bool addDelay = false;
		private int m_templateid = 0;
		public AddTurnEquipEffect(int count, int probability, int templateid) : base(eEffectType.AddTurnEquipEffect)
		{
			this.m_count = count;
			this.m_probability = probability;
			this.m_turn = 5;
			this.m_templateid = templateid;
		}
		public override bool Start(Living living)
		{
			AddTurnEquipEffect effect = living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) as AddTurnEquipEffect;
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
			player.BeginNextTurn += new LivingEventHandle(this.player_BeginNewTurn);
			player.BeginSelfTurn += new LivingEventHandle(this.player_BeginSelfTurn);
		}
		protected void player_BeginSelfTurn(Living living)
		{
			this.m_turn++;
			if (this.m_turn == 1)
			{
				this.addDelay = true;
			}
		}
		protected override void OnRemovedFromPlayer(Player player)
		{
			player.BeforePlayerShoot -= new PlayerShootEventHandle(this.ChangeProperty);
			player.BeginNextTurn -= new LivingEventHandle(this.player_BeginNewTurn);
			player.BeginSelfTurn -= new LivingEventHandle(this.player_BeginSelfTurn);
		}
		public void player_BeginNewTurn(Living living)
		{
			if (this.IsTrigger && living is Player)
			{
				(living as Player).Delay = (living as Player).DefaultDelay;
				this.IsTrigger = false;
				int templateid = this.m_templateid;
				if (templateid != 311229)
				{
					if (templateid != 311312)
					{
						if (templateid != 311329)
						{
							if ((living as Player).Energy > 100)
							{
								(living as Player).Energy = 100;
							}
						}
						else
						{
							if ((living as Player).Energy > 250)
							{
								(living as Player).Energy = 250;
							}
						}
					}
					else
					{
						if ((living as Player).Energy > 210)
						{
							(living as Player).Energy = 210;
						}
					}
				}
				else
				{
					if ((living as Player).Energy > 160)
					{
						(living as Player).Energy = 160;
					}
				}
			}
			if (this.addDelay)
			{
				(living as Player).Delay += ((living as Player).Delay - (living as Player).DefaultDelay) * this.m_count / 100;
				this.addDelay = false;
			}
		}
		private void ChangeProperty(Player player, int ball)
		{
			if (AbstractEffect.random.Next(1000000) < this.m_probability * 10000 && this.m_turn >= 6)
			{
				this.m_turn = 0;
				this.IsTrigger = true;
				player.AttackEffectTrigger = true;
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.msg", new object[0]), 9, 0, 1000));
			}
		}
	}
}
