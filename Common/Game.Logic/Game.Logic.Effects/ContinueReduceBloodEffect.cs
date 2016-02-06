using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class ContinueReduceBloodEffect : AbstractEffect
	{
		private int m_count;
		private int m_turn;
		private Living m_player;
		public ContinueReduceBloodEffect(int count, int turnCount, Living player) : base(eEffectType.ContinueReduceBloodEffect)
		{
			this.m_count = count;
			this.m_turn = turnCount;
			this.m_player = player;
		}
		public override bool Start(Living living)
		{
			ContinueReduceBloodEffect effect = living.EffectList.GetOfType(eEffectType.ContinueReduceBloodEffect) as ContinueReduceBloodEffect;
			bool result;
			if (effect != null)
			{
				effect.m_turn = this.m_turn;
				result = true;
			}
			else
			{
				result = base.Start(living);
			}
			return result;
		}
		public override void OnAttached(Living living)
		{
			living.BeginSelfTurn += new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 2, true);
		}
		public override void OnRemoved(Living living)
		{
			living.BeginSelfTurn -= new LivingEventHandle(this.player_BeginFitting);
			living.Game.SendPlayerPicture(living, 2, false);
		}
		private void player_BeginFitting(Living living)
		{
			this.m_turn--;
			if (this.m_turn < 0)
			{
				this.Stop();
			}
			else
			{
				living.AddBlood(-this.m_count, 1);
				if (living.Blood <= 0)
				{
					living.Die();
					if (this.m_player != null && this.m_player is Player)
					{
						(this.m_player as Player).PlayerDetail.OnKillingLiving(this.m_player.Game, 2, living.Id, living.IsLiving, this.m_count, (this.m_player as Player).PlayerDetail.IsArea);
					}
				}
			}
		}
	}
}
