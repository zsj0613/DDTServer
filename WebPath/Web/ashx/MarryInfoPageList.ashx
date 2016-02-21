<%@ WebHandler Language="C#" Class="MarryInfoPageList" %>

using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;

public class MarryInfoPageList : IHttpHandler
	{
		//
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
			int total = 0;
			XElement result = new XElement("Result");
			try
			{
				int page = int.Parse(context.Request["page"]);
				string name = null;
				if (context.Request["name"] != null)
				{
					name = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
				}
				bool sex = bool.Parse(context.Request["sex"]);
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
			context.Response.ContentType = "text/plain";
			context.Response.Write(result.ToString(false));
		}
	}