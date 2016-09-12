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

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int inviteid = Request.QueryString["i"].ConvertToInt(0);
        if (Request.Cookies["inviteid"].GetSafeValue() == "")
        {
            Response.Cookies.Add(WebHelper.CreateCookie("inviteid", inviteid.ToString(), DateTime.Now.AddYears(1), WebHelper.AppSettings["CookieDomain"]));
        }
        var method = Request.QueryString["method"].ToSafeString();
        if (method == "login")
        {
            Response.RedirectWithPost(WebHelper.AppSettings["InternalWeb"] + "?method=login");
        }
        else if (method == "register")
        {
            Response.RedirectWithPost(WebHelper.AppSettings["InternalWeb"] + "?method=register");
        }
        else if (method == "logout")
        {
            Response.Cookies.Add(WebHelper.CreateCookie("user", "", DateTime.Now.AddYears(-1), WebHelper.AppSettings["CookieDomain"]));
            Response.Cookies.Add(WebHelper.CreateCookie("pass", "", DateTime.Now.AddYears(-1), WebHelper.AppSettings["CookieDomain"]));
            Response.ReturnAndRedirect("注销成功", "login.aspx");
        }
		else if (Request.Cookies["user"].GetSafeValue() != "" && Request.Cookies["pass"].GetSafeValue() != "")
        {
            Response.Redirect("select.aspx");
        }
        else
        {
            Response.WriteFile("vm/login.vm");
        }
    }
}