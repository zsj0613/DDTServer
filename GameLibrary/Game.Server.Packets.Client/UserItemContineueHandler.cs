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
	[PacketHandler(62, "续费")]
	public class UserItemContineueHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result2 = 0;
			}
			else
			{
				StringBuilder payGoods = new StringBuilder();
				int count = packet.ReadInt();
				bool istips = true;
				for (int i = 0; i < count; i++)
				{
					eBageType bag = (eBageType)packet.ReadByte();
					int place = packet.ReadInt();
					int goodsID = packet.ReadInt();
					int type = (int)packet.ReadByte();
					bool isDress = packet.ReadBoolean();
					if (bag == eBageType.MainBag || bag == eBageType.PropBag || bag == eBageType.Bank)
					{
						ItemInfo item = player.GetItemAt(bag, place);
						if (item != null && item.ValidDate != 0 && !item.IsValidItem() && (bag == eBageType.MainBag || (bag == eBageType.PropBag && item.TemplateID == 10200)))
						{
							int gold = 0;
							int money = 0;
							int offer = 0;
							int gifttoken = 0;
							int oldDate = item.ValidDate;
							int oldCount = item.Count;
							bool isValid = item.IsValidItem();
							List<int> needitemsinfo = new List<int>();
							ShopItemInfo shopitem = ShopMgr.GetShopItemInfoById(goodsID);
							needitemsinfo = ShopMgr.GetShopItemBuyConditions(shopitem, type, ref gold, ref money, ref offer, ref gifttoken);
							int icount = player.MainBag.GetItems().Count;
							bool result = true;
							for (int j = 0; j < needitemsinfo.Count; j += 2)
							{
								if (player.GetItemCount(needitemsinfo[j]) < needitemsinfo[j + 1])
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
							if (gold <= player.PlayerCharacter.Gold && money <= player.PlayerCharacter.Money && offer <= player.PlayerCharacter.Offer && gifttoken <= player.PlayerCharacter.GiftToken)
							{
								player.RemoveMoney(money, LogMoneyType.Shop, LogMoneyType.Shop_Continue);
								player.RemoveGold(gold);
								player.RemoveOffer(offer);
								player.RemoveGiftToken(gifttoken);
								for (int j = 0; j < needitemsinfo.Count; j += 2)
								{
									player.RemoveTemplate(needitemsinfo[j], needitemsinfo[j + 1], eItemRemoveType.ShoppingForContinue);
									payGoods.Append(needitemsinfo[j].ToString() + ":");
								}
								if (!isValid && item.ValidDate != 0)
								{
									if (1 == type)
									{
										item.ValidDate = shopitem.AUnit;
										item.BeginDate = DateTime.Now;
										item.IsUsed = true;
									}
									if (2 == type)
									{
										item.ValidDate = shopitem.BUnit;
										item.BeginDate = DateTime.Now;
										item.IsUsed = true;
									}
									if (3 == type)
									{
										item.ValidDate = shopitem.CUnit;
										item.BeginDate = DateTime.Now;
										item.IsUsed = true;
									}
								}
								if (bag == eBageType.MainBag)
								{
									player.MainBag.UpdateItem(item);
								}
								else
								{
									if (bag == eBageType.PropBag)
									{
										player.PropBag.UpdateItem(item);
									}
									else
									{
										player.StoreBag.UpdateItem(item);
									}
								}
								//LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Continue, player.PlayerCharacter.ID, money, gold, gifttoken, offer, "", item.TemplateID, 6, item.Count);
							}
							else
							{
								if (money <= player.PlayerCharacter.Money)
								{
									istips = false;
									item.ValidDate = oldDate;
									item.Count = oldCount;
									player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.NoMoney", new object[0]));
								}
								else
								{
									if (gifttoken <= player.PlayerCharacter.GiftToken)
									{
										istips = false;
										item.ValidDate = oldDate;
										item.Count = oldCount;
										player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.NoMoney", new object[0]));
									}
								}
							}
						}
					}
				}
				if (istips)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.Success", new object[0]));
				}
				result2 = 0;
			}
			return result2;
		}
	}
}
