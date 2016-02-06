using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ServiceBussiness : BaseBussiness
	{
		public ServerInfo GetServiceSingle(int ID)
		{
			SqlDataReader reader = null;
			ServerInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				para[0].Value = ID;
				this.db.GetReader(ref reader, "SP_Service_Single", para);
				if (reader.Read())
				{
					result = new ServerInfo
					{
						ID = (int)reader["ID"],
						IP = reader["IP"].ToString(),
						Name = reader["Name"].ToString(),
						Online = (int)reader["Online"],
						Port = (int)reader["Port"],
						Remark = reader["Remark"].ToString(),
						Room = (int)reader["Room"],
						State = (int)reader["State"],
						Total = (int)reader["Total"],
						RSA = reader["RSA"].ToString(),
						NewerServer = (bool)reader["NewerServer"]
					};
					return result;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = null;
			return result;
		}
		public ServerInfo[] GetServiceByIP(string IP)
		{
			List<ServerInfo> infos = new List<ServerInfo>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@IP", SqlDbType.NVarChar, 50)
				};
				para[0].Value = IP;
				this.db.GetReader(ref reader, "SP_Service_ListByIP", para);
				while (reader.Read())
				{
					infos.Add(new ServerInfo
					{
						ID = (int)reader["ID"],
						IP = reader["IP"].ToString(),
						Name = reader["Name"].ToString(),
						Online = (int)reader["Online"],
						Port = (int)reader["Port"],
						Remark = reader["Remark"].ToString(),
						Room = (int)reader["Room"],
						State = (int)reader["State"],
						Total = (int)reader["Total"],
						RSA = reader["RSA"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public ServerInfo[] GetServerList()
		{
			List<ServerInfo> infos = new List<ServerInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Service_List");
				while (reader.Read())
				{
					infos.Add(new ServerInfo
					{
						ID = (int)reader["ID"],
						IP = reader["IP"].ToString(),
						Name = reader["Name"].ToString(),
						Online = (int)reader["Online"],
						Port = (int)reader["Port"],
						Remark = reader["Remark"].ToString(),
						Room = (int)reader["Room"],
						State = (int)reader["State"],
						Total = (int)reader["Total"],
						RSA = reader["RSA"].ToString(),
						MustLevel = (int)reader["MustLevel"],
						LowestLevel = (int)reader["LowestLevel"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public Dictionary<int, string> GetAreaServerIP()
		{
			Dictionary<int, string> list = new Dictionary<int, string>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_GetAreaServerIP");
				while (reader.Read())
				{
					list.Add((int)reader["ID"], (reader["IP"] == null) ? "" : reader["IP"].ToString());
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAreaServerIP", e);
				}
			}
			return list;
		}
		public RecordInfo GetRecordInfo(DateTime date, int SaveRecordSecond)
		{
			SqlDataReader reader = null;
			RecordInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
					new SqlParameter("@Second", SaveRecordSecond)
				};
				this.db.GetReader(ref reader, "SP_Server_Record", para);
				if (reader.Read())
				{
					result = new RecordInfo
					{
						ActiveExpendBoy = (int)reader["ActiveExpendBoy"],
						ActiveExpendGirl = (int)reader["ActiveExpendGirl"],
						ActviePayBoy = (int)reader["ActviePayBoy"],
						ActviePayGirl = (int)reader["ActviePayGirl"],
						ExpendBoy = (int)reader["ExpendBoy"],
						ExpendGirl = (int)reader["ExpendGirl"],
						OnlineBoy = (int)reader["OnlineBoy"],
						OnlineGirl = (int)reader["OnlineGirl"],
						TotalBoy = (int)reader["TotalBoy"],
						TotalGirl = (int)reader["TotalGirl"],
						ActiveOnlineBoy = (int)reader["ActiveOnlineBoy"],
						ActiveOnlineGirl = (int)reader["ActiveOnlineGirl"]
					};
					return result;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = null;
			return result;
		}
		public bool UpdateService(ServerInfo info)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", info.ID),
					new SqlParameter("@Online", info.Online),
					new SqlParameter("@State", info.State)
				};
				result = this.db.RunProcedure("SP_Service_Update", para);
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool UpdateRSA(int ID, string RSA)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", ID),
					new SqlParameter("@RSA", RSA)
				};
				result = this.db.RunProcedure("SP_Service_UpdateRSA", para);
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public Dictionary<string, string> GetServerConfig()
		{
			Dictionary<string, string> infos = new Dictionary<string, string>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Server_Config");
				while (reader.Read())
				{
					if (!infos.ContainsKey(reader["Name"].ToString()))
					{
						infos.Add(reader["Name"].ToString(), reader["Value"].ToString());
					}
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetServerConfig", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos;
		}
		public ServerProperty GetServerPropertyByKey(string key)
		{
			ServerProperty serverProperty = null;
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Key", key)
				};
				this.db.GetReader(ref reader, "SP_Server_Config_Single", para);
				while (reader.Read())
				{
					serverProperty = new ServerProperty();
					serverProperty.Key = reader["Name"].ToString();
					serverProperty.Value = reader["Value"].ToString();
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetServerConfig", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return serverProperty;
		}
		public bool UpdateServerPropertyByKey(string key, string value)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Key", key),
					new SqlParameter("@Value", value)
				};
				result = this.db.RunProcedure("SP_Server_Config_Update", para);
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Exception in GameProperties Update", e);
				}
			}
			return result;
		}
		public ArrayList GetRate(int serverId)
		{
			SqlDataReader reader = null;
			ArrayList result;
			try
			{
				ArrayList arrryList = new ArrayList();
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ServerID", serverId)
				};
				this.db.GetReader(ref reader, "SP_Rate", para);
				while (reader.Read())
				{
					arrryList.Add(new RateInfo
					{
						ServerID = (int)reader["ServerID"],
						Rate = (float)((decimal)reader["Rate"]),
						BeginDay = (DateTime)reader["BeginDay"],
						EndDay = (DateTime)reader["EndDay"],
						BeginTime = (DateTime)reader["BeginTime"],
						EndTime = (DateTime)reader["EndTime"],
						Type = (int)reader["Type"]
					});
				}
				arrryList.TrimToSize();
				result = arrryList;
				return result;
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetRates", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = null;
			return result;
		}
		public RateInfo GetRateWithType(int serverId, int type)
		{
			SqlDataReader reader = null;
			RateInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ServerID", serverId),
					new SqlParameter("@Type", type)
				};
				this.db.GetReader(ref reader, "SP_Rate_WithType", para);
				if (reader.Read())
				{
					result = new RateInfo
					{
						ServerID = (int)reader["ServerID"],
						Type = type,
						Rate = (float)reader["Rate"],
						BeginDay = (DateTime)reader["BeginDay"],
						EndDay = (DateTime)reader["EndDay"],
						BeginTime = (DateTime)reader["BeginTime"],
						EndTime = (DateTime)reader["EndTime"]
					};
					return result;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetRate type: " + type, e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = null;
			return result;
		}
		public FightRateInfo[] GetFightRate(int serverId)
		{
			SqlDataReader reader = null;
			List<FightRateInfo> infos = new List<FightRateInfo>();
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ServerID", serverId)
				};
				this.db.GetReader(ref reader, "SP_Fight_Rate", para);
				if (reader.Read())
				{
					infos.Add(new FightRateInfo
					{
						ID = (int)reader["ID"],
						ServerID = (int)reader["ServerID"],
						Rate = (int)reader["Rate"],
						BeginDay = (DateTime)reader["BeginDay"],
						EndDay = (DateTime)reader["EndDay"],
						BeginTime = (DateTime)reader["BeginTime"],
						EndTime = (DateTime)reader["EndTime"],
						SelfCue = (reader["SelfCue"] == null) ? "" : reader["SelfCue"].ToString(),
						EnemyCue = (reader["EnemyCue"] == null) ? "" : reader["EnemyCue"].ToString(),
						BoyTemplateID = (int)reader["BoyTemplateID"],
						GirlTemplateID = (int)reader["GirlTemplateID"],
						Name = (reader["Name"] == null) ? "" : reader["Name"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetFightRate", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return infos.ToArray();
		}
		public string GetGameEquip()
		{
			string equip = string.Empty;
			SqlDataReader reader = null;
			string result;
			try
			{
				this.db.GetReader(ref reader, "SP_Server_Equip");
				if (reader.Read())
				{
					equip = ((reader["value"] == null) ? "" : reader["value"].ToString());
					result = equip;
					return result;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			result = equip;
			return result;
		}
	}
}
