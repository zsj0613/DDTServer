using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Bussiness.Interface;
using Center.Server;
using Lsj.Util.Logs;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Interfaces;
using SqlDataProvider.Data;
using Web.Server.Manager;

namespace Web.Server.Modules.Ashx
{
    public class Login
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            string p = Request.Uri.QueryString["p"];
            string site = (Request.Uri.QueryString["site"] == null) ? "" : Request.Uri.QueryString["site"];
            string IP = Request.UserHostAddress;
            bool value = false;
            string message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail1", new object[0]);
            bool isError = false;
            XElement result = new XElement("Result");
            string rsa = "<RSAKeyValue><Modulus>zRSdzFcnZjOCxDMkWUbuRgiOZIQlk7frZMhElQ0a7VqZI9VgU3+lwo0ghZLU3Gg63kOY2UyJ5vFpQdwJUQydsF337ZAUJz4rwGRt/MNL70wm71nGfmdPv4ING+DyJ3ZxFawwE1zSMjMOqQtY4IV8his/HlgXuUfIHVDK87nMNLc=</Modulus><Exponent>AQAB</Exponent><P>7lzjJCmL0/unituEcjoJsZhUDYajgiiIwWwuh0NlCZElmfa5M6l8H+Ahd9yo7ruT6Hrwr4DAdrIKP6LDmFhBdw==</P><Q>3EFKHt4FcDiZXRBLqYZaNSmM1KDrrU97N3CtEDCYS4GimmFOGJhmuK3yGfp/nYLcL2BTKyOZLSQO+/nAjRp2wQ==</Q><DP>SFdkkGsThuiPdrMcxVYb7wxeJiTApxYKOznL/T1VAsxMbyfUGXvMshfh0HDlzF6diycUuQ8IWn26YonRdwECDQ==</DP><DQ>xR9x1NpkB6HAMHhLHzftODMtpYc4Jm5CGsYvPZQgWUN2YbDAkmajWJnlWbbFzBS4N3aAONWtW6cv+ff2itKqgQ==</DQ><InverseQ>oyJzP0Sn+NgdNRRc7/cUKkbbbYaNxkDLDvKLDYMKV6+gcDce85t/FGfaTwkuYQNFqkrRBtDYjtfGsPRTGS6Mow==</InverseQ><D>wM33JNtzUSRwdmDWdZC4BuOYa2vJoD0zc0bNI4x0ml2oyAWdUCMcBfKEds/6i1T6s2e91d2dcJ/aI27o22gO/sfNg3tsr7uYMiUuhSjniqBDB/zyUVig29E4qdfuY1GHxTE8zurroY8mgGEB0aLj+gE0yX9T7sDFkY0QYRqJnwE=</D></RSAKeyValue>";
            //try
            //{
            LSJInterface inter = LSJInterface.CreateInterface();
            if (!string.IsNullOrEmpty(p))
            {
                byte[] src = RsaDecryt2(GetRSACrypto(rsa), p);
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
                        bool needcreate = false;
                        PlayerInfo player = inter.CreateLogin(name, newPwd, ref message, ref isFirst, IP, ref isError, firstValidate, ref isActive, site, nickname, out needcreate);
                        if (needcreate)
                        {
                            OpenAPIs.CreatePlayer(player.ID, name, newPwd, isFirst == 0);
                        }
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
            // }
            //catch
            //{

            //   value = false;
            //   message = LanguageMgr.GetTranslation("Tank.Request.Login.Fail", new object[0]);
            //}
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            Response.Write(result.ToString(false));

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

    }
}
