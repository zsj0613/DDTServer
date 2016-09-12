<%@ WebHandler Language="C#" Class="CheckNickName" %>

using System;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using System.Text;
using System.IO;

public class CheckNickName : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        bool value = false;
        string message = LanguageMgr.GetTranslation("错误!", new object[0]);
        XElement result = new XElement("Result");

        string nickName = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["NickName"]));
        if (!string.IsNullOrEmpty(nickName))
        {
            if (Encoding.Default.GetByteCount(nickName) <= 14)
            {
                FileSystem fileIllegal = new FileSystem();
                //context.Response.Write(nickName);
                if (!fileIllegal.checkIllegalChar(nickName) && !nickName.Contains("\r"))
                {
                    using (PlayerBussiness db = new PlayerBussiness())
                    {
                        if (!db.GetUserCheckByNickName(nickName))
                        {
                            value = true;
                            message = LanguageMgr.GetTranslation("恭喜!角色名可以使用.", new object[0]);
                        }
                        else
                        {
                            value = false;
                            message = LanguageMgr.GetTranslation("用户名已经被使用", new object[0]);
                        }
                    }
                }
                else
                {
                    value = false;
                    message = LanguageMgr.GetTranslation("用户名存在非法字符", new object[0]);
                }
            }
            else
            {
                value = false;
                message = LanguageMgr.GetTranslation("用户名太长", new object[0]);
            }
        }


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