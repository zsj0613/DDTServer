
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class ItemMgr
	{
        private static LogProvider log => LogProvider.Default;
        private static Dictionary<int, ItemTemplateInfo> _items;
		private static ReaderWriterLock m_lock;
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, ItemTemplateInfo> tempItems = new Dictionary<int, ItemTemplateInfo>();
				if (ItemMgr.LoadItem(tempItems))
				{
					ItemMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						ItemMgr._items = tempItems;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						ItemMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				ItemMgr.log.Error("ReLoad", e);
				
			}
			result = false;
			return result;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				ItemMgr.m_lock = new ReaderWriterLock();
				ItemMgr._items = new Dictionary<int, ItemTemplateInfo>();
				result = ItemMgr.LoadItem(ItemMgr._items);
			}
			catch (Exception e)
			{
				ItemMgr.log.Error("Init", e);
				
				result = false;
			}
			return result;
		}
		public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos)
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ItemTemplateInfo[] items = db.GetAllGoods();
				ItemTemplateInfo[] array = items;
				for (int i = 0; i < array.Length; i++)
				{
					ItemTemplateInfo item = array[i];
					if (!infos.Keys.Contains(item.TemplateID))
					{
						infos.Add(item.TemplateID, item);
					}
				}
			}
			return true;
		}
		public static ItemTemplateInfo FindItemTemplate(int templateId)
		{
			if (ItemMgr._items == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(-1);
			ItemTemplateInfo result;
			try
			{
				if (ItemMgr._items.Keys.Contains(templateId))
				{
					result = ItemMgr._items[templateId];
					return result;
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static ItemTemplateInfo GetGoodsbyFusionTypeandQuality(int fusionType, int quality)
		{
			if (ItemMgr._items == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(-1);
			ItemTemplateInfo result;
			try
			{
				foreach (ItemTemplateInfo p in ItemMgr._items.Values)
				{
					if (p.FusionType == fusionType && p.Quality == quality)
					{
						result = p;
						return result;
					}
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static ItemTemplateInfo GetGoodsbyFusionTypeandLevel(int fusionType, int level)
		{
			if (ItemMgr._items == null)
			{
				ItemMgr.Init();
			}
			ItemMgr.m_lock.AcquireReaderLock(-1);
			ItemTemplateInfo result;
			try
			{
				foreach (ItemTemplateInfo p in ItemMgr._items.Values)
				{
					if (p.FusionType == fusionType && p.Level == level)
					{
						result = p;
						return result;
					}
				}
			}
			finally
			{
				ItemMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static List<ItemInfo> SpiltGoodsMaxCount(ItemInfo itemInfo)
		{
			List<ItemInfo> Infos = new List<ItemInfo>();
			for (int maxItem = 0; maxItem < itemInfo.Count; maxItem += itemInfo.Template.MaxCount)
			{
				int tempCount = (itemInfo.Count < itemInfo.Template.MaxCount) ? itemInfo.Count : itemInfo.Template.MaxCount;
				ItemInfo item = itemInfo.Clone();
				item.Count = tempCount;
				Infos.Add(item);
			}
			return Infos;
		}
	}
}
