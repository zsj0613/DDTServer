using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class BoxMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, BoxInfo> m_BoxInfo;
		private static ReaderWriterLock m_lock;
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, BoxInfo> tempTimeBoxInfo = new Dictionary<int, BoxInfo>();
				if (BoxMgr.LoadStrengthen(tempTimeBoxInfo))
				{
					BoxMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						BoxMgr.m_BoxInfo = tempTimeBoxInfo;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						BoxMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (BoxMgr.log.IsErrorEnabled)
				{
					BoxMgr.log.Error("BoxMgr", e);
				}
			}
			result = false;
			return result;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				BoxMgr.m_lock = new ReaderWriterLock();
				BoxMgr.m_BoxInfo = new Dictionary<int, BoxInfo>();
				result = BoxMgr.LoadStrengthen(BoxMgr.m_BoxInfo);
			}
			catch (Exception e)
			{
				if (BoxMgr.log.IsErrorEnabled)
				{
					BoxMgr.log.Error("BoxMgr", e);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadStrengthen(Dictionary<int, BoxInfo> m_TimeBoxInfo)
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				List<BoxInfo> infos = new List<BoxInfo>();
				infos = db.GetAllBoxInfo();
				foreach (BoxInfo info in infos)
				{
					if (!m_TimeBoxInfo.ContainsKey(info.ID))
					{
						m_TimeBoxInfo.Add(info.ID, info);
					}
				}
			}
			return true;
		}
		public static BoxInfo FindTemplateByCondition(int type, int level, int condition)
		{
			BoxInfo result;
			foreach (KeyValuePair<int, BoxInfo> info in BoxMgr.m_BoxInfo)
			{
				if (type == 0)
				{
					if (type == info.Value.Type && level <= info.Value.Level && condition < info.Value.Condition)
					{
						result = info.Value;
						return result;
					}
				}
				else
				{
					if (type == info.Value.Type && level < info.Value.Level && condition == info.Value.Condition)
					{
						result = info.Value;
						return result;
					}
				}
			}
			result = null;
			return result;
		}
	}
}
