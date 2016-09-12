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
    public class ConsortiaApplyUsersList
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
                int consortiaID = int.Parse(Request.Uri.QueryString["consortiaID"]);
                int applyID = int.Parse(Request.Uri.QueryString["applyID"]);
                int userID = int.Parse(Request.Uri.QueryString["userID"]);
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    ConsortiaApplyUserInfo[] infos = db.GetConsortiaApplyUserPage(page, size, ref total, order, consortiaID, applyID, userID);
                    ConsortiaApplyUserInfo[] array = infos;
                    for (int i = 0; i < array.Length; i++)
                    {
                        ConsortiaApplyUserInfo info = array[i];
                        result.Add(FlashUtils.CreateConsortiaApplyUserInfo(info));
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
