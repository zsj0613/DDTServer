using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Bussiness
{
	public class ActiveBussiness : BaseBussiness
	{
		public ActiveInfo[] GetAllActives()
		{
			List<ActiveInfo> infos = new List<ActiveInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Active_All");
				while (reader.Read())
				{
					infos.Add(this.InitActiveInfo(reader));
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
		public ActiveInfo GetSingleActives(int activeID)
		{
			SqlDataReader reader = null;
			ActiveInfo result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ID", SqlDbType.Int, 4)
				};
				para[0].Value = activeID;
				this.db.GetReader(ref reader, "SP_Active_Single", para);
				if (reader.Read())
				{
					result = this.InitActiveInfo(reader);
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
		public ActiveInfo InitActiveInfo(SqlDataReader reader)
		{
			ActiveInfo info = new ActiveInfo();
			info.ActiveID = (int)reader["ActiveID"];
			info.Description = ((reader["Description"] == null) ? "" : reader["Description"].ToString());
			info.Content = ((reader["Content"] == null) ? "" : reader["Content"].ToString());
			info.AwardContent = ((reader["AwardContent"] == null) ? "" : reader["AwardContent"].ToString());
			info.HasKey = (int)reader["HasKey"];
			if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
			{
				info.EndDate = (DateTime)reader["EndDate"];
			}
			info.IsOnly = (bool)reader["IsOnly"];
			info.StartDate = (DateTime)reader["StartDate"];
			info.Title = reader["Title"].ToString();
			info.Type = (int)reader["Type"];
			info.ActionTimeContent = ((reader["ActionTimeContent"] == null) ? "" : reader["ActionTimeContent"].ToString());
			return info;
		}
		public int PullDown(int activeID, string awardID, int userID, ref string msg)
		{
			int result = 1;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@ActiveID", activeID),
					new SqlParameter("@AwardID", awardID),
					new SqlParameter("@UserID", userID),
					new SqlParameter("@Result", SqlDbType.Int)
				};
				para[3].Direction = ParameterDirection.ReturnValue;
				if (this.db.RunProcedure("SP_Active_PullDown", para))
				{
					result = (int)para[3].Value;
					switch (result)
					{
					case 0:
						msg = "ActiveBussiness.Msg0";
						break;
					case 1:
						msg = "ActiveBussiness.Msg1";
						break;
					case 2:
						msg = "ActiveBussiness.Msg2";
						break;
					case 3:
						msg = "ActiveBussiness.Msg3";
						break;
					case 4:
						msg = "ActiveBussiness.Msg4";
						break;
					case 5:
						msg = "ActiveBussiness.Msg5";
						break;
					case 6:
						msg = "ActiveBussiness.Msg6";
						break;
					case 7:
						msg = "ActiveBussiness.Msg7";
						break;
					case 8:
						msg = "ActiveBussiness.Msg8";
						break;
					default:
						msg = "ActiveBussiness.Msg9";
						break;
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
	}
}
