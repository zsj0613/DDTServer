using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
namespace DAL
{
	public sealed class SqlHelperParameterCache
	{
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
		private SqlHelperParameterCache()
		{
		}
		private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			SqlParameter[] result;
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(spName, cn))
				{
					cn.Open();
					cmd.CommandType = CommandType.StoredProcedure;
					SqlCommandBuilder.DeriveParameters(cmd);
					if (!includeReturnValueParameter)
					{
						cmd.Parameters.RemoveAt(0);
					}
					SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];
					cmd.Parameters.CopyTo(discoveredParameters, 0);
					result = discoveredParameters;
				}
			}
			return result;
		}
		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];
			int i = 0;
			int j = originalParameters.Length;
			while (i < j)
			{
				clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
				i++;
			}
			return clonedParameters;
		}
		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			string hashKey = connectionString + ":" + commandText;
			SqlHelperParameterCache.paramCache[hashKey] = commandParameters;
		}
		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			string hashKey = connectionString + ":" + commandText;
			SqlParameter[] cachedParameters = (SqlParameter[])SqlHelperParameterCache.paramCache[hashKey];
			SqlParameter[] result;
			if (cachedParameters == null)
			{
				result = null;
			}
			else
			{
				result = SqlHelperParameterCache.CloneParameters(cachedParameters);
			}
			return result;
		}
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return SqlHelperParameterCache.GetSpParameterSet(connectionString, spName, false);
		}
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
			SqlParameter[] cachedParameters = (SqlParameter[])SqlHelperParameterCache.paramCache[hashKey];
			if (cachedParameters == null)
			{
				cachedParameters = (SqlParameter[])(SqlHelperParameterCache.paramCache[hashKey] = SqlHelperParameterCache.DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
			}
			return SqlHelperParameterCache.CloneParameters(cachedParameters);
		}
	}
}
