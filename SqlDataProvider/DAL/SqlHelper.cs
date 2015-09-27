using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
namespace DAL
{
	public sealed class SqlHelper
	{
		private enum SqlConnectionOwnership
		{
			Internal,
			External
		}
		private SqlHelper()
		{
		}
		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			for (int i = 0; i < commandParameters.Length; i++)
			{
				SqlParameter p = commandParameters[i];
				if (p.Direction == ParameterDirection.InputOutput && p.Value == null)
				{
					p.Value = DBNull.Value;
				}
				command.Parameters.Add(p);
			}
		}
		public static void AssignParameterValues(SqlParameter[] commandParameters, params object[] parameterValues)
		{
			if (commandParameters != null && parameterValues != null)
			{
				if (commandParameters.Length != parameterValues.Length)
				{
					throw new ArgumentException("Parameter count does not match Parameter Value count.");
				}
				int i = 0;
				int j = commandParameters.Length;
				while (i < j)
				{
					if (parameterValues[i] != null && (commandParameters[i].Direction == ParameterDirection.Input || commandParameters[i].Direction == ParameterDirection.InputOutput))
					{
						commandParameters[i].Value = parameterValues[i];
					}
					i++;
				}
			}
		}
		public static void AssignParameterValues(SqlParameter[] commandParameters, Hashtable parameterValues)
		{
			if (commandParameters != null && parameterValues != null)
			{
				if (commandParameters.Length != parameterValues.Count)
				{
					throw new ArgumentException("Parameter count does not match Parameter Value count.");
				}
				int i = 0;
				int j = commandParameters.Length;
				while (i < j)
				{
					if (parameterValues[commandParameters[i].ParameterName] != null && (commandParameters[i].Direction == ParameterDirection.Input || commandParameters[i].Direction == ParameterDirection.InputOutput))
					{
						commandParameters[i].Value = parameterValues[commandParameters[i].ParameterName];
					}
					i++;
				}
			}
		}
		private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
		{
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}
			command.Connection = connection;
			command.CommandText = commandText;
			if (transaction != null)
			{
				command.Transaction = transaction;
			}
			command.CommandType = commandType;
			if (commandParameters != null)
			{
				SqlHelper.AttachParameters(command, commandParameters);
			}
		}
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteNonQuery(connectionString, commandType, commandText, null);
		}
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			int result;
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				cn.Open();
				result = SqlHelper.ExecuteNonQuery(cn, commandType, commandText, commandParameters);
			}
			return result;
		}
		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			int result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static int ExecuteNonQuery(string connectionString, string spName, Hashtable parameterValues)
		{
			int result;
			if (parameterValues != null && parameterValues.Count > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteNonQuery(connection, commandType, commandText, null);
		}
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
			int retval = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return retval;
		}
		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
		{
			int result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
			int retval = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			return retval;
		}
		public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			int result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, new SqlParameter[0]);
			}
			return result;
		}
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(connectionString, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			DataSet result;
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				cn.Open();
				result = SqlHelper.ExecuteDataset(cn, commandType, commandText, commandParameters);
			}
			return result;
		}
		public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			DataSet result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(connection, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			DataSet ds = new DataSet();
			da.Fill(ds);
			cmd.Parameters.Clear();
			return ds;
		}
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
		{
			DataSet result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(transaction, commandType, commandText, null);
		}
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			DataSet ds = new DataSet();
			da.Fill(ds);
			cmd.Parameters.Clear();
			return ds;
		}
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			DataSet result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlHelper.SqlConnectionOwnership connectionOwnership)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);
			SqlDataReader dr;
			if (connectionOwnership == SqlHelper.SqlConnectionOwnership.External)
			{
				dr = cmd.ExecuteReader();
			}
			else
			{
				dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			}
			cmd.Parameters.Clear();
			return dr;
		}
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(connectionString, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlConnection cn = new SqlConnection(connectionString);
			cn.Open();
			SqlDataReader result;
			try
			{
				result = SqlHelper.ExecuteReader(cn, null, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.Internal);
			}
			catch
			{
				cn.Close();
				throw;
			}
			return result;
		}
		public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			SqlDataReader result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(connection, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.External);
		}
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			SqlDataReader result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(transaction, commandType, commandText, null);
		}
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.External);
		}
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			SqlDataReader result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(connectionString, commandType, commandText, null);
		}
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			object result;
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				cn.Open();
				result = SqlHelper.ExecuteScalar(cn, commandType, commandText, commandParameters);
			}
			return result;
		}
		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			object result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(connection, commandType, commandText, null);
		}
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
			object retval = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return retval;
		}
		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
		{
			object result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(transaction, commandType, commandText, null);
		}
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
			object retval = cmd.ExecuteScalar();
			cmd.Parameters.Clear();
			return retval;
		}
		public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			object result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteXmlReader(connection, commandType, commandText, null);
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
			XmlReader retval = cmd.ExecuteXmlReader();
			cmd.Parameters.Clear();
			return retval;
		}
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			XmlReader result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteXmlReader(transaction, commandType, commandText, null);
		}
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
			XmlReader retval = cmd.ExecuteXmlReader();
			cmd.Parameters.Clear();
			return retval;
		}
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			XmlReader result;
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				result = SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				result = SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
			}
			return result;
		}
		public static void BeginExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlCommand cmd = new SqlCommand();
			SqlHelper.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
			cmd.BeginExecuteNonQuery();
			cmd.Parameters.Clear();
		}
		public static void BeginExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				cn.Open();
				SqlHelper.ExecuteNonQuery(cn, commandType, commandText, commandParameters);
			}
		}
		public static void BeginExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (parameterValues != null && parameterValues.Length > 0)
			{
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
				SqlHelper.AssignParameterValues(commandParameters, parameterValues);
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
		}
	}
}
