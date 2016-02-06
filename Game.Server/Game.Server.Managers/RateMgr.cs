using Bussiness;
using Game.Server.GameObjects;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class RateMgr
	{
		protected static ThreadSafeRandom m_random = new ThreadSafeRandom();
		private static LogProvider log => LogProvider.Default;
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		private static ArrayList m_RateInfos = new ArrayList();
        public static bool Init(GameServerConfig config)
        {
            RateMgr.m_lock.AcquireWriterLock(-1);
            bool result;
            try
            {
                using (ServiceBussiness db = new ServiceBussiness())
                {
                    RateMgr.m_RateInfos = db.GetRate(config.ServerID);
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
			return RateMgr.Init(GameServer.Instance.Configuration);

        }
		public static float GetRate(eRateType eType)
		{
			float rate = 1f;
			RateMgr.m_lock.AcquireReaderLock(-1);
			float result;
			try
			{
				RateInfo _RateInfo = RateMgr.GetRateInfoWithType((int)eType);
				if (_RateInfo == null)
				{
					result = rate;
					return result;
				}
				if (_RateInfo.Rate == 0f)
				{
					result = 1f;
					return result;
				}
				if (RateMgr.IsValid(_RateInfo))
				{
					rate = _RateInfo.Rate;
				}
			}
			catch
			{
			}
			finally
			{
				RateMgr.m_lock.ReleaseReaderLock();
			}
			result = rate;
			return result;
		}
		public static float GetRate(GamePlayer player, eRateType eType)
		{
			float result;
			if (eType == eRateType.Auncher_Experience_Rate || eType == eRateType.Auncher_Offer_Rate || eType == eRateType.Auncher_Riches_Rate)
			{
				if (player.ClientType != eClientType.Auncher)
				{
					result = 1f;
					return result;
				}
			}
			result = RateMgr.GetRate(eType);
			return result;
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
		public static string GetAllRate()
		{
			return string.Format("经验倍率:{0} \n财富倍率:{1} \n功勋倍率:{2} \n登陆器登陆经验倍率:{3} \n登陆器登陆财富倍率:{4} \n登陆器登陆功勋倍率:{5}", new object[]
			{
				RateMgr.GetRate(eRateType.Experience_Rate),
				RateMgr.GetRate(eRateType.Riches_Rate),
				RateMgr.GetRate(eRateType.Offer_Rate),
				RateMgr.GetRate(eRateType.Auncher_Experience_Rate),
				RateMgr.GetRate(eRateType.Auncher_Riches_Rate),
				RateMgr.GetRate(eRateType.Auncher_Offer_Rate)
			});
		}
	}
}
