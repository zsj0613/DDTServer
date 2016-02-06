
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public static class ShopMgr
	{
        private static LogProvider log => LogProvider.Default;
		private static Dictionary<int, ShopItemInfo> m_shop = new Dictionary<int, ShopItemInfo>();
		private static Dictionary<int, int> m_LimitCount = new Dictionary<int, int>();
		private static Dictionary<int, List<int>> m_isNoticeInfos = new Dictionary<int, List<int>>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static bool Init()
		{
			return ShopMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, ShopItemInfo> tempShop = ShopMgr.LoadFromDatabase();
				Dictionary<int, int> tempMaxLimit = new Dictionary<int, int>();
				Dictionary<int, List<int>> tempNoticeInfos = new Dictionary<int, List<int>>();
				foreach (int key in tempShop.Keys)
				{
					if (!tempMaxLimit.ContainsKey(key))
					{
						tempMaxLimit.Add(key, tempShop[key].LimitCount);
					}
					if (!tempNoticeInfos.ContainsKey(key) && tempShop[key].LimitCount != -1)
					{
						tempNoticeInfos.Add(key, ShopMgr.InitNotice());
					}
				}
				if (tempShop.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, ShopItemInfo>>(ref ShopMgr.m_shop, tempShop);
					Interlocked.Exchange<Dictionary<int, int>>(ref ShopMgr.m_LimitCount, tempMaxLimit);
					Interlocked.Exchange<Dictionary<int, List<int>>>(ref ShopMgr.m_isNoticeInfos, tempNoticeInfos);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				ShopMgr.log.Error("ShopInfoMgr", e);
			}
			result = false;
			return result;
		}
		private static Dictionary<int, ShopItemInfo> LoadFromDatabase()
		{
			Dictionary<int, ShopItemInfo> list = new Dictionary<int, ShopItemInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				ShopItemInfo[] infos = db.GetALllShop();
				ShopItemInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					ShopItemInfo info = array[i];
					if (!list.ContainsKey(info.ID))
					{
						list.Add(info.ID, info);
					}
				}
			}
			return list;
		}
		public static void RefreshLimitCount()
		{
			try
			{
				Dictionary<int, ShopItemInfo> tempShop = ShopMgr.m_shop;
				Dictionary<int, int> tempMaxLimit = new Dictionary<int, int>();
				int[] keys = tempShop.Keys.ToArray<int>();
				for (int i = 0; i < keys.Length; i++)
				{
					if (!tempMaxLimit.ContainsKey(keys[i]))
					{
						tempMaxLimit.Add(keys[i], tempShop[keys[i]].LimitCount);
					}
				}
				if (tempMaxLimit.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, int>>(ref ShopMgr.m_LimitCount, tempMaxLimit);
					ShopMgr.InitNotice();
				}
			}
			catch (Exception e)
			{
				ShopMgr.log.Error("ShopInfoMgr", e);
			}
		}
		public static ShopItemInfo GetShopItemInfoById(int id)
		{
			ShopItemInfo result;
			if (ShopMgr.m_shop.ContainsKey(id))
			{
				result = ShopMgr.m_shop[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static Dictionary<int, int> GetLimitShopItemInfo()
		{
			return ShopMgr.m_LimitCount;
		}
		public static int GetLimitMax(int id)
		{
			return ShopMgr.m_shop[id].LimitCount;
		}
		public static void SubtractShopLimit(int id)
		{
			Dictionary<int, int> limitCount;
			Monitor.Enter(limitCount = ShopMgr.m_LimitCount);
			try
			{
				Dictionary<int, int> limitCount2;
				(limitCount2 = ShopMgr.m_LimitCount)[id] = limitCount2[id] - 1;
			}
			finally
			{
				Monitor.Exit(limitCount);
			}
		}
		public static void UpdateLimitCount(Dictionary<int, int> info)
		{
			Dictionary<int, int> limitCount;
			Monitor.Enter(limitCount = ShopMgr.m_LimitCount);
			try
			{
				foreach (int key in info.Keys)
				{
					if (info[key] < ShopMgr.m_LimitCount[key])
					{
						ShopMgr.m_LimitCount[key] = info[key];
					}
				}
			}
			finally
			{
				Monitor.Exit(limitCount);
			}
		}
		public static void UpdateNotice(Dictionary<int, List<int>> notice)
		{
			Dictionary<int, List<int>> isNoticeInfos;
			Monitor.Enter(isNoticeInfos = ShopMgr.m_isNoticeInfos);
			try
			{
				foreach (int key in notice.Keys)
				{
					for (int i = 0; i < notice[key].Count; i++)
					{
						if (notice[key][i] != ShopMgr.m_isNoticeInfos[key][i])
						{
							ShopMgr.m_isNoticeInfos[key][i] = notice[key][i];
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(isNoticeInfos);
			}
		}
		public static void CloseNotice(int id, int index)
		{
			Dictionary<int, List<int>> isNoticeInfos;
			Monitor.Enter(isNoticeInfos = ShopMgr.m_isNoticeInfos);
			try
			{
				if (ShopMgr.m_isNoticeInfos.ContainsKey(id))
				{
					ShopMgr.m_isNoticeInfos[id][index] = 0;
				}
			}
			finally
			{
				Monitor.Exit(isNoticeInfos);
			}
		}
		public static int GetLimitCountByID(int id)
		{
			return ShopMgr.m_LimitCount[id];
		}
		public static int GetIsNotice(int id, int index)
		{
			return ShopMgr.m_isNoticeInfos[id][index];
		}
		public static bool CanBuy(int shopID, int consortiaShopLevel, ref bool isBinds, int cousortiaID, int playerRiches)
		{
			bool result = false;
			using (ConsortiaBussiness csbs = new ConsortiaBussiness())
			{
				switch (shopID)
				{
				case 1:
					result = true;
					isBinds = false;
					break;
				case 2:
					result = true;
					isBinds = false;
					break;
				case 3:
					result = true;
					isBinds = false;
					break;
				case 4:
					result = true;
					isBinds = false;
					break;
				case 11:
				{
					ConsortiaEquipControlInfo cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 1, 1);
					if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
					{
						result = true;
						isBinds = true;
					}
					break;
				}
				case 12:
				{
					ConsortiaEquipControlInfo cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 2, 1);
					if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
					{
						result = true;
						isBinds = true;
					}
					break;
				}
				case 13:
				{
					ConsortiaEquipControlInfo cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 3, 1);
					if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
					{
						result = true;
						isBinds = true;
					}
					break;
				}
				case 14:
				{
					ConsortiaEquipControlInfo cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 4, 1);
					if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
					{
						result = true;
						isBinds = true;
					}
					break;
				}
				case 15:
				{
					ConsortiaEquipControlInfo cecInfo = csbs.GetConsortiaEuqipRiches(cousortiaID, 5, 1);
					if (consortiaShopLevel >= cecInfo.Level && playerRiches >= cecInfo.Riches)
					{
						result = true;
						isBinds = true;
					}
					break;
				}
				}
			}
			return result;
		}
		public static int FindItemTemplateID(int id)
		{
			int result;
			if (ShopMgr.m_shop.ContainsKey(id))
			{
				result = ShopMgr.m_shop[id].TemplateID;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public static List<ShopItemInfo> FindShopbyTemplatID(int TemplatID)
		{
			List<ShopItemInfo> shopItem = new List<ShopItemInfo>();
			foreach (ShopItemInfo shop in ShopMgr.m_shop.Values)
			{
				if (shop.TemplateID == TemplatID)
				{
					shopItem.Add(shop);
				}
			}
			return shopItem;
		}
		public static List<ShopItemInfo> FindShopByGroupID(int GroupID)
		{
			List<ShopItemInfo> shopItem = new List<ShopItemInfo>();
			foreach (ShopItemInfo shop in ShopMgr.m_shop.Values)
			{
				if (shop.GroupID == GroupID && shop.ShopID == 22)
				{
					shopItem.Add(shop);
				}
			}
			return shopItem;
		}
		public static List<int> GetShopItemBuyConditions(ShopItemInfo shop, int type, ref int gold, ref int money, ref int offer, ref int gifttoken)
		{
			int iTemplateID = 0;
			int iCount = 0;
			gold = 0;
			money = 0;
			offer = 0;
			gifttoken = 0;
			List<int> itemsInfo = new List<int>();
			if (!ShopMgr.isTypeIn(shop, type))
			{
				throw new ArgumentNullException("type isn't in!");
			}
			if (type == 1)
			{
				ShopMgr.GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
				ShopMgr.GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
				ShopMgr.GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
			}
			if (type == 2)
			{
				ShopMgr.GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
				ShopMgr.GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
				ShopMgr.GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
			}
			if (type == 3)
			{
				ShopMgr.GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
				ShopMgr.GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
				ShopMgr.GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);
				if (iTemplateID > 0)
				{
					itemsInfo.Add(iTemplateID);
					itemsInfo.Add(iCount);
				}
			}
			return itemsInfo;
		}
		public static void GetItemPrice(int Prices, int Values, decimal beat, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int iTemplateID, ref int iCount)
		{
			iTemplateID = 0;
			iCount = 0;
			switch (Prices)
			{
			case -4:
				gifttoken += (int)(Values * beat);
				break;
			case -3:
				offer += (int)(Values * beat);
				break;
			case -2:
				gold += (int)(Values * beat);
				break;
			case -1:
				money += (int)(Values * beat);
				break;
			default:
				if (Prices > 0)
				{
					iTemplateID = Prices;
					iCount = Values;
				}
				break;
			}
		}
		public static ItemInfo CreateItem(ShopItemInfo shopItem, int addtype, int valuetype, string color, string skin, bool isBinding)
		{
			if (shopItem == null)
			{
				throw new ArgumentNullException("shopItem");
			}
			ItemTemplateInfo template = ItemMgr.FindItemTemplate(shopItem.TemplateID);
			ItemInfo item = ItemInfo.CreateFromTemplate(template, 1, addtype);
			if (0 == shopItem.BuyType)
			{
				if (1 == valuetype)
				{
					item.ValidDate = shopItem.AUnit;
				}
				if (2 == valuetype)
				{
					item.ValidDate = shopItem.BUnit;
				}
				if (3 == valuetype)
				{
					item.ValidDate = shopItem.CUnit;
				}
			}
			else
			{
				if (1 == valuetype)
				{
					item.Count = shopItem.AUnit;
				}
				if (2 == valuetype)
				{
					item.Count = shopItem.BUnit;
				}
				if (3 == valuetype)
				{
					item.Count = shopItem.CUnit;
				}
			}
			item.Color = ((color == null) ? "" : color);
			item.Skin = ((skin == null) ? "" : skin);
			if (isBinding)
			{
				item.IsBinds = true;
			}
			else
			{
				item.IsBinds = Convert.ToBoolean(shopItem.IsBind);
			}
			return item;
		}
		public static bool isTypeIn(ShopItemInfo shopItem, int type)
		{
			return (type == 1 && shopItem.AUnit != -1) || (type == 2 && shopItem.BUnit != -1) || (type == 3 && shopItem.CUnit != -1);
		}
		private static List<int> InitNotice()
		{
			List<int> temp = new List<int>();
			for (int i = 0; i < 4; i++)
			{
				temp.Add(i + 1);
			}
			return temp;
		}
	}
}
