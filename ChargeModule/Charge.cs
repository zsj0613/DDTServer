using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web;
using Lsj.Util;
using Game.Base;
using System.Reflection;
using Bussiness;
using Game.Base.Events;

namespace Web.Server.Module
{
    public class Charge : IModule
    {
        public HttpResponse Process(HttpRequest request)
        {
            var response = new HttpResponse();
            response.contenttype = "text/html";

            //  "/charge.do?"
            string uri = request.uri.Substring(11);
            var para = new Dictionary<string, string>();
            try
            {
                var a = uri.Split('&');
                foreach (var b in a)
                {
                    var c = b.Split('=');
                    para.Add(c[0], c[1]);
                }
            }
            catch
            {
                response.WriteError(404);
                return response;
            }

            if (!para.ContainsKey("step"))
            {
                response.WriteError(404);
            }
            else if (para["step"].ConvertToInt(0) == 1)
            {
                response = ProcessStep1(para);
            }
            else if (para["step"].ConvertToInt(0) == 2)
            {
                response = ProcessStep2(para);
            }
            else
            {
                response.WriteError(404);
            }
            return response;
        }



        private HttpResponse ProcessStep1(Dictionary<string, string> para)
        {
            var response = new HttpResponse();
            response.contenttype = "text/html";
            string userid = "";
            var content = ResourceUtil.GetResourceStream("PayIndex.html", Assembly.GetAssembly(typeof(Charge))).ReadFromStream(Encoding.UTF8).ToStringBuilder();
            if (!para.ContainsKey("userid") || para["userid"].ConvertToInt(0) == 0)
            {
                response.WriteError(404);
            }
            else
            {
                userid = para["userid"];
                content.Replace("$userid", userid);
                response.Write(content);
            }
            return response;
        }

        private HttpResponse ProcessStep2(Dictionary<string, string> para)
        {
            var response = new HttpResponse();
            response.contenttype = "text/html";
            string userid = "";
            string paytype = "";

            if (!para.ContainsKey("userid") || para["userid"].ConvertToInt(0) == 0)
            {
                response.WriteError(404);
                return response;
            }
            else
            {
                userid = para["userid"];
            }

            if (!para.ContainsKey("paytype") || para["paytype"].ConvertToInt(0) == 0)
            {
                response.WriteError(404);
                return response;
            }
            else
            {
                paytype = para["paytype"];
            }

            if (paytype.ConvertToInt(0) >= 1 && paytype.ConvertToInt(0) <= 24)
            {
                string username = "";
                using (PlayerBussiness a = new PlayerBussiness())
                {
                    var b = a.GetUserSingleByUserID(userid.ConvertToInt(0));
                    {
                        if (b != null)
                        {
                            username = b.UserName;
                        }
                        else
                        {
                            response.WriteError(404);
                            return response;
                        }
                    }
                }
                var content = ResourceUtil.GetResourceStream($"Pay{paytype}.html", Assembly.GetAssembly(typeof(Charge))).ReadFromStream(Encoding.UTF8).ToStringBuilder();
                content.Replace("$userid", userid);
                content.Replace("$username", username);
                content.Replace("/queryorder.asp", "http://p5m0.357p.com/queryorder.asp");
                response.Write(content);

            }
            else
            {
                response.WriteError(404);
            }
            return response;
        }
        [ScriptLoadedEventAttribute]
        public static void AddModule(RoadEvent e, object sender, EventArgs arguments)
        {
            Server.AddModule("/charge.do?", "Web.Server.Module.Charge");
        }
    }
}

