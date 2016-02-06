using Bussiness;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class StrengthenMgr
	{
		private static LogProvider log => LogProvider.Default;
		private static Dictionary<int, StrengthenInfo> _strengthens;
		private static Dictionary<int, StrengthenInfo> m_Refinery_Strengthens;
		private static Dictionary<int, StrengthenGoodsInfo> Strengthens_Goods;
		private static ReaderWriterLock m_lock;
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, StrengthenInfo> tempStrengthens = new Dictionary<int, StrengthenInfo>();
				Dictionary<int, StrengthenInfo> tempRefineryStrengthens = new Dictionary<int, StrengthenInfo>();
				Dictionary<int, StrengthenGoodsInfo> tempStrengthenGoodsInfos = new Dictionary<int, StrengthenGoodsInfo>();
				if (StrengthenMgr.LoadStrengthen(tempStrengthens, tempRefineryStrengthens))
				{
					StrengthenMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						StrengthenMgr._strengthens = tempStrengthens;
						StrengthenMgr.m_Refinery_Strengthens = tempRefineryStrengthens;
						StrengthenMgr.Strengthens_Goods = tempStrengthenGoodsInfos;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						StrengthenMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				//if (StrengthenMgr.log.IsErrorEnabled)
				{
					StrengthenMgr.log.Error("StrengthenMgr", e);
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
				StrengthenMgr.m_lock = new ReaderWriterLock();
				StrengthenMgr._strengthens = new Dictionary<int, StrengthenInfo>();
				StrengthenMgr.m_Refinery_Strengthens = new Dictionary<int, StrengthenInfo>();
				StrengthenMgr.Strengthens_Goods = new Dictionary<int, StrengthenGoodsInfo>();
				result = StrengthenMgr.LoadStrengthen(StrengthenMgr._strengthens, StrengthenMgr.m_Refinery_Strengthens);
			}
			catch (Exception e)
			{
				//if (StrengthenMgr.log.IsErrorEnabled)
				{
					StrengthenMgr.log.Error("StrengthenMgr", e);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadStrengthen(Dictionary<int, StrengthenInfo> strengthen, Dictionary<int, StrengthenInfo> RefineryStrengthen)
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				StrengthenInfo[] infos = db.GetAllStrengthen();
				StrengthenGoodsInfo[] StrengthGoodInfos = db.GetAllStrengthenGoodsInfo();
				StrengthenInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					StrengthenInfo info = array[i];
					if (!strengthen.ContainsKey(info.StrengthenLevel))
					{
						strengthen.Add(info.StrengthenLevel, info);
					}
				}
				StrengthenGoodsInfo[] array2 = StrengthGoodInfos;
				for (int i = 0; i < array2.Length; i++)
				{
					StrengthenGoodsInfo info2 = array2[i];
					if (!StrengthenMgr.Strengthens_Goods.ContainsKey(info2.ID))
					{
						StrengthenMgr.Strengthens_Goods.Add(info2.ID, info2);
					}
				}
			}
			return true;
		}
		public static StrengthenInfo FindStrengthenInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(-1);
			StrengthenInfo result;
			try
			{
				if (StrengthenMgr._strengthens.ContainsKey(level))
				{
					result = StrengthenMgr._strengthens[level];
					return result;
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static StrengthenInfo FindRefineryStrengthenInfo(int level)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(-1);
			StrengthenInfo result;
			try
			{
				if (StrengthenMgr.m_Refinery_Strengthens.ContainsKey(level))
				{
					result = StrengthenMgr.m_Refinery_Strengthens[level];
					return result;
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int level, int TemplateId)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(-1);
			StrengthenGoodsInfo result;
			try
			{
				foreach (int i in StrengthenMgr.Strengthens_Goods.Keys)
				{
					if (StrengthenMgr.Strengthens_Goods[i].Level == level && TemplateId == StrengthenMgr.Strengthens_Goods[i].CurrentEquip)
					{
						result = StrengthenMgr.Strengthens_Goods[i];
						return result;
					}
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static StrengthenGoodsInfo FindStrengthenFailGoodsInfo(int level, int templateid)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(-1);
			StrengthenGoodsInfo result;
			try
			{
				List<StrengthenGoodsInfo> list = new List<StrengthenGoodsInfo>();
				foreach (int i in StrengthenMgr.Strengthens_Goods.Keys)
				{
					if (StrengthenMgr.Strengthens_Goods[i].Level == level && templateid == StrengthenMgr.Strengthens_Goods[i].GainEquip && (level == 10 || level == 12 || level == 14))
					{
						list.Add(StrengthenMgr.Strengthens_Goods[i]);
					}
				}
				StrengthenGoodsInfo max = null;
				foreach (StrengthenGoodsInfo info in list)
				{
					if (max == null)
					{
						max = info;
					}
					else
					{
						if (info.CurrentEquip > max.CurrentEquip)
						{
							max = info;
						}
					}
				}
				result = max;
				return result;
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int TemplateId)
		{
			StrengthenMgr.m_lock.AcquireReaderLock(-1);
			StrengthenGoodsInfo result;
			try
			{
				foreach (int i in StrengthenMgr.Strengthens_Goods.Keys)
				{
					if (TemplateId == StrengthenMgr.Strengthens_Goods[i].GainEquip)
					{
						result = StrengthenMgr.Strengthens_Goods[i];
						return result;
					}
				}
			}
			catch
			{
			}
			finally
			{
				StrengthenMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static void InheritProperty(ItemInfo parent, ItemInfo child)
		{
			if (parent.Hole1 >= 0)
			{
				child.Hole1 = parent.Hole1;
			}
			if (parent.Hole2 >= 0)
			{
				child.Hole2 = parent.Hole2;
			}
			if (parent.Hole3 >= 0)
			{
				child.Hole3 = parent.Hole3;
			}
			if (parent.Hole4 >= 0)
			{
				child.Hole4 = parent.Hole4;
			}
			if (parent.Hole5 >= 0)
			{
				child.Hole5 = parent.Hole5;
			}
			if (parent.Hole6 >= 0)
			{
				child.Hole6 = parent.Hole6;
			}
			child.AttackCompose = parent.AttackCompose;
			child.DefendCompose = parent.DefendCompose;
			child.LuckCompose = parent.LuckCompose;
			child.AgilityCompose = parent.AgilityCompose;
			child.IsBinds = parent.IsBinds;
			child.ValidDate = parent.ValidDate;
			child.IsUsed = parent.IsUsed;
			child.BeginDate = parent.BeginDate;
		}
	}
}
