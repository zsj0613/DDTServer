<%@ WebHandler Language="C#" Class="MailSenderList" %>

using Bussiness;

using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

	[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class MailSenderList : IHttpHandler
	{
		
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
		public void ProcessRequest(HttpContext context)
		{
			bool value = false;
			string message = "Fail!";
			XElement result = new XElement("Result");
			try
			{
				int id = int.Parse(context.Request.QueryString["ID"]);
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
			context.Response.ContentType = "text/plain";
			context.Response.BinaryWrite(csFunction.Compress(result.ToString(false)));
		}
	}
