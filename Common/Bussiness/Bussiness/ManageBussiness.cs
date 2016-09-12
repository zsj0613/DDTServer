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
                }
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
                    Sex = (bool)reader["Sex"],
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
                if (true)
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
                if (true)
                {
                    BaseBussiness.log.Error("Init", e);
                }
            }
            return result;

        }

        public bool UpdateName()
        {
            bool result = false;
            try
            {
                result = this.db.RunProcedure("Sp_Renames_Batch");
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
    }
}
