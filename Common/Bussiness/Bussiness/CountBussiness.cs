using DAL;
using Lsj.Util.Logs;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Bussiness
{
	public class CountBussiness
	{
        private static LogProvider log => LogProvider.Default;
        private static string _connectionString;
		private static int _appID;
		private static int _subID;
		private static int _serverID;
		private static bool _conutRecord;
		public static string ConnectionString
		{
			get
			{
				return CountBussiness._connectionString;
			}
		}
		public static int AppID
		{
			get
			{
				return CountBussiness._appID;
			}
		}
		public static int SubID
		{
			get
			{
				return CountBussiness._subID;
			}
		}
		public static int ServerID
		{
			get
			{
				return CountBussiness._serverID;
			}
		}
		public static bool CountRecord
		{
			get
			{
				return CountBussiness._conutRecord;
			}
		}
		public static void SetConfig(string connectionString, int appID, int subID, int serverID, bool countRecord)
		{
			CountBussiness._connectionString = connectionString;
			CountBussiness._appID = appID;
			CountBussiness._subID = subID;
			CountBussiness._serverID = serverID;
			CountBussiness._conutRecord = countRecord;
		}
		public static void InsertGameInfo(DateTime begin, int mapID, int money, int gold, string users)
		{
			CountBussiness.InsertGameInfo(CountBussiness.AppID, CountBussiness.SubID, CountBussiness.ServerID, begin, DateTime.Now, users.Split(new char[]
			{
				','
			}).Length, mapID, money, gold, users);
		}
		public static void InsertGameInfo(int appid, int subid, int serverid, DateTime begin, DateTime end, int usercount, int mapID, int money, int gold, string users)
		{
			try
			{
				if (CountBussiness.CountRecord)
				{
					SqlHelper.BeginExecuteNonQuery(CountBussiness.ConnectionString, "SP_Insert_Count_FightInfo", new object[]
					{
						appid,
						subid,
						serverid,
						begin,
						end,
						usercount,
						mapID,
						money,
						gold,
						users
					});
				}
			}
			catch (Exception ex)
			{
				CountBussiness.log.Error("Insert Log Error!", ex);
			}
		}
		public static void InsertServerInfo(int usercount, int gamecount)
		{
			CountBussiness.InsertServerInfo(CountBussiness.AppID, CountBussiness.SubID, CountBussiness.ServerID, usercount, gamecount, DateTime.Now);
		}
		public static void InsertServerInfo(int appid, int subid, int serverid, int usercount, int gamecount, DateTime time)
		{
			try
			{
				if (CountBussiness.CountRecord)
				{
					SqlHelper.BeginExecuteNonQuery(CountBussiness.ConnectionString, "SP_Insert_Count_Server", new object[]
					{
						appid,
						subid,
						serverid,
						usercount,
						gamecount,
						time
					});
				}
			}
			catch (Exception ex)
			{
				CountBussiness.log.Error("Insert Log Error!!", ex);
			}
		}
		public static void InsertSystemPayCount(int consumerid, int money, int gold, int consumertype, int subconsumertype)
		{
			CountBussiness.InsertSystemPayCount(CountBussiness.AppID, CountBussiness.SubID, consumerid, money, gold, consumertype, subconsumertype, DateTime.Now);
		}
		public static void InsertSystemPayCount(int appid, int subid, int consumerid, int money, int gold, int consumertype, int subconsumertype, DateTime datime)
		{
			try
			{
				if (CountBussiness.CountRecord)
				{
					SqlHelper.BeginExecuteNonQuery(CountBussiness.ConnectionString, "SP_Insert_Count_SystemPay", new object[]
					{
						appid,
						subid,
						consumerid,
						money,
						gold,
						consumertype,
						subconsumertype,
						datime
					});
				}
			}
			catch (Exception ex)
			{
				CountBussiness.log.Error("InsertSystemPayCount Log Error!!!", ex);
			}
		}
		public static void InsertContentCount(Dictionary<string, string> clientInfos)
		{
			try
			{
				if (CountBussiness.CountRecord)
				{
					SqlHelper.BeginExecuteNonQuery(CountBussiness.ConnectionString, "Modify_Count_Content", new object[]
					{
						clientInfos["Application_Id"],
						clientInfos["Cpu"],
						clientInfos["OperSystem"],
						clientInfos["IP"],
						clientInfos["IPAddress"],
						clientInfos["NETCLR"],
						clientInfos["Browser"],
						clientInfos["ActiveX"],
						clientInfos["Cookies"],
						clientInfos["CSS"],
						clientInfos["Language"],
						clientInfos["Computer"],
						clientInfos["Platform"],
						clientInfos["Win16"],
						clientInfos["Win32"],
						clientInfos["Referry"],
						clientInfos["Redirect"],
						clientInfos["TimeSpan"],
						clientInfos["ScreenWidth"] + clientInfos["ScreenHeight"],
						clientInfos["Color"],
						clientInfos["Flash"],
						"Insert"
					});
				}
			}
			catch (Exception ex)
			{
				CountBussiness.log.Error("Insert Log Error!!!!", ex);
			}
		}
	}
}
