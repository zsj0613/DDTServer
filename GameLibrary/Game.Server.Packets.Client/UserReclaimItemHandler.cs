using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using Game.Language;
using System.Collections.Generic;
using Bussiness.Managers;

namespace Game.Server.Packets.Client
{
	[PacketHandler(127, "回收物品")]
	public class UserReclaimItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int result;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 0;
			}
			else
			{
				eBageType bag = (eBageType)packet.ReadByte();
				int place = packet.ReadInt();
				int count = packet.ReadInt();
				PlayerInventory bag_current = client.Player.GetInventory(bag);
				ItemInfo goods = bag_current.GetItemAt(place);
				if (goods != null && goods.Count == count && goods.Template.CanDelete)
				{
					if (goods.Template.ReclaimType == 0)
					{
						int goodsValue = 0;
						client.Player.AddGold(goodsValue);
						client.Player.RemoveItem(goods, eItemRemoveType.Reclaim);
						client.Player.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ExchangeMoneyToGoldHandler.success", new object[]
						{
							goodsValue
						}));
					}
					else
					{
						if (goods.Template.ReclaimType == 1)
						{
							int goodsValue = int.Parse(goods.Template.ReclaimValue) * count;
							if (goodsValue >= 0)
							{
								client.Player.AddGold(goodsValue);
								client.Player.RemoveItem(goods, eItemRemoveType.Reclaim);
								client.Player.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ExchangeMoneyToGoldHandler.success", new object[]
								{
									goodsValue
								}));
							}
							else
							{
								client.Player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserReclaimItemHandler.fail", new object[0]));
							}
						}
						else
						{
                            if (goods.Template.ReclaimType == 2)
                            {
                                int goodsValue = int.Parse(goods.Template.ReclaimValue) * count;
                                if (goodsValue >= 0)
                                {
                                    client.Player.AddGiftToken(goodsValue);
                                    client.Player.RemoveItem(goods, eItemRemoveType.Reclaim);
                                    client.Player.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ReclaimToGifttokenHandler.success", new object[]
                                    {
                                        goodsValue
                                    }));
                                }
                                else
                                {
                                    client.Player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserReclaimItemHandler.fail", new object[0]));
                                }
                            }
                            else
                            {
                                if (goods.Template.ReclaimType == 3)
                                {
                                    List<int> a = new List<int>();
                                    if (goods.Template.ReclaimValue.IndexOf(',') != -1)
                                    {
                                        string[] b = goods.Template.ReclaimValue.Split(',');
                                        foreach (var c in b)
                                        {
                                            a.Add(int.Parse(c));
                                        }
                                    }
                                    else
                                    {
                                        a.Add(int.Parse(goods.Template.ReclaimValue));
                                    }
                                    foreach (var d in a)
                                    {

                                        client.Player.AddItem(ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(d),1,0));
                                    }
                                    client.Player.RemoveItem(goods, eItemRemoveType.Reclaim);
                                    client.Player.SendMessage(eMessageType.ChatNormal, "回收成功！");
                                }
                            }
						}
					}
				}
				result = 0;
			}
			return result;
		}
	}
}
