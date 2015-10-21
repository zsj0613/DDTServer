using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.Buffer;
using Game.Server.GameObjects;

using SqlDataProvider.Data;
using System;

using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(183, "卡片使用")]
	public class CardUseHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int bagType = packet.ReadInt();
			int place = packet.ReadInt();
			int goodsID = packet.ReadInt();
			int type = packet.ReadInt();
			bool ignoreBagLock = packet.ReadBoolean();
			int result;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked && !ignoreBagLock)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 0;
			}
			else
			{
				ItemInfo item;
				string msg;
				if (place == -1)
				{
					int gold = 0;
					int money = 0;
					int offer = 0;
					int gifttoken = 0;
					ShopItemInfo info = ShopMgr.GetShopItemInfoById(goodsID);
					if (null == info)
					{
						result = 1;
						return result;
					}
					List<int> payGoods = ShopMgr.GetShopItemBuyConditions(info, type, ref gold, ref money, ref offer, ref gifttoken);
					if (offer > 0 || payGoods.Count > 0)
					{
						result = 1;
						return result;
					}
					item = ShopMgr.CreateItem(info, 102, type, "", "", true);
					if (item == null)
					{
						result = 1;
						return result;
					}
					player.RemoveMoney(money, LogMoneyType.Shop, LogMoneyType.Shop_Card);
					player.RemoveGold(gold);
					player.RemoveOffer(offer);
					player.RemoveGiftToken(gifttoken);
					//LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Card, player.PlayerCharacter.ID, money, gold, 0, 0, "", item.TemplateID, 1, item.Count);
					msg = "CardUseHandler.Success";
				}
				else
				{
					item = player.PropBag.GetItemAt(place);
					msg = "CardUseHandler.Success";
				}
				if (item != null)
				{
					string msg2 = string.Empty;
					if (item.Template.Property1 != 21)
					{
						AbstractBuffer buffer = BufferList.CreateBuffer(item.Template, item.ValidDate);
						if (buffer != null)
						{
							buffer.Start(player);
							if (place != -1)
							{
								player.OnUsingItem(item.TemplateID);
								player.PropBag.RemoveItem(item, eItemRemoveType.Use);
							}
						}
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg, new object[0]));
					}
					else
					{
						if (item.IsValidItem())
						{
							player.AddGpDirect(item.Template.Property2);
							player.OnUsingItem(item.TemplateID);
							player.RemoveItem(item, eItemRemoveType.Use);
							msg2 = "GPDanUser.Success";
						}
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg2, new object[]
						{
							item.Template.Property2
						}));
					}
				}
				result = 0;
			}
			return result;
		}
	}
}
