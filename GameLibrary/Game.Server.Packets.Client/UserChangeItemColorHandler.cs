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
	[PacketHandler(182, "改变物品颜色")]
	public class UserChangeItemColorHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			eMessageType eMsg = eMessageType.Normal;
			string msg = "UserChangeItemColorHandler.Success";
			int Card_bagType = packet.ReadInt();
			int Card_place = packet.ReadInt();
			int bagType = packet.ReadInt();
			int place = packet.ReadInt();
			string color = packet.ReadString();
			string skin = packet.ReadString();
			int templateID = packet.ReadInt();
			ItemInfo item = player.MainBag.GetItemAt(place);
			ItemInfo card = player.PropBag.GetItemAt(Card_place);
			if (item != null)
			{
				player.BeginChanges();
				try
				{
					bool changed = false;
					if (card != null && card.IsValidItem())
					{
						player.PropBag.RemoveItem(card, eItemRemoveType.Use);
						changed = true;
					}
					else
					{
						ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateID);
						List<ShopItemInfo> Template = ShopMgr.FindShopbyTemplatID(templateID);
						int Money = 0;
						for (int i = 0; i < Template.Count; i++)
						{
							if (Template[i].APrice1 == -1 && Template[i].AValue1 != 0)
							{
								Money = Template[i].AValue1;
							}
						}
						if (Money <= player.PlayerCharacter.Money)
						{
							player.RemoveMoney(Money, LogMoneyType.Item, LogMoneyType.Item_Color);
							changed = true;
						}
					}
					if (changed)
					{
						item.Color = ((color == null) ? "" : color);
						item.Skin = ((skin == null) ? "" : skin);
						player.MainBag.UpdateItem(item);
					}
				}
				finally
				{
					player.CommitChanges();
				}
			}
			player.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg, new object[0]));
			return 0;
		}
	}
}
