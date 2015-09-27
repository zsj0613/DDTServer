/*using Bussiness;
using Game.Logic;
using Game.Logic.LogEnum;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Data;
using System.Reflection;
using System.Threading;
namespace Game.Server.Statics
{
	public class LogMgr
	{
		public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static object _syncStop;
		private static int _gameType;
		private static int _serverId;
		private static int _areaId;
		public static DataTable m_LogItem;
		public static DataTable m_LogMoney;
		public static DataTable m_LogFight;
		public static DataTable m_LogWealth;
		public static DataTable m_LogDropItem;
		public static DataTable m_LogMail;
		public static bool Setup(int gametype, int areaid, int serverid)
		{
			LogMgr._gameType = gametype;
			LogMgr._areaId = areaid;
			LogMgr._serverId = serverid;
			LogMgr._syncStop = new object();
			LogMgr.m_LogItem = new DataTable("Log_Item");
			LogMgr.m_LogItem.Columns.Add("ApplicationId", Type.GetType("System.Int32"));
			LogMgr.m_LogItem.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogItem.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogItem.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
			LogMgr.m_LogItem.Columns.Add("UserId", typeof(int));
			LogMgr.m_LogItem.Columns.Add("Operation", typeof(int));
			LogMgr.m_LogItem.Columns.Add("ItemName", typeof(string));
			LogMgr.m_LogItem.Columns.Add("ItemID", typeof(int));
			LogMgr.m_LogItem.Columns.Add("AddItem", typeof(string));
			LogMgr.m_LogItem.Columns.Add("BeginProperty", typeof(string));
			LogMgr.m_LogItem.Columns.Add("EndProperty", typeof(string));
			LogMgr.m_LogItem.Columns.Add("Result", typeof(int));
			LogMgr.m_LogMoney = new DataTable("Log_Money");
			LogMgr.m_LogMoney.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("MastType", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("SonType", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("UserId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
			LogMgr.m_LogMoney.Columns.Add("Moneys", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("Gold", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("GiftToken", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("Offer", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("OtherPay", typeof(string));
			LogMgr.m_LogMoney.Columns.Add("GoodId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("ShopId", typeof(int));
			LogMgr.m_LogMoney.Columns.Add("Datas", typeof(int));
			LogMgr.m_LogFight = new DataTable("Log_Fight");
			LogMgr.m_LogFight.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("RoomId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("RoomType", typeof(int));
			LogMgr.m_LogFight.Columns.Add("FightType", typeof(int));
			LogMgr.m_LogFight.Columns.Add("ChangeTeam", typeof(int));
			LogMgr.m_LogFight.Columns.Add("PlayBegin", Type.GetType("System.DateTime"));
			LogMgr.m_LogFight.Columns.Add("PlayEnd", Type.GetType("System.DateTime"));
			LogMgr.m_LogFight.Columns.Add("UserCount", typeof(int));
			LogMgr.m_LogFight.Columns.Add("MapId", typeof(int));
			LogMgr.m_LogFight.Columns.Add("TeamA", typeof(string));
			LogMgr.m_LogFight.Columns.Add("TeamB", typeof(string));
			LogMgr.m_LogFight.Columns.Add("PlayResult", typeof(string));
			LogMgr.m_LogFight.Columns.Add("WinTeam", typeof(int));
			LogMgr.m_LogFight.Columns.Add("Detail", typeof(string));
			LogMgr.m_LogWealth = new DataTable("Log_Wealth");
			LogMgr.m_LogWealth.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogWealth.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogWealth.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogWealth.Columns.Add("MastType", typeof(int));
			LogMgr.m_LogWealth.Columns.Add("SonType", typeof(int));
			LogMgr.m_LogWealth.Columns.Add("UserId", typeof(int));
			LogMgr.m_LogWealth.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
			LogMgr.m_LogWealth.Columns.Add("Moneys", typeof(int));
			LogMgr.m_LogWealth.Columns.Add("SpareMoney", typeof(int));
			LogMgr.m_LogDropItem = new DataTable("Log_DropItem");
			LogMgr.m_LogDropItem.Columns.Add("ApplicationId", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("SubId", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("LineId", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("UserId", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("ItemId", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("TemplateID", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("DropId", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("DropData", typeof(int));
			LogMgr.m_LogDropItem.Columns.Add("EnterTime", Type.GetType("System.DateTime"));
			LogMgr.m_LogMail = new DataTable("Log_Mail");
			LogMgr.m_LogMail.Columns.Add("UserID", typeof(int));
			LogMgr.m_LogMail.Columns.Add("MailID", typeof(int));
			LogMgr.m_LogMail.Columns.Add("Money", typeof(int));
			LogMgr.m_LogMail.Columns.Add("GiftToken", typeof(int));
			LogMgr.m_LogMail.Columns.Add("Annex1", typeof(string));
			LogMgr.m_LogMail.Columns.Add("Annex2", typeof(string));
			LogMgr.m_LogMail.Columns.Add("Annex3", typeof(string));
			LogMgr.m_LogMail.Columns.Add("Annex4", typeof(string));
			LogMgr.m_LogMail.Columns.Add("Annex5", typeof(string));
			return true;
		}
		public static void Reset()
		{
			DataTable obj;
			Monitor.Enter(obj = LogMgr.m_LogItem);
			try
			{
				LogMgr.m_LogItem.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			Monitor.Enter(obj = LogMgr.m_LogMoney);
			try
			{
				LogMgr.m_LogMoney.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			Monitor.Enter(obj = LogMgr.m_LogFight);
			try
			{
				LogMgr.m_LogFight.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			Monitor.Enter(obj = LogMgr.m_LogWealth);
			try
			{
				LogMgr.m_LogWealth.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			Monitor.Enter(obj = LogMgr.m_LogDropItem);
			try
			{
				LogMgr.m_LogDropItem.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			Monitor.Enter(obj = LogMgr.m_LogMail);
			try
			{
				LogMgr.m_LogMail.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static void Save()
		{
			if (LogMgr._syncStop != null)
			{
				object syncStop;
				Monitor.Enter(syncStop = LogMgr._syncStop);
				try
				{
					using (ItemRecordBussiness db = new ItemRecordBussiness())
					{
						LogMgr.SaveLogItem(db);
						LogMgr.SaveLogMoney(db);
						LogMgr.SaveLogFight(db);
						LogMgr.SaveLogWealth(db);
						LogMgr.SaveLogDropItem(db);
						LogMgr.SaveLogMail(db);
					}
				}
				finally
				{
					Monitor.Exit(syncStop);
				}
			}
		}
		public static void SaveLogItem(ItemRecordBussiness db)
		{
			DataTable logItem;
			Monitor.Enter(logItem = LogMgr.m_LogItem);
			try
			{
				db.LogItemDb(LogMgr.m_LogItem);
			}
			finally
			{
				Monitor.Exit(logItem);
			}
		}
		public static void SaveLogMoney(ItemRecordBussiness db)
		{
			DataTable logMoney;
			Monitor.Enter(logMoney = LogMgr.m_LogMoney);
			try
			{
				db.LogMoneyDb(LogMgr.m_LogMoney);
			}
			finally
			{
				Monitor.Exit(logMoney);
			}
		}
		public static void SaveLogWealth(ItemRecordBussiness db)
		{
			DataTable logWealth;
			Monitor.Enter(logWealth = LogMgr.m_LogWealth);
			try
			{
				db.LogWealthDb(LogMgr.m_LogWealth);
			}
			finally
			{
				Monitor.Exit(logWealth);
			}
		}
		public static void SaveLogFight(ItemRecordBussiness db)
		{
			DataTable logFight;
			Monitor.Enter(logFight = LogMgr.m_LogFight);
			try
			{
				db.LogFightDb(LogMgr.m_LogFight);
			}
			finally
			{
				Monitor.Exit(logFight);
			}
		}
		public static void SaveLogDropItem(ItemRecordBussiness db)
		{
			DataTable logDropItem;
			Monitor.Enter(logDropItem = LogMgr.m_LogDropItem);
			try
			{
				db.LogDropItemDb(LogMgr.m_LogDropItem);
			}
			finally
			{
				Monitor.Exit(logDropItem);
			}
		}
		public static void SaveLogMail(ItemRecordBussiness db)
		{
			DataTable logMail;
			Monitor.Enter(logMail = LogMgr.m_LogMail);
			try
			{
				db.LogMailDB(LogMgr.m_LogMail);
			}
			finally
			{
				Monitor.Exit(logMail);
			}
		}
		public static void LogItemAdd(int userId, LogItemType itemType, string beginProperty, ItemInfo item, string AddItem, int result)
		{
			try
			{
				string endProperty = "";
				if (item != null)
				{
					endProperty = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", new object[]
					{
						item.StrengthenLevel,
						item.Attack,
						item.Defence,
						item.Agility,
						item.Luck,
						item.AttackCompose,
						item.DefendCompose,
						item.AgilityCompose,
						item.LuckCompose
					});
				}
				object[] info = new object[]
				{
					LogMgr._gameType,
					LogMgr._areaId,
					LogMgr._serverId,
					DateTime.Now,
					userId,
					(int)itemType,
					(item == null) ? "" : item.Template.Name,
					(item == null) ? 0 : item.ItemID,
					AddItem,
					beginProperty,
					endProperty,
					result
				};
				DataTable logItem;
				Monitor.Enter(logItem = LogMgr.m_LogItem);
				try
				{
					LogMgr.m_LogItem.Rows.Add(info);
				}
				finally
				{
					Monitor.Exit(logItem);
				}
			}
			catch (Exception e)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：ItemAdd @ " + e);
				}
			}
		}
		public static void LogMoneyAdd(LogMoneyType masterType, LogMoneyType sonType, int userId, int moneys, int gold, int giftToken, int offer, string otherPay, int goodId, int ShopId, int Datas)
		{
			try
			{
				object[] info = new object[]
				{
					LogMgr._gameType,
					LogMgr._areaId,
					LogMgr._serverId,
					masterType,
					sonType,
					userId,
					DateTime.Now,
					moneys,
					gold,
					giftToken,
					offer,
					otherPay,
					goodId,
					ShopId,
					Datas
				};
				DataTable logMoney;
				Monitor.Enter(logMoney = LogMgr.m_LogMoney);
				try
				{
					if (moneys > 0 || giftToken > 0 || offer > 0 || otherPay != "")
					{
						LogMgr.m_LogMoney.Rows.Add(info);
					}
				}
				finally
				{
					Monitor.Exit(logMoney);
				}
			}
			catch (Exception e)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：LogMoney @ " + e);
				}
			}
		}
		public static void LogWealthAdd(LogMoneyType masterType, LogMoneyType sonType, int userId, int moneys, int SpareMoney)
		{
			try
			{
				object[] info = new object[]
				{
					LogMgr._gameType,
					LogMgr._areaId,
					LogMgr._serverId,
					masterType,
					sonType,
					userId,
					DateTime.Now,
					moneys,
					SpareMoney
				};
				DataTable logWealth;
				Monitor.Enter(logWealth = LogMgr.m_LogWealth);
				try
				{
					LogMgr.m_LogWealth.Rows.Add(info);
				}
				finally
				{
					Monitor.Exit(logWealth);
				}
			}
			catch (Exception e)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：LogWealth @ " + e);
				}
			}
		}
		public static void LogFightAdd(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar)
		{
			try
			{
				object[] info = new object[]
				{
					LogMgr._gameType,
					LogMgr._areaId,
					LogMgr._serverId,
					roomId,
					(int)roomType,
					(int)fightType,
					changeTeam,
					playBegin,
					playEnd,
					userCount,
					mapId,
					teamA,
					teamB,
					playResult,
					winTeam,
					BossWar
				};
				DataTable logFight;
				Monitor.Enter(logFight = LogMgr.m_LogFight);
				try
				{
					LogMgr.m_LogFight.Rows.Add(info);
				}
				finally
				{
					Monitor.Exit(logFight);
				}
			}
			catch (Exception e)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：Fight @ " + e);
				}
			}
		}
		public static void LogDropItemAdd(int userId, int itemId, int templateId, int dropId, int dropData)
		{
			try
			{
				object[] info = new object[]
				{
					LogMgr._gameType,
					LogMgr._areaId,
					LogMgr._serverId,
					userId,
					itemId,
					templateId,
					dropId,
					dropData,
					DateTime.Now
				};
				DataTable logDropItem;
				Monitor.Enter(logDropItem = LogMgr.m_LogDropItem);
				try
				{
					LogMgr.m_LogDropItem.Rows.Add(info);
				}
				finally
				{
					Monitor.Exit(logDropItem);
				}
			}
			catch (Exception e)
			{
				if (LogMgr.log.IsErrorEnabled)
				{
					LogMgr.log.Error("LogMgr Error：LogDropItem @ " + e);
				}
			}
		}
		public static void LogMailDelete(int userid, MailInfo mail)
		{
			if (!string.IsNullOrEmpty(mail.Annex1) || !string.IsNullOrEmpty(mail.Annex2) || !string.IsNullOrEmpty(mail.Annex3) || !string.IsNullOrEmpty(mail.Annex4) || !string.IsNullOrEmpty(mail.Annex5) || mail.Money != 0 || mail.GiftToken != 0)
			{
				try
				{
					object[] info = new object[]
					{
						userid,
						mail.ID,
						mail.Money,
						mail.GiftToken,
						mail.Annex1,
						mail.Annex2,
						mail.Annex3,
						mail.Annex4,
						mail.Annex5
					};
					DataTable logMail;
					Monitor.Enter(logMail = LogMgr.m_LogMail);
					try
					{
						LogMgr.m_LogMail.Rows.Add(info);
					}
					finally
					{
						Monitor.Exit(logMail);
					}
				}
				catch (Exception e)
				{
					if (LogMgr.log.IsErrorEnabled)
					{
						LogMgr.log.Error("LogMgr Error:LogMail @" + e);
					}
				}
			}
		}
	}
}*/
