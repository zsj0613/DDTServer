using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;

using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(46, "购买优惠物品")]
	public class UserBuySaleItemHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int gold = 0;
			int money = 0;
			int offer = 0;
			int gifttoken = 0;
			int totalGold = 0;
			int totalMoney = 0;
			int totalOffer = 0;
			int totalGifttoken = 0;
			List<ItemJoinShopInfo> itemPrice = new List<ItemJoinShopInfo>();
			int GroupID = packet.ReadInt();
			eMessageType eMsg = eMessageType.Normal;
			string msg = "UserBuyItemHandler.Success";
			List<ItemInfo> buyItems = new List<ItemInfo>();
			int result;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 1;
			}
			else
			{
				List<ShopItemInfo> infos = ShopMgr.FindShopByGroupID(GroupID);
				foreach (ShopItemInfo info in infos)
				{
					ItemInfo item = ShopMgr.CreateItem(info, 102, 1, "", "", true);
					buyItems.Add(item);
					ShopMgr.GetShopItemBuyConditions(info, 1, ref gold, ref money, ref offer, ref gifttoken);
					itemPrice.Add(new ItemJoinShopInfo(info.TemplateID, item.Count, money, gold, gifttoken, offer, ""));
					totalGold += gold;
					totalMoney += money;
					totalOffer += offer;
					totalGifttoken += gifttoken;
				}
				if (buyItems.Count == 0)
				{
					result = 1;
				}
				else
				{
					if (totalGold <= player.PlayerCharacter.Gold && totalMoney <= player.PlayerCharacter.Money && totalOffer <= player.PlayerCharacter.Offer && totalGifttoken <= player.PlayerCharacter.GiftToken)
					{
						player.RemoveGold(totalGold);
						player.RemoveMoney(totalMoney, LogMoneyType.Shop, LogMoneyType.Shop_BuySale);
						player.RemoveOffer(totalOffer);
						player.RemoveGiftToken(totalGifttoken);
						foreach (ItemJoinShopInfo log in itemPrice)
						{
							//LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_BuySale, player.PlayerCharacter.ID, log.Moneys, log.Gold, log.GiftToken, log.Offer, log.OtherPay, log.TemplateID, 5, log.Data);
						}
						List<ItemInfo> remainitems = new List<ItemInfo>();
						List<ItemInfo> sendMailItem = new List<ItemInfo>();
						for (int i = 0; i < buyItems.Count; i++)
						{
							if (!player.HideBag.AddItemTo(buyItems[i], buyItems.Count - (i + 1)))
							{
								remainitems.Add(buyItems[i]);
							}
						}
						player.StackItem(ref remainitems);
						foreach (ItemInfo info2 in remainitems)
						{
							if (!player.AddItem(info2))
							{
								sendMailItem.Add(info2);
							}
						}
						if (sendMailItem.Count > 0)
						{
							using (new PlayerBussiness())
							{
								string title = "";
								if (player.SendItemsToMail(sendMailItem, "", title, eMailType.BuyItem))
								{
									player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								}
								eMsg = eMessageType.ERROR;
								msg = "UserBuyItemHandler.Mail";
							}
						}
						player.OnPaid(totalMoney, totalGold, totalOffer, totalGifttoken, "");
						player.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg, new object[0]));
						player.SaveIntoDatabase();
						result = 0;
					}
					else
					{
						if (money > player.PlayerCharacter.Money)
						{
							player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.Money", new object[0]));
						}
						result = 1;
					}
				}
			}
			return result;
		}
	}
}
