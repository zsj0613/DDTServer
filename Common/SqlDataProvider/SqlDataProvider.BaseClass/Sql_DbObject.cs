using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Lsj.Util.Config;
namespace SqlDataProvider.BaseClass
{
	public sealed class Sql_DbObject : IDisposable
	{
		private SqlConnection _SqlConnection;
		private SqlCommand _SqlCommand;
		private SqlDataAdapter _SqlDataAdapter;
		public Sql_DbObject()
		{
			this._SqlConnection = new SqlConnection();
		}
        public Sql_DbObject(string conn)
        {
                this._SqlConnection = new SqlConnection(conn);
        }
        public static bool TryConnection()
        {
            bool result = false;
            try
            {

                    Sql_DbObject temptank = new Sql_DbObject(AppConfig.AppSettings["conString"]);
                    result = OpenConnection(temptank._SqlConnection);
                    temptank.Dispose();
            }
            catch
            {
            }
            return result;
        }

        public Sql_DbObject(string Path_Source, string Conn_DB)
		{
			if (Path_Source != null)
			{
				if (Path_Source == "WebConfig")
				{
					this._SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[Conn_DB].ConnectionString);
					return;
				}
				if (Path_Source == "File")
				{
					this._SqlConnection = new SqlConnection(Conn_DB);
					return;
				}
				if (Path_Source == "AppConfig")
				{
					string str = AppConfig.AppSettings[Conn_DB];
					this._SqlConnection = new SqlConnection(str);
					return;
				}
			}
			this._SqlConnection = new SqlConnection(Conn_DB);
		}
		private static bool OpenConnection(SqlConnection _SqlConnection)
		{
			bool result = false;
			//try
			{
				if (_SqlConnection.State != ConnectionState.Open)
				{
					_SqlConnection.Open();
					result = true;
				}
				else
				{
					result = true;
				}
			}
			//catch (SqlException ex)
			//{
			//	ApplicationLog.WriteError("打开数据库连接错误:" + ex.Message.Trim());
			//	result = false;
			//}
			return result;
		}
		public bool Exesqlcomm(string Sqlcomm)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.CommandType = CommandType.Text;
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandText = Sqlcomm;
					this._SqlCommand.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
					result = false;
					return result;
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = true;
			}
			return result;
		}
		public int GetRecordCount(string Sqlcomm)
		{
			int retval = 0;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				retval = 0;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.Text;
					this._SqlCommand.CommandText = Sqlcomm;
					if (this._SqlCommand.ExecuteScalar() == null)
					{
						retval = 0;
					}
					else
					{
						retval = (int)this._SqlCommand.ExecuteScalar();
					}
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
			}
			return retval;
		}
		public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm)
		{
			DataTable ResultTable = new DataTable(TableName);
			DataTable result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = ResultTable;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.Text;
					this._SqlCommand.CommandText = Sqlcomm;
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					this._SqlDataAdapter.Fill(ResultTable);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = ResultTable;
			}
			return result;
		}
		public DataSet GetDataSetBySqlcomm(string TableName, string Sqlcomm)
		{
			DataSet ResultTable = new DataSet();
			DataSet result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = ResultTable;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.Text;
					this._SqlCommand.CommandText = Sqlcomm;
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					this._SqlDataAdapter.Fill(ResultTable);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行Sql语句：" + Sqlcomm + "错误信息为：" + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = ResultTable;
			}
			return result;
		}
		public bool FillSqlDataReader(ref SqlDataReader Sdr, string SqlComm)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.Text;
					this._SqlCommand.CommandText = SqlComm;
					Sdr = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
					result = true;
					return result;
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行Sql语句：" + SqlComm + "错误信息为：" + ex.Message.Trim());
				}
				finally
				{
					this.Dispose(true);
				}
				result = false;
			}
			return result;
		}
		public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm, int StartRecordNo, int PageSize)
		{
			DataTable RetTable = new DataTable(TableName);
			DataTable result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				RetTable.Dispose();
				this.Dispose(true);
				result = RetTable;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.Text;
					this._SqlCommand.CommandText = Sqlcomm;
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					DataSet ds = new DataSet();
					ds.Tables.Add(RetTable);
					this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = RetTable;
			}
			return result;
		}
		public bool RunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlCommand.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
                    //throw ex;
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
					result = false;
					return result;
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = true;
			}
			return result;
		}
		public bool RunProcedure(string ProcedureName)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					this._SqlCommand.ExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
					result = false;
					return result;
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = true;
			}
			return result;
		}
		public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					ResultDataReader = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
					result = false;
					return result;
				}
				result = true;
			}
			return result;
		}
		public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName, SqlParameter[] SqlParameters)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					ResultDataReader = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
					result = false;
					return result;
				}
				result = true;
			}
			return result;
		}
		public DataSet GetDataSet(string ProcedureName, SqlParameter[] SqlParameters)
		{
			DataSet FullDataSet = new DataSet();
			DataSet result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				FullDataSet.Dispose();
				result = FullDataSet;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					this._SqlDataAdapter.Fill(FullDataSet);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错信信息为：" + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = FullDataSet;
			}
			return result;
		}
		public bool GetDataSet(ref DataSet ResultDataSet, ref int row_total, string TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					row_total = 0;
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					DataSet ds = new DataSet();
					row_total = this._SqlDataAdapter.Fill(ds);
					this._SqlDataAdapter.Fill(ResultDataSet, StartRecordNo, PageSize, TableName);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错误信息为：" + ex.Message.Trim());
					result = false;
					return result;
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = true;
			}
			return result;
		}
		public DataSet GetDateSet(string DatesetName, string ProcedureName, SqlParameter[] SqlParameters)
		{
			DataSet FullDataSet = new DataSet(DatesetName);
			DataSet result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				FullDataSet.Dispose();
				result = FullDataSet;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					this._SqlDataAdapter.Fill(FullDataSet);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错信信息为：" + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = FullDataSet;
			}
			return result;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters)
		{
			return this.GetDataTable(TableName, ProcedureName, SqlParameters, -1);
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters, int commandTimeout)
		{
			DataTable FullTable = new DataTable(TableName);
			DataTable result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				FullTable.Dispose();
				this.Dispose(true);
				result = FullTable;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					if (commandTimeout >= 0)
					{
						this._SqlCommand.CommandTimeout = commandTimeout;
					}
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					this._SqlDataAdapter.Fill(FullTable);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = FullTable;
			}
			return result;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName)
		{
			DataTable FullTable = new DataTable(TableName);
			DataTable result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				FullTable.Dispose();
				this.Dispose(true);
				result = FullTable;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					this._SqlDataAdapter.Fill(FullTable);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = FullTable;
			}
			return result;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, int StartRecordNo, int PageSize)
		{
			DataTable RetTable = new DataTable(TableName);
			DataTable result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				RetTable.Dispose();
				this.Dispose(true);
				result = RetTable;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					DataSet ds = new DataSet();
					ds.Tables.Add(RetTable);
					this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = RetTable;
			}
			return result;
		}
		public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters, int StartRecordNo, int PageSize)
		{
			DataTable RetTable = new DataTable(TableName);
			DataTable result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				RetTable.Dispose();
				this.Dispose(true);
				result = RetTable;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					DataSet ds = new DataSet();
					ds.Tables.Add(RetTable);
					this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = RetTable;
			}
			return result;
		}
		public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize)
		{
			ResultTable = null;
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					DataSet ds = new DataSet();
					ds.Tables.Add(ResultTable);
					this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
					ResultTable = ds.Tables[TableName];
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
					result = false;
					return result;
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = true;
			}
			return result;
		}
		public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
		{
			bool result;
			if (!Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				result = false;
			}
			else
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlDataAdapter = new SqlDataAdapter();
					this._SqlDataAdapter.SelectCommand = this._SqlCommand;
					DataSet ds = new DataSet();
					ds.Tables.Add(ResultTable);
					this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
					ResultTable = ds.Tables[TableName];
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
					result = false;
					return result;
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
				result = true;
			}
			return result;
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(true);
		}
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this._SqlDataAdapter != null)
				{
					if (this._SqlDataAdapter.SelectCommand != null)
					{
						if (this._SqlCommand.Connection != null)
						{
							this._SqlDataAdapter.SelectCommand.Connection.Dispose();
						}
						this._SqlDataAdapter.SelectCommand.Dispose();
					}
					this._SqlDataAdapter.Dispose();
					this._SqlDataAdapter = null;
				}
			}
		}
		public void BeginRunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
		{
			if (Sql_DbObject.OpenConnection(this._SqlConnection))
			{
				try
				{
					this._SqlCommand = new SqlCommand();
					this._SqlCommand.Connection = this._SqlConnection;
					this._SqlCommand.CommandType = CommandType.StoredProcedure;
					this._SqlCommand.CommandText = ProcedureName;
					for (int i = 0; i < SqlParameters.Length; i++)
					{
						SqlParameter parameter = SqlParameters[i];
						this._SqlCommand.Parameters.Add(parameter);
					}
					this._SqlCommand.BeginExecuteNonQuery();
				}
				catch (SqlException ex)
				{
					ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
				}
				finally
				{
					this._SqlConnection.Close();
					this.Dispose(true);
				}
			}
		}
	}
}
