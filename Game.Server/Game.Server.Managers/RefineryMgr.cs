using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class RefineryMgr
	{
		private static Dictionary<int, RefineryInfo> m_Item_Refinery = new Dictionary<int, RefineryInfo>();
		private static LogProvider log => LogProvider.Default;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public static bool Init()
		{
			return RefineryMgr.Reload();
		}
		public static bool Reload()
		{
			bool result;
			try
			{
				Dictionary<int, RefineryInfo> Temp_Refinery = new Dictionary<int, RefineryInfo>();
				Temp_Refinery = RefineryMgr.LoadFromBD();
				if (Temp_Refinery.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, RefineryInfo>>(ref RefineryMgr.m_Item_Refinery, Temp_Refinery);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				RefineryMgr.log.Error("NPCInfoMgr", e);
			}
			result = false;
			return result;
		}
		public static Dictionary<int, RefineryInfo> LoadFromBD()
		{
			List<RefineryInfo> infos = new List<RefineryInfo>();
			Dictionary<int, RefineryInfo> Temp_Refinery = new Dictionary<int, RefineryInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				infos = db.GetAllRefineryInfo();
				foreach (RefineryInfo info in infos)
				{
					if (!Temp_Refinery.ContainsKey(info.RefineryID))
					{
						Temp_Refinery.Add(info.RefineryID, info);
					}
				}
			}
			return Temp_Refinery;
		}
		public static ItemTemplateInfo Refinery(GamePlayer player, List<ItemInfo> Items, ItemInfo Item, bool Luck, int OpertionType, ref bool result, ref int defaultprobability, ref bool IsFormula)
		{
			ItemTemplateInfo TempItem = new ItemTemplateInfo();
			ItemTemplateInfo result2;
			foreach (int i in RefineryMgr.m_Item_Refinery.Keys)
			{
				if (RefineryMgr.m_Item_Refinery[i].m_Equip.Contains(Item.TemplateID))
				{
					IsFormula = true;
					int j = 0;
					List<int> Template = new List<int>();
					foreach (ItemInfo info in Items)
					{
						if (info.TemplateID == RefineryMgr.m_Item_Refinery[i].Item1 && info.Count >= RefineryMgr.m_Item_Refinery[i].Item1Count && !Template.Contains(info.TemplateID))
						{
							Template.Add(info.TemplateID);
							if (OpertionType != 0)
							{
								info.Count -= RefineryMgr.m_Item_Refinery[i].Item1Count;
							}
							j++;
						}
						if (info.TemplateID == RefineryMgr.m_Item_Refinery[i].Item2 && info.Count >= RefineryMgr.m_Item_Refinery[i].Item2Count && !Template.Contains(info.TemplateID))
						{
							Template.Add(info.TemplateID);
							if (OpertionType != 0)
							{
								info.Count -= RefineryMgr.m_Item_Refinery[i].Item2Count;
							}
							j++;
						}
						if (info.TemplateID == RefineryMgr.m_Item_Refinery[i].Item3 && info.Count >= RefineryMgr.m_Item_Refinery[i].Item3Count && !Template.Contains(info.TemplateID))
						{
							Template.Add(info.TemplateID);
							if (OpertionType != 0)
							{
								info.Count -= RefineryMgr.m_Item_Refinery[i].Item3Count;
							}
							j++;
						}
					}
					if (j == 3)
					{
						for (int k = 0; k < RefineryMgr.m_Item_Refinery[i].m_Reward.Count; k++)
						{
							if (Items[Items.Count - 1].TemplateID == RefineryMgr.m_Item_Refinery[i].m_Reward[k])
							{
								if (Luck)
								{
									defaultprobability += 20;
								}
								if (OpertionType == 0)
								{
									int TempItemID = RefineryMgr.m_Item_Refinery[i].m_Reward[k + 1];
									result2 = ItemMgr.FindItemTemplate(TempItemID);
									return result2;
								}
								if (RefineryMgr.random.Next(100) < defaultprobability)
								{
									int TempItemID = RefineryMgr.m_Item_Refinery[i].m_Reward[k + 1];
									result = true;
									result2 = ItemMgr.FindItemTemplate(TempItemID);
									return result2;
								}
							}
						}
					}
				}
				else
				{
					IsFormula = false;
				}
			}
			result2 = null;
			return result2;
		}
		public static ItemTemplateInfo RefineryTrend(int Operation, ItemInfo Item, ref bool result)
		{
			ItemTemplateInfo result2;
			if (Item != null)
			{
				foreach (int i in RefineryMgr.m_Item_Refinery.Keys)
				{
					if (RefineryMgr.m_Item_Refinery[i].m_Reward.Contains(Item.TemplateID))
					{
						for (int j = 0; j < RefineryMgr.m_Item_Refinery[i].m_Reward.Count; j++)
						{
							if (RefineryMgr.m_Item_Refinery[i].m_Reward[j] == Operation)
							{
								int TemplateId = RefineryMgr.m_Item_Refinery[i].m_Reward[j + 2];
								result = true;
								result2 = ItemMgr.FindItemTemplate(TemplateId);
								return result2;
							}
						}
					}
				}
			}
			result2 = null;
			return result2;
		}
	}
}
