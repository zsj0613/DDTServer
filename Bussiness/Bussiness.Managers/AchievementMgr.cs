using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class AchievementMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, AchievementInfo> m_achievement = new Dictionary<int, AchievementInfo>();
		private static Dictionary<int, List<AchievementConditionInfo>> m_achievementCondition = new Dictionary<int, List<AchievementConditionInfo>>();
		private static Dictionary<int, List<AchievementRewardInfo>> m_achievementReward = new Dictionary<int, List<AchievementRewardInfo>>();
		private static Dictionary<int, List<ItemRecordTypeInfo>> m_itemRecordType = new Dictionary<int, List<ItemRecordTypeInfo>>();
		private static Hashtable m_distinctCondition = new Hashtable();
		private static Hashtable m_ItemRecordTypeInfo = new Hashtable();
		public static Hashtable ItemRecordType
		{
			get
			{
				return AchievementMgr.m_ItemRecordTypeInfo;
			}
		}
		public static Dictionary<int, AchievementInfo> Achievement
		{
			get
			{
				return AchievementMgr.m_achievement;
			}
		}
		public static bool Init()
		{
			return AchievementMgr.Reload();
		}
		public static bool Reload()
		{
			bool result;
			try
			{
				AchievementMgr.LoadItemRecordTypeInfoDB();
				Dictionary<int, AchievementInfo> tempAchievementInfo = AchievementMgr.LoadAchievementInfoDB();
				Dictionary<int, List<AchievementConditionInfo>> tempAchievementConditionInfo = AchievementMgr.LoadAchievementConditionInfoDB(tempAchievementInfo);
				Dictionary<int, List<AchievementRewardInfo>> tempAchievementRewardInfo = AchievementMgr.LoadAchievementRewardInfoDB(tempAchievementInfo);
				if (tempAchievementInfo.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, AchievementInfo>>(ref AchievementMgr.m_achievement, tempAchievementInfo);
					Interlocked.Exchange<Dictionary<int, List<AchievementConditionInfo>>>(ref AchievementMgr.m_achievementCondition, tempAchievementConditionInfo);
					Interlocked.Exchange<Dictionary<int, List<AchievementRewardInfo>>>(ref AchievementMgr.m_achievementReward, tempAchievementRewardInfo);
				}
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				AchievementMgr.log.Error("AchievementMgr", ex);
			}
			result = false;
			return result;
		}
		public static void LoadItemRecordTypeInfoDB()
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ItemRecordTypeInfo[] infos = db.GetAllItemRecordType();
				ItemRecordTypeInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					ItemRecordTypeInfo info = array[i];
					if (!AchievementMgr.m_ItemRecordTypeInfo.Contains(info.RecordID))
					{
						AchievementMgr.m_ItemRecordTypeInfo.Add(info.RecordID, info.Name);
					}
				}
			}
		}
		public static Dictionary<int, AchievementInfo> LoadAchievementInfoDB()
		{
			Dictionary<int, AchievementInfo> list = new Dictionary<int, AchievementInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				AchievementInfo[] infos = db.GetALlAchievement();
				AchievementInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					AchievementInfo info = array[i];
					if (!list.ContainsKey(info.ID))
					{
						list.Add(info.ID, info);
					}
				}
			}
			return list;
		}
		public static Dictionary<int, List<AchievementConditionInfo>> LoadAchievementConditionInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
		{
			Dictionary<int, List<AchievementConditionInfo>> list = new Dictionary<int, List<AchievementConditionInfo>>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				AchievementConditionInfo[] infos = db.GetALlAchievementCondition();
				foreach (AchievementInfo achievementInfo in achievementInfos.Values)
				{
					IEnumerable<AchievementConditionInfo> temp = 
						from s in infos
						where s.AchievementID == achievementInfo.ID
						select s;
					list.Add(achievementInfo.ID, temp.ToList<AchievementConditionInfo>());
					if (temp != null)
					{
						foreach (AchievementConditionInfo info in temp)
						{
							if (!AchievementMgr.m_distinctCondition.Contains(info.CondictionType))
							{
								AchievementMgr.m_distinctCondition.Add(info.CondictionType, info.CondictionType);
							}
						}
					}
				}
			}
			return list;
		}
		public static Dictionary<int, List<AchievementRewardInfo>> LoadAchievementRewardInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
		{
			Dictionary<int, List<AchievementRewardInfo>> list = new Dictionary<int, List<AchievementRewardInfo>>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				AchievementRewardInfo[] infos = db.GetALlAchievementReward();
				foreach (AchievementInfo achievementInfo in achievementInfos.Values)
				{
					IEnumerable<AchievementRewardInfo> temp = 
						from s in infos
						where s.AchievementID == achievementInfo.ID
						select s;
					list.Add(achievementInfo.ID, temp.ToList<AchievementRewardInfo>());
				}
			}
			return list;
		}
		public static AchievementInfo GetSingleAchievement(int id)
		{
			AchievementInfo result;
			if (AchievementMgr.m_achievement.ContainsKey(id))
			{
				result = AchievementMgr.m_achievement[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static List<AchievementConditionInfo> GetAchievementCondition(AchievementInfo info)
		{
			List<AchievementConditionInfo> result;
			if (AchievementMgr.m_achievementCondition.ContainsKey(info.ID))
			{
				List<AchievementConditionInfo> items = AchievementMgr.m_achievementCondition[info.ID];
				result = items;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static List<AchievementRewardInfo> GetAchievementReward(AchievementInfo info)
		{
			List<AchievementRewardInfo> result;
			if (AchievementMgr.m_achievementReward.ContainsKey(info.ID))
			{
				List<AchievementRewardInfo> items = AchievementMgr.m_achievementReward[info.ID];
				result = items;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
