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
using Bussiness;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        


        int inviteid = Request.QueryString["i"].ConvertToInt(0);
        if (Request.Cookies["inviteid"].GetSafeValue() == "")
        {
            Response.Cookies.Add(WebHelper.CreateCookie("inviteid", inviteid.ToString(), DateTime.Now.AddYears(1), "hqgddt.com"));
        }
        var method = Request.QueryString["method"].ToSafeString();
        if (method == "login")
        {
            ProcessLogin();
        }
        else if (method == "register")
        {
            ProcessRegister();
        }
        else if (method == "logout")
        {
            Response.Cookies.Add(WebHelper.CreateCookie("username", "", DateTime.Now.AddYears(-1), "hqgddt.com"));
            Response.Cookies.Add(WebHelper.CreateCookie("password", "", DateTime.Now.AddYears(-1), "hqgddt.com"));
            Response.ReturnAndRedirect("注销成功", "login");
        }
		else if (Request.Cookies["username"].GetSafeValue() != "" && Request.Cookies["password"].GetSafeValue() != "")
        {
            Response.Redirect("select");
        }
        else
        {
            Response.WriteFile("vm/login.vm");
        }
    }


    private void ProcessLogin()
    {
        string username = Request.Form["user"].ToSafeString();
        string password = Request.Form["password1"].ToSafeString();
        Response.Cookies.Add(WebHelper.CreateCookie("username", username, DateTime.Now.AddYears(1), "hqgddt.com"));
        Response.Cookies.Add(WebHelper.CreateCookie("password", password, DateTime.Now.AddYears(1), "hqgddt.com"));
        Response.Redirect("select");
    }
    private void ProcessRegister()
    {
        string username = Request.Form["user"].ToSafeString();
        string password = Request.Form["password1"].ToSafeString();
        int inviteid = Request.Cookies["inviteid"].GetSafeValue().ConvertToInt(0);
        using (var a = new MemberShipbussiness())
        {
            if (!a.ExistsUsername(username)&&a.CreateUsername(username, password, inviteid))
            {
                Response.Cookies.Add(WebHelper.CreateCookie("username", username, DateTime.Now.AddYears(1), "hqgddt.com"));
                Response.Cookies.Add(WebHelper.CreateCookie("password", password, DateTime.Now.AddYears(1), "hqgddt.com"));
                Response.ReturnAndRedirect("注册成功", "select");
            }
            else
            {
                Response.ReturnAndRedirect("该用户名已被注册", "login");
            }
        }

    }
}