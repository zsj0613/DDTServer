using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web.Error;
using Lsj.Util.Net.Web.Event;
using Lsj.Util.Net.Web.Interfaces;
using Lsj.Util.Net.Web.Message;
using Web.Server.Modules.Ashx;

namespace Web.Server.Modules
{
    public class AshxModule : IModule
    {
        public void Process(object website, ProcessEventArgs args)
        {

            var Request = args.Request;

            var Uri = Request.Uri;
            if (Uri.ToString().StartsWith("/ashx/"))
            {
                args.IsProcessed = true;
                args.Response = new HttpResponse();
                var Response = args.Response;
                switch (Uri.FileName)
                {

                    case "AuctionPageList.ashx":
                        AuctionPageList.Process(Request, Response);
                        break;
                    case "CheckNickName.ashx":
                        CheckNickName.Process(Request, Response);
                        break;
                    case "ConsortiaApplyUsersList.ashx":
                        ConsortiaApplyUsersList.Process(Request, Response);
                        break;
                    case "ConsortiaDutyList.ashx":
                        ConsortiaDutyList.Process(Request, Response);
                        break;
                    case "ConsortiaEquipControlList.ashx":
                        ConsortiaEquipControlList.Process(Request, Response);
                        break;
                    case "ConsortiaEventList.ashx":
                        ConsortiaEventList.Process(Request, Response);
                        break;
                    case "ConsortiaInviteUsersList.ashx":
                        ConsortiaInviteUsersList.Process(Request, Response);
                        break;
                    case "ConsortiaList.ashx":
                        ConsortiaList.Process(Request, Response);
                        break;
                    case "ConsortiaNameCheck.ashx":
                        ConsortiaNameCheck.Process(Request, Response);
                        break;
                    case "ConsortiaUsersList.ashx":
                        ConsortiaUsersList.Process(Request, Response);
                        break;
                    case "IMListLoad.ashx":
                        IMListLoad.Process(Request, Response);
                        break;
                    case "LoadUserEquip.ashx":
                        LoadUserEquip.Process(Request, Response);
                        break;
                    case "Login.ashx":
                        Login.Process(Request, Response);
                        break;
                    case "LoginSelectList.ashx":
                        LoginSelectList.Process(Request, Response);
                        break;
                    case "MailSenderList.ashx":
                        MailSenderList.Process(Request, Response);
                        break;
                    case "MarryInfoPageList.ashx":
                        MarryInfoPageList.Process(Request, Response);
                        break;
                    case "ServerList.ashx":
                        ServerList.Process(Request, Response);
                        break;
                    case "UserMail.ashx":
                        UserMail.Process(Request, Response);
                        break;
                    case "VisualizeRegister.ashx":
                        VisualizeRegister.Process(Request, Response);
                        break;
                    default:
                        args.Response = ErrorHelper.Build(404, 0, args.ServerName);
                        break;
                }
            }
        }
    }
}
