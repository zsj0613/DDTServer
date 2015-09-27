using System;
namespace Game.Server.Packets
{
	public enum eMailType
	{
		Default,
		Common,
		AuctionSuccess,
		AuctionFail,
		BidSuccess,
		BidFail,
		ReturnPayment,
		PaymentCancel,
		BuyItem,
		ItemOverdue,
		PresentItem,
		PaymentFinish,
		OpenUpArk,
		StoreCanel,
		Marry,
		DailyAward,
		Manage = 51,
		Active,
		Payment = 101,
		TimeBox
	}
}
