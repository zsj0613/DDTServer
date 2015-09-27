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

namespace Web.Server
{
    public class Server: MyHttpWebServer
    {
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
        protected override void Process(HttpRequest request, TcpSocket handle)
        {
            try
            {
                if (request.uri.ToLower().StartsWith("/modules"))
                {
                    SendErrorAndDisconnect(handle, 403);
                }
                else
                {
                    IModule module = null;
                    var a = request.uri.ToLower();
                    foreach (string header in m_modules.Keys)
                    {
                        if (request.uri.ToLower().StartsWith(header))
                        {
                            module = (IModule)ScriptMgr.CreateInstance(m_modules[header]);
                        }
                    }
                    if (module != null)
                    {
                        var response = (module as IModule).Process(request);
                        base.Response(handle, response);
                    }
                    else
                    {
                        base.Process(request, handle);
                    }
                }
            }
            catch (Exception e)
            {
                WebServer.log.Error(e);
                SendErrorAndDisconnect(handle, 400);
            }
        }
    }
}
