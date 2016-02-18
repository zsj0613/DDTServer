using Bussiness;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Center.Server.Managers
{
	public class ServerMgr
	{
		private static LogProvider log => CenterServer.log;
		private static Dictionary<int, ServerInfo> _list = new Dictionary<int, ServerInfo>();
		private static object _syncStop = new object();
		public static ServerInfo[] Servers
		{
			get
			{
				return ServerMgr._list.Values.ToArray<ServerInfo>();
			}
		}
		public static bool Start()
		{
			bool result;
			try
			{
				using (ServiceBussiness db = new ServiceBussiness())
				{
					ServerInfo[] list = db.GetServerList();
					ServerInfo[] array = list;
					for (int i = 0; i < array.Length; i++)
					{
						ServerInfo s = array[i];
						s.State = 1;
						s.Online = 0;
						ServerMgr._list.Add(s.ID, s);
					}
				}
				ServerMgr.log.Info("Load server list from db.");
				result = true;
			}
			catch (Exception ex)
			{
				ServerMgr.log.ErrorFormat("Load server list from db failed:{0}", ex);
				result = false;
			}
			return result;
		}
		public static bool ReLoadServerList()
		{
			bool result;
			try
			{
				using (ServiceBussiness db = new ServiceBussiness())
				{
					object syncStop;
					Monitor.Enter(syncStop = ServerMgr._syncStop);
					try
					{
						ServerInfo[] list = db.GetServerList();
						ServerInfo[] array = list;
						for (int i = 0; i < array.Length; i++)
						{
							ServerInfo s = array[i];
							if (ServerMgr._list.ContainsKey(s.ID))
							{
								ServerMgr._list[s.ID].IP = s.IP;
								ServerMgr._list[s.ID].Name = s.Name;
								ServerMgr._list[s.ID].Port = s.Port;
								ServerMgr._list[s.ID].Room = s.Room;
								ServerMgr._list[s.ID].Total = s.Total;
								ServerMgr._list[s.ID].MustLevel = s.MustLevel;
								ServerMgr._list[s.ID].LowestLevel = s.LowestLevel;
								ServerMgr._list[s.ID].Online = s.Online;
								ServerMgr._list[s.ID].State = s.State;
							}
							else
							{
								s.State = 1;
								s.Online = 0;
								ServerMgr._list.Add(s.ID, s);
							}
							ServerMgr.log.InfoFormat("Load server [{0}]  {1}   {2} {3}", new object[]
							{
								s.Name,
								s.ID,
								s.Port,
								s.State
							});
						}
					}
					finally
					{
						Monitor.Exit(syncStop);
					}
				}
				ServerMgr.log.InfoFormat("Load server list completed!", new object[0]);
				result = true;
			}
			catch (Exception ex)
			{
				ServerMgr.log.ErrorFormat("ReLoad server list from db failed:{0}", ex);
				result = false;
			}
			return result;
		}
		public static ServerInfo GetServerInfo(int id)
		{
			ServerInfo result;
			if (ServerMgr._list.ContainsKey(id))
			{
				result = ServerMgr._list[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static int GetState(int count, int total)
		{
			int result;
			if (count >= total)
			{
				result = 5;
			}
			else
			{
				if ((double)count > (double)total * 0.5)
				{
					result = 4;
				}
				else
				{
					result = 2;
				}
			}
			return result;
		}
		public static void SaveToDatabase()
		{
			try
			{
				using (ServiceBussiness db = new ServiceBussiness())
				{
					foreach (ServerInfo info in ServerMgr._list.Values)
					{
						db.UpdateService(info);
					}
				}
			}
			catch (Exception ex)
			{
				ServerMgr.log.Error("Save server state", ex);
			}
		}
	}
}
