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
    public class MailSenderList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                int id = int.Parse(Request.Uri.QueryString["ID"]);
                if (id != 0)
                {
                    using (PlayerBussiness db = new PlayerBussiness())
                    {
                        MailInfo[] sInfos = db.GetMailBySenderID(id);
                        MailInfo[] array = sInfos;
                        for (int i = 0; i < array.Length; i++)
                        {
                            MailInfo info = array[i];
                            result.Add(FlashUtils.CreateMailInfo(info, "Item"));
                        }
                    }
                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception ex)
            {
                //MailSenderList.log.Error("MailSenderList", ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            Response.Write(csFunction.Compress(result.ToString(false)));
        }
       
    }
}
