using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class PropItemMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		private static ReaderWriterLock m_lock;
		private static Dictionary<int, ItemTemplateInfo> _allProp;
		private static int[] PropBag = new int[]
		{
			10001,
			10002,
			10003,
			10004,
			10005,
			10006,
			10007,
			10008
		};
		public static bool Reload()
		{
			bool result;
			try
			{
				Dictionary<int, ItemTemplateInfo> tempProp = new Dictionary<int, ItemTemplateInfo>();
				if (PropItemMgr.LoadProps(tempProp))
				{
					PropItemMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						PropItemMgr._allProp = tempProp;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						PropItemMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (PropItemMgr.log.IsErrorEnabled)
				{
					PropItemMgr.log.Error("ReloadProps", e);
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
				PropItemMgr.m_lock = new ReaderWriterLock();
				PropItemMgr._allProp = new Dictionary<int, ItemTemplateInfo>();
				result = PropItemMgr.LoadProps(PropItemMgr._allProp);
			}
			catch (Exception e)
			{
				if (PropItemMgr.log.IsErrorEnabled)
				{
					PropItemMgr.log.Error("InitProps", e);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadProps(Dictionary<int, ItemTemplateInfo> allProp)
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ItemTemplateInfo[] list = db.GetSingleCategory(10);
				ItemTemplateInfo[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					ItemTemplateInfo p = array[i];
					allProp.Add(p.TemplateID, p);
				}
			}
			return true;
		}
		public static ItemTemplateInfo FindAllProp(int id)
		{
			PropItemMgr.m_lock.AcquireReaderLock(-1);
			ItemTemplateInfo result;
			try
			{
				if (PropItemMgr._allProp.ContainsKey(id))
				{
					result = PropItemMgr._allProp[id];
					return result;
				}
			}
			catch
			{
			}
			finally
			{
				PropItemMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static ItemTemplateInfo FindFightingProp(int id)
		{
			PropItemMgr.m_lock.AcquireReaderLock(-1);
			ItemTemplateInfo result;
			try
			{
				if (!PropItemMgr.PropBag.Contains(id))
				{
					result = null;
					return result;
				}
				if (PropItemMgr._allProp.ContainsKey(id))
				{
					result = PropItemMgr._allProp[id];
					return result;
				}
			}
			catch
			{
			}
			finally
			{
				PropItemMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
	}
}
