using Bussiness;
using Game.Base.Packets;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
namespace Center.Server.Managers
{
	public class MacroDropMgr
	{
        private static LogProvider log => CenterServer.log;
		private static ReaderWriterLock m_lock;
		private static Dictionary<int, DropInfo> m_DropInfo;
		private static string FilePath;
		private static int counter;
		public static bool Init()
		{
			MacroDropMgr.m_lock = new ReaderWriterLock();
			MacroDropMgr.FilePath = Directory.GetCurrentDirectory() + "\\macrodrop\\macroDrop.ini";
			return MacroDropMgr.Reload();
		}
		public static bool Reload()
		{
			bool result;
			try
			{
				Dictionary<int, DropInfo> tempInfo = new Dictionary<int, DropInfo>();
				MacroDropMgr.m_DropInfo = new Dictionary<int, DropInfo>();
				tempInfo = MacroDropMgr.LoadDropInfo();
				if (tempInfo != null && tempInfo.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, DropInfo>>(ref MacroDropMgr.m_DropInfo, tempInfo);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				
					MacroDropMgr.log.Error("DropInfoMgr", e);
				
			}
			result = false;
			return result;
		}
		private static void MacroDropReset()
		{
			MacroDropMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, DropInfo> kvp in MacroDropMgr.m_DropInfo)
				{
					int templateId = kvp.Key;
					DropInfo dropInfo = kvp.Value;
					if (MacroDropMgr.counter > dropInfo.Time && dropInfo.Time > 0)
					{
						if (MacroDropMgr.counter % dropInfo.Time == 0)
						{
							dropInfo.Count = dropInfo.MaxCount;
						}
					}
				}
			}
			catch (Exception e)
			{
				
					MacroDropMgr.log.Error("DropInfoMgr MacroDropReset", e);
				
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
		}
		private static void MacroDropSync()
		{
			bool syncMacroDrop = true;
			ServerClient[] serverClients = CenterServer.Instance.GetAllClients();
			ServerClient[] array = serverClients;
			for (int i = 0; i < array.Length; i++)
			{
				ServerClient serverClient = array[i];
				if (!serverClient.NeedSyncMacroDrop)
				{
					syncMacroDrop = false;
					break;
				}
			}
			if (serverClients.Length > 0 && syncMacroDrop)
			{
				GSPacketIn pkg = new GSPacketIn(178);
				int count = MacroDropMgr.m_DropInfo.Count;
				pkg.WriteInt(count);
				MacroDropMgr.m_lock.AcquireReaderLock(-1);
				try
				{
					foreach (KeyValuePair<int, DropInfo> kvp in MacroDropMgr.m_DropInfo)
					{
						DropInfo di = kvp.Value;
						pkg.WriteInt(di.ID);
						pkg.WriteInt(di.Count);
						pkg.WriteInt(di.MaxCount);
					}
				}
				catch (Exception e)
				{
					
						MacroDropMgr.log.Error("DropInfoMgr MacroDropReset", e);
					
				}
				finally
				{
					MacroDropMgr.m_lock.ReleaseReaderLock();
				}
				array = serverClients;
				for (int i = 0; i < array.Length; i++)
				{
					ServerClient serverClient = array[i];
					serverClient.NeedSyncMacroDrop = false;
					serverClient.SendTCP(pkg);
				}
			}
		}
		private static void OnTimeEvent(object source, ElapsedEventArgs e)
		{
			MacroDropMgr.counter++;
			if (MacroDropMgr.counter % 12 == 0)
			{
				MacroDropMgr.MacroDropReset();
			}
			MacroDropMgr.MacroDropSync();
		}
		public static bool Start()
		{
			MacroDropMgr.counter = 0;
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(MacroDropMgr.OnTimeEvent);
			timer.Interval = 300000.0;
			timer.Enabled = true;
            return true;
		}
		private static Dictionary<int, DropInfo> LoadDropInfo()
		{
			Dictionary<int, DropInfo> items = new Dictionary<int, DropInfo>();
			Dictionary<int, DropInfo> result;
			if (File.Exists(MacroDropMgr.FilePath))
			{
				IniReader reader = new IniReader(MacroDropMgr.FilePath);
				int i = 1;
				while (reader.GetIniString(i.ToString(), "TemplateId") != "")
				{
					string section = i.ToString();
					int id = Convert.ToInt32(reader.GetIniString(section, "TemplateId"));
					int time = Convert.ToInt32(reader.GetIniString(section, "Time"));
					int count = Convert.ToInt32(reader.GetIniString(section, "Count"));
					DropInfo info = new DropInfo(id, time, count, count);
					items.Add(info.ID, info);
					i++;
				}
				result = items;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static void DropNotice(Dictionary<int, int> temp)
		{
			MacroDropMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				foreach (KeyValuePair<int, int> kvp in temp)
				{
					if (MacroDropMgr.m_DropInfo.ContainsKey(kvp.Key))
					{
						DropInfo dropInfo = MacroDropMgr.m_DropInfo[kvp.Key];
						if (dropInfo.Count > 0)
						{
							dropInfo.Count -= kvp.Value;
						}
					}
				}
			}
			catch (Exception ex)
			{
				
					MacroDropMgr.log.Error("DropInfoMgr CanDrop", ex);
				
			}
			finally
			{
				MacroDropMgr.m_lock.ReleaseWriterLock();
			}
		}
	}
}
