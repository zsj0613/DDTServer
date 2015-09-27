/*
using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Threading;
namespace Center.Server.Statics
{
	public class LogMgr
	{
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static object _syncStop = new object();
		private static int _gameType;
		private static int _serverId;
		private static int _areaId;
		public static DataTable m_LogServer;
		public static DataTable m_LogServerOnline;
		private static int regCount;
		public static object _sysObj = new object();
		public static int GameType
		{
			get
			{
				return int.Parse(ConfigurationSettings.AppSettings["GameType"]);
			}
		}
		public static int ServerID
		{
			get
			{
				return int.Parse(ConfigurationSettings.AppSettings["ServerID"]);
			}
		}
		public static int AreaID
		{
			get
			{
				return int.Parse(ConfigurationSettings.AppSettings["AreaID"]);
			}
		}
		public static int SaveRecordSecond
		{
			get
			{
				return int.Parse(ConfigurationSettings.AppSettings["SaveRecordInterval"]) * 60;
			}
		}
		public static int RegCount
		{
			get
			{
				object sysObj;
				Monitor.Enter(sysObj = LogMgr._sysObj);
				int result;
				try
				{
					result = LogMgr.regCount;
				}
				finally
				{
					Monitor.Exit(sysObj);
				}
				return result;
			}
			set
			{
				object sysObj;
				Monitor.Enter(sysObj = LogMgr._sysObj);
				try
				{
					LogMgr.regCount = value;
				}
				finally
				{
					Monitor.Exit(sysObj);
				}
			}
		}
		public static bool Setup()
		{
			return LogMgr.Setup(LogMgr.GameType, LogMgr.AreaID, LogMgr.ServerID);
		}
		public static bool Setup(int gametype, int areaid, int serverid)
		{
			LogMgr._gameType = gametype;
			LogMgr._serverId = serverid;
			LogMgr._areaId = areaid;
			LogMgr.m_LogServer = new DataTable("Log_Server");
			LogMgr.m_LogServer.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogServer.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogServer.Columns.Add("EnterTime", typeof(DateTime));
			LogMgr.m_LogServer.Columns.Add("Online", typeof(int));
			LogMgr.m_LogServer.Columns.Add("Reg", typeof(int));
			LogMgr.m_LogServerOnline = new DataTable("LogServerOnline");
			LogMgr.m_LogServerOnline.Columns.Add("ServerID", typeof(int));
			LogMgr.m_LogServerOnline.Columns.Add("EnterTime", typeof(DateTime));
			LogMgr.m_LogServerOnline.Columns.Add("Online", typeof(int));
			return true;
		}
		public static void Reset()
		{
			DataTable logServer;
			Monitor.Enter(logServer = LogMgr.m_LogServer);
			try
			{
				LogMgr.m_LogServer.Clear();
			}
			finally
			{
				Monitor.Exit(logServer);
			}
		}
		public static void Save()
		{
			if (LogMgr.m_LogServer == null)
			{
				LogMgr.Setup();
			}
			int online = LoginMgr.GetOnlineCount();
			object[] info = new object[]
			{
				LogMgr._gameType,
				LogMgr._serverId,
				DateTime.Now,
				online,
				LogMgr.RegCount
			};
			DataTable obj;
			Monitor.Enter(obj = LogMgr.m_LogServer);
			try
			{
				LogMgr.m_LogServer.Rows.Add(info);
			}
			finally
			{
				Monitor.Exit(obj);
			}
			LogMgr.RegCount = 0;
			int interval = LogMgr.SaveRecordSecond;
			using (ItemRecordBussiness db = new ItemRecordBussiness())
			{
				db.LogServerDb(LogMgr.m_LogServer);
			}
			ServerInfo[] servers = ServerMgr.Servers;
			for (int i = 0; i < servers.Length; i++)
			{
				ServerInfo server = servers[i];
				object[] onlineInfo = new object[]
				{
					server.ID,
					DateTime.Now,
					server.Online
				};
				Monitor.Enter(obj = LogMgr.m_LogServerOnline);
				try
				{
					LogMgr.m_LogServerOnline.Rows.Add(onlineInfo);
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
			using (ItemRecordBussiness db = new ItemRecordBussiness())
			{
				db.LogServerOnlineDb(LogMgr.m_LogServerOnline);
			}
		}
		public static void AddRegCount()
		{
			object sysObj;
			Monitor.Enter(sysObj = LogMgr._sysObj);
			try
			{
				LogMgr.regCount++;
			}
			finally
			{
				Monitor.Exit(sysObj);
			}
		}
	}
}*/
