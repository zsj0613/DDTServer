<%@ WebHandler Language="C#" Class="UserMail" %>

using System;
using System.Web;

using Bussiness;
using Bussiness.CenterService;

using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

	[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class UserMail : IHttpHandler
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
				int id = (context.Request.QueryString["ID"] == null) ? 0 : int.Parse(context.Request.QueryString["ID"]);
				string key = context.Request.QueryString["key"];
				if (key != null && !(key.Trim() == string.Empty))
				{
					//using (CenterServiceClient client = new CenterServiceClient())
					//{
					//	if (client.CheckUserValidate(id, key))
					//	{
							if (id != 0)
							{
								using (PlayerBussiness db = new PlayerBussiness())
								{
									MailInfo[] infos = db.GetMailByUserID(id);
									MailInfo[] array = infos;
									for (int i = 0; i < array.Length; i++)
									{
										MailInfo info = array[i];
										XElement node = new XElement("Item", new object[]
										{
											new XAttribute("ID", info.ID),
											new XAttribute("Title", info.Title),
											new XAttribute("Content", info.Content),
											new XAttribute("Sender", info.Sender),
											new XAttribute("SendTime", info.SendTime.ToString("yyyy-MM-dd HH:mm:ss")),
											new XAttribute("Gold", info.Gold),
											new XAttribute("Money", info.Money),
											new XAttribute("Annex1ID", (info.Annex1 == null) ? "" : info.Annex1),
											new XAttribute("Annex2ID", (info.Annex2 == null) ? "" : info.Annex2),
											new XAttribute("Annex3ID", (info.Annex3 == null) ? "" : info.Annex3),
											new XAttribute("Annex4ID", (info.Annex4 == null) ? "" : info.Annex4),
											new XAttribute("Annex5ID", (info.Annex5 == null) ? "" : info.Annex5),
											new XAttribute("Type", info.Type),
											new XAttribute("ValidDate", info.ValidDate),
											new XAttribute("IsRead", info.IsRead),
											new XAttribute("GiftToken", info.GiftToken)
										});
										UserMail.AddAnnex(node, info.Annex1);
										UserMail.AddAnnex(node, info.Annex2);
										UserMail.AddAnnex(node, info.Annex3);
										UserMail.AddAnnex(node, info.Annex4);
										UserMail.AddAnnex(node, info.Annex5);
										result.Add(node);
									}
								}
								value = true;
								message = "Success!";
							}
					//	}
					//}
				}
			}
			catch (Exception ex)
			{
				//UserMail.log.Error("LoadUserMail", ex);
			}
			finally
			{
				result.Add(new XAttribute("value", value));
				result.Add(new XAttribute("message", message));
				context.Response.ContentType = "text/plain";
				context.Response.BinaryWrite(csFunction.Compress(result.ToString(false)));
			}
		}
		public static void AddAnnex(XElement node, string value)
		{
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				if (!string.IsNullOrEmpty(value))
				{
					ItemInfo pr = pb.GetUserItemSingle(int.Parse(value));
					if (pr != null)
					{
						node.Add(FlashUtils.CreateGoodsInfo(pr));
					}
				}
			}
		}
	}

