<%@ WebHandler Language="C#" Class="VisualizeRegister" %>

using Bussiness;

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class VisualizeRegister : IHttpHandler
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
			string message = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Fail1", new object[0]);
			XElement result = new XElement("Result");
			//try
			{
				NameValueCollection para = context.Request.Params;
				string name = para["Name"];
				string pass = para["Pass"];
				string nickName = para["NickName"].Trim().Replace(",", "");
				string armColor = para["Arm"];
				string hairColor = para["Hair"];
				string faceColor = para["Face"];
				string ClothColor = para["Cloth"];
				string armID = para["ArmID"];
				string hairID = para["HairID"];
				string faceID = para["FaceID"];
				string ClothID = para["ClothID"];
				int sex = -1;

					sex = (bool.Parse(para["Sex"]) ? 1 : 0);
				
				using (ServiceBussiness db = new ServiceBussiness())
				{
					string equip = db.GetGameEquip();
					string curr_Equip = (sex == 1) ? equip.Split(new char[]
					{
						'|'
					})[0] : equip.Split(new char[]
					{
						'|'
					})[1];
					hairID = curr_Equip.Split(new char[]
					{
						','
					})[0];
					faceID = curr_Equip.Split(new char[]
					{
						','
					})[1];
					ClothID = curr_Equip.Split(new char[]
					{
						','
					})[2];
					armID = curr_Equip.Split(new char[]
					{
						','
					})[3];
				}
				if (Encoding.Default.GetByteCount(nickName) <= 14)
				{
					FileSystem fileIllegal = new FileSystem();
					if (fileIllegal.checkIllegalChar(nickName))
					{
						value = false;
						message = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Illegalcharacters", new object[0]);
					}
					else
					{
						if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(pass) && !string.IsNullOrEmpty(nickName))
						{
							using (PlayerBussiness db2 = new PlayerBussiness())
							{
								string style = string.Concat(new string[]
								{
									armID,
									",",
									hairID,
									",",
									faceID,
									",",
									ClothID
								});
								if (db2.RegisterPlayer(name, pass, nickName, style, style, armColor, hairColor, faceColor, ClothColor, sex, ref message, int.Parse(ConfigurationSettings.AppSettings["ValidDate"])))
								{
									value = true;
									message = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Success", new object[0]);
								}
							}
						}
					}
				}
				else
				{
					message = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Long", new object[0]);
				}
			}
			//catch (Exception ex)
			//{
				//VisualizeRegister.log.Error("VisualizeRegister", ex);
			//}
			result.Add(new XAttribute("value", value));
			result.Add(new XAttribute("message", message));
			context.Response.ContentType = "text/plain";
			context.Response.Write(result.ToString(false));
		}
	}
