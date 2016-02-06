
using Lsj.Util.Logs;
using SqlDataProvider.BaseClass;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
namespace Bussiness
{
	public class BaseBussiness : IDisposable
	{
        protected static LogProvider log => LogProvider.Default;
        protected Sql_DbObject db;
		public BaseBussiness()
		{
			this.db = new Sql_DbObject("AppConfig", "conString");
		}
		public DataTable GetPage(string queryStr, string queryWhere, int pageCurrent, int pageSize, string fdShow, string fdOreder, string fdKey, ref int total)
		{
			DataTable result;
			try
			{
				SqlParameter[] para = new SqlParameter[]
				{
					new SqlParameter("@QueryStr", queryStr),
					new SqlParameter("@QueryWhere", queryWhere),
					new SqlParameter("@PageSize", pageSize),
					new SqlParameter("@PageCurrent", pageCurrent),
					new SqlParameter("@FdShow", fdShow),
					new SqlParameter("@FdOrder", fdOreder),
					new SqlParameter("@FdKey", fdKey),
					new SqlParameter("@TotalRow", total)
				};
				para[7].Direction = ParameterDirection.Output;
				DataTable dt = this.db.GetDataTable(queryStr, "SP_CustomPage", para, 120);
				total = (int)para[7].Value;
				result = dt;
				return result;
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error(string.Format("Custome Page queryStr@{0},queryWhere@{1},pageCurrent@{2},pageSize@{3},fdShow@{4},fdOrder@{5},fdKey@{6}", new object[]
					{
						queryStr,
						queryWhere,
						pageCurrent,
						pageSize,
						fdShow,
						fdOreder,
						fdKey
					}), e);
				}
			}
			finally
			{
			}
			result = new DataTable(queryStr);
			return result;
		}
		public DataTable GetFetch_List(int page, int size, string sOrder, string sWhere, string tableName, ref int total)
		{
			DataTable result;
			try
			{
				SqlParameter[] para = new SqlParameter[6];
				para[0] = new SqlParameter("@page_num", page);
				para[1] = new SqlParameter("@row_in_page", size);
				para[2] = new SqlParameter("@order_column", sOrder);
				para[3] = new SqlParameter("@row_total", total);
				para[3].Direction = ParameterDirection.Output;
				para[4] = new SqlParameter("@comb_condition", sWhere);
				para[5] = new SqlParameter("@tablename", tableName);
				DataTable dt = this.db.GetDataTable("table1", "Sp_Fetch_List", para);
				total = (int)para[3].Value;
				result = dt;
				return result;
			}
			catch (Exception e)
			{
				if (true)
				{
					BaseBussiness.log.Error(string.Format("Sp_Fetch_List  page@{0},sOrder@{1},sWhere{2},tableName{3}", new object[]
					{
						page,
						size,
						sOrder,
						sWhere,
						tableName
					}), e);
				}
			}
			finally
			{
			}
			result = new DataTable(tableName);
			return result;
		}
		public void Dispose()
		{
			this.db.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
