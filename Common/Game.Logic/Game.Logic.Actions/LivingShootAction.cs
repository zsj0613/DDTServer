using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Actions
{
	public class LivingShootAction : BaseAction
	{
		private Living m_living;
		private int m_tx;
		private int m_ty;
		private int m_bombId;
		private int m_force;
		private int m_angle;
		private int m_bombCount;
		private int m_minTime;
		private int m_maxTime;
		private float m_Time;
		public LivingShootAction(Living living, int bombId, int x, int y, int force, int angle, int bombCount, int minTime, int maxTime, float time, int delay) : base(delay, 1000)
		{
			this.m_living = living;
			this.m_bombId = bombId;
			this.m_tx = x;
			this.m_ty = y;
			this.m_force = force;
			this.m_angle = angle;
			this.m_bombCount = bombCount;
			this.m_bombId = bombId;
			this.m_minTime = minTime;
			this.m_maxTime = maxTime;
			this.m_Time = time;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!(this.m_living is Player))
			{
				this.m_living.GetShootForceAndAngle(ref this.m_tx, ref this.m_ty, this.m_bombId, this.m_minTime, this.m_maxTime, this.m_bombCount, this.m_Time, ref this.m_force, ref this.m_angle);
			}
			this.m_living.ShootImp(this.m_bombId, this.m_tx, this.m_ty, this.m_force, this.m_angle, this.m_bombCount);
			base.Finish(tick);
		}
	}
}
