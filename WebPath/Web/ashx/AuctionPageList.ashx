<%@ WebHandler Language="C#" Class="AuctionPageList" Debug ="true"%>

using Bussiness;
using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

public class AuctionPageList : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
		public void ProcessRequest(HttpContext context)
		{
			bool value = false;
			string message = "Fail!";
			int total = 0;
			XElement result = new XElement("Result");
			//try
			//{
				int page = int.Parse(context.Request["page"]);
				string name = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["name"]));
				int type = int.Parse(context.Request["type"]);
				int pay = int.Parse(context.Request["pay"]);
				int userID = int.Parse(context.Request["userID"]);
				int buyID = int.Parse(context.Request["buyID"]);
				int order = int.Parse(context.Request["order"]);
				bool sort = bool.Parse(context.Request["sort"]);
				string AuctionIDs = csFunction.ConvertSql(HttpUtility.UrlDecode(context.Request["Auctions"]));
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
			//}
			//catch (Exception ex)
			//{
			//	AuctionPageList.log.Error("AuctionPageList", ex);
			//}
			result.Add(new XAttribute("total", total));
			result.Add(new XAttribute("vaule", value));
			result.Add(new XAttribute("message", message));
			context.Response.ContentType = "text/plain";
			context.Response.Write(result.ToString(false));
		}
	}