using System;
namespace Game.Logic.Phy.Object
{
	public class CanShootInfo
	{
		private bool m_canShoot;
		private int m_force;
		private int m_angle;
		public bool CanShoot
		{
			get
			{
				return this.m_canShoot;
			}
		}
		public int Force
		{
			get
			{
				return this.m_force;
			}
		}
		public int Angle
		{
			get
			{
				return this.m_angle;
			}
		}
		public CanShootInfo(bool canShoot, int force, int angle)
		{
			this.m_canShoot = canShoot;
			this.m_force = force;
			this.m_angle = angle;
		}
	}
}
