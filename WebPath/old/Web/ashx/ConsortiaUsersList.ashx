<%@ WebHandler Language="C#" Class="ConsortiaUsersList" %>

using Bussiness;

using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

	[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ConsortiaUsersList : IHttpHandler
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
				int userID = int.Parse(context.Request["userID"]);
				int state = int.Parse(context.Request["state"]);
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					ConsortiaUserInfo[] infos = db.GetConsortiaUsersPage(page, size, ref total, order, consortiaID, userID, state);
					ConsortiaUserInfo[] array = infos;
					for (int i = 0; i < array.Length; i++)
					{
						ConsortiaUserInfo info = array[i];
						result.Add(FlashUtils.CreateConsortiaUserInfo(info));
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
			result.Add(new XAttribute("currentDate", DateTime.Now.ToString()));
			context.Response.ContentType = "text/plain";
			context.Response.Write(result.ToString(false));
		}
	}

