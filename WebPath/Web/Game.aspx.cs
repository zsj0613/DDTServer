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

public partial class Game : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        using (var a = new WebHelperClient())
        {
            if (!a.IsOpen())
            {
                Response.Write("服务器尚未开放！");
            }
            else
            {
                string name = Request.Cookies["username"].GetSafeValue();
                string pass = Request.Cookies["password"].GetSafeValue();
                int inviteid = Request.Cookies["inviteid"].GetSafeValue().ConvertToInt(0);
                int b = 0;
                if (a.CheckUser(name, pass,inviteid))
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


                    string content = "user="+name+"&key="+pass;

                    INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);

                    IDictionary context = new Hashtable();
                    context.Add("Username", name);
                    context.Add("Content", content);
                    context.Add("Edition", "0");
                    context.Add("Rand", DateTime.Now.Ticks.ToString());
                    context.Add("UserType", b.ToString());
                    Response.Write(FileEngine.Process(context, "Game.vm"));


                }
                else
                {
                    Response.Cookies.Add(WebHelper.CreateCookie("username", "", DateTime.Now.AddYears(-1), "hqgddt.com"));
                    Response.Cookies.Add(WebHelper.CreateCookie("password", "", DateTime.Now.AddYears(-1), "hqgddt.com"));
                    Response.ReturnAndRedirect("用户名或密码错误！", "login");
                }
            }
        }
    }
}