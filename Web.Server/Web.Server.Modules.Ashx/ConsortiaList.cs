using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Interfaces;
using SqlDataProvider.Data;

namespace Web.Server.Modules.Ashx
{
    public class ConsortiaList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            int total = 0;
            int systemConsortiaCount = 0;
            try
            {
                int page = int.Parse(Request.Uri.QueryString["page"]);
                int size = int.Parse(Request.Uri.QueryString["size"]);
                int order = int.Parse(Request.Uri.QueryString["order"]);
                int consortiaID = int.Parse(Request.Uri.QueryString["consortiaID"]);
                string name = csFunction.ConvertSql(HttpUtility.UrlDecode(Request.Uri.QueryString["name"]));
                int level = int.Parse(Request.Uri.QueryString["level"]);
                int openApply = int.Parse(Request.Uri.QueryString["openApply"]);
                if (name == "" && consortiaID == -1 && level == -1)
                {
                    using (ConsortiaBussiness db = new ConsortiaBussiness())
                    {
                        List<ConsortiaInfo> systemConsortia = db.GetAllSystemConsortia().ToList<ConsortiaInfo>();
                        List<ConsortiaInfo> activeSystemConsortia = new List<ConsortiaInfo>();
                        if (page <= 3)
                        {
                            systemConsortiaCount = 3;
                        }
                        else
                        {
                            systemConsortiaCount = 2;
                        }
                        if (systemConsortia.Count <= systemConsortiaCount)
                        {
                            foreach (ConsortiaInfo info in systemConsortia)
                            {
                                result.Add(FlashUtils.CreateConsortiaInfo(info));
                            }
                            systemConsortiaCount = systemConsortia.Count;
                        }
                        else
                        {
                            ThreadSafeRandom random = new ThreadSafeRandom();
                            foreach (ConsortiaInfo info in systemConsortia)
                            {
                                if (info.IsActive)
                                {
                                    activeSystemConsortia.Add(info);
                                }
                            }
                            if (activeSystemConsortia.Count > systemConsortiaCount)
                            {
                                for (int i = 0; i < systemConsortiaCount; i++)
                                {
                                    int index = random.Next(activeSystemConsortia.Count);
                                    result.Add(FlashUtils.CreateConsortiaInfo(activeSystemConsortia.ElementAt(index)));
                                    activeSystemConsortia.RemoveAt(index);
                                }
                            }
                            else
                            {
                                int otherConsortia = systemConsortiaCount - activeSystemConsortia.Count;
                                foreach (ConsortiaInfo info in activeSystemConsortia)
                                {
                                    result.Add(FlashUtils.CreateConsortiaInfo(info));
                                }
                                while (otherConsortia > 0)
                                {
                                    int index = random.Next(systemConsortia.Count);
                                    if (!activeSystemConsortia.Contains(systemConsortia[index]))
                                    {
                                        result.Add(FlashUtils.CreateConsortiaInfo(systemConsortia.ElementAt(index)));
                                        systemConsortia.RemoveAt(index);
                                        otherConsortia--;
                                    }
                                }
                            }
                        }
                        size -= systemConsortiaCount;
                        if (size < 0)
                        {
                            size = 0;
                        }
                        consortiaID = -2;
                    }
                }
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    ConsortiaInfo[] infos = db.GetConsortiaPage(page, size, ref total, order, name, consortiaID, level, openApply);
                    ConsortiaInfo[] array = infos;
                    for (int j = 0; j < array.Length; j++)
                    {
                        ConsortiaInfo info = array[j];
                        result.Add(FlashUtils.CreateConsortiaInfo(info));
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception ex)
            {

            }
            result.Add(new XAttribute("total", total));
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            Response.Write(csFunction.Compress(result.ToString(false)));
        }
       
    }
}
