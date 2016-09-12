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
using Lsj.Util.Text;
using Lsj.Util.Net.Web;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var method = Request.QueryString["method"].ToSafeString();
        if (method == "logout")
        {
            Response.Cookies.Add(WebHelper.CreateCookie("user", "", DateTime.Now.AddYears(-1), WebHelper.AppSettings["CookieDomain"]));
            Response.Cookies.Add(WebHelper.CreateCookie("pass", "", DateTime.Now.AddYears(-1), WebHelper.AppSettings["CookieDomain"]));
            Response.ReturnAndRedirect("注销成功", "Default.aspx");
        }
        else if (method == "pay")
        {
            ProcessPay();
        }
        else if (method == "getinvite")
        {
            ProcessInvite();
        }

        else if (Request.Cookies["user"].GetSafeValue() != "" && Request.Cookies["pass"].GetSafeValue() != "")
        {
            Response.Redirect("game.aspx");
        }
        else
        {
            Response.Redirect(WebHelper.AppSettings["IndexPage"]);
        }
    }
    private void ProcessInvite()
    {

        string username = Request.QueryString["user"].ToSafeString();
        int c = 0;
        using (var a = new WebHelperClient())
        {
            c = a.GetIDByUserName(username);
        }

        if (c == 0)
        {
            Response.ReturnAndRedirect("用户不存在，请确保已经注册角色！", "Default.aspx");
        }
        else
        {
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["VMPath"], false);

            IDictionary context = new Hashtable();
            context.Add("content", $"{AppConfig.AppSettings["IndexPage"]}?i=" + c);
            Response.Write(FileEngine.Process(context, "invite.vm"));
        }
    }

    private void ProcessPay()
    {
        string username = Request.QueryString["user"].ToSafeString();
        int c = 0;
        using (var a = new WebHelperClient())
        {
            c = a.GetIDByUserName(username);
        }


        if (c == 0)
        {
            Response.ReturnAndRedirect("用户不存在，请确保已经注册角色！", "Default.aspx");
        }
        else
        {
            Response.Redirect("charge.aspx?userid=" + c.ToString() + "&step=1");
        }
    }
}