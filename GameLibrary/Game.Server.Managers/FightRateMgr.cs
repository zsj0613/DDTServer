using Bussiness;
using Game.Base.Packets;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class FightRateMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		private static ReaderWriterLock m_lock;
		protected static Dictionary<int, FightRateInfo> _fightRate;
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, FightRateInfo> tempfightRate = new Dictionary<int, FightRateInfo>();
				if (FightRateMgr.LoadFightRate(tempfightRate))
				{
					FightRateMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						FightRateMgr._fightRate = tempfightRate;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						FightRateMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (FightRateMgr.log.IsErrorEnabled)
				{
					FightRateMgr.log.Error("AwardMgr", e);
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
				FightRateMgr.m_lock = new ReaderWriterLock();
				FightRateMgr._fightRate = new Dictionary<int, FightRateInfo>();
				result = FightRateMgr.LoadFightRate(FightRateMgr._fightRate);
			}
			catch (Exception e)
			{
				if (FightRateMgr.log.IsErrorEnabled)
				{
					FightRateMgr.log.Error("AwardMgr", e);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadFightRate(Dictionary<int, FightRateInfo> fighRate)
		{
			using (ServiceBussiness db = new ServiceBussiness())
			{
				FightRateInfo[] infos = db.GetFightRate(GameServer.Instance.Configuration.ServerID);
				FightRateInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					FightRateInfo info = array[i];
					if (!fighRate.ContainsKey(info.ID))
					{
						fighRate.Add(info.ID, info);
					}
				}
			}
			return true;
		}
		public static FightRateInfo[] GetAllFightRateInfo()
		{
			FightRateInfo[] infos = null;
			FightRateMgr.m_lock.AcquireReaderLock(-1);
			try
			{
				infos = FightRateMgr._fightRate.Values.ToArray<FightRateInfo>();
			}
			catch
			{
			}
			finally
			{
				FightRateMgr.m_lock.ReleaseReaderLock();
			}
			return (infos == null) ? new FightRateInfo[0] : infos;
		}
		public static bool CanChangeStyle(BaseRoom game, GSPacketIn pkg)
		{
			FightRateInfo[] infos = FightRateMgr.GetAllFightRateInfo();
			bool result;
			try
			{
				FightRateInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					FightRateInfo info = array[i];
					if (info.BeginDay.Year <= DateTime.Now.Year && DateTime.Now.Year <= info.EndDay.Year)
					{
						if (info.BeginDay.DayOfYear <= DateTime.Now.DayOfYear && DateTime.Now.DayOfYear <= info.EndDay.DayOfYear)
						{
							if (info.BeginTime.TimeOfDay <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= info.EndTime.TimeOfDay)
							{
								if (FightRateMgr.random.Next(1000000) < info.Rate)
								{
									result = true;
									return result;
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			pkg.WriteBoolean(false);
			result = false;
			return result;
		}
	}
}
