using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;

using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(54, "购买道具")]
	public class PropBuyHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int gold = 0;
			int money = 0;
			int offer = 0;
			int gifttoken = 0;
			StringBuilder payGoods = new StringBuilder();
			int GoodsID = packet.ReadInt();
			int type = 1;
			ShopItemInfo shopItem = ShopMgr.GetShopItemInfoById(GoodsID);
			int result2;
			if (shopItem == null)
			{
				result2 = 1;
			}
			else
			{
				List<int> needitemsinfo = ShopMgr.GetShopItemBuyConditions(shopItem, type, ref gold, ref money, ref offer, ref gifttoken);
				ItemInfo prop = ShopMgr.CreateItem(shopItem, 102, 1, "", "", false);
				if (prop == null)
				{
					result2 = 1;
				}
				else
				{
					if (prop.Template.CategoryID == 10)
					{
						PlayerInfo pi = player.PlayerCharacter;
						if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked && (money > 0 || offer > 0 || gifttoken > 0 || GoodsID == 11408))
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
							result2 = 1;
							return result2;
						}
						int icount = player.MainBag.GetItems().Count;
						bool result = true;
						for (int i = 0; i < needitemsinfo.Count; i += 2)
						{
							if (player.GetItemCount(needitemsinfo[i]) < needitemsinfo[i + 1])
							{
								result = false;
							}
						}
						if (!result)
						{
							eMessageType eMsg = eMessageType.ERROR;
							string msg = "UserBuyItemHandler.NoBuyItem";
							player.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg, new object[0]));
							result2 = 1;
							return result2;
						}
						if (gold <= pi.Gold && money <= ((pi.Money < 0) ? 0 : pi.Money) && offer <= pi.Offer && gifttoken <= pi.GiftToken)
						{
							if (player.FightBag.AddItem(prop, 0))
							{
								player.RemoveGold(gold);
								player.RemoveMoney(money, LogMoneyType.Shop, LogMoneyType.Shop_Buy);
								player.RemoveOffer(offer);
								player.RemoveGiftToken(gifttoken);
								for (int i = 0; i < needitemsinfo.Count; i += 2)
								{
									player.RemoveTemplate(needitemsinfo[i], needitemsinfo[i + 1], eItemRemoveType.Shopping);
									payGoods.Append(needitemsinfo[i].ToString() + ":");
								}
								//LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, player.PlayerCharacter.ID, money, gold, gifttoken, offer, "", prop.TemplateID, 3, prop.Count);
							}
						}
						else
						{
							player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("PropBuyHandler.NoMoney", new object[0]));
						}
					}
					result2 = 0;
				}
			}
			return result2;
		}
	}
}
