using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web.Event;
using Lsj.Util.Net.Web.Interfaces;
using Lsj.Util.Net.Web.Message;

namespace Web.Server.Modules
{
    public class CrossdomainModule : IModule
    {
        public void Process(object website, ProcessEventArgs args)
        {
            var Request = args.Request;

            var Uri = Request.Uri;
            if (Uri.ToString() == "/crossdomain.xml")
            {
                args.IsProcessed = true;
                args.Response = new HttpResponse();
                var Response = args.Response;
                Response.Write(
                    @"<?xml version=""1.0""?>
<!DOCTYPE cross-domain-policy SYSTEM ""http://www.adobe.com/xml/dtds/cross-domain-policy.dtd"">
<cross-domain-policy>
    <site-control permitted-cross-domain-policies=""all""/>
    <allow-access-from domain=""*""/>
</cross-domain-policy>"
                    );
            }
        }
    }
}
