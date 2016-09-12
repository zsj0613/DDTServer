using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Interfaces;
using SqlDataProvider.Data;

namespace Web.Server.Modules.Ashx
{
    public class CheckNickName
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {
            bool value = false;
            string message = LanguageMgr.GetTranslation("错误!", new object[0]);
            XElement result = new XElement("Result");

            string nickName = csFunction.ConvertSql(HttpUtility.UrlDecode(Request.Uri.QueryString["NickName"]));
            if (!string.IsNullOrEmpty(nickName))
            {
                if (Encoding.Default.GetByteCount(nickName) <= 14)
                {
                    if (!checkIllegalChar(nickName) && !nickName.Contains("\r"))
                    {
                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            if (!db.GetUserCheckByNickName(nickName))
                            {
                                value = true;
                                message = LanguageMgr.GetTranslation("恭喜!角色名可以使用.", new object[0]);
                            }
                            else
                            {
                                value = false;
                                message = LanguageMgr.GetTranslation("用户名已经被使用", new object[0]);
                            }
                        }
                    }
                    else
                    {
                        value = false;
                        message = LanguageMgr.GetTranslation("用户名存在非法字符", new object[0]);
                    }
                }
                else
                {
                    value = false;
                    message = LanguageMgr.GetTranslation("用户名太长", new object[0]);
                }
            }


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
