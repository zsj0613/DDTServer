using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web;
using Lsj.Util;
using Lsj.Util.Log;
using Game.Base;
using System.Reflection;
using Bussiness;
using Game.Base.Events;
using NVelocityTemplateEngine.Interfaces;
using NVelocityTemplateEngine;
using System.Collections;
using Lsj.Util.Net.Web.Modules;

namespace Web.Server.Module
{
    public class Admin :IModule
    {
        public static Log log = new Log(new LogConfig { FilePath = "log/Admin/", UseConsole = true });
        string username;
        string password;
        public HttpResponse Process(HttpRequest request)
        {
            var response = new HttpResponse();
            response.ContentType = "text/html";
            username = request.Cookies["username"].content;
            password = request.Cookies["password"].content;
            if (GetUserType(username, password) <= 1)
            {
                response.ReturnAndRedict("你无权访问GM管理平台！", "../login.do");
            }
            else
            {
                switch (request.QueryString["page"])
                {
                    case "":
                        ProcessIndex(ref response);
                        break;
                    case "left":
                        ProcessLeft(ref response);
                        break;
                    case "top":
                        ProcessTop(ref response);
                        break;
                    case "version":
                        ProcessVersion(ref response);
                        break;
                    case "userlist":
                        ProcessUserList(ref response);
                        break;
                    default:
                        response.WriteError(404);
                        break;
                }
            }
            return response;
        }

        private void ProcessUserList(ref HttpResponse response)
        {
            
        }

        private void ProcessIndex(ref HttpResponse response)
        {
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(Server.ModulePath + @"vm", false);
            IDictionary context = new Hashtable();
            response.Write(FileEngine.Process(context, "Admin.vm"));
        }
        private void ProcessLeft(ref HttpResponse response)
        {
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(Server.ModulePath + @"vm", false);
            IDictionary context = new Hashtable();
            response.Write(FileEngine.Process(context, "Admin_Left.vm"));
        }
        private void ProcessVersion(ref HttpResponse response)
        {
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(Server.ModulePath + @"vm", false);
            IDictionary context = new Hashtable();
            response.Write(FileEngine.Process(context, "Admin_Version.vm"));
        }
        private void ProcessTop(ref HttpResponse response)
        {
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(Server.ModulePath + @"vm", false);
            IDictionary context = new Hashtable();
            context.Add("UserName", username);
            response.Write(FileEngine.Process(context, "Admin_Top.vm"));
        }

        public int GetUserType(string name,string pass)
        {
            int b = 0;
            using (MemberShipbussiness a = new MemberShipbussiness())
            {
                if (a.CheckUser(name, pass))
                    b = a.GetUserType(name);
            }
            return b;
        }

        [ScriptLoadedEvent]
        public static void AddModule(RoadEvent e, object sender, EventArgs arguments)
        {
            Admin.log.Info("Load Admin Module");
            Server.AddModule(typeof(Admin));            
        }
        public static bool CanProcess(HttpRequest request)
        {
            bool result = false;
            if (request.Method == eHttpMethod.GET|| request.Method == eHttpMethod.POST)
            {
                if (request.uri==(@"\admin\"))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}

