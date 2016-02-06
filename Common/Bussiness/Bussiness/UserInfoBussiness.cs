using System;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class UserInfoBussiness : BaseBussiness
	{
		public bool GetFromDbByUid(string uid, ref string userName, ref string portrait)
		{
			SqlDataReader reader = null;
			bool result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Uid", uid)
				};
				this.db.GetReader(ref reader, "SP_User_Info_QueryByUid", para);
				while (reader.Read())
				{
					userName = ((reader["UserName"] == null) ? "" : reader["UserName"].ToString());
					portrait = ((reader["Portrait"] == null) ? "" : reader["Portrait"].ToString());
				}
				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(portrait))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("Init", e);
				}
				result = false;
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}
			}
			return result;
		}
		public bool AddUserInfo(string uid, string userName, string portrait)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Uid", uid),
					new SqlParameter("@UserName", userName),
					new SqlParameter("@Portrait", portrait),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_User_Info_Insert", para);
				int returnValue = (int)para[3].Value;
				result = (returnValue == 0);
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
