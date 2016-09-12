using Lsj.Util.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net;
using Web.Server.Modules;

namespace Web.Server
{
    public class Server : Lsj.Util.Net.Web.WebServer
    {
        public Server() : base()
        {
            var config = WebServer.Instance.Config;
            var website = new Website(config.APIDomain);
            this.Websites.Add(config.APIDomain,website );
            website.Modules.Add(new DDTAPIModule());
            website.Modules.Add(new CrossdomainModule());
            website.Modules.Add(new XMLModule());
            website.Modules.Add(new AshxModule());
            website.Modules.Add(new AdminModule());
            website.Modules.Add(new CDNModule());
        }
    }
}
