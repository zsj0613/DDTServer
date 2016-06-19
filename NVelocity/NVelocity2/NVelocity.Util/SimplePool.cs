using System;

namespace NVelocity.Util
{
	public sealed class SimplePool
	{
		private object[] pool;

		private int max;

		private int current = -1;

		public int Max
		{
			get
			{
				return this.max;
			}
		}

		public SimplePool(int max)
		{
			this.max = max;
			this.pool = new object[max];
		}

		public void put(object o)
		{
			int num = -1;
			lock (this)
			{
				if (this.current < this.max - 1)
				{
					num = ++this.current;
				}
				if (num >= 0)
				{
					this.pool[num] = o;
				}
			}
		}

		public object get()
		{
			object result;
			lock (this)
			{
				if (this.current >= 0)
				{
					object obj = this.pool[this.current];
					this.pool[this.current] = null;
					this.current--;
					result = obj;
					return result;
				}
			}
			result = null;
			return result;
		}

		private object[] getPool()
		{
			return this.pool;
		}
	}
}
