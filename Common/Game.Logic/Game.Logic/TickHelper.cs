using System;
using System.Diagnostics;
namespace Game.Logic
{
	public static class TickHelper
	{
		private static long StopwatchFrequencyMilliseconds = Stopwatch.Frequency / 1000L;
		public static long GetTickCount()
		{
			return Stopwatch.GetTimestamp() / TickHelper.StopwatchFrequencyMilliseconds;
		}
	}
}
