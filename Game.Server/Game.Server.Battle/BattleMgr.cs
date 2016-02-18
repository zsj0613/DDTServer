using Bussiness;
using Game.Base;
using Game.Server.Rooms;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
namespace Game.Server.Battle
{
	public class BattleMgr
	{
		public static LogProvider log => LogProvider.Default;
		public static readonly int MAX_RECOUNT_MINS = 3;
		public static readonly int MAX_RECONNECT_TIME = 3600;
		public static List<BattleServer> m_list = new List<BattleServer>();
		private static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static string OpenBeginTime;
		public static string OpenCloseTime;
		public static bool AutoReconnect = true;
        public static bool IsOpenAreaFight = false;
		public static bool Setup()
		{
			BattleMgr.m_list.Clear();
            if (!File.Exists("battle.xml"))
            {
                log.Warn("Cannot find battle.xml");
            }
            if (File.Exists("battle.xml"))
			{
				try
				{
					XDocument doc = XDocument.Load("battle.xml");
					using (IEnumerator<XNode> enumerator = doc.Root.Nodes().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							XElement server = (XElement)enumerator.Current;
							try
							{
								int id = int.Parse(server.Attribute("id").Value);
								string ip = server.Attribute("ip").Value;
								int port = int.Parse(server.Attribute("port").Value);
								string key = server.Attribute("key").Value;
								bool isOpen = bool.Parse(server.Attribute("IsOpen").Value);
                                bool isArea = bool.Parse(server.Attribute("IsArea").Value);
                                BattleMgr.AddBattleServer(new BattleServer(id, ip, port, key, isOpen,isArea));
								BattleMgr.log.DebugFormat("Battle server {0}:{1} loaded...", ip, port);
							}
							catch (Exception ex)
							{
								BattleMgr.log.Error("BattleMgr setup error:", ex);
							}
						}
					}
                    if (m_list.Count == 2)
                    {
                        IsOpenAreaFight = true;
                    }
				}
				catch (Exception ex)
				{
					BattleMgr.log.Error("BattleMgr setup error:", ex);
				}
			}
			BattleMgr.log.InfoFormat("BattlMgr:total {0} battle server loaded.", BattleMgr.m_list.Count);
			return true;
		}
		public static void Start()
		{
			foreach (BattleServer server in BattleMgr.m_list)
			{
				server.Start();
			}
		}
		public static void Stop()
		{
			foreach (BattleServer server in BattleMgr.m_list)
			{
				server.Disconnected -= new EventHandler(BattleMgr.battle_Disconnected);
				server.Stop();
			}
		}
		public static BattleServer FindActiveServer(BaseRoom room)
		{
			int type = 0;			
			return BattleMgr.FindFightServerByType(type,room.IsArea);
		}
		public static BattleServer FindFightServerByType(int type,bool isArea)
		{
			List<BattleServer> list;
			Monitor.Enter(list = BattleMgr.m_list);
			BattleServer result;
			try
			{
				BattleServer m_server = null;
				foreach (BattleServer server in BattleMgr.m_list)
				{
					if (server.IsActive && server.ServerType == type && server.IsOpen&&server.IsArea == isArea)
					{
						if (m_server == null)
						{
							m_server = server;
						}
						else
						{
							if (server.RoomCount < m_server.RoomCount)
							{
								m_server = server;
							}
						}
					}
				}
				if (m_server != null)
				{
					m_server.RoomCount++;
					m_server.WaitingRoomCount++;
				}
				result = m_server;
			}
			finally
			{
				Monitor.Exit(list);
			}
			return result;
		}
		public static BattleServer AddRoom(BaseRoom room)
		{
			BattleServer server = BattleMgr.FindActiveServer(room);
			BattleServer result;
			if (server != null && server.AddRoom(room))
			{
				result = server;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static void AddBattleServer(BattleServer server)
		{
			if (server != null)
			{
				BattleMgr.m_list.Add(server);
				server.Disconnected += new EventHandler(BattleMgr.battle_Disconnected);
			}
		}
		public static void RemoveServer(BattleServer server)
		{
			if (server != null)
			{
				BattleMgr.m_list.Remove(server);
				server.Disconnected -= new EventHandler(BattleMgr.battle_Disconnected);
				server.Stop();
			}
		}
		public static void Disconnet(int id)
		{
			BattleServer server = BattleMgr.GetServer(id);
			if (server != null && server.IsActive)
			{
				server.LastRetryTime = DateTime.Now;
				server.RetryCount = BattleMgr.MAX_RECONNECT_TIME;
				server.Stop();
			}
		}
		public static void ReconnectAllBattle()
		{
			foreach (BattleServer server in BattleMgr.GetAllBattles())
			{
				if (server != null)
				{
					server.LastRetryTime = DateTime.Now;
					server.RetryCount = BattleMgr.MAX_RECONNECT_TIME;
					server.m_server_Disconnected(server.Server);
				}
			}
			BattleMgr.Setup();
			BattleMgr.Start();
		}
		public static BattleServer GetServer(int id)
		{
			BattleServer result;
			foreach (BattleServer server in BattleMgr.m_list)
			{
				if (server.ServerId == id)
				{
					result = server;
					return result;
				}
			}
			result = null;
			return result;
		}
		private static void battle_Disconnected(object sender, EventArgs e)
		{
			BattleServer server = sender as BattleServer;
			BattleMgr.log.ErrorFormat("Disconnect from battle server {0}:{1}", server.Ip, server.Port);
			if (server != null && BattleMgr.AutoReconnect && BattleMgr.m_list.Contains(server))
			{
				BattleMgr.RemoveServer(server);
				if ((DateTime.Now - server.LastRetryTime).TotalMinutes > (double)BattleMgr.MAX_RECOUNT_MINS)
				{
					server.RetryCount = 0;
				}
				if (server.RetryCount < BattleMgr.MAX_RECONNECT_TIME)
				{
					BattleServer newserver = server.Clone();
					BattleMgr.AddBattleServer(newserver);
					newserver.RetryCount = server.RetryCount + 1;
					newserver.LastRetryTime = DateTime.Now;
					newserver.Start();
				}
			}
		}
		public static List<BattleServer> GetAllBattles()
		{
			List<BattleServer> list;
			Monitor.Enter(list = BattleMgr.m_list);
			List<BattleServer> result;
			try
			{
				result = new List<BattleServer>(BattleMgr.m_list);
			}
			finally
			{
				Monitor.Exit(list);
			}
			return result;
		}
		private static bool IsValid(RateInfo areaFightInfo)
		{
			//DateTime arg_07_0 = areaFightInfo.BeginDay;
			//DateTime arg_0E_0 = areaFightInfo.EndDay;
			//bool flag = 0 == 0;
			return !(areaFightInfo.BeginTime.TimeOfDay > DateTime.Now.TimeOfDay) && !(areaFightInfo.EndTime.TimeOfDay < DateTime.Now.TimeOfDay);
		}
		public static void UpdateServerProperties()
		{
			List<BattleServer> servers = BattleMgr.GetAllBattles();
			foreach (BattleServer server in servers)
			{
				server.UpdateServerProperties();
			}
		}
	}
}
