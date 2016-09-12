<%@ WebHandler Language="C#" Class="LoginSelectList" %>

using System;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using SqlDataProvider.Data;

public class LoginSelectList : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        bool value = false;
        string message = "Fail!";
        XElement result = new XElement("Result");
        //try
        //{
            string username = HttpUtility.UrlDecode(context.Request["username"]);
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
        //}
        //catch (Exception ex)
        //{
        //}
        result.Add(new XAttribute("value", value));
        result.Add(new XAttribute("message", message));
        context.Response.ContentType = "text/plain";
        context.Response.Write(result.ToString(false));
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}