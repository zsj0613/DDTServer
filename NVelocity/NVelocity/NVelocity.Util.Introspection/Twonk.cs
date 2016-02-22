using System;

namespace NVelocity.Util.Introspection
{
	internal class Twonk
	{
        //public int distance;

        public int[] vec;

		public Twonk(int size)
		{
			this.vec = new int[size];
		}

		public int moreSpecific(Twonk other)
		{
			int result;
			if (other.vec.Length != this.vec.Length)
			{
				result = -1;
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < this.vec.Length; i++)
				{
					if (this.vec[i] > other.vec[i])
					{
						flag2 = true;
					}
					else if (this.vec[i] < other.vec[i])
					{
						flag = true;
					}
				}
				if (flag2 && flag)
				{
					result = 0;
				}
				else if (flag2 && !flag)
				{
					result = -1;
				}
				else if (!flag2 && flag)
				{
					result = 1;
				}
				else
				{
					result = 1;
				}
			}
			return result;
		}
	}
}
