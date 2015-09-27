using Game.Base.Packets;
using Game.Logic;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Timers;
namespace Game.Server.Managers
{
	public class MacroDropMgr
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static bool Init()
		{
			MacroDropMgr.m_lock = new ReaderWriterLock();
			return MacroDropMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				DropInfoMgr.DropInfo = new Dictionary<int, MacroDropInfo>();
				result = true;
				return result;
			}
			catch (Exception e)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("DropInfoMgr", e);
				}
			}
			result = false;
			return result;
		}
		private static void OnTimeEvent(object source, ElapsedEventArgs e)
		{
			Dictionary<int, int> tempDic = new Dictionary<int, int>();
			MacroDropMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, MacroDropInfo> kvp in DropInfoMgr.DropInfo)
				{
					int templateId = kvp.Key;
					MacroDropInfo macroDropInfo = kvp.Value;
					if (macroDropInfo.SelfDropCount > 0)
					{
						tempDic.Add(templateId, macroDropInfo.SelfDropCount);
						macroDropInfo.SelfDropCount = 0;
					}
				}
			}
			catch (Exception ex)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("DropInfoMgr OnTimeEvent", ex);
				}
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
			MacroDropMgr.SendMacroDrop(tempDic);
		}
		private static void SendMacroDrop(Dictionary<int, int> tempDic)
		{
			GSPacketIn pkg = new GSPacketIn(178);
			pkg.WriteInt(tempDic.Count);
			foreach (KeyValuePair<int, int> kvp in tempDic)
			{
				pkg.WriteInt(kvp.Key);
				pkg.WriteInt(kvp.Value);
			}
			GameServer.Instance.LoginServer.SendPacket(pkg);
		}
		public static void Start()
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(MacroDropMgr.OnTimeEvent);
			timer.Interval = 300000.0;
			timer.Enabled = true;
		}
		public static void UpdateDropInfo(Dictionary<int, MacroDropInfo> temp)
		{
			MacroDropMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, MacroDropInfo> kvp in temp)
				{
					if (DropInfoMgr.DropInfo.ContainsKey(kvp.Key))
					{
						DropInfoMgr.DropInfo[kvp.Key].DropCount = kvp.Value.DropCount;
						DropInfoMgr.DropInfo[kvp.Key].MaxDropCount = kvp.Value.MaxDropCount;
					}
					else
					{
						DropInfoMgr.DropInfo.Add(kvp.Key, kvp.Value);
					}
				}
			}
			catch (Exception e)
			{
				if (MacroDropMgr.log.IsErrorEnabled)
				{
					MacroDropMgr.log.Error("MacroDropMgr UpdateDropInfo", e);
				}
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
		}
	}
}
