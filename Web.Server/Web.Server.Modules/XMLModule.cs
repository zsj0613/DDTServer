using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lsj.Util.Net.Web.Error;
using Lsj.Util.Net.Web.Event;
using Lsj.Util.Net.Web.Interfaces;
using Lsj.Util.Net.Web.Message;

namespace Web.Server.Modules
{
    public class XMLModule : IModule
    {
        public void Process(object website, ProcessEventArgs args)
        {

            var Request = args.Request;

            var Uri = Request.Uri;
            if (Uri.ToString().StartsWith("/xml/"))
            {
                args.IsProcessed = true;
                switch (Uri.FileName)
                {

                    case "BoxTemplateList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BoxTemplateList());
                        break;
                    case "FightLabDropItemList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.FightLabDropItemList());
                        break;
                    case "Box.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.LoadUserBox());
                        break;
                    case "AchievementList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.AchievementList());
                        break;
                    case "DropList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.DropList());
                        break;
                    case "NPCInfoList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.NPCInfoList());
                        break;
                    case "DailyAwardList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.DailyAwardList());
                        break;
                    case "ConsortiaLevelList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.ConsortiaLevelList());
                        break;
                    case "MapServerList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.MapServerList());
                        break;
                    case "ItemStrengthenList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.ItemStrengthenList());
                        break;
                    case "ItemsCategory.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.ItemsCategory());
                        break;
                    case "ShopItemList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.ShopItemList());
                        break;
                    case "TemplateAllList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.TemplateAllList());
                        break;
                    case "QuestList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.QuestList());
                        break;
                    case "PVEList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.PVEList());
                        break;
                    case "MapList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.MapList());
                        break;
                    case "BallList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BallList());
                        break;
                    case "ActiveList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.ActiveList());
                        break;
                    case "CelebByConsortiaDayHonor.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(14));
                        break;
                    case "CelebByConsortiaDayRiches.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(11));
                        break;
                    case "CelebByConsortiaFightPower.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(17));
                        break;
                    case "CelebByConsortiaHonor.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(13));
                        break;
                    case "CelebByConsortiaLevel.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(16));
                        break;
                    case "CelebByConsortiaRiches.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(10));
                        break;
                    case "CelebByConsortiaWeekHonor.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(15));
                        break;
                    case "CelebByConsortiaWeekRiches.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebConsortia(12));
                        break;

                    case "CelebByDayFightPowerList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebUsers(6));
                        break;
                    case "CelebByDayGPList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebUsers(2));
                        break;
                    case "CelebByDayOfferList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebUsers(4));
                        break;
                    case "CelebByGpList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebUsers(0));
                        break;
                    case "CelebByOfferList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebUsers(1));
                        break;
                    case "CelebByWeekGPList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebUsers(3));
                        break;
                    case "CelebByWeekOfferList.xml":
                        args.Response = new HttpResponse();
                        args.Response.Write(XMLBuild.BuildCelebUsers(5));
                        break;





                    default:
                        args.Response = ErrorHelper.Build(404, 0, args.ServerName);
                        break;
                }
            }
        }
    }
}
