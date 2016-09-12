using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Interfaces;
using SqlDataProvider.Data;

namespace Web.Server.Modules.Ashx
{
    public class ConsortiaInviteUsersList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            int total = 0;
            try
            {
                int page = int.Parse(Request.Uri.QueryString["page"]);
                int size = int.Parse(Request.Uri.QueryString["size"]);
                int order = int.Parse(Request.Uri.QueryString["order"]);
                int userID = int.Parse(Request.Uri.QueryString["userID"]);
                int inviteID = int.Parse(Request.Uri.QueryString["inviteID"]);
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    ConsortiaInviteUserInfo[] infos = db.GetConsortiaInviteUserPage(page, size, ref total, order, userID, inviteID);
                    ConsortiaInviteUserInfo[] array = infos;
                    for (int i = 0; i < array.Length; i++)
                    {
                        ConsortiaInviteUserInfo info = array[i];
                        result.Add(FlashUtils.CreateConsortiaInviteUserInfo(info));
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception ex)
            {

            }
            result.Add(new XAttribute("total", total));
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            Response.Write(result.ToString(false));
        }
       
    }
}
