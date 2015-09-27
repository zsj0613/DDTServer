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

namespace Web.Server
{
    public class Server: MyHttpWebServer
    {
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
                    if (request.uri.ToLower().StartsWith("/charge.do?"))
                    {
                        var a = ScriptMgr.CreateInstance("Web.Server.Module.Charge");
                        if (a is IModule)
                        {
                            var response = (a as IModule).Process(request);
                            base.Response(handle, response);
                        }
                        else
                        {
                            SendErrorAndDisconnect(handle, 500);
                        }
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
