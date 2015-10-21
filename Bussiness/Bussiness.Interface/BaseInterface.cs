using Bussiness.CenterService;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Security;

using Lsj.Util.Config;
using Lsj.Util;

namespace Bussiness.Interface
{
	public abstract class BaseInterface
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public virtual int ActiveGold
		{
			get
			{
				return AppConfig.AppSettings["DefaultGold"].ConvertToInt();
			}
		}
		public virtual int ActiveMoney
		{
			get
			{
				return AppConfig.AppSettings["DefaultMoney"].ConvertToInt();
			}
		}
		public virtual int ActiveGiftToken
		{
			get
			{
				return AppConfig.AppSettings["DefaultGiftToken"].ConvertToInt();
			}
		}
		public static DateTime ConvertIntDateTime(double d)
		{
			DateTime time = DateTime.MinValue;
			return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(d);
		}
		public static int ConvertDateTimeInt(DateTime time)
		{
			DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			double intResult = (time - startTime).TotalSeconds;
			return (int)intResult;
		}
		public static string md5(string str)
		{
			return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower();
		}
		public static BaseInterface CreateInterface()
		{
            BaseInterface result = new LsjInterface();
			return result;
		}
		public virtual PlayerInfo CreateLogin(string name, string password, ref string message, ref int isFirst, string IP, ref bool isError, bool firstValidate, ref bool isActive, string site, string nickname)
		{
			PlayerInfo result;
			try
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					bool isExist = true;
					DateTime forbidDate = DateTime.Now;
					PlayerInfo info = db.LoginGame(name, ref isFirst, ref isExist, ref isError, firstValidate, ref forbidDate, nickname);
                    string debug = isError.ToString();
                    message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail8" + debug, new object[0]);
                    if (info == null)
                    {
                        message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail99", new object[0]);
                        if (!db.ActivePlayer(ref info, name, password, true, this.ActiveGold, this.ActiveMoney, this.ActiveGiftToken, IP, site))
                        {
                              info = null;
                              message = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Fail", new object[0]);
                        }

                     else
                        {
                            message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail10", new object[0]);
                            isActive = true;
                            using (CenterServiceClient client = new CenterServiceClient())
                            {   
                                client.ActivePlayer(true);
                            }
                        }
					}
					else
					{
						if (!isExist)
						{
							message = String.Format("您的账号已被封停,将于{0}年{1}月{2}日{3}时{4}分解封.\n如有疑问,请联系官方客服.", new object[]
							{
								forbidDate.Year,
								forbidDate.Month,
								forbidDate.Day,
								forbidDate.Hour,
								forbidDate.Minute
							});
							result = null;
							return result;
						}
						using (CenterServiceClient client = new CenterServiceClient())
						{
							if (!client.CreatePlayer(info.ID, name, password, isFirst == 0))
							{
								BaseInterface.log.Error("发送中心服务器失败");
							}
						}
					}
					result = info;
					return result;
				}
			}
			catch (Exception ex)
			{
				BaseInterface.log.Error("LoginAndUpdate", ex);
			}
			result = null;
			return result;
		}
		public virtual PlayerInfo LoginGame(string name, string pass, ref bool isFirst)
		{
			PlayerInfo result;
			try
			{
				using (CenterServiceClient client = new CenterServiceClient())
				{
					int userID = 0;
					if (client.ValidateLoginAndGetID(name, pass, ref userID, ref isFirst))
					{
						result = new PlayerInfo
						{
							ID = userID,
							UserName = name
						};
						return result;
					}
				}
			}
			catch (Exception ex)
			{
				BaseInterface.log.Error("LoginGame", ex);
			}
			result = null;
			return result;
		}
		public virtual bool GetUserSex(string name)
		{
			return true;
		}
	}
}
