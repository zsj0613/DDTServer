<%@ WebHandler Language="C#" Class="ConsortiaNameCheck" %>

using System;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using System.Text;
using System.Linq;
using System.IO;

public class ConsortiaNameCheck : IHttpHandler {

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
                    using (var db = new ConsortiaBussiness())
                    {
                        var x = db.GetConsortiaAll().AsEnumerable().Where((a) => (a.ChairmanName == nickName)).Count() > 0;

                        if (!x)
                        {
                            value = true;
                            message = LanguageMgr.GetTranslation("恭喜!公会名称可以使用.", new object[0]);
                        }
                        else
                        {
                            value = false;
                            message = LanguageMgr.GetTranslation("公会名已经被使用", new object[0]);
                        }
                    }
                }
                else
                {
                    value = false;
                    message = LanguageMgr.GetTranslation("公会名存在非法字符", new object[0]);
                }
            }
            else
            {
                value = false;
                message = LanguageMgr.GetTranslation("公会名太长", new object[0]);
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