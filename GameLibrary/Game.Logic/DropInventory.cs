using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Game.Logic
{
	public class DropInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static int roundDate = 0;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public static bool CardDrop(eRoomType e, ref List<ItemInfo> info)
		{
			eDropType arg_10_0 = eDropType.Cards;
			int num = (int)e;
			int dropId = DropInventory.GetDropCondiction(arg_10_0, num.ToString(), "0");
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.Cards, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool BoxDrop(eRoomType e, ref List<ItemInfo> info)
		{
			eDropType arg_10_0 = eDropType.Box;
			int num = (int)e;
			int dropId = DropInventory.GetDropCondiction(arg_10_0, num.ToString(), "0");
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.Box, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool NPCDrop(int dropId, ref List<ItemInfo> info)
		{
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.NPC, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool BossDrop(int missionId, ref List<ItemInfo> info)
		{
			int dropId = DropInventory.GetDropCondiction(eDropType.Boss, missionId.ToString(), "0");
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.Boss, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool CopyUserDrop(int copyId, ref List<ItemInfo> info)
		{
			int dropId = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), "1");
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.Copy, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static List<ItemInfo> CopySystemDrop(int copyId, int OpenCount)
		{
			int goodData = Convert.ToInt32((double)OpenCount * 0.1);
			int normalData = Convert.ToInt32((double)OpenCount * 0.3);
			int lowData = OpenCount - goodData - normalData;
			List<ItemInfo> resultInfo = new List<ItemInfo>();
			List<ItemInfo> tempInfo = null;
			int goodDropId = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), "2");
			if (goodDropId > 0)
			{
				for (int i = 0; i < goodData; i++)
				{
					if (DropInventory.GetDropItems(eDropType.Copy, goodDropId, ref tempInfo))
					{
						resultInfo.Add(tempInfo[0]);
						tempInfo = null;
					}
				}
			}
			int normalDropId = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), "3");
			if (normalDropId > 0)
			{
				for (int i = 0; i < normalData; i++)
				{
					if (DropInventory.GetDropItems(eDropType.Copy, normalDropId, ref tempInfo))
					{
						resultInfo.Add(tempInfo[0]);
						tempInfo = null;
					}
				}
			}
			int lowDropId = DropInventory.GetDropCondiction(eDropType.Copy, copyId.ToString(), "4");
			if (lowDropId > 0)
			{
				for (int i = 0; i < lowData; i++)
				{
					if (DropInventory.GetDropItems(eDropType.Copy, lowDropId, ref tempInfo))
					{
						resultInfo.Add(tempInfo[0]);
						tempInfo = null;
					}
				}
			}
			return DropInventory.RandomSortList(resultInfo);
		}
		public static bool SpecialDrop(int missionId, int boxType, ref List<ItemInfo> info)
		{
			int dropId = DropInventory.GetDropCondiction(eDropType.Special, missionId.ToString(), boxType.ToString());
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.Special, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool PvPQuestsDrop(int tempId, bool playResult, ref List<ItemInfo> info)
		{
			int dropId = DropInventory.GetDropCondiction(eDropType.PvpQuests, tempId.ToString(), Convert.ToInt16(playResult).ToString());
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.PvpQuests, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool FireDrop(eRoomType e, ref List<ItemInfo> info)
		{
			eDropType arg_10_0 = eDropType.Fire;
			int num = (int)e;
			int dropId = DropInventory.GetDropCondiction(arg_10_0, num.ToString(), "0");
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.Fire, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool PvEQuestsDrop(int npcId, int tempId, ref List<ItemInfo> info)
		{
			int dropId = DropInventory.GetDropCondiction(eDropType.PveQuests, npcId.ToString(), tempId.ToString());
			bool result;
			if (dropId > 0)
			{
				List<ItemInfo> infos = null;
				if (DropInventory.GetDropItems(eDropType.PveQuests, dropId, ref infos))
				{
					info = ((infos != null) ? infos : null);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public static bool AnswerDrop(int answerId, ref List<ItemInfo> info)
		{
			int dropId = DropInventory.GetDropCondiction(eDropType.Answer, answerId.ToString(), "0");
			bool result;
			if (dropId > 0)
			{
				if (dropId > 0)
				{
					List<ItemInfo> infos = null;
					if (DropInventory.GetDropItems(eDropType.Answer, dropId, ref infos))
					{
						info = ((infos != null) ? infos : null);
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}
		public static bool FightLabUserDrop(int copyId, ref List<ItemInfo> info)
		{
			int dropId = DropInventory.GetDropCondiction(eDropType.FightLab, copyId.ToString(), "1");
			bool result;
			if (dropId > 0)
			{
				List<DropItem> unFiltItems = DropMgr.FindDropItem(dropId);
				for (int i = 0; i < unFiltItems.Count; i++)
				{
					int itemCount = DropInventory.random.Next(unFiltItems[i].BeginData, unFiltItems[i].EndData);
					ItemTemplateInfo temp = ItemMgr.FindItemTemplate(unFiltItems[i].ItemId);
					ItemInfo item = ItemInfo.CreateFromTemplate(temp, itemCount, copyId);
					if (item != null)
					{
						item.IsBinds = unFiltItems[i].IsBind;
						item.ValidDate = unFiltItems[i].ValueDate;
						item.IsTips = unFiltItems[i].IsTips;
						item.IsLogs = unFiltItems[i].IsLogs;
						if (info == null)
						{
							info = new List<ItemInfo>();
						}
						info.Add(item);
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
        public static bool VIPRewardDrop(int level, ref List<ItemInfo> info)
        {
            int dropId = DropInventory.GetDropCondiction(eDropType.VIP, level.ToString(), "1");
            bool result;
            if (dropId > 0)
            {
                List<DropItem> unFiltItems = DropMgr.FindDropItem(dropId);
                for (int i = 0; i < unFiltItems.Count; i++)
                {
                    int itemCount = DropInventory.random.Next(unFiltItems[i].BeginData, unFiltItems[i].EndData);
                    ItemTemplateInfo temp = ItemMgr.FindItemTemplate(unFiltItems[i].ItemId);
                    ItemInfo item = ItemInfo.CreateFromTemplate(temp, itemCount, level);
                    if (item != null)
                    {
                        item.IsBinds = unFiltItems[i].IsBind;
                        item.ValidDate = unFiltItems[i].ValueDate;
                        item.IsTips = unFiltItems[i].IsTips;
                        item.IsLogs = unFiltItems[i].IsLogs;
                        if (info == null)
                        {
                            info = new List<ItemInfo>();
                        }
                        info.Add(item);
                    }
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        private static int GetDropCondiction(eDropType type, string para1, string para2)
		{
			int result;
			try
			{
				int dropId = DropMgr.FindCondiction(type, para1, para2);
				result = dropId;
				return result;
			}
			catch (Exception ex)
			{
				if (DropInventory.log.IsErrorEnabled)
				{
					DropInventory.log.Error(string.Concat(new object[]
					{
						"Drop Error：",
						type,
						" @ ",
						ex
					}));
				}
			}
			result = 0;
			return result;
		}
		private static bool GetDropItems(eDropType type, int dropId, ref List<ItemInfo> itemInfos)
		{
			bool result;
			if (dropId == 0)
			{
				result = false;
			}
			else
			{
				try
				{
					int dropItemCount = 1;
					List<DropItem> unFiltItems = DropMgr.FindDropItem(dropId);
					int maxRound = DropInventory.random.Next((
						from s in unFiltItems
						select s.Random).Max());
					List<DropItem> filtItems = (
						from s in unFiltItems
						where s.Random >= maxRound
						select s).ToList<DropItem>();
					int maxItems = filtItems.Count<DropItem>();
					if (maxItems == 0)
					{
						result = false;
						return result;
					}
					dropItemCount = ((dropItemCount > maxItems) ? maxItems : dropItemCount);
					int[] randomArray = DropInventory.GetRandomUnrepeatArray(0, maxItems - 1, dropItemCount);
					int[] array = randomArray;
					for (int j = 0; j < array.Length; j++)
					{
						int i = array[j];
						int itemCount = DropInventory.random.Next(filtItems[i].BeginData, filtItems[i].EndData);
						ItemTemplateInfo temp = ItemMgr.FindItemTemplate(filtItems[i].ItemId);
						ItemInfo item = ItemInfo.CreateFromTemplate(temp, itemCount, 101);
						if (item != null)
						{
							item.IsBinds = filtItems[i].IsBind;
							item.ValidDate = filtItems[i].ValueDate;
							item.IsTips = filtItems[i].IsTips;
							item.IsLogs = filtItems[i].IsLogs;
							if (itemInfos == null)
							{
								itemInfos = new List<ItemInfo>();
							}
							switch (type)
							{
							    case eDropType.Cards:
							    case eDropType.Box:
							    case eDropType.NPC:
							    case eDropType.Boss:
							    case eDropType.Special:
								    if (DropInfoMgr.CanDrop(temp.TemplateID))
								    {
									    itemInfos.Add(item);
								    }
								    break;
							    default:
                                    itemInfos.Add(item);
                                    break;
                            }							
						}
					}
					if (itemInfos != null && itemInfos.Count > 0)
					{
						result = true;
						return result;
					}
				}
				catch (Exception ex)
				{
					if (DropInventory.log.IsErrorEnabled)
					{
						DropInventory.log.Error(string.Concat(new object[]
						{
							"Drop Error：",
							type,
							" @ ",
							ex
						}));
					}
				}
				result = false;
			}
			return result;
		}
		public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
		{
			int[] resultRound = new int[count];
			for (int i = 0; i < count; i++)
			{
				int j = DropInventory.random.Next(minValue, maxValue + 1);
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
		public static List<ItemInfo> RandomSortList(List<ItemInfo> list)
		{
			return (
				from key in list
				orderby DropInventory.random.Next()
				select key).ToList<ItemInfo>();
		}
	}
}
