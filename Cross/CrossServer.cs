using Game.Base;
using Lsj.Util;
using Lsj.Util.Config;
using Lsj.Util.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Cross
{
    public class CrossServer : BaseServer
    {

        public override bool Start()
        {
            this.InitSocket(IPAddress.Parse(AppConfig.AppSettings["IP"]), AppConfig.AppSettings["Port"].ConvertToInt(30000));
            LogProvider.Default = new LogProvider(new LogConfig { FilePath = "./log/Cross/" });
            base.Start();
            log.Info("CrossServer Started");
            return true;
        }
        protected override BaseClient GetNewClient()
        {
            return new CenterServerClient(this);
        }
    }
}
