using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using Center.Server;
using Lsj.Util;
using Lsj.Util.Collections;
using Lsj.Util.Net.Web.Interfaces;
using Lsj.Util.Net.Web.Post;
using Lsj.Util.Text;
using Web.Server.Manager;

namespace Web.Server.Modules.Admin
{
    public class GMAction
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {
            string username = Request.Cookies["user"].content;
            string password = Request.Cookies["pass"].content;
            int usertype = 0;

            using (var a = new MemberShipBussiness())
            {
                var result = a.CheckUser(username, password);
                if (result)
                {
                    using (var b = new PlayerBussiness())
                    {
                        usertype = b.GetUserType(username);
                    }
                }
            }

            if (usertype > 1)
            {
                var page = Request.Uri.QueryString["method"].ToSafeString();
                switch (page)
                {
                    case "charge":
                        ProcessCharge(Request, Response);
                        break;
                    case "forbid":
                        ProcessForbid(Request, Response);
                        break;
                    case "kitoff":
                        ProcessKitoff(Request, Response);
                        break;
                    case "notice":
                        ProcessNotice(Request, Response);
                        break;
                    case "start":
                        ProcessStart(Request, Response);
                        break;
                    case "stop":
                        ProcessStop(Request, Response);
                        break;
                    case "reconnect":
                        ProcessReconnect(Request, Response);
                        break;
                    case "loginkey":
                        ProcessKey(Request, Response, usertype);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                Response.Write("你无权访问GM管理平台！");
            }
        }

        private static void ProcessStop(IHttpRequest Request, IHttpResponse Response)
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("type", Request.Uri.QueryString["type"].ToSafeString());
            Response.Write(Action("stop", dic.ToDictionary()));
        }

        private static void ProcessStart(IHttpRequest Request, IHttpResponse Response)
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("type", Request.Uri.QueryString["type"].ToSafeString());
            Response.Write(Action("start", dic.ToDictionary()));
        }

        private static void ProcessNotice(IHttpRequest Request, IHttpResponse Response)
        {
            var postdata = Request.Content.ReadAll().ConvertFromBytes(Encoding.UTF8);
            var Form = FormParser.Parse(postdata);
            var dic = new SafeStringToStringDirectionary();
            dic.Add("str", Form["Tx_url"].ToSafeString());
            Response.Write(Action("notice", dic.ToDictionary()));
        }

        private static void ProcessKitoff(IHttpRequest Request, IHttpResponse Response)
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("UserID", Request.Uri.QueryString["UserID"].ToSafeString());
            Response.Write(Action("kitoff", dic.ToDictionary()));
        }

        private static void ProcessReconnect(IHttpRequest Request, IHttpResponse Response)
        {

            if (WebServer.Instance.Reconnect())
            {
                Response.Write("成功");
            }
            else
            {
                Response.Write("错误");
            }

        }

        private static void ProcessForbid(IHttpRequest Request, IHttpResponse Response)
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("UserID", Request.Uri.QueryString["UserID"].ToSafeString());
            dic.Add("reason", Request.Uri.QueryString["reason"].ToSafeString());
            dic.Add("day", Request.Uri.QueryString["day"].ToSafeString());
            Response.Write(Action("forbid", dic.ToDictionary()));
        }
        private static void ProcessCharge(IHttpRequest Request, IHttpResponse Response)
        {
            var dic = new SafeStringToStringDirectionary();
            dic.Add("UserID", Request.Uri.QueryString["UserID"].ToSafeString());
            dic.Add("money", Request.Uri.QueryString["money"].ToSafeString());
            Response.Write(Action("charge", dic.ToDictionary()));
        }
        public static string Action(string action, Dictionary<string, string> param)
        {
            string msg = "错误";
            var dic = new SafeStringToStringDirectionary(param);
            var runmgr = WebServer.Runmgr;
            switch (action)
            {
                case "start":
                    #region start
                    if (dic["type"] == "center")
                    {
                        if (!runmgr.CenterStatus)
                        {
                            if (runmgr.StartCenter())
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
                            if (runmgr.StartFight())
                                msg = "成功";
                        }
                    }
                    else if (dic["type"] == "game")
                    {
                        if (!runmgr.GameStatus)
                        {
                            if (runmgr.StartGame())
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
                        if (OpenAPIs.SendNotice(str))
                        {
                            msg = "发送成功";
                        }
                    }
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
                        if (OpenAPIs.KitoffUser(UserID, "你被管理员踢出游戏"))
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

        private static void ProcessKey(IHttpRequest Request, IHttpResponse Response, int usertype)
        {
            using (var a = new MemberShipBussiness())
            {
                using (var b = new PlayerBussiness())
                {
                    var name = Request.Uri.QueryString["UserName"].ToSafeString();
                    if (name != "" && a.ExistsUsername(name))
                    {
                        var type = b.GetUserType(name);
                        if (usertype <= type)
                        {
                            Response.Write("对不起,你的权限不足");
                            return;
                        }
                        var pass = Guid.NewGuid().ToString();
                        PlayerManager.Add(name, pass);
                        string content = "user=" + name + "&key=" + pass;
                        Response.Write(content);
                    }
                    else
                    {
                        Response.Write("错误");
                    }
                }
            }
        }
    }
}
