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
    public class AdminModule : IModule
    {
        public void Process(object website, ProcessEventArgs args)
        {

            var Request = args.Request;

            var Uri = Request.Uri;
            if (Uri.ToString().StartsWith("/admin/"))
            {
                args.IsProcessed = true;
                args.Response = new HttpResponse();
                var Response = args.Response;
                switch (Uri.FileName)
                {
                    case "":
                    case "admin.aspx":
                        AdminPage.Process(Request, Response);
                        break;
                    case "GMAction.action":
                        GMAction.Process(Request, Response);
                        break;
                    default:
                        args.Response = ErrorHelper.Build(404, 0, args.ServerName);
                        break;
                }
            }
        }
    }
}
