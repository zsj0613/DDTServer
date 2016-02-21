<%@ WebHandler Language="C#" Class="ConsortiaEventList" %>

using Bussiness;

using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

	[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ConsortiaEventList : IHttpHandler
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
			int total = 0;
			try
			{
				int page = int.Parse(context.Request["page"]);
				int size = int.Parse(context.Request["size"]);
				int order = int.Parse(context.Request["order"]);
				int consortiaID = int.Parse(context.Request["consortiaID"]);
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					ConsortiaEventInfo[] infos = db.GetConsortiaEventPage(page, size, ref total, order, consortiaID);
					ConsortiaEventInfo[] array = infos;
					for (int i = 0; i < array.Length; i++)
					{
						ConsortiaEventInfo info = array[i];
						result.Add(FlashUtils.CreateConsortiaEventInfo(info));
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
			context.Response.ContentType = "text/plain";
			context.Response.Write(result.ToString(false));
		}
	}

