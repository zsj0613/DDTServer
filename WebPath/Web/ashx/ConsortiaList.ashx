<%@ WebHandler Language="C#" Class="ConsortiaList" %>

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
	public class ConsortiaList : IHttpHandler
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
			int systemConsortiaCount = 0;
			try
			{
				int page = int.Parse(context.Request["page"]);
				int size = int.Parse(context.Request["size"]);
				int order = int.Parse(context.Request["order"]);
				int consortiaID = int.Parse(context.Request["consortiaID"]);
				string name = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
				int level = int.Parse(context.Request["level"]);
				int openApply = int.Parse(context.Request["openApply"]);
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
			context.Response.ContentType = "text/plain";
			context.Response.BinaryWrite(csFunction.Compress(result.ToString(false)));
		}
	}