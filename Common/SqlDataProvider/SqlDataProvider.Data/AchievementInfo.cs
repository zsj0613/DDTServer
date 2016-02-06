using System;
namespace SqlDataProvider.Data
{
	public class AchievementInfo : DataObject
	{
		public int ID
		{
			get;
			set;
		}
		public int PlaceID
		{
			get;
			set;
		}
		public string Title
		{
			get;
			set;
		}
		public string Detail
		{
			get;
			set;
		}
		public int NeedMinLevel
		{
			get;
			set;
		}
		public int NeedMaxLevel
		{
			get;
			set;
		}
		public string PreAchievementID
		{
			get;
			set;
		}
		public int IsOther
		{
			get;
			set;
		}
		public int AchievementType
		{
			get;
			set;
		}
		public bool CanHide
		{
			get;
			set;
		}
		public DateTime StartDate
		{
			get;
			set;
		}
		public DateTime EndDate
		{
			get;
			set;
		}
		public int AchievementPoint
		{
			get;
			set;
		}
	}
}
