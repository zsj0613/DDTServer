using Lsj.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lsj.Util.Collections;

public partial class admin_GMAction : System.Web.UI.Page
{
    private string username;
    private string password;
    private int usertype = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        this.username = Request.Cookies["username"].GetSafeValue();
        this.password = Request.Cookies["password"].GetSafeValue();
        int inviteid = Request.Cookies["inviteid"].GetSafeValue().ConvertToInt(0);

        using (var a = new WebHelperClient())
        {
            if (a.CheckUser(username, password, inviteid))
            {
                this.usertype = a.GetUserType(username);
            }
        }

        if (this.usertype > 1)
        {
            var page = this.Request.QueryString["method"].ToSafeString();
            switch (page)
            {
                case "charge":
                    ProcessCharge();
                    break;
                case "forbid":
                    ProcessForbid();
                    break;
                case "kitoff":
                    ProcessKitoff();
                    break;
                case "xml":
                    ProcessXml();
                    break;
                case "celeb":
                    ProcessCeleb();
                    break;
                case "notice":
                    ProcessNotice();
                    break;
                case "start":
                    ProcessStart();
                    break;
                case "stop":
                    ProcessStop();
                    break;
                case "reconnect":
                    ProcessReconnect();
                    break;
                case "loginkey":
                    ProcessKey();
                    break;

                default:
                    break;
            }
        }
        else
        {
            Response.ReturnAndRedirect("你无权访问GM管理平台！", "../login");
        }
    }

    private void ProcessKey()
    {
        using (var a = new WebHelperClient())
        {
            var name = Request.QueryString["UserName"].ToSafeString();
            if (name != "" && a.ExistsUsername(name))
            {
                var type = a.GetUserType(name);
                if (this.usertype <= type)
                {
                    Response.Write("对不起,你的权限不足");
                    return;
                }
                var pass = Guid.NewGuid().ToString();
                a.AddPlayer(name, pass);
                string content = "user="+name+"&key="+pass;
                Response.Write(content);
            }
            else
            {
                Response.Write("错误");
            }
        }
    }

    private void ProcessReconnect()
    {
        if (this.usertype <= 2)
        {
            Response.Write("对不起,你的权限不足");
            return;
        }
        using (var a = new WebHelperClient())
        {
            if (a.Reconnect())
            {
                Response.Write("成功");
            }
            Response.Write("错误");
        }
    }

    private void ProcessStop()
    {
        if (this.usertype <= 2)
        {
            Response.Write("对不起,你的权限不足");
            return;
        }
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("type", Request.QueryString["type"].ToSafeString());
            Response.Write(a.GMAction("stop", dic.ToDictionary()));
        }
    }

    private void ProcessStart()
    {
        if (this.usertype <= 2)
        {
            Response.Write("对不起,你的权限不足");
            return;
        }
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("type", Request.QueryString["type"].ToSafeString());
            Response.Write(a.GMAction("start",dic.ToDictionary()));
        }
    }

    private void ProcessNotice()
    {
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("str", Request.Form["Tx_url"].ToSafeString());
            Response.Write(a.GMAction("notice", dic.ToDictionary()));
        }
    }

    private void ProcessCeleb()
    {
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            Response.Write(a.GMAction("celeb", dic.ToDictionary()));
        }
    }

    private void ProcessXml()
    {
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            Response.Write(a.GMAction("xml", dic.ToDictionary()));
        }
    }

    private void ProcessKitoff()
    {
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("UserID", Request.QueryString["UserID"].ToSafeString());
            Response.Write(a.GMAction("kitoff", dic.ToDictionary()));
        }
    }

    private void ProcessForbid()
    {
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("UserID", Request.QueryString["UserID"].ToSafeString());
            dic.Add("reason", Request.QueryString["reason"].ToSafeString());
            dic.Add("day", Request.QueryString["day"].ToSafeString());
            Response.Write(a.GMAction("forbid", dic.ToDictionary()));
        }
    }

    private void ProcessCharge()
    {
        if (this.usertype <= 2)
        {
            Response.Write("对不起,你的权限不足");
            return;
        }
        using (var a = new WebHelperClient())
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("UserID", Request.QueryString["UserID"].ToSafeString());
            dic.Add("money", Request.QueryString["money"].ToSafeString());
            Response.Write(a.GMAction("charge", dic.ToDictionary()));
        }
    }
}