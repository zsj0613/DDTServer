using System;
namespace SqlDataProvider.Data
{
	public class DropInfo
	{
		public int ID
		{
			get;
			set;
		}
		public int Time
		{
			get;
			set;
		}
		public int Count
		{
			get;
			set;
		}
		public int MaxCount
		{
			get;
			set;
		}
		public DropInfo(int id, int time, int count, int maxCount)
		{
			this.ID = id;
			this.Time = time;
			this.Count = count;
			this.MaxCount = maxCount;
		}
	}
}
