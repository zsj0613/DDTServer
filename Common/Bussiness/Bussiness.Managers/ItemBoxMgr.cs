
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class ItemBoxMgr
	{
        private static LogProvider log => LogProvider.Default;
        private static ItemBoxInfo[] m_itemBox;
		private static Dictionary<int, List<ItemBoxInfo>> m_itemBoxs;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public static bool ReLoad()
		{
			bool result;
			try
			{
				ItemBoxInfo[] tempItemBox = ItemBoxMgr.LoadItemBoxDb();
				Dictionary<int, List<ItemBoxInfo>> tempItemBoxs = ItemBoxMgr.LoadItemBoxs(tempItemBox);
				if (tempItemBox != null)
				{
					Interlocked.Exchange<ItemBoxInfo[]>(ref ItemBoxMgr.m_itemBox, tempItemBox);
					Interlocked.Exchange<Dictionary<int, List<ItemBoxInfo>>>(ref ItemBoxMgr.m_itemBoxs, tempItemBoxs);
				}
			}
			catch (Exception e)
			{
				ItemBoxMgr.log.Error("ReLoad", e);
				
				result = false;
				return result;
			}
			result = true;
			return result;
		}
		public static bool Init()
		{
			return ItemBoxMgr.ReLoad();
		}
		public static ItemBoxInfo[] LoadItemBoxDb()
		{
			Dictionary<int, ItemBoxInfo> list = new Dictionary<int, ItemBoxInfo>();
			ItemBoxInfo[] result;
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ItemBoxInfo[] infos = db.GetItemBoxInfos();
				result = infos;
			}
			return result;
		}
		public static Dictionary<int, List<ItemBoxInfo>> LoadItemBoxs(ItemBoxInfo[] itemBoxs)
		{
			Dictionary<int, List<ItemBoxInfo>> infos = new Dictionary<int, List<ItemBoxInfo>>();
			ItemBoxInfo info;
			for (int i = 0; i < itemBoxs.Length; i++)
			{
				info = itemBoxs[i];
				if (!infos.Keys.Contains(info.DataId))
				{
					IEnumerable<ItemBoxInfo> temp = 
						from s in itemBoxs
						where s.DataId == info.DataId
						select s;
					infos.Add(info.DataId, temp.ToList<ItemBoxInfo>());
				}
			}
			return infos;
		}
		public static bool LoadItemBoxs(Dictionary<int, List<ItemBoxInfo>> infos)
		{
			bool result;
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ItemBoxInfo[] items = db.GetItemBoxInfos();
				ItemBoxInfo[] array = items;
				ItemBoxInfo item;
				for (int i = 0; i < array.Length; i++)
				{
					item = array[i];
					if (!infos.Keys.Contains(item.DataId))
					{
						IEnumerable<ItemBoxInfo> temp = 
							from s in items
							where s.DataId == item.DataId
							select s;
						infos.Add(item.DataId, temp.ToList<ItemBoxInfo>());
					}
				}
				result = true;
			}
			return result;
		}
		public static List<ItemBoxInfo> FindItemBox(int DataId)
		{
			List<ItemBoxInfo> result;
			if (ItemBoxMgr.m_itemBoxs.ContainsKey(DataId))
			{
				List<ItemBoxInfo> items = ItemBoxMgr.m_itemBoxs[DataId];
				result = items;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int gp)
		{
			List<ItemBoxInfo> FiltInfos = new List<ItemBoxInfo>();
			List<ItemBoxInfo> unFiltInfos = ItemBoxMgr.FindItemBox(DateId);
			bool result;
			if (unFiltInfos == null)
			{
				result = false;
			}
			else
			{
				FiltInfos = (
					from s in unFiltInfos
					where s.IsSelect
					select s).ToList<ItemBoxInfo>();
				int dropItemCount = 1;
				int maxRound = 0;
				foreach (ItemBoxInfo boxInfo in unFiltInfos)
				{
					if (!boxInfo.IsSelect && maxRound < boxInfo.Random)
					{
						maxRound = boxInfo.Random;
					}
				}
				maxRound = ItemBoxMgr.random.Next(maxRound);
				List<ItemBoxInfo> RoundInfos = (
					from s in unFiltInfos
					where !s.IsSelect && s.Random >= maxRound
					select s).ToList<ItemBoxInfo>();
				int maxItems = RoundInfos.Count<ItemBoxInfo>();
				if (maxItems > 0)
				{
					dropItemCount = ((dropItemCount > maxItems) ? maxItems : dropItemCount);
					int[] randomArray = ItemBoxMgr.GetRandomUnrepeatArray(0, maxItems - 1, dropItemCount);
					int[] array = randomArray;
					for (int j = 0; j < array.Length; j++)
					{
						int i = array[j];
						ItemBoxInfo item = RoundInfos[i];
						if (FiltInfos == null)
						{
							FiltInfos = new List<ItemBoxInfo>();
						}
						FiltInfos.Add(item);
					}
				}
				foreach (ItemBoxInfo info in FiltInfos)
				{
					if (info == null)
					{
						result = false;
						return result;
					}
					int templateId = info.TemplateId;
					if (templateId <= -200)
					{
						if (templateId == -300)
						{
							giftToken += info.ItemCount;
							continue;
						}
						if (templateId == -200)
						{
							point += info.ItemCount;
							continue;
						}
					}
					else
					{
						if (templateId == -100)
						{
							gold += info.ItemCount;
							continue;
						}
						if (templateId == 11107)
						{
							gp += info.ItemCount;
							continue;
						}
					}
					ItemTemplateInfo temp = ItemMgr.FindItemTemplate(info.TemplateId);
					ItemInfo item2 = ItemInfo.CreateFromTemplate(temp, info.ItemCount, 101);
					if (item2 != null)
					{
						item2.IsBinds = info.IsBind;
						item2.ValidDate = info.ItemValid;
						item2.StrengthenLevel = info.StrengthenLevel;
						item2.AttackCompose = info.AttackCompose;
						item2.DefendCompose = info.DefendCompose;
						item2.AgilityCompose = info.AgilityCompose;
						item2.LuckCompose = info.LuckCompose;
						item2.IsTips = info.IsTips;
						item2.IsLogs = info.IsLogs;
						if (itemInfos == null)
						{
							itemInfos = new List<ItemInfo>();
						}
						itemInfos.Add(item2);
					}
				}
				result = true;
			}
			return result;
		}
		public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
		{
			int[] resultRound = new int[count];
			for (int i = 0; i < count; i++)
			{
				int j = ItemBoxMgr.random.Next(minValue, maxValue + 1);
				int num = 0;
				for (int k = 0; k < i; k++)
				{
					if (resultRound[k] == j)
					{
						num++;
					}
				}
				if (num == 0)
				{
					resultRound[i] = j;
				}
				else
				{
					i--;
				}
			}
			return resultRound;
		}
	}
}
