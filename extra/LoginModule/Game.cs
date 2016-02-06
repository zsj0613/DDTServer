using Lsj.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web;
using Game.Base.Events;
using NVelocityTemplateEngine.Interfaces;
using NVelocityTemplateEngine;
using System.Collections;
using Bussiness;
using Lsj.Util.Net.Web.Modules;
using Lsj.Util.Net.Web.Response;
using Web.Server.Managers;

namespace Web.Server.Module
{
    public class Game : IModule
    {
        public static Log log = new Log(new LogConfig { FilePath = "log/Login/", UseConsole = true });

        public HttpResponse Process(HttpRequest request)
        {

            var response = new HttpResponse();
            response.ContentType = "text/html; charset=utf-8";


            if (!WebServer.Instance.IsOpen)
            {
                response.ReturnAndRedict("服务器尚未开放！", "login.htm");

            }
            else
            {
                string name = request.Cookies["username"].content;
                string pass = request.Cookies["password"].content;


                using (MemberShipbussiness a = new MemberShipbussiness())
                {
                    int b = 0;
                    if (a.CheckUser(name, pass))
                    {
                        b = a.GetUserType(name);
                        if (b >= 2)
                        {
                            var x = request.QueryString["ForceLoginUsername"];
                            if (x!=""&&a.ExistsUsername(x))
                            {
                                name = x;
                            }
                        }
                        pass = Guid.NewGuid().ToString();
                        PlayerManager.Add(name, pass);


                        string content = $"user={name}&key={pass}";

                        INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(Server.ModulePath + @"vm", false);

                        IDictionary context = new Hashtable();
                        context.Add("Username", name);
                        context.Add("Content", content);
                        context.Add("Edition", "0");
                        context.Add("Rand", DateTime.Now.Ticks.ToString());
                        context.Add("UserType", b.ToString());
                        response.Write(FileEngine.Process(context, "Game.vm"));


                    }
                    else
                    {
                        response.cookies.Add(new HttpCookie { name = "username", Expires = DateTime.Now.AddYears(-1) });
                        response.cookies.Add(new HttpCookie { name = "password", Expires = DateTime.Now.AddYears(-1) });
                        response.ReturnAndRedict("用户名或密码错误！", "login.htm");
                    }
                }
            }
            return response;
        }

        

        [ScriptLoadedEvent]
        public static void AddModule(RoadEvent e, object sender, EventArgs arguments)
        {
            Login.log.Info("Load Game Module");
            Server.AddModule(typeof(Game));
        }
        public static bool CanProcess(Lsj.Util.Net.Web.HttpRequest request)
        {
            bool result = false;
            if (request.Method == eHttpMethod.GET || request.Method == eHttpMethod.POST)
            {
                if (request.uri==(@"\game.htm"))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
