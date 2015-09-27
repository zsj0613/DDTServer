using System;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class CookieInfoBussiness : BaseBussiness
	{
		public bool GetFromDbByUser(string bdSigUser, ref string bdSigPortrait, ref string bdSigSessionKey)
		{
			SqlDataReader reader = null;
			bool result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@BdSigUser", bdSigUser)
				};
				this.db.GetReader(ref reader, "SP_Cookie_Info_QueryByUser", para);
				while (reader.Read())
				{
					bdSigPortrait = ((reader["BdSigPortrait"] == null) ? "" : reader["BdSigPortrait"].ToString());
					bdSigSessionKey = ((reader["BdSigSessionKey"] == null) ? "" : reader["BdSigSessionKey"].ToString());
				}
				if (!string.IsNullOrEmpty(bdSigPortrait) && !string.IsNullOrEmpty(bdSigSessionKey))
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
				if (BaseBussiness.log.IsErrorEnabled)
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
		public bool AddCookieInfo(string bdSigUser, string bdSigPortrait, string bdSigSessionKey)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@BdSigUser", bdSigUser),
					new SqlParameter("@BdSigPortrait", bdSigPortrait),
					new SqlParameter("@BdSigSessionKey", bdSigSessionKey),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Cookie_Info_Insert", para);
				int returnValue = (int)para[3].Value;
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
	}
}
