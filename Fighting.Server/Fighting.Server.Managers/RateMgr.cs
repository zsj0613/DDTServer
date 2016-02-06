using Bussiness;
using Fighting.Server;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Fighting.Server.Managers
{
	public class RateMgr
	{
		protected static ThreadSafeRandom m_random = new ThreadSafeRandom();
		private static LogProvider log => LogProvider.Default;
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		private static ArrayList m_RateInfos = new ArrayList();
        public static bool Init(FightServerConfig config)
        {
            RateMgr.m_lock.AcquireWriterLock(-1);
            bool result;
            try
            {
                using (ServiceBussiness db = new ServiceBussiness())
                {
                    RateMgr.m_RateInfos = db.GetRate(config.Id);
                }
                result = true;
            }
            catch (Exception e)
            {
                
                RateMgr.log.Error("RateMgr", e);
                
                result = false;
            }
            finally
            {
                RateMgr.m_lock.ReleaseWriterLock();
            }
            return result;
        }
        public static bool ReLoad()
		{
            return RateMgr.Init(FightServer.Instance.Configuration);
        }
		private static RateInfo GetRateInfoWithType(int type)
		{
			RateInfo result;
			foreach (RateInfo ri in RateMgr.m_RateInfos)
			{
				if (ri.Type == type)
				{
					result = ri;
					return result;
				}
			}
			result = null;
			return result;
		}
		private static bool IsValid(RateInfo _RateInfo)
		{
			//DateTime arg_07_0 = _RateInfo.BeginDay;
			//DateTime arg_0E_0 = _RateInfo.EndDay;
		//	bool flag = 0 == 0;
			return _RateInfo.BeginDay.Ticks <= DateTime.Now.Ticks && _RateInfo.EndDay.Ticks >= DateTime.Now.Ticks && !(_RateInfo.BeginTime.TimeOfDay > DateTime.Now.TimeOfDay) && !(_RateInfo.EndTime.TimeOfDay < DateTime.Now.TimeOfDay);
		}

        internal static int GetNpcID()
        {
            return 0;
        }
    }
}
