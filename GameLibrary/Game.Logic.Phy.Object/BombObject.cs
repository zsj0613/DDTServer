using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using System;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
	public class BombObject : Physics
	{
		private float m_mass;
		private float m_gravityFactor;
		private float m_windFactor;
		private float m_airResitFactor;
		private EulerVector m_vx;
		private EulerVector m_vy;
		private float m_arf;
		private float m_gf;
		private float m_wf;
		public float vX
		{
			get
			{
				return this.m_vx.x1;
			}
		}
		public float vY
		{
			get
			{
				return this.m_vy.x1;
			}
		}
		public float Arf
		{
			get
			{
				return this.m_arf;
			}
		}
		public float Gf
		{
			get
			{
				return this.m_gf;
			}
		}
		public float Wf
		{
			get
			{
				return this.m_wf;
			}
		}
		public BombObject(int id, float mass, float gravityFactor, float windFactor, float airResitFactor) : base(id)
		{
			this.m_mass = mass;
			this.m_gravityFactor = gravityFactor;
			this.m_windFactor = windFactor;
			this.m_airResitFactor = airResitFactor;
			this.m_vx = new EulerVector(0, 0, 0f);
			this.m_vy = new EulerVector(0, 0, 0f);
			this.m_rect = new Rectangle(-3, -3, 6, 6);
		}
		public void setSpeedXY(int vx, int vy)
		{
			this.m_vx.x1 = (float)vx;
			this.m_vy.x1 = (float)vy;
		}
		public override void SetXY(int x, int y)
		{
			base.SetXY(x, y);
			this.m_vx.x0 = (float)x;
			this.m_vy.x0 = (float)y;
		}
		public override void SetMap(Map map)
		{
			base.SetMap(map);
			this.UpdateAGW();
		}
		protected void UpdateForceFactor(float air, float gravity, float wind)
		{
			this.m_airResitFactor = air;
			this.m_gravityFactor = gravity;
			this.m_windFactor = wind;
			this.UpdateAGW();
		}
		private void UpdateAGW()
		{
			if (this.m_map != null)
			{
				this.m_arf = this.m_map.airResistance * this.m_airResitFactor;
				this.m_gf = this.m_map.gravity * this.m_gravityFactor * this.m_mass;
				this.m_wf = this.m_map.wind * this.m_windFactor;
			}
		}
		protected Point CompleteNextMovePoint(float dt)
		{
			this.m_vx.ComputeOneEulerStep(this.m_mass, this.m_arf, this.m_wf, dt);
			this.m_vy.ComputeOneEulerStep(this.m_mass, this.m_arf, this.m_gf, dt);
			return new Point((int)this.m_vx.x0, (int)this.m_vy.x0);
		}
		public void MoveTo(int px, int py)
		{
			if (px != this.m_x || py != this.m_y)
			{
				int dx = px - this.m_x;
				int dy = py - this.m_y;
				bool useX;
				int count;
				int dt;
				if (Math.Abs(dx) > Math.Abs(dy))
				{
					useX = true;
					count = Math.Abs(dx);
					dt = dx / count;
				}
				else
				{
					useX = false;
					count = Math.Abs(dy);
					dt = dy / count;
				}
				Point dest = new Point(this.m_x, this.m_y);
				for (int i = 1; i <= count; i += 3)
				{
					if (useX)
					{
						dest = this.GetNextPointByX(this.m_x, px, this.m_y, py, this.m_x + i * dt);
					}
					else
					{
						dest = this.GetNextPointByY(this.m_x, px, this.m_y, py, this.m_y + i * dt);
					}
					Rectangle rect = this.m_rect;
					rect.Offset(dest.X, dest.Y);
					Physics[] list = this.m_map.FindPhysicalObjects(rect, this);
					if (list.Length > 0)
					{
						base.SetXY(dest.X, dest.Y);
						this.CollideObjects(list);
					}
					else
					{
						if (!this.m_map.IsRectangleEmpty(rect))
						{
							base.SetXY(dest.X, dest.Y);
							this.CollideGround();
						}
						else
						{
							if (this.m_map.IsOutMap(dest.X, dest.Y))
							{
								base.SetXY(dest.X, dest.Y);
								this.FlyoutMap();
							}
						}
					}
					if (!this.m_isLiving || !this.m_isMoving)
					{
						return;
					}
				}
				base.SetXY(px, py);
			}
		}
		protected virtual void CollideObjects(Physics[] list)
		{
		}
		protected virtual void CollideGround()
		{
			this.StopMoving();
		}
		protected virtual void FlyoutMap()
		{
			this.StopMoving();
			if (this.m_isLiving)
			{
				this.Die();
			}
		}
		private Point GetNextPointByX(int x1, int x2, int y1, int y2, int x)
		{
			Point result;
			if (x2 == x1)
			{
				result = new Point(x, y1);
			}
			else
			{
				result = new Point(x, (x - x1) * (y2 - y1) / (x2 - x1) + y1);
			}
			return result;
		}
		private Point GetNextPointByY(int x1, int x2, int y1, int y2, int y)
		{
			Point result;
			if (y2 == y1)
			{
				result = new Point(x1, y);
			}
			else
			{
				result = new Point((y - y1) * (x2 - x1) / (y2 - y1) + x1, y);
			}
			return result;
		}
	}
}
