using System;
namespace Game.Server.Quests
{
	public class TimeHelper
	{
		public static int GetDaysBetween(DateTime min, DateTime max)
		{
			int minday = (int)Math.Floor((min - DateTime.MinValue).TotalDays);
			int maxday = (int)Math.Floor((max - DateTime.MinValue).TotalDays);
			return maxday - minday;
		}
	}
}
