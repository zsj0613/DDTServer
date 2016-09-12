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
    public class LoginSelectList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
            string username = HttpUtility.UrlDecode(Request.Uri.QueryString["username"]);
            using (PlayerBussiness db = new PlayerBussiness())
            {
                PlayerInfo[] infos = db.GetUserLoginList(username);
                bool isEmpty = infos.Length > 1;
                PlayerInfo[] array = infos;
                for (int i = 0; i < array.Length; i++)
                {
                    PlayerInfo info = array[i];
                    if (!string.IsNullOrEmpty(info.NickName))
                    {
                        result.Add(FlashUtils.CreateUserLoginList(info));
                    }
                }
            }
            value = true;
            message = "Success!";
            }
            catch (Exception ex)
            {
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            Response.Write(result.ToString(false));
        }
       
    }
}
