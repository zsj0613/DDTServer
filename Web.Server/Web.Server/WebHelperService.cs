using Bussiness;
using Bussiness.Interface;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Xml.Linq;
using Web.Server.Managers;
using Lsj.Util.Collections;
using Lsj.Util.Config;
using Lsj.Util;
using Bussiness.CenterService;
using System.Web;

namespace Web.Server
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“WebHelperService”。
    public class WebHelperService : IWebHelperService
    {

        private static ServiceHost host;
        public static bool Start()
        {
            bool result;
            try
            {
                var myBinding = new WSHttpBinding();
                Uri baseAddress = new Uri("http://127.0.0.1:"+AppConfig.AppSettings["WCFPort"]+"/");
                WebHelperService.host = new ServiceHost(typeof(WebHelperService), baseAddress);
                WebHelperService.host.AddServiceEndpoint(typeof(IWebHelperService), myBinding, "WebHelperService");
               // ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
               // behavior.HttpGetEnabled = true;
               // behavior.HttpGetUrl = new Uri("http://127.0.0.1:46000/");
               // WebHelperService.host.Description.Behaviors.Add(behavior);
                WebHelperService.host.Open();

                WebHelperService.log.Info("WCF Service started!");
                result = true;
            }
            catch (Exception ex)
            {
                WebHelperService.log.ErrorFormat("Start failed:{0}", ex);
                result = false;
            }
            return result;
        }
        public static void Stop()
        {
            try
            {
                if (WebHelperService.host != null)
                {
                    WebHelperService.host.Close();
                    WebHelperService.host = null;
                }
            }
            catch
            {
            }
        }
        static LogProvider log => LogProvider.Default;
        public bool Register(string username, string password, int inviteid)
        {
            log.Debug("register" + username + password + inviteid);
            using (MemberShipbussiness a = new MemberShipbussiness())
            {
                if (a.ExistsUsername(username))
                {
                }
                else
                {
                    if (a.CreateUsername(username, password, inviteid))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public int[] GetIDByUserName(string username)
        {
            using (PlayerBussiness a = new PlayerBussiness())
            {
                PlayerInfo[] b = a.GetUserSingleByUserName(username);
                if (b != null)
                {
                    return b.Where((x) => (x.ID != 0)).Select((x) => (x.ID)).ToArray();
                }
                return null;
            }
        }
        public bool IsOpen()
        {
            return WebServer.Instance.IsOpen;
        }

        public bool CheckUser(string username, string password ,int inviteid)
        {
            using (var a = new MemberShipbussiness())
            {
                var result = a.CheckUser(username, password);
                if (result)
                {
                    using (var b = new PlayerBussiness())
                    {
                        b.CreateUsername(username, inviteid);
                        return true;
                    }
                }
                return false;
            }

        }

        public int GetUserType(string name)
        {
            using (var a = new PlayerBussiness())
            {
                return a.GetUserType(name);
            }
        }

        public bool ExistsUsername(string name)
        {
            using (var a = new MemberShipbussiness())
            {
                return a.ExistsUsername(name);
            }
        }

        public void AddPlayer(string user, string pass)
        {
            PlayerManager.Add(user, pass);
        }

        public string Login(string p, string site, string IP)
        {
            log.Debug(p);
            log.Debug(site);
            log.Debug(IP);
            bool value = false;
            string message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail1", new object[0]);
            bool isError = false;
            XElement result = new XElement("Result");
            string rsa = "<RSAKeyValue><Modulus>zRSdzFcnZjOCxDMkWUbuRgiOZIQlk7frZMhElQ0a7VqZI9VgU3+lwo0ghZLU3Gg63kOY2UyJ5vFpQdwJUQydsF337ZAUJz4rwGRt/MNL70wm71nGfmdPv4ING+DyJ3ZxFawwE1zSMjMOqQtY4IV8his/HlgXuUfIHVDK87nMNLc=</Modulus><Exponent>AQAB</Exponent><P>7lzjJCmL0/unituEcjoJsZhUDYajgiiIwWwuh0NlCZElmfa5M6l8H+Ahd9yo7ruT6Hrwr4DAdrIKP6LDmFhBdw==</P><Q>3EFKHt4FcDiZXRBLqYZaNSmM1KDrrU97N3CtEDCYS4GimmFOGJhmuK3yGfp/nYLcL2BTKyOZLSQO+/nAjRp2wQ==</Q><DP>SFdkkGsThuiPdrMcxVYb7wxeJiTApxYKOznL/T1VAsxMbyfUGXvMshfh0HDlzF6diycUuQ8IWn26YonRdwECDQ==</DP><DQ>xR9x1NpkB6HAMHhLHzftODMtpYc4Jm5CGsYvPZQgWUN2YbDAkmajWJnlWbbFzBS4N3aAONWtW6cv+ff2itKqgQ==</DQ><InverseQ>oyJzP0Sn+NgdNRRc7/cUKkbbbYaNxkDLDvKLDYMKV6+gcDce85t/FGfaTwkuYQNFqkrRBtDYjtfGsPRTGS6Mow==</InverseQ><D>wM33JNtzUSRwdmDWdZC4BuOYa2vJoD0zc0bNI4x0ml2oyAWdUCMcBfKEds/6i1T6s2e91d2dcJ/aI27o22gO/sfNg3tsr7uYMiUuhSjniqBDB/zyUVig29E4qdfuY1GHxTE8zurroY8mgGEB0aLj+gE0yX9T7sDFkY0QYRqJnwE=</D></RSAKeyValue>";
            try
            {
                BaseInterface inter = BaseInterface.CreateInterface();
                if (!string.IsNullOrEmpty(p))
                {
                    byte[] src = RsaDecryt2(GetRSACrypto(rsa), HttpUtility.HtmlDecode(p));
                    string[] strList = Encoding.UTF8.GetString(src, 7, src.Length - 7).Split(new char[]
                    {
                        ','
                    });
                    if (strList.Length == 4)
                    {
                        message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail100", new object[0]);
                        string name = strList[0];
                        string pwd = strList[1];
                        string newPwd = strList[2];
                        string nickname = strList[3];
                        if (PlayerManager.Login(name, pwd))
                        {
                            message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail1000", new object[0]);
                            int isFirst = 0;
                            bool isActive = false;
                            bool firstValidate = PlayerManager.GetByUserIsFirst(name);
      
                            PlayerInfo player = inter.CreateLogin(name, newPwd, ref message, ref isFirst, IP, ref isError, firstValidate, ref isActive, site, nickname);
                            //message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail10", new object[0]);
                            if (player != null && !isError)
                            {
                                message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail10000", new object[0]);
                                if (isFirst == 0)
                                {
                                    PlayerManager.Update(name, newPwd);
                                }
                                else
                                {
                                    PlayerManager.Remove(name);
                                }
                                string style = string.IsNullOrEmpty(player.Style) ? ",,,,,,,," : player.Style;
                                player.Colors = (string.IsNullOrEmpty(player.Colors) ? ",,,,,,,," : player.Colors);
                                XElement node = new XElement("Item", new object[]
                                {
                                    new XAttribute("ID", player.ID),
                                    new XAttribute("IsFirst", isFirst),
                                    new XAttribute("NickName", player.NickName),
                                    new XAttribute("Date", ""),
                                    new XAttribute("IsConsortia", 0),
                                    new XAttribute("ConsortiaID", player.ConsortiaID),
                                    new XAttribute("Sex", player.Sex),
                                    new XAttribute("WinCount", player.Win),
                                    new XAttribute("TotalCount", player.Total),
                                    new XAttribute("EscapeCount", player.Escape),
                                    new XAttribute("DutyName", (player.DutyName == null) ? "" : player.DutyName),
                                    new XAttribute("GP", player.GP),
                                    new XAttribute("Honor", ""),
                                    new XAttribute("Style", style),
                                    new XAttribute("Gold", player.Gold),
                                    new XAttribute("Colors", (player.Colors == null) ? "" : player.Colors),
                                    new XAttribute("Attack", player.Attack),
                                    new XAttribute("Defence", player.Defence),
                                    new XAttribute("Agility", player.Agility),
                                    new XAttribute("Luck", player.Luck),
                                    new XAttribute("Grade", player.Grade),
                                    new XAttribute("Hide", player.Hide),
                                    new XAttribute("Repute", player.Repute),
                                    new XAttribute("ConsortiaName", (player.ConsortiaName == null) ? "" : player.ConsortiaName),
                                    new XAttribute("Offer", player.Offer),
                                    new XAttribute("Skin", (player.Skin == null) ? "" : player.Skin),
                                    new XAttribute("ReputeOffer", player.ReputeOffer),
                                    new XAttribute("ConsortiaHonor", player.ConsortiaHonor),
                                    new XAttribute("ConsortiaLevel", player.ConsortiaLevel),
                                    new XAttribute("ConsortiaRepute", player.ConsortiaRepute),
                                    new XAttribute("Money", player.Money),
                                    new XAttribute("AntiAddiction", player.AntiAddiction),
                                    new XAttribute("IsMarried", player.IsMarried),
                                    new XAttribute("SpouseID", player.SpouseID),
                                    new XAttribute("SpouseName", (player.SpouseName == null) ? "" : player.SpouseName),
                                    new XAttribute("MarryInfoID", player.MarryInfoID),
                                    new XAttribute("IsCreatedMarryRoom", player.IsCreatedMarryRoom),
                                    new XAttribute("IsGotRing", player.IsGotRing),
                                    new XAttribute("LoginName", (player.UserName == null) ? "" : player.UserName),
                                    new XAttribute("Nimbus", player.Nimbus),
                                    new XAttribute("FightPower", player.FightPower),
                                    new XAttribute("AnswerSite", player.AnswerSite),
                                    new XAttribute("VIPLevel",player.VipLevel),
                                    new XAttribute("ChargedMoney",player.ChargedMoney)
                                });
                                result.Add(node);
                                value = true;
                                message = LanguageMgr.GetTranslation("Tank.Request.Login.Success", new object[0]);
                            }
                            else
                            {
                                PlayerManager.Remove(name);
                            }
                        }
                        else
                        {
                            message = LanguageMgr.GetTranslation("登录已失效，请重新登录", new object[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WebHelperService.log.Error("User Login error", ex);
                value = false;
                message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail", new object[0]);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            return result.ToString(false);
        }
        public static RSACryptoServiceProvider GetRSACrypto(string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(new CspParameters
            {
                Flags = CspProviderFlags.UseMachineKeyStore
            });
            rsa.FromXmlString(privateKey);
            return rsa;
        }
        public static string RsaDecrypt(string privateKey, string src)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(new CspParameters
            {
                Flags = CspProviderFlags.UseMachineKeyStore
            });
            rsa.FromXmlString(privateKey);
            return RsaDecrypt(rsa, src);
        }
        public static string RsaDecrypt(RSACryptoServiceProvider rsa, string src)
        {
            byte[] srcData = Convert.FromBase64String(src);
            byte[] destData = rsa.Decrypt(srcData, false);
            return Encoding.UTF8.GetString(destData);
        }
        public static byte[] RsaDecryt2(RSACryptoServiceProvider rsa, string src)
        {
            byte[] srcData = Convert.FromBase64String(src);
            return rsa.Decrypt(srcData, false);
        }

        public string GetUserNameByID(int userid)
        {
            using (PlayerBussiness a = new PlayerBussiness())
            {
                var b = a.GetUserSingleByUserID(userid);
                {
                    if (b != null)
                    {
                        return b.UserName;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }

        public List<bool> GetRunMgr()
        {
            var x = WebServer.Instance.runmgr;
            var a = new bool[] { x.CenterStatus, x.FightStatus, x.GameStatus, };
            return a.ToList();
        }

        public List<UserInfo> GetAllUserInfo()
        {
            using (var a = new ManageBussiness())
            {
                return a.GetAllUserInfo();
            }
        }

        public List<ItemTemplateInfo> GetSingleCategoryItemTemplates(int i)
        {
            using (var a = new ProduceBussiness())
            {
                return a.GetSingleCategory(i).ToList();
            }
        }

        public ItemTemplateInfo GetSingleItemTemplate(int i)
        {
            using (var a = new ProduceBussiness())
            {
                return a.GetSingleGoods(i);
            }
        }

        public void SendMailAndItem(string title, string content, int b, int gold, int money, int giftToken, string str)
        {
            using (var a = new PlayerBussiness())
            {
                a.SendMailAndItem(title, content, b, gold, money, giftToken, str);
            }
        }

        public bool Reconnect()
        {
            return WebServer.Instance.Reconnect();
        }

        public string GMAction(string action, Dictionary<string, string> param)
        {
            string msg = "错误";
            var dic = new SafeStringToStringDirectionary(param);
            var runmgr = WebServer.Instance.runmgr;
            var WebPath = AppConfig.AppSettings["WebPath"].ToSafeString();
            string path = WebPath + @"xml\";
            switch (action)
            {
                case "start":
                    #region start
                    string key = "$^&^(*&)*(J1534765";
                    if (dic["type"] == "center")
                    {
                        if (!runmgr.CenterStatus)
                        {
                            if (runmgr.StartCenter(key, false))
                            {
                                WebServer.Instance.Reconnect();
                                msg = "成功";
                            }
                        }
                    }
                    else if (dic["type"] == "fight")
                    {
                        if (!runmgr.FightStatus)
                        {
                            if (runmgr.StartFight(key, false))
                                msg = "成功";
                        }
                    }
                    else if (dic["type"] == "game")
                    {
                        if (!runmgr.GameStatus)
                        {
                            if (runmgr.StartGame(key, false))
                                msg = "成功";
                        }
                    }
                    #endregion 
                    break;
                case "stop":
                    #region stop
                    if (dic["type"] == "center")
                    {
                        if (runmgr.CenterStatus)
                        {
                            if (runmgr.StopCenter())
                                msg = "成功";
                        }
                    }
                    else if (dic["type"] == "fight")
                    {
                        if (runmgr.FightStatus)
                        {
                            if (runmgr.StopFight())
                                msg = "成功";
                        }
                    }
                    else if (dic["type"] == "game")
                    {
                        if (runmgr.GameStatus)
                        {
                            if (runmgr.StopGame())
                                msg = "成功";
                        }
                    }
                    #endregion 
                    break;
                case "notice":
                    #region notice
                    var str = dic["str"];
                    if (str != "")
                    {
                        if (new ManageBussiness().SystemNotice(str))
                        {
                            msg = "发送成功";
                        }
                    }
                    #endregion
                    break;
                case "celeb":
                    #region celeb
                    StringBuilder build = new StringBuilder();
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaDayHonor", 14, "CelebByConsortiaDayHonor_Out"));
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaDayRiches", 11, "CelebByConsortiaDayRiches_Out"));
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaFightPower", 17, "CelebByConsortiaFightPower_Out"));
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaHonor", 13, "CelebByConsortiaHonor_Out"));
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaLevel", 16, "CelebByConsortiaLevel_Out"));
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaRiches", 10, "CelebByConsortiaRiches_Out"));
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaWeekHonor", 15, "CelebByConsortiaWeekHonor_Out"));
                    build.Append(XMLBuild.BuildCelebConsortia(path, "CelebByConsortiaWeekRiches", 12, "CelebByConsortiaWeekRiches_Out"));
                    build.Append(XMLBuild.CelebByDayBestEquipBuild(path));
                    build.Append(XMLBuild.BuildCelebUsers(path, "CelebByDayFightPowerList", 6, "CelebByDayFightPowerList_Out"));
                    build.Append(XMLBuild.BuildCelebUsers(path, "CelebByDayGPList", 2, "CelebByDayGPList_Out"));
                    build.Append(XMLBuild.BuildCelebUsers(path, "CelebByDayOfferList", 4, "CelebByDayOfferList_Out"));
                    build.Append(XMLBuild.BuildCelebUsers(path, "CelebByGpList", 0, "CelebByGpList"));
                    build.Append(XMLBuild.BuildCelebUsers(path, "CelebByOfferList", 1, "CelebByOfferList_Out"));
                    build.Append(XMLBuild.BuildCelebUsers(path, "CelebByWeekGPList", 3, "CelebByWeekGPList_Out"));
                    build.Append(XMLBuild.BuildCelebUsers(path, "CelebByWeekOfferList", 5, "CelebByWeekOfferList"));
                    msg = build.ToString();
                    #endregion
                    break;
                case "xml":
                    #region xml                   
                    StringBuilder bulid = new StringBuilder();
                    bulid.Append(XMLBuild.ActiveListBulid(path));
                    bulid.Append(XMLBuild.BallListBulid(path));
                    bulid.Append(XMLBuild.LoadMapsItemsBulid(path));
                    bulid.Append(XMLBuild.LoadPVEItemsBuild(path));
                    bulid.Append(XMLBuild.QuestListBulid(path));
                    bulid.Append(XMLBuild.TemplateAllListBulid(path));
                    bulid.Append(XMLBuild.ShopItemListBulid(path));
                    bulid.Append(XMLBuild.LoadItemsCategoryBulid(path));
                    bulid.Append(XMLBuild.ItemStrengthenListBulid(path));
                    bulid.Append(XMLBuild.MapServerListBulid(path));
                    bulid.Append(XMLBuild.ConsortiaLevelListBulid(path));
                    bulid.Append(XMLBuild.DailyAwardListBulid(path));
                    bulid.Append(XMLBuild.NPCInfoListBulid(path));
                    bulid.Append(XMLBuild.DropItemForNewRegisterBulid(path));
                    bulid.Append(XMLBuild.AchievementListBulid(path));
                    bulid.Append(XMLBuild.LoadUserBoxBuild(path));
                    bulid.Append(XMLBuild.FightLabDropItemListBulid(path));
                    bulid.Append(XMLBuild.LoadBoxTempBuild(path));
                    msg = bulid.ToString();
                    new ManageBussiness().Reload("shop");
                    new ManageBussiness().Reload("item");
                    new ManageBussiness().Reload("quest");
                    new ManageBussiness().Reload("fb");
                    #endregion
                    break;
                case "kitoff":
                    #region kitoff
                    var UserID = dic["UserID"].ConvertToInt(0);
                    if (UserID == 0)
                    {
                        msg = "用户ID错误！";
                    }
                    else
                    {
                        if (new ManageBussiness().KitoffUser(UserID, "你被管理员踢出游戏") == 0)
                            msg = "成功！";
                        else
                            msg = "失败！";

                    }
                    #endregion
                    break;
                case "forbid":
                    #region forbid
                    var UserID2 = dic["UserID"].ConvertToInt(0);
                    if (UserID2 == 0)
                    {
                        msg = "用户ID错误！";
                    }
                    else
                    {
                        var reason = dic["reason"];
                        var day = dic["day"].ConvertToInt(1);
                        if (day > 0)
                        {
                            if (new ManageBussiness().ForbidPlayerByUserID(UserID2, DateTime.Now.AddDays(day), false, reason))
                                msg = "成功！";
                            else
                                msg = "失败！";
                        }
                        else
                        {
                            if (new ManageBussiness().ForbidPlayerByUserID(UserID2, DateTime.Now, true, ""))
                                msg = "成功！";
                            else
                                msg = "失败！";
                        }
                    }
                    #endregion
                    break;
                case "charge":
                    #region charge
                    var UserID3 = dic["UserID"].ConvertToInt(0);
                    if (UserID3 == 0)
                    {
                        msg = "用户ID错误！";
                    }
                    else
                    {
                        int money = dic["money"].ConvertToInt(0);
                        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                        int ID = (int)(DateTime.Now - startTime).TotalSeconds;
                        if (new ManageBussiness().AddCharge(ID, UserID3, money))
                            msg = "成功！";
                        else
                            msg = "失败！";

                    }
                    #endregion
                    break;
                default:
                    break;
            }

            return msg;
        }

        public string ServerList()
        {
            bool value = false;
            string message = "Fail!";
            int total = 0;
            XElement result = new XElement("Result");
            try
            {
                using (CenterServiceClient temp = new CenterServiceClient())
                {
                    IList<ServerData> list = temp.GetServerList();
                    foreach (ServerData s in list)
                    {
                        if (s.State != -1)
                        {
                            total += s.Online;
                            result.Add(FlashUtils.CreateServerInfo(s.Id, s.Name, s.Ip, s.Port, s.State, s.MustLevel, s.LowestLevel, s.Online));
                        }
                    }
                }
                value = true;
                message = "Success!";
            }
            catch (Exception ex)
            {
                //serverList.log.Error("Load server list error:", ex);
            }
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            result.Add(new XAttribute("total", total));
            result.Add(new XAttribute("agentId", "1"));
            result.Add(new XAttribute("AreaName", "DDT"));
            return result.ToString(false);
        }

        public void MailNotice(int userid)
        {
            using (var a = new CenterServiceClient())
            {
                a.MailNotice(userid);
            }
        }
    }
}
