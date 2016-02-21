<%@ WebHandler Language="C#" Class="ConsortiaDutyList" %>

using Bussiness;

using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;


	[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ConsortiaDutyList : IHttpHandler
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
				int dutyID = int.Parse(context.Request["dutyID"]);
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					ConsortiaDutyInfo[] infos = db.GetConsortiaDutyPage(page, size, ref total, order, consortiaID, dutyID);
					ConsortiaDutyInfo[] array = infos;
					for (int i = 0; i < array.Length; i++)
					{
						ConsortiaDutyInfo info = array[i];
						result.Add(FlashUtils.CreateConsortiaDutyInfo(info));
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