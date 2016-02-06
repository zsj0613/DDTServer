using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace Bussiness
{
	public class MapBussiness : BaseBussiness
	{
		public MapInfo[] GetAllMap()
		{
			List<MapInfo> infos = new List<MapInfo>();
			SqlDataReader reader = null;
			//try
			//{
				this.db.GetReader(ref reader, "SP_Maps_All");
				while (reader.Read())
				{
					infos.Add(new MapInfo
					{
						BackMusic = (reader["BackMusic"] == null) ? "" : reader["BackMusic"].ToString(),
						BackPic = (reader["BackPic"] == null) ? "" : reader["BackPic"].ToString(),
						BackroundHeight = (int)reader["BackroundHeight"],
						BackroundWidht = (int)reader["BackroundWidht"],
						DeadHeight = (int)reader["DeadHeight"],
						DeadPic = (reader["DeadPic"] == null) ? "" : reader["DeadPic"].ToString(),
						DeadWidth = (int)reader["DeadWidth"],
						Description = (reader["Description"] == null) ? "" : reader["Description"].ToString(),
						DragIndex = (int)reader["DragIndex"],
						ForegroundHeight = (int)reader["ForegroundHeight"],
						ForegroundWidth = (int)reader["ForegroundWidth"],
						ForePic = (reader["ForePic"] == null) ? "" : reader["ForePic"].ToString(),
						ID = (int)reader["ID"],
						Name = (reader["Name"] == null) ? "" : reader["Name"].ToString(),
						Pic = (reader["Pic"] == null) ? "" : reader["Pic"].ToString(),
						Remark = (reader["Remark"] == null) ? "" : reader["Remark"].ToString(),
						Weight = (int)reader["Weight"],
						PosX = (reader["PosX"] == null) ? "" : reader["PosX"].ToString(),
						PosX1 = (reader["PosX1"] == null) ? "" : reader["PosX1"].ToString(),
						PosX2 = (reader["PosX2"] == null) ? "" : reader["PosX2"].ToString(),
						Type = (byte)((int)reader["Type"])
					});
				}
			//}
			//catch (Exception e)
			//{
			//	if (true)
			//	{
			//		BaseBussiness.log.Error("GetAllMap", e);
			//	}
			//}
			//finally
			//{
			//	if (reader != null && !reader.IsClosed)
			//	{
			//		reader.Close();
			//	}
			//}
			return infos.ToArray();
		}
		public ServerMapInfo[] GetAllServerMap()
		{
			List<ServerMapInfo> infos = new List<ServerMapInfo>();
			SqlDataReader reader = null;
			try
			{
				this.db.GetReader(ref reader, "SP_Maps_Server_All");
				while (reader.Read())
				{
					infos.Add(new ServerMapInfo
					{
						ServerID = (int)reader["ServerID"],
						OpenMap = reader["OpenMap"].ToString(),
						IsSpecial = (int)reader["IsSpecial"]
					});
				}
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error("GetAllMapWeek", e);
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
	}
}
