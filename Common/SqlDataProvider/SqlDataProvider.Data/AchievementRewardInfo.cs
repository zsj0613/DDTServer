using System;
namespace SqlDataProvider.Data
{
	public class AchievementRewardInfo : DataObject
	{
		public int AchievementID
		{
			get;
			set;
		}
		public int RewardType
		{
			get;
			set;
		}
		public string RewardPara
		{
			get;
			set;
		}
		public int RewardValueId
		{
			get;
			set;
		}
		public int RewardCount
		{
			get;
			set;
		}
	}
}
