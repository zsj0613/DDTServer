using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Center.Server;
using Center.Server.Managers;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Interfaces;
using SqlDataProvider.Data;

namespace Web.Server.Modules.Ashx
{
    public class ServerList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            int total = 0;
            XElement result = new XElement("Result");
            try
            {
                if (CenterServer.IsRun)
                {
                    ServerInfo[] sl = ServerMgr.Servers;
                    foreach (var s in sl)
                    {
                        if (s.State != -1)
                        {
                            total += s.Online;
                            result.Add(FlashUtils.CreateServerInfo(s.ID, s.Name, s.IP, s.Port, s.State, s.MustLevel, s.LowestLevel, s.Online));
                        }
                    }
                }

                value = true;
                message = "Success!";
            }
            catch
            {
                //serverList.log.Error("Load server list error:", ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            result.Add(new XAttribute("total", total));
            result.Add(new XAttribute("agentId", "1"));
            result.Add(new XAttribute("AreaName", "DDT"));
            Response.Write(result.ToString(false));
        }
       
    }
}
