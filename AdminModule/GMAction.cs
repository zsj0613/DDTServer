using Bussiness;
using Lsj.Util;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Modules;
using Lsj.Util.Net.Web.Protocol;
using Lsj.Util.Net.Web.Request;
using Lsj.Util.Net.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Server.Managers;

namespace Web.Server.Module
{
    public class GMAction : IModule
    {
        string username;
        string password;
        public HttpResponse Process(HttpRequest request)
        {
            var response = new HttpResponse(request);
            response.ContentType = "text/plain; charset=utf-8";
            username = request.Cookies["username"].content;
            password = request.Cookies["password"].content;
            if (Admin.GetUserType(username, password) <= 1)
            {
                response.ReturnAndRedict("你无权进行此操作！", "../login.do");
            }
            else
            {
                switch (request.QueryString["method"])
                {
                    case "charge":
                        ProcessCharge(request, ref response);
                        break;
                    case "forbid":
                        ProcessForbid(request, ref response);
                        break;
                    case "kitoff":
                        ProcessKitoff(request, ref response);
                        break;
                    case "xml":
                        ProcessXml(ref response);
                        break;
                    case "celeb":
                        ProcessCeleb(ref response);
                        break;
                    case "notice":
                        ProcessNotice(request, ref response);
                        break;
                    case "start":
                        ProcessStart(request, ref response);
                        break;
                    case "stop":
                        ProcessStop(request, ref response);
                        break;
                    case "reconnect":
                        ProcessReconnect(request, ref response);
                        break;
                    case "loginkey":
                        ProcessKey(request, ref response);
                        break;
                    default:
                        return new ErrorResponse(404);
                }
            }
            return response;
        }

        private void ProcessKey(HttpRequest request, ref HttpResponse response)
        {
            using (MemberShipbussiness a = new MemberShipbussiness())
            {
                var name = request.QueryString["UserName"];
                if (name != "" && a.ExistsUsername(name))
                {
                    var pass = Guid.NewGuid().ToString();
                    PlayerManager.Add(name, pass);
                    string content = $"user={name}&key={pass}";
                    response.Write(content);
                }
                else
                {
                    response.Write("错误");
                }
            }

        }

        private void ProcessReconnect(HttpRequest request, ref HttpResponse response)
        {
            if (WebServer.Instance.Reconnect())
            {
                response.Write("成功");
            }
            response.Write("错误");
        }

        private void ProcessStop(HttpRequest request, ref HttpResponse response)
        {
            var runmgr = WebServer.Instance.runmgr;
            var msg = "错误";
            if (request.QueryString["type"] == "center")
            {
                if (runmgr.CenterStatus)
                {
                    if (runmgr.StopCenter())
                        msg = "成功";
                }
            }
            else if (request.QueryString["type"] == "fight")
            {
                if(runmgr.FightStatus)
                {
                    if (runmgr.StopFight())
                        msg = "成功";
                }
            }
            else if (request.QueryString["type"] == "game")
            {
                if(runmgr.GameStatus)
                {
                    if (runmgr.StopGame())
                        msg = "成功";
                }
            }
            response.Write(msg);
        }

        
        private void ProcessStart(HttpRequest request, ref HttpResponse response)
        {
            string key = "$^&^(*&)*(J1534765";
            var runmgr = WebServer.Instance.runmgr;
            var msg = "错误";
            if (request.QueryString["type"] == "center")
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
            else if (request.QueryString["type"] == "fight")
            {
                if (!runmgr.FightStatus)
                {
                    if (runmgr.StartFight(key, false))
                        msg = "成功";
                }
            }
            else if (request.QueryString["type"] == "game")
            {
                if (!runmgr.GameStatus)
                {
                    if (runmgr.StartGame(key, false))
                        msg = "成功";
                }
            }
            response.Write(msg);
        }

        private void ProcessNotice(HttpRequest request, ref HttpResponse response)
        {
            var msg = request.Form["Tx_url"];
            if (msg != "")
            {
                if (new ManageBussiness().SystemNotice(msg))
                {
                    response.Write("发送成功");
                    return;
                }
            }
            response.Write("错误");
        }

        private void ProcessCeleb(ref HttpResponse response)
        {
            string path = Server.WebPath + @"xml\";
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
            response.Write(build.ToString());
        }

        private void ProcessXml(ref HttpResponse response)
        {
            string path = Server.WebPath + @"xml\";
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
            response.Write(bulid.ToString());
            new ManageBussiness().Reload("shop");
            new ManageBussiness().Reload("item");
            new ManageBussiness().Reload("quest");
            new ManageBussiness().Reload("fb");
        }

        private void ProcessKitoff(HttpRequest request, ref HttpResponse response)
        {
            var UserID = request.QueryString["UserID"].ConvertToInt(0);
            if (UserID == 0)
            {
                response.Write("用户ID错误！");
            }
            else
            {
                if (new ManageBussiness().KitoffUser(UserID, "你被管理员踢出游戏") == 0)
                    response.Write("成功！");
                else
                    response.Write("失败！");

            }
        }

        private void ProcessForbid(HttpRequest request, ref HttpResponse response)
        {
            var UserID = request.QueryString["UserID"].ConvertToInt(0);
            if (UserID == 0)
            {
                response.Write("用户ID错误！");
            }
            else
            {
                var reason = request.QueryString["reason"];
                var day = request.QueryString["day"].ConvertToInt(1);
                if (day > 0)
                {
                    if (new ManageBussiness().ForbidPlayerByUserID(UserID, DateTime.Now.AddDays(day), false, reason))
                        response.Write("成功！");
                    else
                        response.Write("失败！");
                }
                else
                {
                    if (new ManageBussiness().ForbidPlayerByUserID(UserID, DateTime.Now, true, ""))
                        response.Write("成功！");
                    else
                        response.Write("失败！");
                }
            }
        }

        private void ProcessCharge(HttpRequest request, ref HttpResponse response)
        {
            var UserID = request.QueryString["UserID"].ConvertToInt(0);
            if (UserID == 0)
            {
                response.Write("用户ID错误！");
            }
            else
            {
                int money = request.QueryString["money"].ConvertToInt(0);
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                int ID = (int)(DateTime.Now - startTime).TotalSeconds;
                if (new ManageBussiness().AddCharge(ID, UserID, money))
                    response.Write("成功！");
                else
                    response.Write("失败！");

            }
        }
        
        public bool CanProcess(HttpRequest request,ref int code)
        {
            bool result = false;
            if (request.Method == eHttpMethod.GET|| request.Method == eHttpMethod.POST)
            {
                if (request.uri == (@"\admin\GMAction.action"))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
