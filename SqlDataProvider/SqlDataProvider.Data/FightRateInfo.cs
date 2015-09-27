using System;
namespace SqlDataProvider.Data
{
	public class FightRateInfo
	{
		public int ID
		{
			get;
			set;
		}
		public int ServerID
		{
			get;
			set;
		}
		public int Rate
		{
			get;
			set;
		}
		public DateTime BeginDay
		{
			get;
			set;
		}
		public DateTime EndDay
		{
			get;
			set;
		}
		public DateTime BeginTime
		{
			get;
			set;
		}
		public DateTime EndTime
		{
			get;
			set;
		}
		public int BoyTemplateID
		{
			get;
			set;
		}
		public int GirlTemplateID
		{
			get;
			set;
		}
		public string SelfCue
		{
			get;
			set;
		}
		public string EnemyCue
		{
			get;
			set;
		}
		public string Name
		{
			get;
			set;
		}
	}
}
