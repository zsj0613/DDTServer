using System;
namespace Game.Logic.Phy.Maths
{
	public class EulerVector
	{
		public float x0;
		public float x1;
		public float x2;
		public EulerVector(int x0, int x1, float x2)
		{
			this.x0 = (float)x0;
			this.x1 = (float)x1;
			this.x2 = x2;
		}
		public void clear()
		{
			this.x0 = 0f;
			this.x1 = 0f;
			this.x2 = 0f;
		}
		public void clearMotion()
		{
			this.x1 = 0f;
			this.x2 = 0f;
		}
		public void ComputeOneEulerStep(float m, float af, float f, float dt)
		{
			this.x2 = (f - af * this.x1) / m;
			this.x1 += this.x2 * dt;
			this.x0 += this.x1 * dt;
		}
		public string toString()
		{
			return string.Concat(new object[]
			{
				"x:",
				this.x0,
				",v:",
				this.x1,
				",a",
				this.x2
			});
		}
	}
}
