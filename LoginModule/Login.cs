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
using Lsj.Util;

namespace Web.Server.Module
{
    public class Login : IModule
    {
        public static Log log = new Log(new LogConfig { FilePath = "log/Login/", UseConsole = true });

        public void Process(ref HttpClient client)
        {

            var request = client.request;
            var response = new HttpResponse();
            if (request.QueryString["method"] == "login")
            {
                ProcessLogin(ref response, ref request);
            }
            else if (request.QueryString["method"] == "register")
            {
                ProcessRegister(ref response, ref request);
            }
            else
            {
                ProcessLoginPage(ref response);
            }
            client.response = response;

        }

        private void ProcessLoginPage(ref HttpResponse response)
        {
            response.Write302("http://www.hqgddt.com/login.htm");
        }
        private void ProcessLogin(ref HttpResponse response, ref HttpRequest request)
        {
            Login.log.Info(request.postdata.ToSafeString());
            string username = request.Form["username"];
            string password = request.Form["password1"];
            Login.log.Info(username.ToSafeString());
            Login.log.Info(password.ToSafeString());
            response.cookies.Add(new HttpCookie { name = "username", content = username.ToSafeString(), Expires = DateTime.Now.AddYears(1) });
            response.cookies.Add(new HttpCookie { name = "password", content = password.ToSafeString(), Expires = DateTime.Now.AddYears(1) });
            response.Write302("http://www.hqgddt.com/game.htm");
        }
        private void ProcessRegister(ref HttpResponse response, ref HttpRequest request)
        {
            response.Write302("http://www.hqgddt.com/register.do");
        }


        [ScriptLoadedEvent]
        public static void AddModule(RoadEvent e, object sender, EventArgs arguments)
        {
            Login.log.Info("Load Login Module");
            Server.AddModule(@"\", "Web.Server.Module.Login");
            Server.AddModule(@"\login.do", "Web.Server.Module.Login");
        }
    }
}
