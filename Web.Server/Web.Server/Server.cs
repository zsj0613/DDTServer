using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web;
using System.Net;
using Lsj.Util.Net.Sockets;
using Game.Base.Events;
using Game.Base.Managers;
using Web.Server;
using System.Threading;
using Lsj.Util;

namespace Web.Server
{
    public class Server: MyHttpWebServer
    {
        public static string ModulePath = Static.CurrentPath() + @"web\Modules\";
        public static string WebPath = Static.CurrentPath() + @"web\";
        private static Server instance;
        public static void AddModule(Type type)
        {
            instance.InsertModule(type);
        }

        public Server(IPAddress ip,int port) : base(ip,port)
        {
            instance = this;
        }
        

    }
}
