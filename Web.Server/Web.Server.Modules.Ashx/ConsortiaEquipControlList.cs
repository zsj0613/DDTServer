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
    public class ConsortiaEquipControlList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            int total = 0;
            try
            {
                int page = 1;
                int size = 10;
                int order = 1;
                int consortiaID = int.Parse(Request.Uri.QueryString["consortiaID"]);
                int level = int.Parse(Request.Uri.QueryString["level"]);
                int type = int.Parse(Request.Uri.QueryString["type"]);
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    ConsortiaEquipControlInfo[] infos = db.GetConsortiaEquipControlPage(page, size, ref total, order, consortiaID, level, type);
                    ConsortiaEquipControlInfo[] array = infos;
                    for (int i = 0; i < array.Length; i++)
                    {
                        ConsortiaEquipControlInfo info = array[i];
                        result.Add(FlashUtils.CreateConsortiaEquipControlInfo(info));
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
            Response.Write(result.ToString(false));
        }
       
    }
}
