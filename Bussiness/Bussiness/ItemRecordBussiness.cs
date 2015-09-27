/*
using SqlDataProvider.Data;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
    
	public class ItemRecordBussiness : BaseBussiness
	{
		public static void FusionItem(ItemInfo item, ref string property)
		{
			if (item != null)
			{
				property = property + string.Format("{0}:{1},{2}", item.ItemID, item.Template.Name, Convert.ToInt32(item.IsBinds)) + "|";
			}
		}
		public bool LogItemDb(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_Item";
					sqlbulk.ColumnMappings.Add(0, "ApplicationId");
					sqlbulk.ColumnMappings.Add(1, "SubId");
					sqlbulk.ColumnMappings.Add(2, "LineId");
					sqlbulk.ColumnMappings.Add(3, "EnterTime");
					sqlbulk.ColumnMappings.Add(4, "UserId");
					sqlbulk.ColumnMappings.Add(5, "Operation");
					sqlbulk.ColumnMappings.Add(6, "ItemName");
					sqlbulk.ColumnMappings.Add(7, "ItemID");
					sqlbulk.ColumnMappings.Add(8, "AddItem");
					sqlbulk.ColumnMappings.Add(9, "BeginProperty");
					sqlbulk.ColumnMappings.Add(10, "EndProperty");
					sqlbulk.ColumnMappings.Add(11, "Result");
					sqlbulk.WriteToServer(dt);
					result = true;
					dt.Clear();
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("Smith Log Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
				}
				result2 = result;
			}
			return result2;
		}
		public bool LogMoneyDb(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_Money";
					sqlbulk.ColumnMappings.Add(0, "ApplicationId");
					sqlbulk.ColumnMappings.Add(1, "SubId");
					sqlbulk.ColumnMappings.Add(2, "LineId");
					sqlbulk.ColumnMappings.Add(3, "MastType");
					sqlbulk.ColumnMappings.Add(4, "SonType");
					sqlbulk.ColumnMappings.Add(5, "UserId");
					sqlbulk.ColumnMappings.Add(6, "EnterTime");
					sqlbulk.ColumnMappings.Add(7, "Moneys");
					sqlbulk.ColumnMappings.Add(8, "Gold");
					sqlbulk.ColumnMappings.Add(9, "GiftToken");
					sqlbulk.ColumnMappings.Add(10, "Offer");
					sqlbulk.ColumnMappings.Add(11, "OtherPay");
					sqlbulk.ColumnMappings.Add(12, "GoodId");
					sqlbulk.ColumnMappings.Add(13, "ShopId");
					sqlbulk.ColumnMappings.Add(14, "Datas");
					sqlbulk.WriteToServer(dt);
					result = true;
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("Money Log Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
					dt.Clear();
				}
				result2 = result;
			}
			return result2;
		}
		public bool LogWealthDb(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_Wealth";
					sqlbulk.ColumnMappings.Add(0, "ApplicationId");
					sqlbulk.ColumnMappings.Add(1, "SubId");
					sqlbulk.ColumnMappings.Add(2, "LineId");
					sqlbulk.ColumnMappings.Add(3, "MastType");
					sqlbulk.ColumnMappings.Add(4, "SonType");
					sqlbulk.ColumnMappings.Add(5, "UserId");
					sqlbulk.ColumnMappings.Add(6, "EnterTime");
					sqlbulk.ColumnMappings.Add(7, "Moneys");
					sqlbulk.ColumnMappings.Add(8, "SpareMoney");
					sqlbulk.WriteToServer(dt);
					result = true;
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("Wealth Log Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
					dt.Clear();
				}
				result2 = result;
			}
			return result2;
		}
		public bool LogFightDb(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_Fight";
					sqlbulk.ColumnMappings.Add(0, "ApplicationId");
					sqlbulk.ColumnMappings.Add(1, "SubId");
					sqlbulk.ColumnMappings.Add(2, "LineId");
					sqlbulk.ColumnMappings.Add(3, "RoomId");
					sqlbulk.ColumnMappings.Add(4, "RoomType");
					sqlbulk.ColumnMappings.Add(5, "FightType");
					sqlbulk.ColumnMappings.Add(6, "ChangeTeam");
					sqlbulk.ColumnMappings.Add(7, "PlayBegin");
					sqlbulk.ColumnMappings.Add(8, "PlayEnd");
					sqlbulk.ColumnMappings.Add(9, "UserCount");
					sqlbulk.ColumnMappings.Add(10, "MapId");
					sqlbulk.ColumnMappings.Add(11, "TeamA");
					sqlbulk.ColumnMappings.Add(12, "TeamB");
					sqlbulk.ColumnMappings.Add(13, "PlayResult");
					sqlbulk.ColumnMappings.Add(14, "WinTeam");
					sqlbulk.ColumnMappings.Add(15, "Detail");
					sqlbulk.WriteToServer(dt);
					result = true;
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("Fight Log Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
					dt.Clear();
				}
				result2 = result;
			}
			return result2;
		}
		public bool LogServerDb(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_Server";
					sqlbulk.ColumnMappings.Add(0, "ApplicationId");
					sqlbulk.ColumnMappings.Add(1, "SubId");
					sqlbulk.ColumnMappings.Add(2, "EnterTime");
					sqlbulk.ColumnMappings.Add(3, "Online");
					sqlbulk.ColumnMappings.Add(4, "Reg");
					sqlbulk.WriteToServer(dt);
					result = true;
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("Server Log Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
					dt.Clear();
				}
				result2 = result;
			}
			return result2;
		}
		public bool LogServerOnlineDb(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_ServerOnline";
					sqlbulk.ColumnMappings.Add(0, "ServerID");
					sqlbulk.ColumnMappings.Add(1, "EnterTime");
					sqlbulk.ColumnMappings.Add(2, "Online");
					sqlbulk.WriteToServer(dt);
					result = true;
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("Server Log Online Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
					dt.Clear();
				}
				result2 = result;
			}
			return result2;
		}
		public bool LogDropItemDb(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_DropItem";
					sqlbulk.ColumnMappings.Add(0, "ApplicationId");
					sqlbulk.ColumnMappings.Add(1, "SubId");
					sqlbulk.ColumnMappings.Add(2, "LineId");
					sqlbulk.ColumnMappings.Add(3, "UserId");
					sqlbulk.ColumnMappings.Add(4, "ItemId");
					sqlbulk.ColumnMappings.Add(5, "TemplateID");
					sqlbulk.ColumnMappings.Add(6, "DropId");
					sqlbulk.ColumnMappings.Add(7, "DropData");
					sqlbulk.ColumnMappings.Add(8, "EnterTime");
					sqlbulk.WriteToServer(dt);
					result = true;
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("DropItem Log Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
					dt.Clear();
				}
				result2 = result;
			}
			return result2;
		}
		public bool LogMailDB(DataTable dt)
		{
			bool result = false;
			bool result2;
			if (dt == null)
			{
				result2 = result;
			}
			else
			{
				SqlBulkCopy sqlbulk = new SqlBulkCopy(ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
				try
				{
					sqlbulk.NotifyAfter = dt.Rows.Count;
					sqlbulk.DestinationTableName = "Log_Mail";
					sqlbulk.ColumnMappings.Add(0, "UserID");
					sqlbulk.ColumnMappings.Add(1, "MailID");
					sqlbulk.ColumnMappings.Add(2, "Money");
					sqlbulk.ColumnMappings.Add(3, "GiftToken");
					sqlbulk.ColumnMappings.Add(4, "Annex1");
					sqlbulk.ColumnMappings.Add(5, "Annex2");
					sqlbulk.ColumnMappings.Add(6, "Annex3");
					sqlbulk.ColumnMappings.Add(7, "Annex4");
					sqlbulk.ColumnMappings.Add(8, "Annex5");
					sqlbulk.WriteToServer(dt);
					result = true;
				}
				catch (Exception ex)
				{
					if (BaseBussiness.log.IsErrorEnabled)
					{
						BaseBussiness.log.Error("Log_Mail Log Error:" + ex.ToString());
					}
				}
				finally
				{
					sqlbulk.Close();
					dt.Clear();
				}
				result2 = result;
			}
			return result2;
		}
	}
}*/
