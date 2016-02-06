
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public static class ActiveMgr
	{
        private static LogProvider log => LogProvider.Default;
		public static Dictionary<int, List<ActiveConditionInfo>> m_ActiveConditionInfo = new Dictionary<int, List<ActiveConditionInfo>>();
		public static Dictionary<int, ActiveAwardInfo> m_ActiveAwardInfo = new Dictionary<int, ActiveAwardInfo>();
		public static bool Init()
		{
			return ActiveMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, List<ActiveConditionInfo>> tempActiveConditionInfo = ActiveMgr.LoadActiveConditionDb();
				Dictionary<int, ActiveAwardInfo> tempActiveAwardInfo = ActiveMgr.LoadActiveAwardDb(tempActiveConditionInfo);
				if (tempActiveConditionInfo.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, List<ActiveConditionInfo>>>(ref ActiveMgr.m_ActiveConditionInfo, tempActiveConditionInfo);
					Interlocked.Exchange<Dictionary<int, ActiveAwardInfo>>(ref ActiveMgr.m_ActiveAwardInfo, tempActiveAwardInfo);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				ActiveMgr.log.Error("QuestMgr", e);
			}
			result = false;
			return result;
		}
		public static Dictionary<int, List<ActiveConditionInfo>> LoadActiveConditionDb()
		{
			Dictionary<int, List<ActiveConditionInfo>> list = new Dictionary<int, List<ActiveConditionInfo>>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ActiveConditionInfo[] infos = db.GetAllActiveConditionInfo();
				ActiveConditionInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					ActiveConditionInfo info = array[i];
					List<ActiveConditionInfo> temp = new List<ActiveConditionInfo>();
					if (!list.ContainsKey(info.ActiveID))
					{
						temp.Add(info);
						list.Add(info.ActiveID, temp);
					}
					else
					{
						list[info.ActiveID].Add(info);
					}
				}
			}
			return list;
		}
		public static Dictionary<int, ActiveAwardInfo> LoadActiveAwardDb(Dictionary<int, List<ActiveConditionInfo>> conditions)
		{
			Dictionary<int, ActiveAwardInfo> list = new Dictionary<int, ActiveAwardInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ActiveAwardInfo[] infos = db.GetAllActiveAwardInfo();
				foreach (int key in conditions.Keys)
				{
					ActiveAwardInfo[] array = infos;
					for (int i = 0; i < array.Length; i++)
					{
						ActiveAwardInfo info = array[i];
						if (key == info.ActiveID && !list.ContainsKey(info.ID))
						{
							list.Add(info.ID, info);
						}
					}
				}
			}
			return list;
		}
		public static bool IsValid(ActiveConditionInfo info)
		{
			//DateTime arg_07_0 = info.StartTime;
			//DateTime arg_0E_0 = info.EndTime;
			//bool flag = 0 == 0;
			return info.StartTime.Ticks <= DateTime.Now.Ticks && info.EndTime.Ticks >= DateTime.Now.Ticks;
		}
		public static List<ActiveAwardInfo> GetAwardInfo(DateTime lastDate, int playerGrade)
		{
			string itemIds = null;
			int days = (DateTime.Now - lastDate).Days;
			if (DateTime.Now.DayOfYear > lastDate.DayOfYear)
			{
				days++;
			}
			List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
			foreach (List<ActiveConditionInfo> infos in ActiveMgr.m_ActiveConditionInfo.Values)
			{
				foreach (ActiveConditionInfo info in infos)
				{
					if (ActiveMgr.IsValid(info) && ActiveMgr.IsInGrade(info.LimitGrade, playerGrade) && info.Condition <= days)
					{
						itemIds = info.AwardId;
						int activeId = info.ActiveID;
					}
				}
			}
			if (!string.IsNullOrEmpty(itemIds))
			{
				string[] itemArray = itemIds.Split(new char[]
				{
					','
				});
				string[] array = itemArray;
				for (int i = 0; i < array.Length; i++)
				{
					string item = array[i];
					if (!string.IsNullOrEmpty(item))
					{
						list.Add(ActiveMgr.m_ActiveAwardInfo[Convert.ToInt32(item)]);
					}
				}
			}
			return list;
		}
		private static bool IsInGrade(string limitGrade, int playerGrade)
		{
			bool result = false;
			int minGrad = 0;
			int maxGrad = 0;
			if (limitGrade != null)
			{
				string[] strs = limitGrade.Split(new char[]
				{
					'-'
				});
				if (strs.Length == 2)
				{
					minGrad = Convert.ToInt32(strs[0]);
					maxGrad = Convert.ToInt32(strs[1]);
				}
				if (minGrad <= playerGrade && maxGrad >= playerGrade)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
