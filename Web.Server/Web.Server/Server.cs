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
        private static readonly Dictionary<string, string> m_modules = new Dictionary<string, string>();
        private static object @lock = new object();
        public static void AddModule(string header,string @class)
        {
            Monitor.Enter(@lock);
            try
            {
                m_modules.Add(header,@class);
            }
            finally
            {
                Monitor.Exit(@lock);
            }
        }

        public Server(IPAddress ip,int port) : base(ip,port)
        {
        }
        protected override void Process(HttpClient client)
        {
            try
            {
                if (client.request.uri.ToLower().StartsWith("/modules"))
                {
                    SendErrorAndDisconnect(client, 403);
                }
                else
                {
                    IModule module = null;
                    var a = client.request.uri.ToLower();
                    WebServer.log.Debug(a);
                    foreach (string header in m_modules.Keys)
                    {
                        if (client.request.uri.ToLower()==header)
                        {
                            module = (IModule)ScriptMgr.CreateInstance(m_modules[header]);
                        }
                    }
                    if (module != null)
                    {
                        var x = client;
                        module.Process(ref x);
                        base.Response(x);
                    }
                    else
                    {
                        base.Process(client);
                    }
                }
            }
            catch (Exception e)
            {
                WebServer.log.Error(e);
                SendErrorAndDisconnect(client, 400);
               // throw;
            }
        }

    }
}
