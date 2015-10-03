using Bussiness.CenterService;
using SqlDataProvider.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
namespace Bussiness
{
	public class ManageBussiness : BaseBussiness
	{
		public int KitoffUserByUserName(string name, string msg)
		{
			int result = 1;
			int result2;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				PlayerInfo player = db.GetUserSingleByUserName(name);
				if (player == null)
				{
					result2 = 2;
					return result2;
				}
				result = this.KitoffUser(player.ID, msg);
			}
			result2 = result;
			return result2;
		}
		public int KitoffUserByNickName(string name, string msg)
		{
			int result = 1;
			int result2;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				PlayerInfo player = db.GetUserSingleByNickName(name);
				if (player == null)
				{
					result2 = 2;
					return result2;
				}
				result = this.KitoffUser(player.ID, msg);
			}
			result2 = result;
			return result2;
		}
		public int KitoffUser(int id, string msg)
		{
			int result;
			try
			{
				using (CenterServiceClient temp = new CenterServiceClient())
				{
					if (temp.KitoffUser(id, msg))
					{
						result = 0;
					}
					else
					{
						result = 3;
					}
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("KitoffUser", e);
				}
				result = 1;
			}
			return result;
		}
		public bool SystemNotice(string msg)
		{
			bool result = false;
			try
			{
				if (!string.IsNullOrEmpty(msg))
				{
					using (CenterServiceClient temp = new CenterServiceClient())
					{
						if (temp.SystemNotice(msg))
						{
							result = true;
						}
					}
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SystemNotice", e);
				}
			}
			return result;
		}
		private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist)
		{
			return this.ForbidPlayer(userName, nickName, userID, forbidDate, isExist, "");
		}
		private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist, string ForbidReason)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[6];
				para[0] = new SqlParameter("@UserName", userName);
				para[1] = new SqlParameter("@NickName", nickName);
				para[2] = new SqlParameter("@UserID", userID);
				para[2].Direction = ParameterDirection.InputOutput;
				para[3] = new SqlParameter("@ForbidDate", forbidDate);
				para[4] = new SqlParameter("@IsExist", isExist);
				para[5] = new SqlParameter("@ForbidReason", ForbidReason);
				this.db.RunProcedure("SP_Admin_ForbidUser", para);
				userID = (int)para[2].Value;
				if (userID > 0)
				{
					result = true;
					if (!isExist)
					{
						this.KitoffUser(userID, "You are kicking out by GM!!");
					}
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
		public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist)
		{
			return this.ForbidPlayer(userName, "", 0, date, isExist);
		}
		public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist)
		{
			return this.ForbidPlayer("", nickName, 0, date, isExist);
		}
		public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist)
		{
			return this.ForbidPlayer("", "", userID, date, isExist);
		}
		public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist, string ForbidReason)
		{
			return this.ForbidPlayer(userName, "", 0, date, isExist, ForbidReason);
		}
		public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist, string ForbidReason)
		{
			return this.ForbidPlayer("", nickName, 0, date, isExist, ForbidReason);
		}
		public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist, string ForbidReason)
		{
			return this.ForbidPlayer("", "", userID, date, isExist, ForbidReason);
		}
		public bool ReLoadServerList()
		{
			bool result = false;
			try
			{
				using (CenterServiceClient temp = new CenterServiceClient())
				{
					if (temp.ReLoadServerList())
					{
						result = true;
					}
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("ReLoadServerList", e);
				}
			}
			return result;
		}
		public int GetConfigState(int type)
		{
			int result = 2;
			int result2;
			try
			{
				using (CenterServiceClient temp = new CenterServiceClient())
				{
					result2 = temp.GetConfigState(type);
					return result2;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("GetConfigState", e);
				}
			}
			result2 = result;
			return result2;
		}
		public bool UpdateConfigState(int type, bool state)
		{
			bool result = false;
			bool result2;
			try
			{
				using (CenterServiceClient temp = new CenterServiceClient())
				{
					result2 = temp.UpdateConfigState(type, state);
					return result2;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("UpdateConfigState", e);
				}
			}
			result2 = result;
			return result2;
		}
		public bool Reload(string type)
		{
			bool result = false;
			bool result2;
			try
			{
				using (CenterServiceClient temp = new CenterServiceClient())
				{
					result2 = temp.Reload(type);
					return result2;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("Reload", e);
				}
			}
			result2 = result;
			return result2;
		}
		public bool SendAreaBigBugle(int playerID, int areaID, string nickName, string msg)
		{
			bool result = false;
			bool result2;
			try
			{
				using (CenterServiceClient temp = new CenterServiceClient())
				{
					result2 = temp.SendAreaBigBugle(playerID, areaID, nickName, msg);
					return result2;
				}
			}
			catch (Exception e)
			{
				if (BaseBussiness.log.IsErrorEnabled)
				{
					BaseBussiness.log.Error("SendAreaBigBugle", e);
				}
			}
			result2 = result;
			return result2;
		}
        public List<UserInfo> GetAllUserInfo()
        {
            List<UserInfo> result = new List<UserInfo>();
            SqlDataReader reader = null;
            db.GetReader(ref reader, "GM_GetAllUserInfo");
            while (reader.Read())
            {
                result.Add(new UserInfo
                {
                    UserType = (int)reader["UserType"],
                    UserName = (string)reader["UserName"],
                    InviteID = (int)reader["InviteID"],
                    NickName = (string)reader["NickName"],
                    Date = (DateTime)reader["Date"],
                    Money = (int)reader["Money"],
                    Grade = (int)reader["Grade"],
                    ActiveIP = (string)reader["ActiveIP"],
                    ChargedMoney = (int)reader["ChargedMoney"],
                    Sex = (bool)reader["Sex"] ? "ÄÐ" : "Å®",
                    UserID = (int)reader["UserID"],
                    State = (int)reader["State"],
                    IsExist = (bool)reader["IsExist"],
                }
                    );
            }

            return result;
        }

        public bool AddCharge(int ID, int UserID, int Money)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ID", ID);
                para[1] = new SqlParameter("@UserID", UserID);
                para[2] = new SqlParameter("@Money", Money);
                result = this.db.RunProcedure("GM_AddCharge", para);
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

        public bool UpdateUser()
        {
            bool result = false;
            try
            {
                result = this.db.RunProcedure("Mem_Users_UpdateID");
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
    }
}
