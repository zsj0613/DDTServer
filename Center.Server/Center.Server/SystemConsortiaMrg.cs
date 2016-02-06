using Bussiness;
using Game.Base.Packets;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Center.Server
{
	public class SystemConsortiaMrg
	{
		private static Dictionary<int, ConsortiaInfo> allSystemConsortia;
		private static Timer autoAcceptTimer;
		private static Timer autoUpdateTimer;
		private static Timer autoActiveTimer;
		private static int autoAcceptInterval = 60000;
		private static int autoUpdateInterval = 3600000;
		private static int autoActiveInterval = 60000;
		private static int minAcceptInterval = 10;
		private static ReaderWriterLock m_lock;
		private static LogProvider log => CenterServer.log;
		public static bool Init()
		{
			bool result;
			try
			{
				SystemConsortiaMrg.m_lock = new ReaderWriterLock();
				SystemConsortiaMrg.allSystemConsortia = new Dictionary<int, ConsortiaInfo>();
				SystemConsortiaMrg.autoAcceptTimer = new Timer(new TimerCallback(SystemConsortiaMrg.autoAcceptHandler), null, SystemConsortiaMrg.autoAcceptInterval, SystemConsortiaMrg.autoAcceptInterval);
				SystemConsortiaMrg.autoUpdateTimer = new Timer(new TimerCallback(SystemConsortiaMrg.autoUpdateHandler), null, SystemConsortiaMrg.autoUpdateInterval, SystemConsortiaMrg.autoUpdateInterval);
				SystemConsortiaMrg.autoActiveTimer = new Timer(new TimerCallback(SystemConsortiaMrg.autoActiveHandler), null, SystemConsortiaMrg.autoActiveInterval, SystemConsortiaMrg.autoActiveInterval);
				result = SystemConsortiaMrg.Load(SystemConsortiaMrg.allSystemConsortia);
			}
			catch (Exception e)
			{
				
			    SystemConsortiaMrg.log.Error("SystemConsortiaMrg", e);
				
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<int, ConsortiaInfo> consortia)
		{
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				ConsortiaInfo[] infos = db.GetAllSystemConsortia();
				ConsortiaInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					ConsortiaInfo info = array[i];
					if (info.IsExist)
					{
						if (!consortia.ContainsKey(info.ConsortiaID))
						{
							consortia.Add(info.ConsortiaID, info);
						}
					}
				}
			}
			return true;
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, ConsortiaInfo> tempSystemConsortia = new Dictionary<int, ConsortiaInfo>();
				if (SystemConsortiaMrg.Load(tempSystemConsortia))
				{
					SystemConsortiaMrg.m_lock.AcquireWriterLock(-1);
					try
					{
						SystemConsortiaMrg.allSystemConsortia = tempSystemConsortia;
						SystemConsortiaMrg.autoUpdateHandler(null);
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						SystemConsortiaMrg.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				
					SystemConsortiaMrg.log.Error("SystemConsortiaMrg", e);
				
			}
			result = false;
			return result;
		}
		public static ConsortiaInfo[] GatAllSystemConsortia()
		{
			ConsortiaInfo[] result;
			try
			{
				SystemConsortiaMrg.m_lock.AcquireReaderLock(-1);
				result = SystemConsortiaMrg.allSystemConsortia.Values.ToArray<ConsortiaInfo>();
			}
			catch
			{
				result = null;
			}
			finally
			{
				SystemConsortiaMrg.m_lock.ReleaseReaderLock();
			}
			return result;
		}
		public static bool Stop()
		{
			SystemConsortiaMrg.autoAcceptTimer.Change(-1, -1);
			SystemConsortiaMrg.autoAcceptTimer.Dispose();
			SystemConsortiaMrg.autoUpdateTimer.Change(-1, -1);
			SystemConsortiaMrg.autoUpdateTimer.Dispose();
			return true;
		}
		private static void autoActiveHandler(object state)
		{
			try
			{
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					db.ActiveConsortia();
				}
			}
			catch (Exception e)
			{
				
					SystemConsortiaMrg.log.Error("SystemConsortiaMrg", e);
				
			}
		}
		private static void autoUpdateHandler(object state)
		{
			try
			{
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					db.UpdateRobotChairman();
				}
			}
			catch (Exception e)
			{
				
					SystemConsortiaMrg.log.Error("SystemConsortiaMrg", e);
				
			}
		}
		private static void autoAcceptHandler(object state)
		{
			int totalApply = 0;
			int currentPage = 0;
			int pageSize = 100;
			int totalPage = 0;
			bool continueFlag = true;
			string msg = "";
			int repute = 0;
			ConsortiaApplyUserInfo[] allUserApply = null;
			SystemConsortiaMrg.m_lock.AcquireReaderLock(-1);
			ConsortiaInfo[] tempSystemConsortia = SystemConsortiaMrg.allSystemConsortia.Values.ToArray<ConsortiaInfo>();
			SystemConsortiaMrg.m_lock.ReleaseReaderLock();
			try
			{
				ConsortiaInfo[] array = tempSystemConsortia;
				for (int i = 0; i < array.Length; i++)
				{
					ConsortiaInfo consortiumInfo = array[i];
					using (ConsortiaBussiness db = new ConsortiaBussiness())
					{
						currentPage = 1;
						totalApply = 0;
						allUserApply = db.GetConsortiaApplyUserPage(currentPage, pageSize, ref totalApply, 2, consortiumInfo.ConsortiaID, -1, -1);
					}
					if (allUserApply != null)
					{
						totalPage = (totalApply + pageSize - 1) / pageSize;
						while (currentPage++ <= totalPage)
						{
							ConsortiaApplyUserInfo[] array2 = allUserApply;
							for (int j = 0; j < array2.Length; j++)
							{
								ConsortiaApplyUserInfo userInfo = array2[j];
								if (DateTime.Compare(userInfo.ApplyDate.AddMinutes((double)SystemConsortiaMrg.minAcceptInterval), DateTime.Now) > 0)
								{
									continueFlag = false;
									break;
								}
								using (ConsortiaBussiness db = new ConsortiaBussiness())
								{
									ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
									if (db.PassConsortiaApplyUsers(userInfo.ID, consortiumInfo.ChairmanID, consortiumInfo.ChairmanName, consortiumInfo.ConsortiaID, ref msg, consortiaUserInfo, ref repute))
									{
										consortiaUserInfo.ConsortiaID = consortiumInfo.ConsortiaID;
										consortiaUserInfo.ConsortiaName = consortiumInfo.ConsortiaName;
										GSPacketIn pkg = new GSPacketIn(128, consortiumInfo.ChairmanID);
										pkg.WriteByte(1);
										pkg.WriteInt(consortiaUserInfo.ID);
										pkg.WriteBoolean(false);
										pkg.WriteInt(consortiaUserInfo.ConsortiaID);
										pkg.WriteString(consortiaUserInfo.ConsortiaName);
										pkg.WriteInt(consortiaUserInfo.UserID);
										pkg.WriteString(consortiaUserInfo.UserName);
										pkg.WriteInt(consortiumInfo.ChairmanID);
										pkg.WriteString(consortiumInfo.ChairmanName);
										pkg.WriteInt(consortiaUserInfo.DutyID);
										pkg.WriteString(consortiaUserInfo.DutyName);
										pkg.WriteInt(consortiaUserInfo.Offer);
										pkg.WriteInt(consortiaUserInfo.RichesOffer);
										pkg.WriteInt(consortiaUserInfo.RichesRob);
										pkg.WriteDateTime(consortiaUserInfo.LastDate);
										pkg.WriteInt(consortiaUserInfo.Grade);
										pkg.WriteInt(consortiaUserInfo.Level);
										pkg.WriteInt(consortiaUserInfo.State);
										pkg.WriteBoolean(consortiaUserInfo.Sex);
										pkg.WriteInt(consortiaUserInfo.Right);
										pkg.WriteInt(consortiaUserInfo.Win);
										pkg.WriteInt(consortiaUserInfo.Total);
										pkg.WriteInt(consortiaUserInfo.Escape);
										pkg.WriteInt(repute);
										pkg.WriteString(consortiaUserInfo.LoginName);
										pkg.WriteInt(consortiaUserInfo.FightPower);
										CenterServer.Instance.SendToALL(pkg, null);
									}
								}
							}
							if (!continueFlag)
							{
								break;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				
					SystemConsortiaMrg.log.Error("SystemConsortiaMrg.autoAcceptHandler", e);
				
			}
		}
	}
}
