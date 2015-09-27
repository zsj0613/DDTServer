using System;
namespace SqlDataProvider.Data
{
	public class ActiveConditionInfo
	{
		public int ID
		{
			get;
			set;
		}
		public int ActiveID
		{
			get;
			set;
		}
		public int Conditiontype
		{
			get;
			set;
		}
		public int Condition
		{
			get;
			set;
		}
		public string LimitGrade
		{
			get;
			set;
		}
		public string AwardId
		{
			get;
			set;
		}
		public bool IsMult
		{
			get;
			set;
		}
		public DateTime StartTime
		{
			get;
			set;
		}
		public DateTime EndTime
		{
			get;
			set;
		}
	}
}
