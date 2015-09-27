using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingFallingAction : BaseAction
	{
		private Living m_living;
		private int m_fallSpeed;
		private int m_toX;
		private int m_toY;
		private bool m_isSent;
		private string m_action;
		private int m_type;
		private LivingCallBack m_callback;
		public LivingFallingAction(Living living, int toX, int toY, int speed, string action, int delay, int type, LivingCallBack callback) : base(delay, 2000)
		{
			this.m_living = living;
			this.m_fallSpeed = speed;
			this.m_action = action;
			this.m_toX = toX;
			this.m_toY = toY;
			this.m_isSent = false;
			this.m_type = type;
			this.m_callback = callback;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!this.m_isSent)
			{
				this.m_isSent = true;
				game.SendLivingFall(this.m_living, this.m_toX, this.m_toY, this.m_fallSpeed, this.m_action, this.m_type);
			}
			if (this.m_toY > this.m_living.Y + this.m_fallSpeed)
			{
				this.m_living.SetXY(this.m_toX, this.m_living.Y + this.m_fallSpeed);
			}
			else
			{
				this.m_living.SetXY(this.m_toX, this.m_toY);
				if (game.Map.IsOutMap(this.m_toX, this.m_toY))
				{
					this.m_living.SyncAtTime = false;
					this.m_living.Die();
				}
				if (this.m_callback != null)
				{
					this.m_living.CallFuction(this.m_callback, 0);
				}
				base.Finish(tick);
			}
		}
	}
}
