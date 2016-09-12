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
    public class MarryInfoPageList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            int total = 0;
            XElement result = new XElement("Result");
            try
            {
                int page = int.Parse(Request.Uri.QueryString["page"]);
                string name = null;
                if (Request.Uri.QueryString["name"] != null)
                {
                    name = csFunction.ConvertSql(HttpUtility.UrlDecode(Request.Uri.QueryString["name"]));
                }
                bool sex = bool.Parse(Request.Uri.QueryString["sex"]);
                int size = 12;
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    MarryInfo[] infos = db.GetMarryInfoPages(page, name, sex, size, ref total);
                    MarryInfo[] array = infos;
                    for (int i = 0; i < array.Length; i++)
                    {
                        MarryInfo info = array[i];
                        XElement temp = FlashUtils.CreateMarryInfo(info);
                        result.Add(temp);
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception ex)
            {
                //MarryInfoPageList.log.Error("MarryInfoPageList", ex);
            }
            result.Add(new XAttribute("total", total));
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            Response.Write(result.ToString(false));
        }
       
    }
}
