using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Security;
using Lsj.Util;
using Lsj.Util.Config;
using Lsj.Util.Logs;
using Lsj.Util.Text;
using SqlDataProvider.Data;

namespace Bussiness.Interface
{
    public class LSJInterface
    {
        private static LogProvider log => LogProvider.Default;
        public int ActiveGold
        {
            get
            {
                return AppConfig.AppSettings["DefaultGold"].ConvertToInt();
            }
        }
        public int ActiveMoney
        {
            get
            {
                return AppConfig.AppSettings["DefaultMoney"].ConvertToInt();
            }
        }
        public int ActiveGiftToken
        {
            get
            {
                return AppConfig.AppSettings["DefaultGiftToken"].ConvertToInt();
            }
        }
        public static LSJInterface CreateInterface()
        {
            LSJInterface result = new LSJInterface();
            return result;
        }
        public PlayerInfo CreateLogin(string name, string password, ref string message, ref int isFirst, string IP, ref bool isError, bool firstValidate, ref bool isActive, string site, string nickname, out bool needcreate)
        {
            PlayerInfo result;
            needcreate = false;
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
                        }
                    }
                    else
                    {
                        if (!isExist)
                        {
                            message = String.Format("您的账号已被封停,将于{0}年{1}月{2}日{3}时{4}分解封.\n", new object[]
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
                        needcreate = true;
                    }
                    result = info;
                    return result;
                }
            }
            catch (Exception ex)
            {
                LSJInterface.log.Error("LoginAndUpdate", ex);
            }
            result = null;
            return result;
        }
    }
}
