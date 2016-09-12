using System;
using System.Collections.Generic;
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
    public class AuctionPageList
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {
            bool value = false;
            string message = "Fail!";
            int total = 0;
            XElement result = new XElement("Result");
            int page = int.Parse(Request.Uri.QueryString["page"]);
            string name = csFunction.ConvertSql(HttpUtility.UrlDecode(Request.Uri.QueryString["name"]));
            int type = int.Parse(Request.Uri.QueryString["type"]);
            int pay = int.Parse(Request.Uri.QueryString["pay"]);
            int userID = int.Parse(Request.Uri.QueryString["userID"]);
            int buyID = int.Parse(Request.Uri.QueryString["buyID"]);
            int order = int.Parse(Request.Uri.QueryString["order"]);
            bool sort = bool.Parse(Request.Uri.QueryString["sort"]);
            string AuctionIDs = csFunction.ConvertSql(HttpUtility.UrlDecode(Request.Uri.QueryString["Auctions"]));
            AuctionIDs = (string.IsNullOrEmpty(AuctionIDs) ? "0" : AuctionIDs);
            int size = 100;
            using (PlayerBussiness db = new PlayerBussiness())
            {
                AuctionInfo[] infos = db.GetAuctionPage(page, name, type, pay, ref total, userID, buyID, order, sort, size, AuctionIDs);
                AuctionInfo[] array = infos;
                for (int i = 0; i < array.Length; i++)
                {
                    AuctionInfo info = array[i];
                    XElement temp = FlashUtils.CreateAuctionInfo(info);
                    using (PlayerBussiness pb = new PlayerBussiness())
                    {
                        ItemInfo item = pb.GetUserItemSingle(info.ItemID);
                        if (item != null)
                        {
                            temp.Add(FlashUtils.CreateGoodsInfo(item));
                        }
                        result.Add(temp);
                    }
                }
                value = true;
                message = "Success!";
            }
            result.Add(new XAttribute("total", total));
            result.Add(new XAttribute("vaule", value));
            result.Add(new XAttribute("message", message));
            Response.Write(result.ToString(false));
        }
    }
}
