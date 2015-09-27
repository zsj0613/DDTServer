using System;
using System.Threading;
namespace Bussiness
{
	public class ThreadSafeRandom
	{
		private Random random = new Random();
		public int Next()
		{
			Random obj;
			Monitor.Enter(obj = this.random);
			int result;
			try
			{
				result = this.random.Next();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public int Next(int maxValue)
		{
			Random obj;
			Monitor.Enter(obj = this.random);
			int result;
			try
			{
				result = this.random.Next(maxValue);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public int Next(int minValue, int maxValue)
		{
			Random obj;
			Monitor.Enter(obj = this.random);
			int result;
			try
			{
				result = this.random.Next(minValue, maxValue);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
	}
}
