using Bussiness;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Center.Server.Managers
{
	public class ServerMgr
	{
		private static LogProvider log => CenterServer.log;
		private static Dictionary<int, ServerInfo> _list = new Dictionary<int, ServerInfo>();
		private static object _syncStop = new object();
		public static ServerInfo[] Servers
		{
			get
			{
				return _list.Values.ToArray<ServerInfo>();
			}
		}
		
		public static bool Add(int id ,ServerInfo info)
        {
            lock(_syncStop)
            {
                if(_list.ContainsKey(id))
                {
                    return false;
                }
                else
                {
                    _list.Add(id, info);
                    return true;
                }
            }
        }
	}
}
