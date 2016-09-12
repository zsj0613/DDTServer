using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web.Error;
using Lsj.Util.Net.Web.Event;
using Lsj.Util.Net.Web.Interfaces;
using Lsj.Util.Net.Web.Message;
using Web.Server.Modules.Admin;

namespace Web.Server.Modules
{
    public class CDNModule : IModule
    {
        public void Process(object website, ProcessEventArgs args)
        {

            var Request = args.Request;

            var Uri = Request.Uri;
            if (Uri.ToString().StartsWith("/cdn/"))
            {
                args.IsProcessed = true;
                args.Response = new HttpResponse();
                var Response = args.Response;
                var uri = Uri.ToString().Remove(0, 4);
                ((HttpResponse)Response).Redirect($"http://{WebServer.Instance.Config.CDNDomain}{uri}");
            }
        }
    }
}
