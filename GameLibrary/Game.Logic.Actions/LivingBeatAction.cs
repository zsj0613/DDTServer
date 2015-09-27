using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingBeatAction : BaseAction
	{
		private Living m_living;
		private Living m_target;
		private int m_demageAmount;
		private int m_criticalAmount;
		private string m_action;
		private bool m_isdemage;
		public LivingBeatAction(Living living, Living target, int demageAmount, int criticalAmount, string action, int delay, bool isdemage) : base(delay)
		{
			this.m_living = living;
			this.m_target = target;
			this.m_demageAmount = demageAmount;
			this.m_criticalAmount = criticalAmount;
			this.m_action = action;
			this.m_isdemage = isdemage;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			this.m_target.SyncAtTime = false;
			try
			{
				this.m_target.OnMakeDamage(this.m_target);
				int totalDemageAmount = 0;
				if (this.m_isdemage)
				{
					this.m_target.IsFrost = false;
				}
				if (this.m_target.TakeDamage(this.m_living, ref this.m_demageAmount, ref this.m_criticalAmount, 0, 500))
				{
					totalDemageAmount = this.m_demageAmount + this.m_criticalAmount;
				}
				else
				{
					this.m_criticalAmount = 0;
				}
				game.SendLivingBeat(this.m_living, this.m_target, totalDemageAmount, this.m_criticalAmount, this.m_action);
				base.Finish(tick);
			}
			finally
			{
				this.m_target.SyncAtTime = true;
			}
		}
	}
}
