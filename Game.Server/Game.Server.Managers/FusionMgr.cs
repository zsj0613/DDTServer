using Bussiness;
using Bussiness.Managers;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server.Managers
{
	public class FusionMgr
	{
		private static LogProvider log => LogProvider.Default;
		private static Dictionary<string, FusionInfo> _fusions;
		private static ReaderWriterLock m_lock;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<string, FusionInfo> tempFusions = new Dictionary<string, FusionInfo>();
				if (FusionMgr.LoadFusion(tempFusions))
				{
					FusionMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						FusionMgr._fusions = tempFusions;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						FusionMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				//if (FusionMgr.log.IsErrorEnabled)
				{
					FusionMgr.log.Error("FusionMgr", e);
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
				FusionMgr.m_lock = new ReaderWriterLock();
				FusionMgr._fusions = new Dictionary<string, FusionInfo>();
				result = FusionMgr.LoadFusion(FusionMgr._fusions);
			}
			catch (Exception e)
			{
			//	if (FusionMgr.log.IsErrorEnabled)
				{
					FusionMgr.log.Error("FusionMgr", e);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadFusion(Dictionary<string, FusionInfo> fusion)
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				FusionInfo[] infos = db.GetAllFusion();
				FusionInfo[] array = infos;
				for (int j = 0; j < array.Length; j++)
				{
					FusionInfo info = array[j];
					List<int> list = new List<int>();
					list.Add(info.Item1);
					list.Add(info.Item2);
					list.Add(info.Item3);
					list.Add(info.Item4);
					list.Add(info.Formula);
					list.Sort();
					StringBuilder items = new StringBuilder();
					foreach (int i in list)
					{
						if (i != 0)
						{
							items.Append(i);
						}
					}
					string key = items.ToString();
					if (!fusion.ContainsKey(key))
					{
						fusion.Add(key, info);
					}
				}
			}
			return true;
		}
		public static ItemTemplateInfo Fusion(List<ItemInfo> Items, List<ItemInfo> AppendItems, ItemInfo Formul, ref bool isBind, ref bool result)
		{
			List<int> list = new List<int>();
			int MaxLevel = 0;
			int TotalRate = 0;
			ItemTemplateInfo returnItem = null;
			foreach (ItemInfo p in Items)
			{
				list.Add(p.Template.FusionType);
				if (p.Template.Level > MaxLevel)
				{
					MaxLevel = p.Template.Level;
				}
				TotalRate += p.Template.FusionRate;
				if (p.IsBinds)
				{
					isBind = true;
				}
			}
			if (Formul.IsBinds)
			{
				isBind = true;
			}
			foreach (ItemInfo p in AppendItems)
			{
				TotalRate += p.Template.FusionRate / 2;
				if (p.IsBinds)
				{
					isBind = true;
				}
			}
			list.Add(Formul.TemplateID);
			list.Sort();
			StringBuilder itemString = new StringBuilder();
			foreach (int i in list)
			{
				itemString.Append(i);
			}
			string key = itemString.ToString();
			FusionMgr.m_lock.AcquireReaderLock(-1);
			ItemTemplateInfo result2;
			try
			{
				if (FusionMgr._fusions.ContainsKey(key))
				{
					FusionInfo info = FusionMgr._fusions[key];
					ItemTemplateInfo temp_0 = ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel);
					ItemTemplateInfo temp_ = ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 1);
					ItemTemplateInfo temp_2 = ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 2);
					List<ItemTemplateInfo> temps = new List<ItemTemplateInfo>();
					if (temp_2 != null)
					{
						temps.Add(temp_2);
					}
					if (temp_ != null)
					{
						temps.Add(temp_);
					}
					if (temp_0 != null)
					{
						temps.Add(temp_0);
					}
					ItemTemplateInfo tempMax = (
						from s in temps
						where (double)TotalRate / (double)s.FusionNeedRate <= 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate descending
						select s).FirstOrDefault<ItemTemplateInfo>();
					ItemTemplateInfo tempMin = (
						from s in temps
						where (double)TotalRate / (double)s.FusionNeedRate > 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate
						select s).FirstOrDefault<ItemTemplateInfo>();
					if (tempMax != null && tempMin == null)
					{
						returnItem = tempMax;
						if ((double)(100 * TotalRate) / (double)tempMax.FusionNeedRate > (double)FusionMgr.random.Next(100))
						{
							result = true;
						}
					}
					if (tempMax != null && tempMin != null)
					{
						if (tempMax.Level - tempMin.Level == 2)
						{
							double rateMax = (double)(100 * TotalRate) * 0.6 / (double)tempMax.FusionNeedRate;
							double rateMin = 100.0 - rateMax;
						}
						else
						{
							double rateMax = (double)(100 * TotalRate) / (double)tempMax.FusionNeedRate;
							double rateMin = 100.0 - rateMax;
						}
						if ((double)(100 * TotalRate) / (double)tempMax.FusionNeedRate > (double)FusionMgr.random.Next(100))
						{
							returnItem = tempMax;
							result = true;
						}
						else
						{
							returnItem = tempMin;
							result = true;
						}
					}
					if (tempMax == null && tempMin != null)
					{
						returnItem = tempMin;
						result = true;
					}
					if (result)
					{
						foreach (ItemInfo p in Items)
						{
							if (p.Template.TemplateID == returnItem.TemplateID)
							{
								result = false;
								break;
							}
						}
					}
					result2 = returnItem;
					return result2;
				}
			}
			catch
			{
			}
			finally
			{
				FusionMgr.m_lock.ReleaseReaderLock();
			}
			result2 = null;
			return result2;
		}
		public static Dictionary<int, double> FusionPreview(List<ItemInfo> Items, List<ItemInfo> AppendItems, ItemInfo Formul, ref bool isBind)
		{
			List<int> list = new List<int>();
			int MaxLevel = 0;
			int TotalRate = 0;
			Dictionary<int, double> Item_Rate = new Dictionary<int, double>();
			Item_Rate.Clear();
			foreach (ItemInfo p in Items)
			{
				list.Add(p.Template.FusionType);
				if (p.Template.Level > MaxLevel)
				{
					MaxLevel = p.Template.Level;
				}
				TotalRate += p.Template.FusionRate;
				if (p.IsBinds)
				{
					isBind = true;
				}
			}
			if (Formul.IsBinds)
			{
				isBind = true;
			}
			list.Add(Formul.TemplateID);
			foreach (ItemInfo p in AppendItems)
			{
				TotalRate += p.Template.FusionRate / 2;
				if (p.IsBinds)
				{
					isBind = true;
				}
			}
			list.Sort();
			StringBuilder itemString = new StringBuilder();
			foreach (int i in list)
			{
				itemString.Append(i);
			}
			string key = itemString.ToString();
			FusionMgr.m_lock.AcquireReaderLock(-1);
			Dictionary<int, double> result;
			try
			{
				if (FusionMgr._fusions.ContainsKey(key))
				{
					FusionInfo info = FusionMgr._fusions[key];
					ItemTemplateInfo temp_0 = ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel);
					ItemTemplateInfo temp_ = ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 1);
					ItemTemplateInfo temp_2 = ItemMgr.GetGoodsbyFusionTypeandLevel(info.Reward, MaxLevel + 2);
					List<ItemTemplateInfo> temps = new List<ItemTemplateInfo>();
					if (temp_2 != null)
					{
						temps.Add(temp_2);
					}
					if (temp_ != null)
					{
						temps.Add(temp_);
					}
					if (temp_0 != null)
					{
						temps.Add(temp_0);
					}
					ItemTemplateInfo tempMax = (
						from s in temps
						where (double)TotalRate / (double)s.FusionNeedRate <= 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate descending
						select s).FirstOrDefault<ItemTemplateInfo>();
					ItemTemplateInfo tempMin = (
						from s in temps
						where (double)TotalRate / (double)s.FusionNeedRate > 1.1
						orderby (double)TotalRate / (double)s.FusionNeedRate
						select s).FirstOrDefault<ItemTemplateInfo>();
					if (tempMax != null && tempMin == null)
					{
						Item_Rate.Add(tempMax.TemplateID, (double)(100 * TotalRate) / (double)tempMax.FusionNeedRate);
					}
					if (tempMax != null && tempMin != null)
					{
						double rateMax;
						double rateMin;
						if (tempMax.Level - tempMin.Level == 2)
						{
							rateMax = (double)(100 * TotalRate) * 0.6 / (double)tempMax.FusionNeedRate;
							rateMin = 100.0 - rateMax;
						}
						else
						{
							rateMax = (double)(100 * TotalRate) / (double)tempMax.FusionNeedRate;
							rateMin = 100.0 - rateMax;
						}
						Item_Rate.Add(tempMax.TemplateID, rateMax);
						if (rateMin > 0.0)
						{
							Item_Rate.Add(tempMin.TemplateID, rateMin);
						}
					}
					if (tempMax == null && tempMin != null)
					{
						Item_Rate.Add(tempMin.TemplateID, 100.0);
					}
					int[] templist = Item_Rate.Keys.ToArray<int>();
					int[] array = templist;
					for (int j = 0; j < array.Length; j++)
					{
						int ID = array[j];
						foreach (ItemInfo p in Items)
						{
							if (ID == p.Template.TemplateID)
							{
								if (Item_Rate.ContainsKey(ID))
								{
									Item_Rate.Remove(ID);
								}
							}
						}
					}
					result = Item_Rate;
					return result;
				}
				result = Item_Rate;
				return result;
			}
			catch
			{
			}
			finally
			{
				FusionMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
	}
}
