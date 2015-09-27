using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public static class PveInfoMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, PveInfo> m_pveInfos = new Dictionary<int, PveInfo>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static bool Init()
		{
			return PveInfoMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, PveInfo> tempPve = PveInfoMgr.LoadFromDatabase();
				if (tempPve.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, PveInfo>>(ref PveInfoMgr.m_pveInfos, tempPve);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				PveInfoMgr.log.Error("PveInfoMgr", e);
			}
			result = false;
			return result;
		}
		public static Dictionary<int, PveInfo> LoadFromDatabase()
		{
			Dictionary<int, PveInfo> list = new Dictionary<int, PveInfo>();
			using (PveBussiness db = new PveBussiness())
			{
				PveInfo[] infos = db.GetAllPveInfos();
				PveInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					PveInfo info = array[i];
					if (!list.ContainsKey(info.ID))
					{
						list.Add(info.ID, info);
					}
				}
			}
			return list;
		}
		public static PveInfo GetPveInfoById(int id)
		{
			PveInfo result;
			if (PveInfoMgr.m_pveInfos.ContainsKey(id))
			{
				result = PveInfoMgr.m_pveInfos[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static PveInfo GetPveInfoByType(eRoomType roomType, int levelLimits)
		{
			PveInfo result;
			if (roomType == eRoomType.Boss || roomType == eRoomType.Treasure || roomType == eRoomType.Training)
			{
				foreach (PveInfo pveInfo in PveInfoMgr.m_pveInfos.Values)
				{
					if (pveInfo.Type == (int)roomType)
					{
						result = pveInfo;
						return result;
					}
				}
			}
			else
			{
				if (roomType == eRoomType.Exploration)
				{
					foreach (PveInfo pveInfo in PveInfoMgr.m_pveInfos.Values)
					{
						if (pveInfo.Type == (int)roomType && pveInfo.LevelLimits == levelLimits)
						{
							result = pveInfo;
							return result;
						}
					}
				}
			}
			result = null;
			return result;
		}
	}
}
