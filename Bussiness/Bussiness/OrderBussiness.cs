using System;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class OrderBussiness : BaseBussiness
	{
		public bool AddOrder(string order, double amount, string username, string payWay, string serverId)
		{
			bool result = false;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Order", order),
					new SqlParameter("@Amount", amount),
					new SqlParameter("@Username", username),
					new SqlParameter("@PayWay", payWay),
					new SqlParameter("@ServerId", serverId),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[5].Direction = ParameterDirection.ReturnValue;
				this.db.RunProcedure("SP_Charge_Order", para);
				int returnValue = (int)para[5].Value;
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
		public string GetOrderToName(string order, ref string serverId)
		{
			SqlDataReader reader = null;
			string result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@Order", order)
				};
				this.db.GetReader(ref reader, "SP_Charge_Order_Single", para);
				if (reader.Read())
				{
					serverId = ((reader["ServerId"] == null) ? "" : reader["ServerId"].ToString());
					string userName = (reader["UserName"] == null) ? "" : reader["UserName"].ToString();
					result = userName;
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
			result = "";
			return result;
		}
	}
}
