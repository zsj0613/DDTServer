using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class WaitLivingAttackingAction : BaseAction
	{
		private TurnedLiving m_living;
		private int m_turnIndex;
		public WaitLivingAttackingAction(TurnedLiving living, int turnIndex, int delay) : base(delay)
		{
			this.m_living = living;
			this.m_turnIndex = turnIndex;
			living.EndAttacking += new LivingEventHandle(this.player_EndAttacking);
		}
		private void player_EndAttacking(Living player)
		{
			player.EndAttacking -= new LivingEventHandle(this.player_EndAttacking);
			base.Finish(TickHelper.GetTickCount());
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			base.Finish(tick);
			if (game.TurnIndex == this.m_turnIndex && this.m_living.IsAttacking)
			{
				this.m_living.StopAttacking();
				game.CheckState(0);
			}
		}
	}
}
