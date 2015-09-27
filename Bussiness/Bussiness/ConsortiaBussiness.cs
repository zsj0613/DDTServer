using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ConsortiaBussiness : BaseBussiness
	{
		public bool AddConsortia(ConsortiaInfo info, ref string msg, ref ConsortiaDutyInfo dutyInfo)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[25];
				para[0] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				para[0].Direction = ParameterDirection.InputOutput;
				para[1] = new SqlParameter("@BuildDate", info.BuildDate);
				para[2] = new SqlParameter("@CelebCount", info.CelebCount);
				para[3] = new SqlParameter("@ChairmanID", info.ChairmanID);
				para[4] = new SqlParameter("@ChairmanName", (info.ChairmanName == null) ? "" : info.ChairmanName);
				para[5] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
				para[6] = new SqlParameter("@CreatorID", info.CreatorID);
				para[7] = new SqlParameter("@CreatorName", (info.CreatorName == null) ? "" : info.CreatorName);
				para[8] = new SqlParameter("@Description", info.Description);
				para[9] = new SqlParameter("@Honor", info.Honor);
				para[10] = new SqlParameter("@IP", info.IP);
				para[11] = new SqlParameter("@IsExist", info.IsExist);
				para[12] = new SqlParameter("@Level", info.Level);
				para[13] = new SqlParameter("@MaxCount", info.MaxCount);
				para[14] = new SqlParameter("@Placard", info.Placard);
				para[15] = new SqlParameter("@Port", info.Port);
				para[16] = new SqlParameter("@Repute", info.Repute);
				para[17] = new SqlParameter("@Count", info.Count);
				para[18] = new SqlParameter("@Riches", info.Riches);
				para[19] = new SqlParameter("@Result", SqlDbType.Int);
				para[19].Direction = ParameterDirection.ReturnValue;
				para[20] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
				para[20].Direction = ParameterDirection.InputOutput;
				para[20].Value = dutyInfo.Level;
				para[21] = new SqlParameter("@tempDutyName", SqlDbType.VarChar, 100);
				para[21].Direction = ParameterDirection.InputOutput;
				para[21].Value = "";
				para[22] = new SqlParameter("@tempRight", SqlDbType.Int);
				para[22].Direction = ParameterDirection.InputOutput;
				para[22].Value = dutyInfo.Right;
				para[23] = new SqlParameter("@IsSystemCreate", info.IsSystemCreate);
				para[24] = new SqlParameter("@IsActive", info.IsActive);
				result = this.db.RunProcedure("SP_Consortia_Add", para);
				int returnValue = (int)para[19].Value;
				result = (returnValue == 0);
				if (result)
				{
					info.ConsortiaID = (int)para[0].Value;
					dutyInfo.Level = (int)para[20].Value;
					dutyInfo.DutyName = para[21].Value.ToString();
					dutyInfo.Right = (int)para[22].Value;
				}
				int num = returnValue;
				if (num == 2)
				{
					msg = "ConsortiaBussiness.AddConsortia.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool DeleteConsortia(int consortiaID, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Delete", para);
				int returnValue = (para[2].Value == null) ? 2 : ((int)para[2].Value);
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.DeleteConsortia.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.DeleteConsortia.Msg3";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public ConsortiaInfo[] GetConsortiaPage(int page, int size, ref int total, int order, string name, int consortiaID, int level, int openApply)
		{
			List<ConsortiaInfo> infos = new List<ConsortiaInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (!string.IsNullOrEmpty(name))
				{
					sWhere = sWhere + " and ConsortiaName like '%" + name + "%' ";
				}
				if (consortiaID == -2)
				{
					sWhere += " and IsSystemCreate=0 ";
				}
				else
				{
					if (consortiaID != -1)
					{
						object obj = sWhere;
						sWhere = string.Concat(new object[]
						{
							obj,
							" and ConsortiaID =",
							consortiaID,
							" "
						});
					}
				}
				if (level != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and Level =",
						level,
						" "
					});
				}
				if (openApply != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and OpenApply =",
						openApply,
						" "
					});
				}
				string sOrder = "ConsortiaName";
				switch (order)
				{
				case 1:
					sOrder = "ReputeSort";
					break;
				case 2:
					sOrder = "ChairmanName";
					break;
				case 3:
					sOrder = "Count desc";
					break;
				case 4:
					sOrder = "Level desc";
					break;
				case 5:
					sOrder = "Honor desc";
					break;
				case 10:
					sOrder = "LastDayRiches desc";
					break;
				case 11:
					sOrder = "AddDayRiches desc";
					break;
				case 12:
					sOrder = "AddWeekRiches desc";
					break;
				case 13:
					sOrder = "LastDayHonor desc";
					break;
				case 14:
					sOrder = "AddDayHonor desc";
					break;
				case 15:
					sOrder = "AddWeekHonor desc";
					break;
				case 16:
					sOrder = "level desc,LastDayRiches desc";
					break;
				case 17:
					sOrder = "FightPower desc";
					break;
				}
				sOrder += ",ConsortiaID ";
				DataTable dt = base.GetPage("V_Consortia", sWhere, page, size, "*", sOrder, "ConsortiaID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaInfo
					{
						ConsortiaID = (int)dr["ConsortiaID"],
						BuildDate = (DateTime)dr["BuildDate"],
						CelebCount = (int)dr["CelebCount"],
						ChairmanID = (int)dr["ChairmanID"],
						ChairmanName = dr["ChairmanName"].ToString(),
						ConsortiaName = dr["ConsortiaName"].ToString(),
						CreatorID = (int)dr["CreatorID"],
						CreatorName = dr["CreatorName"].ToString(),
						Description = dr["Description"].ToString(),
						Honor = (int)dr["Honor"],
						IsExist = (bool)dr["IsExist"],
						Level = (int)dr["Level"],
						MaxCount = (int)dr["MaxCount"],
						Placard = dr["Placard"].ToString(),
						IP = dr["IP"].ToString(),
						Port = (int)dr["Port"],
						Repute = (int)dr["Repute"],
						Count = (int)dr["Count"],
						Riches = (int)dr["Riches"],
						DeductDate = (DateTime)dr["DeductDate"],
						AddDayHonor = (int)dr["AddDayHonor"],
						AddDayRiches = (int)dr["AddDayRiches"],
						AddWeekHonor = (int)dr["AddWeekHonor"],
						AddWeekRiches = (int)dr["AddWeekRiches"],
						LastDayRiches = (int)dr["LastDayRiches"],
						OpenApply = (bool)dr["OpenApply"],
						StoreLevel = (int)dr["StoreLevel"],
						SmithLevel = (int)dr["SmithLevel"],
						ShopLevel = (int)dr["ShopLevel"],
						FightPower = (int)dr["FightPower"],
						IsSystemCreate = (bool)dr["IsSystemCreate"],
						IsActive = (bool)dr["IsActive"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public bool UpdateConsortiaDescription(int consortiaID, int userID, string description, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Description", description),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaDescription_Update", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0);
				int num = returnValue;
				if (num == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsortiaDescription.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool UpdateConsortiaPlacard(int consortiaID, int userID, string placard, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Placard", placard),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaPlacard_Update", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0);
				int num = returnValue;
				if (num == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsortiaPlacard.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool UpdateConsortiaChairman(string nickName, int consortiaID, int userID, ref string msg, ref ConsortiaDutyInfo info, ref int tempUserID, ref string tempUserName)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[9];
				para[0] = new SqlParameter("@NickName", nickName);
				para[1] = new SqlParameter("@ConsortiaID", consortiaID);
				para[2] = new SqlParameter("@UserID", userID);
				para[3] = new SqlParameter("@Result", SqlDbType.Int);
				para[3].Direction = ParameterDirection.ReturnValue;
				para[4] = new SqlParameter("@tempUserID", SqlDbType.Int);
				para[4].Direction = ParameterDirection.InputOutput;
				para[4].Value = tempUserID;
				para[5] = new SqlParameter("@tempUserName", SqlDbType.NVarChar, 100);
				para[5].Direction = ParameterDirection.InputOutput;
				para[5].Value = tempUserName;
				para[6] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
				para[6].Direction = ParameterDirection.InputOutput;
				para[6].Value = info.Level;
				para[7] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				para[7].Direction = ParameterDirection.InputOutput;
				para[7].Value = "";
				para[8] = new SqlParameter("@tempRight", SqlDbType.Int);
				para[8].Direction = ParameterDirection.InputOutput;
				para[8].Value = info.Right;
				result = this.db.RunProcedure("SP_ConsortiaChangeChairman", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0);
				if (result)
				{
					tempUserID = (int)para[4].Value;
					tempUserName = para[5].Value.ToString();
					info.Level = (int)para[6].Value;
					info.DutyName = para[7].Value.ToString();
					info.Right = (int)para[8].Value;
				}
				switch (returnValue)
				{
				case 1:
					msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg3";
					break;
				case 2:
					msg = "ConsortiaBussiness.UpdateConsortiaChairman.Msg2";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool UpGradeConsortia(int consortiaID, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_UpGrade", para);
				int returnValue = (int)para[2].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.UpGradeConsortia.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.UpGradeConsortia.Msg3";
					break;
				case 4:
					msg = "ConsortiaBussiness.UpGradeConsortia.Msg4";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool UpGradeShopConsortia(int consortiaID, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Shop_UpGrade", para);
				int returnValue = (int)para[2].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg3";
					break;
				case 4:
					msg = "ConsortiaBussiness.UpGradeShopConsortia.Msg4";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool UpGradeStoreConsortia(int consortiaID, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Store_UpGrade", para);
				int returnValue = (int)para[2].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg3";
					break;
				case 4:
					msg = "ConsortiaBussiness.UpGradeStoreConsortia.Msg4";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool UpGradeSmithConsortia(int consortiaID, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Smith_UpGrade", para);
				int returnValue = (int)para[2].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg3";
					break;
				case 4:
					msg = "ConsortiaBussiness.UpGradeSmithConsortia.Msg4";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public ConsortiaInfo[] GetAllSystemConsortia()
		{
			SqlDataReader reader = null;
			List<ConsortiaInfo> infos = new List<ConsortiaInfo>();
			try
			{
				this.db.GetReader(ref reader, "SP_Consortia_System_All");
				while (reader.Read())
				{
					infos.Add(new ConsortiaInfo
					{
						ConsortiaID = (int)reader["ConsortiaID"],
						BuildDate = (DateTime)reader["BuildDate"],
						CelebCount = (int)reader["CelebCount"],
						ChairmanID = (int)reader["ChairmanID"],
						ChairmanName = reader["ChairmanName"].ToString(),
						ConsortiaName = reader["ConsortiaName"].ToString(),
						CreatorID = (int)reader["CreatorID"],
						CreatorName = reader["CreatorName"].ToString(),
						Description = reader["Description"].ToString(),
						Honor = (int)reader["Honor"],
						IsExist = (bool)reader["IsExist"],
						Level = (int)reader["Level"],
						MaxCount = (int)reader["MaxCount"],
						Placard = reader["Placard"].ToString(),
						IP = reader["IP"].ToString(),
						Port = (int)reader["Port"],
						Repute = (int)reader["Repute"],
						Count = (int)reader["Count"],
						Riches = (int)reader["Riches"],
						DeductDate = (DateTime)reader["DeductDate"],
						AddDayHonor = (int)reader["AddDayHonor"],
						AddDayRiches = (int)reader["AddDayRiches"],
						AddWeekHonor = (int)reader["AddWeekHonor"],
						AddWeekRiches = (int)reader["AddWeekRiches"],
						LastDayRiches = (int)reader["LastDayRiches"],
						OpenApply = (bool)reader["OpenApply"],
						StoreLevel = (int)reader["StoreLevel"],
						SmithLevel = (int)reader["SmithLevel"],
						ShopLevel = (int)reader["ShopLevel"],
						FightPower = (int)reader["FightPower"],
						IsSystemCreate = (bool)reader["IsSystemCreate"],
						IsActive = (bool)reader["IsActive"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
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
		public ConsortiaInfo[] GetConsortiaAll()
		{
			List<ConsortiaInfo> infos = new List<ConsortiaInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Consortia_All");
				while (reader.Read())
				{
					infos.Add(new ConsortiaInfo
					{
						ConsortiaID = (int)reader["ConsortiaID"],
						ChairmanName = (reader["ChairmanName"] == null) ? "" : reader["ChairmanName"].ToString(),
						Honor = (int)reader["Honor"],
						Level = (int)reader["Level"],
						Riches = (int)reader["Riches"],
						MaxCount = (int)reader["MaxCount"],
						BuildDate = (DateTime)reader["BuildDate"],
						IsExist = (bool)reader["IsExist"],
						DeductDate = (DateTime)reader["DeductDate"],
						StoreLevel = (int)reader["StoreLevel"],
						SmithLevel = (int)reader["SmithLevel"],
						ShopLevel = (int)reader["ShopLevel"],
						ConsortiaName = (reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString(),
						IsDirty = false,
						IsSystemCreate = (bool)reader["IsSystemCreate"],
						IsActive = (bool)reader["IsActive"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
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
		public ConsortiaInfo GetConsortiaSingle(int id)
		{
			SqlDataReader reader = null;
			ConsortiaInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", id)
				};
				this.db.GetReader(ref reader, "SP_Consortia_Single", para);
				if (reader.Read())
				{
					result = new ConsortiaInfo
					{
						ConsortiaID = (int)reader["ConsortiaID"],
						BuildDate = (DateTime)reader["BuildDate"],
						CelebCount = (int)reader["CelebCount"],
						ChairmanID = (int)reader["ChairmanID"],
						ChairmanName = reader["ChairmanName"].ToString(),
						ConsortiaName = reader["ConsortiaName"].ToString(),
						CreatorID = (int)reader["CreatorID"],
						CreatorName = reader["CreatorName"].ToString(),
						Description = reader["Description"].ToString(),
						Honor = (int)reader["Honor"],
						IsExist = (bool)reader["IsExist"],
						Level = (int)reader["Level"],
						MaxCount = (int)reader["MaxCount"],
						Placard = reader["Placard"].ToString(),
						IP = reader["IP"].ToString(),
						Port = (int)reader["Port"],
						Repute = (int)reader["Repute"],
						Count = (int)reader["Count"],
						Riches = (int)reader["Riches"],
						DeductDate = (DateTime)reader["DeductDate"],
						StoreLevel = (int)reader["StoreLevel"],
						SmithLevel = (int)reader["SmithLevel"],
						ShopLevel = (int)reader["ShopLevel"],
						IsSystemCreate = (bool)reader["IsSystemCreate"],
						IsActive = (bool)reader["IsActive"]
					};
					return result;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
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
		public ConsortiaInfo GetConsortiaSingleByName(string consortiumName)
		{
			SqlDataReader reader = null;
			ConsortiaInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaName", consortiumName)
				};
				this.db.GetReader(ref reader, "SP_Consortia_Single_By_Name", para);
				if (reader.Read())
				{
					result = new ConsortiaInfo
					{
						ConsortiaID = (int)reader["ConsortiaID"],
						BuildDate = (DateTime)reader["BuildDate"],
						CelebCount = (int)reader["CelebCount"],
						ChairmanID = (int)reader["ChairmanID"],
						ChairmanName = reader["ChairmanName"].ToString(),
						ConsortiaName = reader["ConsortiaName"].ToString(),
						CreatorID = (int)reader["CreatorID"],
						CreatorName = reader["CreatorName"].ToString(),
						Description = reader["Description"].ToString(),
						Honor = (int)reader["Honor"],
						IsExist = (bool)reader["IsExist"],
						Level = (int)reader["Level"],
						MaxCount = (int)reader["MaxCount"],
						Placard = reader["Placard"].ToString(),
						IP = reader["IP"].ToString(),
						Port = (int)reader["Port"],
						Repute = (int)reader["Repute"],
						Count = (int)reader["Count"],
						Riches = (int)reader["Riches"],
						DeductDate = (DateTime)reader["DeductDate"],
						StoreLevel = (int)reader["StoreLevel"],
						SmithLevel = (int)reader["SmithLevel"],
						ShopLevel = (int)reader["ShopLevel"],
						IsSystemCreate = (bool)reader["IsSystemCreate"],
						IsActive = (bool)reader["IsActive"]
					};
					return result;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
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
		public bool ConsortiaFight(int consortiWin, int consortiaLose, int playerCount, out int riches, int state, int totalKillHealth, float richesRate)
		{
			bool result = false;
			riches = 0;
			try
			{
				SqlParameter[] para = new SqlParameter[8];
				para[0] = new SqlParameter("@ConsortiaWin", consortiWin);
				para[1] = new SqlParameter("@ConsortiaLose", consortiaLose);
				para[2] = new SqlParameter("@PlayerCount", playerCount);
				para[3] = new SqlParameter("@Riches", SqlDbType.Int);
				para[3].Direction = ParameterDirection.InputOutput;
				para[3].Value = riches;
				para[4] = new SqlParameter("@Result", SqlDbType.Int);
				para[4].Direction = ParameterDirection.ReturnValue;
				para[5] = new SqlParameter("@State", state);
				para[6] = new SqlParameter("@TotalKillHealth", totalKillHealth);
				para[7] = new SqlParameter("@RichesRate", richesRate);
				result = this.db.RunProcedure("SP_Consortia_Fight", para);
				riches = (int)para[3].Value;
				int returnValue = (int)para[4].Value;
				result = (returnValue == 0);
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("ConsortiaFight", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool ConsortiaRichAdd(int consortiID, ref int riches)
		{
			return this.ConsortiaRichAdd(consortiID, ref riches, 0, "");
		}
		public bool ConsortiaRichAdd(int consortiID, ref int riches, int type, string username)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[5];
				para[0] = new SqlParameter("@ConsortiaID", consortiID);
				para[1] = new SqlParameter("@Riches", SqlDbType.Int);
				para[1].Direction = ParameterDirection.InputOutput;
				para[1].Value = riches;
				para[2] = new SqlParameter("@Result", SqlDbType.Int);
				para[2].Direction = ParameterDirection.ReturnValue;
				para[3] = new SqlParameter("@Type", type);
				para[4] = new SqlParameter("@UserName", username);
				result = this.db.RunProcedure("SP_Consortia_Riches_Add", para);
				riches = (int)para[1].Value;
				int returnValue = (int)para[2].Value;
				result = (returnValue == 0);
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("ConsortiaRichAdd", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool ScanConsortia(ref string noticeID)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[2];
				para[0] = new SqlParameter("@NoticeID", SqlDbType.VarChar, -1);
				para[0].Direction = ParameterDirection.Output;
				para[1] = new SqlParameter("@Result", SqlDbType.Int);
				para[1].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_Consortia_Scan", para);
				int returnValue = (int)para[1].Value;
				result = (returnValue == 0);
				if (result)
				{
					noticeID = para[0].Value.ToString();
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool UpdateConsotiaApplyState(int consortiaID, int userID, bool state, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@State", state),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Consortia_Apply_State", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0);
				int num = returnValue;
				if (num == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsotiaApplyState.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool AddConsortiaApplyUsers(ConsortiaApplyUserInfo info, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[9];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.InputOutput;
				para[1] = new SqlParameter("@ApplyDate", info.ApplyDate);
				para[2] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				para[3] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
				para[4] = new SqlParameter("@IsExist", info.IsExist);
				para[5] = new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark);
				para[6] = new SqlParameter("@UserID", info.UserID);
				para[7] = new SqlParameter("@UserName", (info.UserName == null) ? "" : info.UserName);
				para[8] = new SqlParameter("@Result", SqlDbType.Int);
				para[8].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaApplyUser_Add", para);
				info.ID = (int)para[0].Value;
				int returnValue = (para[8].Value == null) ? 7 : ((int)para[8].Value);
				result = (returnValue == 0);
				int num = returnValue;
				if (num != 2)
				{
					switch (num)
					{
					case 6:
						msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg6";
						break;
					case 7:
						msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg7";
						break;
					}
				}
				else
				{
					msg = "ConsortiaBussiness.AddConsortiaApplyUsers.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool DeleteConsortiaApplyUsers(int applyID, int userID, int consortiaID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", applyID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaApplyUser_Delete", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0 || returnValue == 3);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.DeleteConsortiaApplyUsers.Msg2";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool PassConsortiaApplyUsers(int applyID, int userID, string userName, int consortiaID, ref string msg, ConsortiaUserInfo info, ref int consortiaRepute)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[25];
				para[0] = new SqlParameter("@ID", applyID);
				para[1] = new SqlParameter("@UserID", userID);
				para[2] = new SqlParameter("@UserName", userName);
				para[3] = new SqlParameter("@ConsortiaID", consortiaID);
				para[4] = new SqlParameter("@Result", SqlDbType.Int);
				para[4].Direction = ParameterDirection.ReturnValue;
				para[5] = new SqlParameter("@tempID", SqlDbType.Int);
				para[5].Direction = ParameterDirection.InputOutput;
				para[5].Value = info.UserID;
				para[6] = new SqlParameter("@tempName", SqlDbType.NVarChar, 100);
				para[6].Direction = ParameterDirection.InputOutput;
				para[6].Value = "";
				para[7] = new SqlParameter("@tempDutyID", SqlDbType.Int);
				para[7].Direction = ParameterDirection.InputOutput;
				para[7].Value = info.DutyID;
				para[8] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				para[8].Direction = ParameterDirection.InputOutput;
				para[8].Value = "";
				para[9] = new SqlParameter("@tempOffer", SqlDbType.Int);
				para[9].Direction = ParameterDirection.InputOutput;
				para[9].Value = info.Offer;
				para[10] = new SqlParameter("@tempRichesOffer", SqlDbType.Int);
				para[10].Direction = ParameterDirection.InputOutput;
				para[10].Value = info.RichesOffer;
				para[11] = new SqlParameter("@tempRichesRob", SqlDbType.Int);
				para[11].Direction = ParameterDirection.InputOutput;
				para[11].Value = info.RichesRob;
				para[12] = new SqlParameter("@tempLastDate", SqlDbType.DateTime);
				para[12].Direction = ParameterDirection.InputOutput;
				para[12].Value = DateTime.Now;
				para[13] = new SqlParameter("@tempWin", SqlDbType.Int);
				para[13].Direction = ParameterDirection.InputOutput;
				para[13].Value = info.Win;
				para[14] = new SqlParameter("@tempTotal", SqlDbType.Int);
				para[14].Direction = ParameterDirection.InputOutput;
				para[14].Value = info.Total;
				para[15] = new SqlParameter("@tempEscape", SqlDbType.Int);
				para[15].Direction = ParameterDirection.InputOutput;
				para[15].Value = info.Escape;
				para[16] = new SqlParameter("@tempGrade", SqlDbType.Int);
				para[16].Direction = ParameterDirection.InputOutput;
				para[16].Value = info.Grade;
				para[17] = new SqlParameter("@tempLevel", SqlDbType.Int);
				para[17].Direction = ParameterDirection.InputOutput;
				para[17].Value = info.Level;
				para[18] = new SqlParameter("@tempCUID", SqlDbType.Int);
				para[18].Direction = ParameterDirection.InputOutput;
				para[18].Value = info.ID;
				para[19] = new SqlParameter("@tempState", SqlDbType.Int);
				para[19].Direction = ParameterDirection.InputOutput;
				para[19].Value = info.State;
				para[20] = new SqlParameter("@tempSex", SqlDbType.Bit);
				para[20].Direction = ParameterDirection.InputOutput;
				para[20].Value = info.Sex;
				para[21] = new SqlParameter("@tempDutyRight", SqlDbType.Int);
				para[21].Direction = ParameterDirection.InputOutput;
				para[21].Value = info.Right;
				para[22] = new SqlParameter("@tempConsortiaRepute", SqlDbType.Int);
				para[22].Direction = ParameterDirection.InputOutput;
				para[22].Value = consortiaRepute;
				para[23] = new SqlParameter("@tempLoginName", SqlDbType.NVarChar, 200);
				para[23].Direction = ParameterDirection.InputOutput;
				para[23].Value = consortiaRepute;
				para[24] = new SqlParameter("@tempFightPower", SqlDbType.Int);
				para[24].Direction = ParameterDirection.InputOutput;
				para[24].Value = info.FightPower;
				this.db.RunProcedure("SP_ConsortiaApplyUser_Pass", para);
				int returnValue = (int)para[4].Value;
				result = (returnValue == 0);
				if (result)
				{
					info.UserID = (int)para[5].Value;
					info.UserName = para[6].Value.ToString();
					info.DutyID = (int)para[7].Value;
					info.DutyName = para[8].Value.ToString();
					info.Offer = (int)para[9].Value;
					info.RichesOffer = (int)para[10].Value;
					info.RichesRob = (int)para[11].Value;
					info.LastDate = (DateTime)para[12].Value;
					info.Win = (int)para[13].Value;
					info.Total = (int)para[14].Value;
					info.Escape = (int)para[15].Value;
					info.Grade = (int)para[16].Value;
					info.Level = (int)para[17].Value;
					info.ID = (int)para[18].Value;
					info.State = (int)para[19].Value;
					info.Sex = (bool)para[20].Value;
					info.Right = (int)para[21].Value;
					consortiaRepute = (int)para[22].Value;
					info.LoginName = para[23].Value.ToString();
					info.FightPower = (int)para[24].Value;
				}
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg3";
					break;
				case 6:
					msg = "ConsortiaBussiness.PassConsortiaApplyUsers.Msg6";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public ConsortiaApplyUserInfo[] GetConsortiaApplyUserPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int userID)
		{
			List<ConsortiaApplyUserInfo> infos = new List<ConsortiaApplyUserInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (applyID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ID =",
						applyID,
						" "
					});
				}
				if (userID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and UserID ='",
						userID,
						"' "
					});
				}
				string sOrder = "ID";
				switch (order)
				{
				case 1:
					sOrder = "UserName,ID";
					break;
				case 2:
					sOrder = "ApplyDate,ID";
					break;
				}
				DataTable dt = base.GetPage("V_Consortia_Apply_Users", sWhere, page, size, "*", sOrder, "ID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaApplyUserInfo
					{
						ID = (int)dr["ID"],
						ApplyDate = (DateTime)dr["ApplyDate"],
						ConsortiaID = (int)dr["ConsortiaID"],
						ConsortiaName = dr["ConsortiaName"].ToString(),
						//ID = (int)dr["ID"],
						IsExist = (bool)dr["IsExist"],
						Remark = dr["Remark"].ToString(),
						UserID = (int)dr["UserID"],
						UserName = dr["UserName"].ToString(),
						UserLevel = (int)dr["Grade"],
						Win = (int)dr["Win"],
						Total = (int)dr["Total"],
						Repute = (int)dr["Repute"],
						FightPower = (int)dr["FightPower"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public bool AddConsortiaInviteUsers(ConsortiaInviteUserInfo info, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[11];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.InputOutput;
				para[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				para[2] = new SqlParameter("@ConsortiaName", (info.ConsortiaName == null) ? "" : info.ConsortiaName);
				para[3] = new SqlParameter("@InviteDate", info.InviteDate);
				para[4] = new SqlParameter("@InviteID", info.InviteID);
				para[5] = new SqlParameter("@InviteName", (info.InviteName == null) ? "" : info.InviteName);
				para[6] = new SqlParameter("@IsExist", info.IsExist);
				para[7] = new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark);
				para[8] = new SqlParameter("@UserID", info.UserID);
				para[8].Direction = ParameterDirection.InputOutput;
				para[9] = new SqlParameter("@UserName", (info.UserName == null) ? "" : info.UserName);
				para[10] = new SqlParameter("@Result", SqlDbType.Int);
				para[10].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaInviteUser_Add", para);
				info.ID = (int)para[0].Value;
				info.UserID = (int)para[8].Value;
				int returnValue = (int)para[10].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg2";
					break;
				case 4:
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg4";
					break;
				case 5:
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg5";
					break;
				case 6:
					msg = "ConsortiaBussiness.AddConsortiaInviteUsers.Msg6";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool DeleteConsortiaInviteUsers(int intiveID, int userID)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", intiveID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[2].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaInviteUser_Delete", para);
				int returnValue = (int)para[2].Value;
				result = (returnValue == 0);
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool PassConsortiaInviteUsers(int inviteID, int userID, string userName, ref int consortiaID, ref string consortiaName, ref string msg, ConsortiaUserInfo info, ref int tempID, ref string tempName, ref int consortiaRepute)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[24];
				para[0] = new SqlParameter("@ID", inviteID);
				para[1] = new SqlParameter("@UserID", userID);
				para[2] = new SqlParameter("@UserName", userName);
				para[3] = new SqlParameter("@ConsortiaID", consortiaID);
				para[3].Direction = ParameterDirection.InputOutput;
				para[4] = new SqlParameter("@ConsortiaName", SqlDbType.NVarChar, 100);
				para[4].Value = consortiaName;
				para[4].Direction = ParameterDirection.InputOutput;
				para[5] = new SqlParameter("@Result", SqlDbType.Int);
				para[5].Direction = ParameterDirection.ReturnValue;
				para[6] = new SqlParameter("@tempName", SqlDbType.NVarChar, 100);
				para[6].Direction = ParameterDirection.InputOutput;
				para[6].Value = tempName;
				para[7] = new SqlParameter("@tempDutyID", SqlDbType.Int);
				para[7].Direction = ParameterDirection.InputOutput;
				para[7].Value = info.DutyID;
				para[8] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				para[8].Direction = ParameterDirection.InputOutput;
				para[8].Value = "";
				para[9] = new SqlParameter("@tempOffer", SqlDbType.Int);
				para[9].Direction = ParameterDirection.InputOutput;
				para[9].Value = info.Offer;
				para[10] = new SqlParameter("@tempRichesOffer", SqlDbType.Int);
				para[10].Direction = ParameterDirection.InputOutput;
				para[10].Value = info.RichesOffer;
				para[11] = new SqlParameter("@tempRichesRob", SqlDbType.Int);
				para[11].Direction = ParameterDirection.InputOutput;
				para[11].Value = info.RichesRob;
				para[12] = new SqlParameter("@tempLastDate", SqlDbType.DateTime);
				para[12].Direction = ParameterDirection.InputOutput;
				para[12].Value = DateTime.Now;
				para[13] = new SqlParameter("@tempWin", SqlDbType.Int);
				para[13].Direction = ParameterDirection.InputOutput;
				para[13].Value = info.Win;
				para[14] = new SqlParameter("@tempTotal", SqlDbType.Int);
				para[14].Direction = ParameterDirection.InputOutput;
				para[14].Value = info.Total;
				para[15] = new SqlParameter("@tempEscape", SqlDbType.Int);
				para[15].Direction = ParameterDirection.InputOutput;
				para[15].Value = info.Escape;
				para[16] = new SqlParameter("@tempID", SqlDbType.Int);
				para[16].Direction = ParameterDirection.InputOutput;
				para[16].Value = tempID;
				para[17] = new SqlParameter("@tempGrade", SqlDbType.Int);
				para[17].Direction = ParameterDirection.InputOutput;
				para[17].Value = info.Level;
				para[18] = new SqlParameter("@tempLevel", SqlDbType.Int);
				para[18].Direction = ParameterDirection.InputOutput;
				para[18].Value = info.Level;
				para[19] = new SqlParameter("@tempCUID", SqlDbType.Int);
				para[19].Direction = ParameterDirection.InputOutput;
				para[19].Value = info.ID;
				para[20] = new SqlParameter("@tempState", SqlDbType.Int);
				para[20].Direction = ParameterDirection.InputOutput;
				para[20].Value = info.State;
				para[21] = new SqlParameter("@tempSex", SqlDbType.Bit);
				para[21].Direction = ParameterDirection.InputOutput;
				para[21].Value = info.Sex;
				para[22] = new SqlParameter("@tempRight", SqlDbType.Int);
				para[22].Direction = ParameterDirection.InputOutput;
				para[22].Value = info.Right;
				para[23] = new SqlParameter("@tempConsortiaRepute", SqlDbType.Int);
				para[23].Direction = ParameterDirection.InputOutput;
				para[23].Value = consortiaRepute;
				this.db.RunProcedure("SP_ConsortiaInviteUser_Pass", para);
				int returnValue = (int)para[5].Value;
				result = (returnValue == 0);
				if (result)
				{
					consortiaID = (int)para[3].Value;
					consortiaName = para[4].Value.ToString();
					tempName = para[6].Value.ToString();
					info.DutyID = (int)para[7].Value;
					info.DutyName = para[8].Value.ToString();
					info.Offer = (int)para[9].Value;
					info.RichesOffer = (int)para[10].Value;
					info.RichesRob = (int)para[11].Value;
					info.LastDate = (DateTime)para[12].Value;
					info.Win = (int)para[13].Value;
					info.Total = (int)para[14].Value;
					info.Escape = (int)para[15].Value;
					tempID = (int)para[16].Value;
					info.Grade = (int)para[17].Value;
					info.Level = (int)para[18].Value;
					info.ID = (int)para[19].Value;
					info.State = (int)para[20].Value;
					info.Sex = (bool)para[21].Value;
					info.Right = (int)para[22].Value;
					consortiaRepute = (int)para[23].Value;
				}
				int num = returnValue;
				if (num != 3)
				{
					if (num == 6)
					{
						msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg6";
					}
				}
				else
				{
					msg = "ConsortiaBussiness.PassConsortiaInviteUsers.Msg3";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public ConsortiaInviteUserInfo[] GetConsortiaInviteUserPage(int page, int size, ref int total, int order, int userID, int inviteID)
		{
			List<ConsortiaInviteUserInfo> infos = new List<ConsortiaInviteUserInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (userID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and UserID =",
						userID,
						" "
					});
				}
				if (inviteID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and UserID =",
						inviteID,
						" "
					});
				}
				string sOrder = "ConsortiaName";
				switch (order)
				{
				case 1:
					sOrder = "Repute";
					break;
				case 2:
					sOrder = "ChairmanName";
					break;
				case 3:
					sOrder = "Count";
					break;
				case 4:
					sOrder = "CelebCount";
					break;
				case 5:
					sOrder = "Honor";
					break;
				}
				sOrder += ",ID ";
				DataTable dt = base.GetPage("V_Consortia_Invite", sWhere, page, size, "*", sOrder, "ID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaInviteUserInfo
					{
						ID = (int)dr["ID"],
						CelebCount = (int)dr["CelebCount"],
						ChairmanName = dr["ChairmanName"].ToString(),
						ConsortiaID = (int)dr["ConsortiaID"],
						ConsortiaName = dr["ConsortiaName"].ToString(),
						Count = (int)dr["Count"],
						Honor = (int)dr["Honor"],
						InviteDate = (DateTime)dr["InviteDate"],
						InviteID = (int)dr["InviteID"],
						InviteName = dr["InviteName"].ToString(),
						IsExist = (bool)dr["IsExist"],
						Remark = dr["Remark"].ToString(),
						Repute = (int)dr["Repute"],
						UserID = (int)dr["UserID"],
						UserName = dr["UserName"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public bool DeleteConsortiaUser(int userID, int kickUserID, int consortiaID, ref string msg, ref string nickName)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[5];
				para[0] = new SqlParameter("@UserID", userID);
				para[1] = new SqlParameter("@KickUserID", kickUserID);
				para[2] = new SqlParameter("@ConsortiaID", consortiaID);
				para[3] = new SqlParameter("@Result", SqlDbType.Int);
				para[3].Direction = ParameterDirection.ReturnValue;
				para[4] = new SqlParameter("@NickName", SqlDbType.NVarChar, 200);
				para[4].Direction = ParameterDirection.InputOutput;
				para[4].Value = nickName;
				this.db.RunProcedure("SP_ConsortiaUser_Delete", para);
				int returnValue = (int)para[3].Value;
				if (returnValue == 0)
				{
					result = true;
					nickName = para[4].Value.ToString();
				}
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg3";
					break;
				case 4:
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg4";
					break;
				case 5:
					msg = "ConsortiaBussiness.DeleteConsortiaUser.Msg5";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool UpdateConsortiaIsBanChat(int banUserID, int consortiaID, int userID, bool isBanChat, ref int tempID, ref string tempName, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[7];
				para[0] = new SqlParameter("@ID", banUserID);
				para[1] = new SqlParameter("@ConsortiaID", consortiaID);
				para[2] = new SqlParameter("@UserID", userID);
				para[3] = new SqlParameter("@IsBanChat", isBanChat);
				para[4] = new SqlParameter("@TempID", tempID);
				para[4].Direction = ParameterDirection.InputOutput;
				para[5] = new SqlParameter("@TempName", SqlDbType.NVarChar, 100);
				para[5].Value = tempName;
				para[5].Direction = ParameterDirection.InputOutput;
				para[6] = new SqlParameter("@Result", SqlDbType.Int);
				para[6].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaIsBanChat_Update", para);
				int returnValue = (int)para[6].Value;
				tempID = (int)para[4].Value;
				tempName = para[5].Value.ToString();
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.UpdateConsortiaIsBanChat.Msg3";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool UpdateConsortiaUserRemark(int id, int consortiaID, int userID, string remark, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", id),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Remark", remark),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[4].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaUserRemark_Update", para);
				int returnValue = (int)para[4].Value;
				result = (returnValue == 0);
				int num = returnValue;
				if (num == 2)
				{
					msg = "ConsortiaBussiness.UpdateConsortiaUserRemark.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool UpdateConsortiaUserGrade(int id, int consortiaID, int userID, bool upGrade, ref string msg, ref ConsortiaDutyInfo info, ref string tempUserName)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[9];
				para[0] = new SqlParameter("@ID", id);
				para[1] = new SqlParameter("@ConsortiaID", consortiaID);
				para[2] = new SqlParameter("@UserID", userID);
				para[3] = new SqlParameter("@UpGrade", upGrade);
				para[4] = new SqlParameter("@Result", SqlDbType.Int);
				para[4].Direction = ParameterDirection.ReturnValue;
				para[5] = new SqlParameter("@tempUserName", SqlDbType.NVarChar, 100);
				para[5].Direction = ParameterDirection.InputOutput;
				para[5].Value = tempUserName;
				para[6] = new SqlParameter("@tempDutyLevel", SqlDbType.Int);
				para[6].Direction = ParameterDirection.InputOutput;
				para[6].Value = info.Level;
				para[7] = new SqlParameter("@tempDutyName", SqlDbType.NVarChar, 100);
				para[7].Direction = ParameterDirection.InputOutput;
				para[7].Value = "";
				para[8] = new SqlParameter("@tempRight", SqlDbType.Int);
				para[8].Direction = ParameterDirection.InputOutput;
				para[8].Value = info.Right;
				result = this.db.RunProcedure("SP_ConsortiaUserGrade_Update", para);
				int returnValue = (int)para[4].Value;
				result = (returnValue == 0);
				if (result)
				{
					tempUserName = para[5].Value.ToString();
					info.Level = (int)para[6].Value;
					info.DutyName = para[7].Value.ToString();
					info.Right = (int)para[8].Value;
				}
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg2";
					break;
				case 3:
					msg = (upGrade ? "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg3" : "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg10");
					break;
				case 4:
					msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg4";
					break;
				case 5:
					msg = "ConsortiaBussiness.UpdateConsortiaUserGrade.Msg5";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public ConsortiaUserInfo[] GetConsortiaUsersPage(int page, int size, ref int total, int order, int consortiaID, int userID, int state)
		{
			List<ConsortiaUserInfo> infos = new List<ConsortiaUserInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (userID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and UserID =",
						userID,
						" "
					});
				}
				if (state != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and state =",
						state,
						" "
					});
				}
				string sOrder = "UserName";
				switch (order)
				{
				case 1:
					sOrder = "DutyID";
					break;
				case 2:
					sOrder = "Grade";
					break;
				case 3:
					sOrder = "Repute";
					break;
				case 4:
					sOrder = "GP";
					break;
				case 5:
					sOrder = "State";
					break;
				case 6:
					sOrder = "Offer";
					break;
				}
				sOrder += ",ID ";
				DataTable dt = base.GetPage("V_Consortia_Users", sWhere, page, size, "*", sOrder, "ID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					ConsortiaUserInfo info = new ConsortiaUserInfo();
					info.ID = (int)dr["ID"];
					info.ConsortiaID = (int)dr["ConsortiaID"];
					info.DutyID = (int)dr["DutyID"];
					info.DutyName = dr["DutyName"].ToString();
					info.IsExist = (bool)dr["IsExist"];
					info.RatifierID = (int)dr["RatifierID"];
					info.RatifierName = dr["RatifierName"].ToString();
					info.Remark = dr["Remark"].ToString();
					info.UserID = (int)dr["UserID"];
					info.UserName = dr["UserName"].ToString();
					info.Grade = (int)dr["Grade"];
					info.GP = (int)dr["GP"];
					info.Repute = (int)dr["Repute"];
					info.State = (int)dr["State"];
					info.Right = (int)dr["Right"];
					info.Offer = (int)dr["Offer"];
					info.Colors = dr["Colors"].ToString();
					info.Style = dr["Style"].ToString();
					info.Hide = (int)dr["Hide"];
					info.Skin = ((dr["Skin"] == null) ? "" : info.Skin);
					info.Level = (int)dr["Level"];
					info.LastDate = (DateTime)dr["LastDate"];
					info.Sex = (bool)dr["Sex"];
					info.IsBanChat = (bool)dr["IsBanChat"];
					info.Win = (int)dr["Win"];
					info.Total = (int)dr["Total"];
					info.Escape = (int)dr["Escape"];
					info.RichesOffer = (int)dr["RichesOffer"];
					info.RichesRob = (int)dr["RichesRob"];
					info.LoginName = ((dr["LoginName"] == null) ? "" : dr["LoginName"].ToString());
					info.Nimbus = (int)dr["Nimbus"];
					info.FightPower = (int)dr["FightPower"];
					infos.Add(info);
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public ConsortiaUserInfo GetConsortiaUsersByUserID(int userID)
		{
			int total = 0;
			ConsortiaUserInfo[] infos = this.GetConsortiaUsersPage(1, 1, ref total, -1, -1, userID, -1);
			ConsortiaUserInfo result;
			if (infos.Length == 1)
			{
				result = infos[0];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public bool UpdateRobotChairman()
		{
			bool result;
			try
			{
				result = this.db.RunProcedure("SP_Update_Robot_Chairman");
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
				result = false;
			}
			return result;
		}
		public bool ActiveConsortia()
		{
			bool result;
			try
			{
				result = this.db.RunProcedure("SP_Active_Consortia");
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Active", e);
				}
				result = false;
			}
			return result;
		}
		public bool AddConsortiaDuty(ConsortiaDutyInfo info, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[7];
				para[0] = new SqlParameter("@DutyID", info.DutyID);
				para[0].Direction = ParameterDirection.InputOutput;
				para[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				para[2] = new SqlParameter("@DutyName", info.DutyName);
				para[3] = new SqlParameter("@Level", info.Level);
				para[4] = new SqlParameter("@UserID", userID);
				para[5] = new SqlParameter("@Right", info.Right);
				para[6] = new SqlParameter("@Result", SqlDbType.Int);
				para[6].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaDuty_Add", para);
				info.DutyID = (int)para[0].Value;
				int returnValue = (int)para[6].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.AddConsortiaDuty.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.AddConsortiaDuty.Msg3";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool DeleteConsortiaDuty(int dutyID, int userID, int consortiaID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@UserID", userID),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@DutyID", dutyID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaDuty_Delete", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg3";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool UpdateConsortiaDuty(ConsortiaDutyInfo info, int userID, int updateType, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[8];
				para[0] = new SqlParameter("@DutyID", info.DutyID);
				para[0].Direction = ParameterDirection.InputOutput;
				para[1] = new SqlParameter("@ConsortiaID", info.ConsortiaID);
				para[2] = new SqlParameter("@DutyName", SqlDbType.NVarChar, 100);
				para[2].Direction = ParameterDirection.InputOutput;
				para[2].Value = info.DutyName;
				para[3] = new SqlParameter("@Right", SqlDbType.Int);
				para[3].Direction = ParameterDirection.InputOutput;
				para[3].Value = info.Right;
				para[4] = new SqlParameter("@Level", SqlDbType.Int);
				para[4].Direction = ParameterDirection.InputOutput;
				para[4].Value = info.Level;
				para[5] = new SqlParameter("@UserID", userID);
				para[6] = new SqlParameter("@UpdateType", updateType);
				para[7] = new SqlParameter("@Result", SqlDbType.Int);
				para[7].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaDuty_Update", para);
				int returnValue = (int)para[7].Value;
				result = (returnValue == 0);
				if (result)
				{
					info.DutyID = (int)para[0].Value;
					info.DutyName = ((para[2].Value == null) ? "" : para[2].Value.ToString());
					info.Right = (int)para[3].Value;
					info.Level = (int)para[4].Value;
				}
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg2";
					break;
				case 3:
				case 4:
					msg = "ConsortiaBussiness.UpdateConsortiaDuty.Msg3";
					break;
				case 5:
					msg = "ConsortiaBussiness.DeleteConsortiaDuty.Msg5";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public ConsortiaDutyInfo[] GetConsortiaDutyPage(int page, int size, ref int total, int order, int consortiaID, int dutyID)
		{
			List<ConsortiaDutyInfo> infos = new List<ConsortiaDutyInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (dutyID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and DutyID =",
						dutyID,
						" "
					});
				}
				string sOrder = "Level";
				if (order == 1)
				{
					sOrder = "DutyName";
				}
				sOrder += ",DutyID ";
				DataTable dt = base.GetPage("Consortia_Duty", sWhere, page, size, "*", sOrder, "DutyID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaDutyInfo
					{
						DutyID = (int)dr["DutyID"],
						ConsortiaID = (int)dr["ConsortiaID"],
						//DutyID = (int)dr["DutyID"],
						DutyName = dr["DutyName"].ToString(),
						IsExist = (bool)dr["IsExist"],
						Right = (int)dr["Right"],
						Level = (int)dr["Level"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public int[] GetConsortiaByAllyByState(int consortiaID, int state)
		{
			List<int> infos = new List<int>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@State", state)
				};
				this.db.GetReader(ref reader, "SP_Consortia_AllyByState", para);
				while (reader.Read())
				{
					infos.Add((int)reader["Consortia2ID"]);
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
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
		public bool AddConsortiaApplyAlly(ConsortiaApplyAllyInfo info, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[9];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.InputOutput;
				para[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
				para[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
				para[3] = new SqlParameter("@Date", info.Date);
				para[4] = new SqlParameter("@Remark", info.Remark);
				para[5] = new SqlParameter("@IsExist", info.IsExist);
				para[6] = new SqlParameter("@UserID", userID);
				para[7] = new SqlParameter("@State", info.State);
				para[8] = new SqlParameter("@Result", SqlDbType.Int);
				para[8].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_ConsortiaApplyAlly_Add", para);
				info.ID = (int)para[0].Value;
				int returnValue = (int)para[8].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg3";
					break;
				case 4:
					msg = "ConsortiaBussiness.AddConsortiaApplyAlly.Msg4";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public bool DeleteConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", applyID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaApplyAlly_Delete", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0);
				int num = returnValue;
				if (num == 2)
				{
					msg = "ConsortiaBussiness.DeleteConsortiaApplyAlly.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public bool PassConsortiaApplyAlly(int applyID, int userID, int consortiaID, ref int tempID, ref int state, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[6];
				para[0] = new SqlParameter("@ID", applyID);
				para[1] = new SqlParameter("@UserID", userID);
				para[2] = new SqlParameter("@ConsortiaID", consortiaID);
				para[3] = new SqlParameter("@tempID", tempID);
				para[3].Direction = ParameterDirection.InputOutput;
				para[4] = new SqlParameter("@State", state);
				para[4].Direction = ParameterDirection.InputOutput;
				para[5] = new SqlParameter("@Result", SqlDbType.Int);
				para[5].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaApplyAlly_Pass", para);
				int returnValue = (int)para[5].Value;
				if (returnValue == 0)
				{
					result = true;
					tempID = (int)para[3].Value;
					state = (int)para[4].Value;
				}
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.PassConsortiaApplyAlly.Msg3";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public ConsortiaApplyAllyInfo[] GetConsortiaApplyAllyPage(int page, int size, ref int total, int order, int consortiaID, int applyID, int state)
		{
			List<ConsortiaApplyAllyInfo> infos = new List<ConsortiaApplyAllyInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and Consortia2ID =",
						consortiaID,
						" "
					});
				}
				if (applyID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ID =",
						applyID,
						" "
					});
				}
				if (state != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and State =",
						state,
						" "
					});
				}
				string sOrder = "ConsortiaName";
				switch (order)
				{
				case 1:
					sOrder = "Repute";
					break;
				case 2:
					sOrder = "ChairmanName";
					break;
				case 3:
					sOrder = "Count";
					break;
				case 4:
					sOrder = "Level";
					break;
				case 5:
					sOrder = "Honor";
					break;
				}
				sOrder += ",ID ";
				DataTable dt = base.GetPage("V_Consortia_Apply_Ally", sWhere, page, size, "*", sOrder, "ID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaApplyAllyInfo
					{
						ID = (int)dr["ID"],
						CelebCount = (int)dr["CelebCount"],
						ChairmanName = dr["ChairmanName"].ToString(),
						Consortia1ID = (int)dr["Consortia1ID"],
						Consortia2ID = (int)dr["Consortia2ID"],
						ConsortiaName = dr["ConsortiaName"].ToString(),
						Count = (int)dr["Count"],
						Date = (DateTime)dr["Date"],
						Honor = (int)dr["Honor"],
						IsExist = (bool)dr["IsExist"],
						Remark = dr["Remark"].ToString(),
						Repute = (int)dr["Repute"],
						State = (int)dr["State"],
						Level = (int)dr["Level"],
						Description = (dr["Description"] == null) ? "" : dr["Description"].ToString()
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public Dictionary<int, int> GetConsortiaByAlly(int consortiaID)
		{
			Dictionary<int, int> consortiaIDs = new Dictionary<int, int>();
			SqlDataReader reader = null;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID)
				};
				this.db.GetReader(ref reader, "SP_Consortia_Ally_Neutral", para);
				while (reader.Read())
				{
					if ((int)reader["Consortia1ID"] != consortiaID)
					{
						consortiaIDs.Add((int)reader["Consortia1ID"], (int)reader["State"]);
					}
					else
					{
						consortiaIDs.Add((int)reader["Consortia2ID"], (int)reader["State"]);
					}
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetConsortiaByAlly", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return consortiaIDs;
		}
		public bool AddConsortiaAlly(ConsortiaAllyInfo info, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[9];
				para[0] = new SqlParameter("@ID", info.ID);
				para[0].Direction = ParameterDirection.InputOutput;
				para[1] = new SqlParameter("@Consortia1ID", info.Consortia1ID);
				para[2] = new SqlParameter("@Consortia2ID", info.Consortia2ID);
				para[3] = new SqlParameter("@State", info.State);
				para[4] = new SqlParameter("@Date", info.Date);
				para[5] = new SqlParameter("@ValidDate", info.ValidDate);
				para[6] = new SqlParameter("@IsExist", info.IsExist);
				para[7] = new SqlParameter("@UserID", userID);
				para[8] = new SqlParameter("@Result", SqlDbType.Int);
				para[8].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_ConsortiaAlly_Add", para);
				int returnValue = (int)para[8].Value;
				result = (returnValue == 0);
				switch (returnValue)
				{
				case 2:
					msg = "ConsortiaBussiness.AddConsortiaAlly.Msg2";
					break;
				case 3:
					msg = "ConsortiaBussiness.AddConsortiaAlly.Msg3";
					break;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			return result;
		}
		public ConsortiaAllyInfo[] GetConsortiaAllyPage(int page, int size, ref int total, int order, int consortiaID, int state, string name)
		{
			List<ConsortiaAllyInfo> infos = new List<ConsortiaAllyInfo>();
			string sWhere = " IsExist=1 and ConsortiaID<>" + consortiaID;
			Dictionary<int, int> consortiaIDs = this.GetConsortiaByAlly(consortiaID);
			try
			{
				if (state != -1)
				{
					string ids = string.Empty;
					foreach (int id in consortiaIDs.Keys)
					{
						ids = ids + id + ",";
					}
					ids += 0;
					if (state == 0)
					{
						sWhere = sWhere + " and ConsortiaID not in (" + ids + ") ";
					}
					else
					{
						sWhere = sWhere + " and ConsortiaID in (" + ids + ") ";
					}
				}
				if (!string.IsNullOrEmpty(name))
				{
					sWhere = sWhere + " and ConsortiaName like '%" + name + "%' ";
				}
				DataTable dt = base.GetPage("Consortia", sWhere, page, size, "*", "ConsortiaID", "ConsortiaID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					ConsortiaAllyInfo info = new ConsortiaAllyInfo();
					info.Consortia1ID = (int)dr["ConsortiaID"];
					info.ConsortiaName1 = ((dr["ConsortiaName"] == null) ? "" : dr["ConsortiaName"].ToString());
					info.ConsortiaName2 = "";
					info.Count1 = (int)dr["Count"];
					info.Repute1 = (int)dr["Repute"];
					info.ChairmanName1 = ((dr["ChairmanName"] == null) ? "" : dr["ChairmanName"].ToString());
					info.ChairmanName2 = "";
					info.Level1 = (int)dr["Level"];
					info.Honor1 = (int)dr["Honor"];
					info.Description1 = ((dr["Description"] == null) ? "" : dr["Description"].ToString());
					info.Description2 = "";
					info.Riches1 = (int)dr["Riches"];
					info.Date = DateTime.Now;
					info.IsExist = true;
					if (consortiaIDs.ContainsKey(info.Consortia1ID))
					{
						info.State = consortiaIDs[info.Consortia1ID];
					}
					info.ValidDate = 0;
					infos.Add(info);
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetConsortiaAllyPage", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public ConsortiaAllyInfo[] GetConsortiaAllyAll()
		{
			List<ConsortiaAllyInfo> infos = new List<ConsortiaAllyInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_ConsortiaAlly_All");
				while (reader.Read())
				{
					infos.Add(new ConsortiaAllyInfo
					{
						Consortia1ID = (int)reader["Consortia1ID"],
						Consortia2ID = (int)reader["Consortia2ID"],
						Date = (DateTime)reader["Date"],
						ID = (int)reader["ID"],
						State = (int)reader["State"],
						ValidDate = (int)reader["ValidDate"],
						IsExist = (bool)reader["IsExist"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
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
		public ConsortiaEventInfo[] GetConsortiaEventPage(int page, int size, ref int total, int order, int consortiaID)
		{
			List<ConsortiaEventInfo> infos = new List<ConsortiaEventInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				string sOrder = " ID desc ";
				DataTable dt = base.GetFetch_List(page, size, sOrder, sWhere, "Consortia_Event", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaEventInfo
					{
						ID = (int)dr["ID"],
						ConsortiaID = (int)dr["ConsortiaID"],
						Date = (DateTime)dr["Date"],
						IsExist = (bool)dr["IsExist"],
						Remark = dr["Remark"].ToString(),
						Type = (int)dr["Type"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public ConsortiaEventInfo[] GetConsortiaEventPages(int page, int size, ref int total, int order, int consortiaID)
		{
			List<ConsortiaEventInfo> infos = new List<ConsortiaEventInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				string sOrder = "Date desc,ID ";
				DataTable dt = base.GetPage("Consortia_Event", sWhere, page, size, "*", sOrder, "ID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaEventInfo
					{
						ID = (int)dr["ID"],
						ConsortiaID = (int)dr["ConsortiaID"],
						Date = (DateTime)dr["Date"],
						IsExist = (bool)dr["IsExist"],
						Remark = dr["Remark"].ToString(),
						Type = (int)dr["Type"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
		public ConsortiaLevelInfo[] GetAllConsortiaLevel()
		{
			List<ConsortiaLevelInfo> infos = new List<ConsortiaLevelInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Consortia_Level_All");
				while (reader.Read())
				{
					infos.Add(new ConsortiaLevelInfo
					{
						Count = (int)reader["Count"],
						Deduct = (int)reader["Deduct"],
						Level = (int)reader["Level"],
						NeedGold = (int)reader["NeedGold"],
						NeedItem = (int)reader["NeedItem"],
						Reward = (int)reader["Reward"],
						Riches = (int)reader["Riches"],
						ShopRiches = (int)reader["ShopRiches"],
						SmithRiches = (int)reader["SmithRiches"],
						StoreRiches = (int)reader["StoreRiches"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllConsortiaLevel", e);
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
		public bool AddAndUpdateConsortiaEuqipControl(ConsortiaEquipControlInfo info, int userID, ref string msg)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", info.ConsortiaID),
					new SqlParameter("@Level", info.Level),
					new SqlParameter("@Type", info.Type),
					new SqlParameter("@Riches", info.Riches),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[5].Direction = ParameterDirection.ReturnValue;
				result = this.db.RunProcedure("SP_Consortia_Equip_Control_Add", para);
				int returnValue = (int)para[2].Value;
				result = (returnValue == 0);
				int num = returnValue;
				if (num == 2)
				{
					msg = "ConsortiaBussiness.AddAndUpdateConsortiaEuqipControl.Msg2";
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return result;
		}
		public ConsortiaEquipControlInfo GetConsortiaEuqipRiches(int consortiaID, int Level, int type)
		{
			ConsortiaEquipControlInfo info = null;
			SqlDataReader reader = null;
			ConsortiaEquipControlInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ConsortiaID", consortiaID),
					new SqlParameter("@Level", Level),
					new SqlParameter("@Type", type)
				};
				this.db.GetReader(ref reader, "SP_Consortia_Equip_Control_Single", para);
				if (reader.Read())
				{
					info = new ConsortiaEquipControlInfo();
					info.ConsortiaID = (int)reader["ConsortiaID"];
					info.Level = (int)reader["Level"];
					info.Riches = (int)reader["Riches"];
					info.Type = (int)reader["Type"];
					result = info;
					return result;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetAllConsortiaLevel", e);
				}
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			if (info == null)
			{
				info = new ConsortiaEquipControlInfo();
				info.ConsortiaID = consortiaID;
				info.Level = Level;
				info.Riches = 100;
				info.Type = type;
			}
			result = info;
			return result;
		}
		public ConsortiaEquipControlInfo[] GetConsortiaEquipControlPage(int page, int size, ref int total, int order, int consortiaID, int level, int type)
		{
			List<ConsortiaEquipControlInfo> infos = new List<ConsortiaEquipControlInfo>();
			try
			{
				string sWhere = " IsExist=1 ";
				if (consortiaID != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and ConsortiaID =",
						consortiaID,
						" "
					});
				}
				if (level != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and Level =",
						level,
						" "
					});
				}
				if (type != -1)
				{
					object obj = sWhere;
					sWhere = string.Concat(new object[]
					{
						obj,
						" and Type =",
						type,
						" "
					});
				}
				string sOrder = "ConsortiaID ";
				DataTable dt = base.GetPage("Consortia_Equip_Control", sWhere, page, size, "*", sOrder, "ConsortiaID", ref total);
				foreach (DataRow dr in dt.Rows)
				{
					infos.Add(new ConsortiaEquipControlInfo
					{
						ConsortiaID = (int)dr["ConsortiaID"],
						Level = (int)dr["Level"],
						Riches = (int)dr["Riches"],
						Type = (int)dr["Type"]
					});
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Init", e);
				}
			}
			finally
			{
			}
			return infos.ToArray();
		}
	}
}
