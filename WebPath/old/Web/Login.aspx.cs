using System;
using Lsj.Util;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NVelocityTemplateEngine.Interfaces;
using NVelocityTemplateEngine;
using System.Collections;
using Lsj.Util.Config;


public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var method = Request.QueryString["method"].ToSafeString();
        if (method == "logout")
        {
            Response.Cookies.Add(WebHelper.CreateCookie("username", "", DateTime.Now.AddYears(-1), "hqgddt.com"));
            Response.Cookies.Add(WebHelper.CreateCookie("password", "", DateTime.Now.AddYears(-1), "hqgddt.com"));
            Response.ReturnAndRedirect("注销成功", "login");
        }
        else if (method == "pay")
        {
            ProcessPay();
        }
        else if (method == "getinvite")
        {
            ProcessInvite();
        }

        else if (Request.Cookies["username"].GetSafeValue() != "" && Request.Cookies["password"].GetSafeValue() != "")
        {
            Response.Redirect("game");
        }
        else
        {
            Response.Redirect("http://www.hqgddt.com");
        }
    }
    private void ProcessInvite()
    {
        string username = Request.QueryString["user"].ToSafeString();
        int c = 0;
        using (var a = new WebHelperClient())
        {
            c = a.GetIDByUserName(username).FirstOrDefault();
        }

        if (c == 0)
        {
            Response.ReturnAndRedirect("用户不存在，请确保已经注册角色！", "login.htm");
        }
        else
        {
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);

            IDictionary context = new Hashtable();
            context.Add("content", "http://www.hqgddt.com/?i=" + c);
            Response.Write(FileEngine.Process(context, "invite.vm"));
        }
    }

    private void ProcessPay()
    {
        string username = Request.QueryString["username"].ToSafeString();
        int c = 0;
        using (var a = new WebHelperClient())
        {
            c = a.GetIDByUserName(username).FirstOrDefault();
        }

        if (c == 0)
        {
            Response.ReturnAndRedirect("用户不存在，请确保已经注册角色！", "login");
        }
        else
        {
            Response.Redirect("charge?userid=" + c.ToString() + "&step=1");
        }
    }
}