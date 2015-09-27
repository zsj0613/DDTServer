using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public static class NPCInfoMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, NpcInfo> m_npcs = new Dictionary<int, NpcInfo>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		private static ThreadSafeRandom m_rand = new ThreadSafeRandom();
		public static bool Init()
		{
			return NPCInfoMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, NpcInfo> tempNpc = NPCInfoMgr.LoadFromDatabase();
				if (tempNpc != null && tempNpc.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, NpcInfo>>(ref NPCInfoMgr.m_npcs, tempNpc);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				NPCInfoMgr.log.Error("NPCInfoMgr", e);
			}
			result = false;
			return result;
		}
		private static Dictionary<int, NpcInfo> LoadFromDatabase()
		{
			Dictionary<int, NpcInfo> list = new Dictionary<int, NpcInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				NpcInfo[] infos = db.GetAllNPCInfo();
				NpcInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					NpcInfo info = array[i];
					if (!list.ContainsKey(info.ID))
					{
						list.Add(info.ID, info);
					}
				}
			}
			return list;
		}
		public static NpcInfo GetNpcInfoById(int id)
		{
			NpcInfo result;
			if (NPCInfoMgr.m_npcs.ContainsKey(id))
			{
				result = NPCInfoMgr.m_npcs[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
