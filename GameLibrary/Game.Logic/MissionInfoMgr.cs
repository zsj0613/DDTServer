using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public static class MissionInfoMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, MissionInfo> m_missionInfos = new Dictionary<int, MissionInfo>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static Dictionary<int, MissionInfo> MissionInfos
		{
			get
			{
				return MissionInfoMgr.m_missionInfos;
			}
		}
		public static bool Init()
		{
			return MissionInfoMgr.Reload();
		}
		public static bool Reload()
		{
			bool result;
			try
			{
				Dictionary<int, MissionInfo> tempMissionInfo = MissionInfoMgr.LoadFromDatabase();
				if (tempMissionInfo.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, MissionInfo>>(ref MissionInfoMgr.m_missionInfos, tempMissionInfo);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				MissionInfoMgr.log.Error("MissionInfoMgr", e);
			}
			result = false;
			return result;
		}
		private static Dictionary<int, MissionInfo> LoadFromDatabase()
		{
			Dictionary<int, MissionInfo> dic = new Dictionary<int, MissionInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				MissionInfo[] infos = db.GetAllMissionInfo();
				MissionInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					MissionInfo info = array[i];
					if (!dic.ContainsKey(info.Id))
					{
						dic.Add(info.Id, info);
					}
				}
			}
			return dic;
		}
		public static MissionInfo GetMissionInfo(int id)
		{
			MissionInfo result;
			if (MissionInfoMgr.m_missionInfos.ContainsKey(id))
			{
				result = MissionInfoMgr.m_missionInfos[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
