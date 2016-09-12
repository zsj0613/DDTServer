using Lsj.Util;
using Lsj.Util.Config;
using NVelocityTemplateEngine;
using NVelocityTemplateEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lsj.Util.Text;

public partial class Game : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            using (var a = new WebHelperClient())
            {
                if (!a.IsOpen())
                {
                    Response.Write("服务器尚未开放！");
                }
                else
                {
                    string name = Request.Cookies["user"].GetSafeValue();
                    string pass = Request.Cookies["pass"].GetSafeValue();
                    int inviteid = Request.Cookies["inviteid"].GetSafeValue().ConvertToInt(0);
                    int b = 0;
                    if (a.CheckUser(name, pass, inviteid))
                    {
                        b = a.GetUserType(name);
                        if (b >= 2)
                        {
                            var x = Request.QueryString["ForceLoginUsername"].ToSafeString();
                            if (x != "" && a.ExistsUsername(x))
                            {
                                name = x;
                            }
                            var type = a.GetUserType(x);
                            if (b <= type)
                            {
                                Response.Write("对不起,你的权限不足");
                                return;
                            }
                        }
                        pass = Guid.NewGuid().ToString();
                        a.AddPlayer(name, pass);


                        string content = "user=" + name + "&key=" + pass;

                        INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["VMPath"], false);

                        IDictionary context = new Hashtable();
                        context.Add("Username", name);
                        context.Add("Content", content);
                        context.Add("Edition", "0");
                        context.Add("Rand", DateTime.Now.Ticks.ToString());
                        context.Add("UserType", b);
                        Response.Write(FileEngine.Process(context, "Game.vm"));


                    }
                    else
                    {
                        Response.Cookies.Add(WebHelper.CreateCookie("user", "", DateTime.Now.AddYears(-1), WebHelper.AppSettings["CookieDomain"]));
                        Response.Cookies.Add(WebHelper.CreateCookie("pass", "", DateTime.Now.AddYears(-1), WebHelper.AppSettings["CookieDomain"]));
                        Response.ReturnAndRedirect("用户名或密码错误！", "Default.aspx");
                    }
                }
            }
        }
        catch
        {
            Response.Write("服务器尚未开放！");
        }
    }
}