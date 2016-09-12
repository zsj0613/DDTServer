<%@ WebHandler Language="C#" Class="ConsortiaEquipControlList" %>

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
	public class ConsortiaEquipControlList : IHttpHandler
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
				int page = 1;
				int size = 10;
				int order = 1;
				int consortiaID = int.Parse(context.Request["consortiaID"]);
				int level = int.Parse(context.Request["level"]);
				int type = int.Parse(context.Request["type"]);
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
			context.Response.ContentType = "text/plain";
			context.Response.Write(result.ToString(false));
		}
	}