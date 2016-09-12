using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Center.Server;
using Center.Server.Managers;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Interfaces;
using SqlDataProvider.Data;

namespace Web.Server.Modules.Ashx
{
    public class VisualizeRegister
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = LanguageMgr.GetTranslation("Tank.Request.VisualizeRegister.Fail1", new object[0]);
            XElement result = new XElement("Result");
            //try
            {
                var para = Request.Uri.QueryString;
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
                    if (checkIllegalChar(nickName) || nickName.Contains("\r"))
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
                                if (db2.RegisterPlayer(name, pass, nickName, style, style, armColor, hairColor, faceColor, ClothColor, sex, ref message, 30))
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
            Response.Write(result.ToString(false));
        }

        public static ArrayList contentList = null;
        public static bool checkIllegalChar(string strRegName)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(strRegName))
            {
                flag = checkChar(strRegName);
            }
            return flag;
        }
        private static bool checkChar(string strRegName)
        {
            if (contentList == null)
            {
                contentList = new ArrayList();
                if (File.Exists("illegal.txt"))
                {
                    StreamReader sr = new StreamReader("illegal.txt", Encoding.GetEncoding("GB2312"));
                    string str = "";
                    while (str != null)
                    {
                        str = sr.ReadLine();
                        if (!string.IsNullOrEmpty(str))
                        {
                            contentList.Add(str);
                        }
                    }
                    if (str == null)
                    {
                        sr.Close();
                    }
                }
            }
            bool flag = false;
            foreach (string strLine in contentList)
            {
                if (!strLine.StartsWith("GM"))
                {
                    string text = strLine;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (strRegName.Contains(text[i].ToString()))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                else
                {
                    string[] keyword = strLine.Split(new char[]
                    {
                        '|'
                    });
                    string[] array = keyword;
                    for (int i = 0; i < array.Length; i++)
                    {
                        string key = array[i];
                        if (strRegName.Contains(key) && key != "")
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            return flag;
        }
    }
}
