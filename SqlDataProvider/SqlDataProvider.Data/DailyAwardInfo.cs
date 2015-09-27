using System;
namespace SqlDataProvider.Data
{
	public class DailyAwardInfo
	{
		public int ID
		{
			get;
			set;
		}
		public int Type
		{
			get;
			set;
		}
		public int TemplateID
		{
			get;
			set;
		}
		public int Count
		{
			get;
			set;
		}
		public int ValidDate
		{
			get;
			set;
		}
		public bool IsBinds
		{
			get;
			set;
		}
		public int Sex
		{
			get;
			set;
		}
		public string Remark
		{
			get;
			set;
		}
		public string CountRemark
		{
			get;
			set;
		}
		public int GetWay
		{
			get;
			set;
		}
	}
}
