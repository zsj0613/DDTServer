using Bussiness.Protocol;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class DropMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static string[] m_DropTypes = Enum.GetNames(typeof(eDropType));
		private static List<DropCondiction> m_dropcondiction = new List<DropCondiction>();
		private static Dictionary<int, List<DropItem>> m_dropitem = new Dictionary<int, List<DropItem>>();
		public static bool Init()
		{
			return DropMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result = false;
			try
			{
				List<DropCondiction> tempDropCondiction = DropMgr.LoadDropConditionDb();
				Interlocked.Exchange<List<DropCondiction>>(ref DropMgr.m_dropcondiction, tempDropCondiction);
				Dictionary<int, List<DropItem>> tempDropItem = DropMgr.LoadDropItemDb();
				Interlocked.Exchange<Dictionary<int, List<DropItem>>>(ref DropMgr.m_dropitem, tempDropItem);
				if (tempDropCondiction.Count > 0 && tempDropItem.Count > 0)
				{
					result = true;
				}
				else
				{
					DropMgr.log.Warn("DropMgr didn't load any data!");
				}
			}
			catch (Exception e)
			{
				DropMgr.log.Error("DropMgr", e);
			}
			return result;
		}
		public static List<DropCondiction> LoadDropConditionDb()
		{
			List<DropCondiction> result;
			using (ProduceBussiness db = new ProduceBussiness())
			{
				DropCondiction[] infos = db.GetAllDropCondictions();
				result = ((infos != null) ? infos.ToList<DropCondiction>() : null);
			}
			return result;
		}
		public static Dictionary<int, List<DropItem>> LoadDropItemDb()
		{
			Dictionary<int, List<DropItem>> list = new Dictionary<int, List<DropItem>>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				DropItem[] infos = db.GetAllDropItems();
				foreach (DropCondiction info in DropMgr.m_dropcondiction)
				{
					IEnumerable<DropItem> temp = 
						from s in infos
						where s.DropId == info.DropId
						select s;
					list.Add(info.DropId, temp.ToList<DropItem>());
				}
			}
			return list;
		}
		public static int FindCondiction(eDropType type, string para1, string para2)
		{
			string temppara = "," + para1 + ",";
			string temppara2 = "," + para2 + ",";
			int result;
			foreach (DropCondiction drop in DropMgr.m_dropcondiction)
			{
				if (drop.CondictionType == (int)type && drop.Para1.IndexOf(temppara) != -1 && drop.Para2.IndexOf(temppara2) != -1)
				{
					result = drop.DropId;
					return result;
				}
			}
			result = 0;
			return result;
		}
		public static List<DropItem> FindDropItem(int dropId)
		{
			List<DropItem> result;
			if (DropMgr.m_dropitem.ContainsKey(dropId))
			{
				result = DropMgr.m_dropitem[dropId];
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
