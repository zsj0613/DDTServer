<%@ WebHandler Language="C#" Class="IMListLoad" %>

using Bussiness;

using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

	[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class IMListLoad : IHttpHandler
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
				int id = int.Parse(context.Request["id"]);
				using (PlayerBussiness db = new PlayerBussiness())
				{
					FriendInfo[] infos = db.GetFriendsAll(id);
					FriendInfo[] array = infos;
					for (int i = 0; i < array.Length; i++)
					{
						FriendInfo g = array[i];
						XElement node = new XElement("Item", new object[]
						{
							new XAttribute("ID", g.FriendID),
							new XAttribute("NickName", g.NickName),
							new XAttribute("LoginName", g.UserName),
							new XAttribute("Style", g.Style),
							new XAttribute("Sex", g.Sex == 1),
							new XAttribute("Colors", g.Colors),
							new XAttribute("Grade", g.Grade),
							new XAttribute("Hide", g.Hide),
							new XAttribute("ConsortiaName", g.ConsortiaName),
							new XAttribute("TotalCount", g.Total),
							new XAttribute("EscapeCount", g.Escape),
							new XAttribute("WinCount", g.Win),
							new XAttribute("Offer", g.Offer),
							new XAttribute("Relation", g.Relation),
							new XAttribute("Repute", g.Repute),
							new XAttribute("State", (g.State == 1) ? 1 : 0),
							new XAttribute("Nimbus", g.Nimbus),
							new XAttribute("DutyName", g.DutyName),
							new XAttribute("FightPower", g.FightPower)
						});
						result.Add(node);
					}
				}
				value = true;
				message = "Success!";
			}
			catch (Exception ex)
			{
			
			}
			result.Add(new XAttribute("value", value));
			result.Add(new XAttribute("message", message));
			context.Response.ContentType = "text/plain";
			context.Response.Write(result.ToString(false));
		}
	}
