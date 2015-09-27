using System;
namespace SqlDataProvider.Data
{
	public class ItemJoinShopInfo
	{
		public int TemplateID
		{
			get;
			set;
		}
		public int Data
		{
			get;
			set;
		}
		public int Moneys
		{
			get;
			set;
		}
		public int Gold
		{
			get;
			set;
		}
		public int GiftToken
		{
			get;
			set;
		}
		public int Offer
		{
			get;
			set;
		}
		public string OtherPay
		{
			get;
			set;
		}
		public ItemJoinShopInfo(int templateID, int data, int moneys, int gold, int giftToken, int offer, string otherPay)
		{
			this.TemplateID = templateID;
			this.Data = data;
			this.Moneys = moneys;
			this.Gold = gold;
			this.GiftToken = giftToken;
			this.Offer = offer;
			this.OtherPay = otherPay;
		}
	}
}
