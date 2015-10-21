using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;

using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(57, "赠送物品")]
	public class GoodsPresentHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int totalMoney = 0;
			eMessageType eMsg = eMessageType.Normal;
			string msg = "GoodsPresentHandler.Success";
			GSPacketIn pkg = new GSPacketIn(57);
			string content = packet.ReadString();
			string nickName = packet.ReadString();
			List<ItemInfo> buyItems = new List<ItemInfo>();
			List<ItemInfo> canNotBuyItems = new List<ItemInfo>();
			Dictionary<int, int> changeLimitShopsID = new Dictionary<int, int>();
			StringBuilder allItemsStr = new StringBuilder();
			StringBuilder allTypesStr = new StringBuilder();
			bool buyResult = false;
			int result;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 0;
			}
			else
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					PlayerInfo receiver = db.GetUserSingleByNickName(nickName);
					if (receiver != null)
					{
						int count = packet.ReadInt();
						for (int i = 0; i < count; i++)
						{
							int goodsId = packet.ReadInt();
							int type = packet.ReadInt();
							string color = packet.ReadString();
							string skin = packet.ReadString();
							int gold = 0;
							int money = 0;
							int offer = 0;
							int gifttoken = 0;
							ShopItemInfo shopItem = ShopMgr.GetShopItemInfoById(goodsId);
							if (shopItem != null && shopItem.ShopID != 21 && shopItem.ShopID != 22)
							{
								List<int> tempInfo = ShopMgr.GetShopItemBuyConditions(shopItem, type, ref gold, ref money, ref offer, ref gifttoken);
								if (gold <= 0 && offer <= 0 && gifttoken <= 0 && tempInfo.Count <= 0)
								{
									ItemInfo item = ShopMgr.CreateItem(shopItem, 102, type, color, skin, false);
									int limitCount = ShopMgr.GetLimitCountByID(goodsId);
									if (limitCount == -1 || limitCount > 0 || !GameProperties.LimitShopState)
									{
										if (player.PlayerCharacter.Money >= money + totalMoney)
										{
											totalMoney += money;
											allTypesStr.Append(type).Append(",");
											allItemsStr.Append(item.TemplateID).Append(",");
											buyItems.Add(item);
											if (GameProperties.LimitShopState && limitCount != -1)
											{
												ShopMgr.SubtractShopLimit(goodsId);
												this.Notice(player, goodsId, item.Template.Name);
												if (!changeLimitShopsID.ContainsKey(goodsId))
												{
													changeLimitShopsID.Add(goodsId, ShopMgr.GetLimitCountByID(goodsId));
												}
												else
												{
													changeLimitShopsID[goodsId] = ShopMgr.GetLimitCountByID(goodsId);
												}
											}
											//LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Present, player.PlayerCharacter.ID, money, gold, gifttoken, offer, "", item.TemplateID, 2, item.Count);
										}
										else
										{
											canNotBuyItems.Add(item);
										}
									}
								}
							}
						}
						if (buyItems.Count == 0)
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.Failed", new object[0]));
							result = 1;
							return result;
						}
						player.RemoveMoney(totalMoney, LogMoneyType.Shop, LogMoneyType.Shop_Present);
						int annexIndex = 0;
						MailInfo message = new MailInfo();
						StringBuilder annexRemark = new StringBuilder();
						annexRemark.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
						player.StackItem(ref buyItems);
						player.OnPlayerGoodsPresent(buyItems.Count);
						foreach (ItemInfo item in buyItems)
						{
						//	ItemInfo item;
							db.AddGoods(item);
							annexIndex++;
							annexRemark.Append(annexIndex);
							annexRemark.Append("、");
							annexRemark.Append(item.Template.Name);
							annexRemark.Append("x");
							annexRemark.Append(item.Count);
							annexRemark.Append(";");
							switch (annexIndex)
							{
							case 1:
								message.Annex1 = item.ItemID.ToString();
								message.Annex1Name = item.Template.Name;
								break;
							case 2:
								message.Annex2 = item.ItemID.ToString();
								message.Annex2Name = item.Template.Name;
								break;
							case 3:
								message.Annex3 = item.ItemID.ToString();
								message.Annex3Name = item.Template.Name;
								break;
							case 4:
								message.Annex4 = item.ItemID.ToString();
								message.Annex4Name = item.Template.Name;
								break;
							case 5:
								message.Annex5 = item.ItemID.ToString();
								message.Annex5Name = item.Template.Name;
								break;
							}
							if (annexIndex == 5)
							{
								annexIndex = 0;
								message.AnnexRemark = annexRemark.ToString();
								annexRemark.Remove(0, annexRemark.Length);
								annexRemark.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark", new object[0]));
								message.Content = content;
								message.Gold = 0;
								message.Money = 0;
								message.GiftToken = 0;
								message.Receiver = receiver.NickName;
								message.ReceiverID = receiver.ID;
								message.Sender = player.PlayerCharacter.NickName;
								message.SenderID = player.PlayerCharacter.ID;
								message.Title = message.Sender + LanguageMgr.GetTranslation("GoodsPresentHandler.Content", new object[0]) + message.Annex1Name + "]";
								message.Type = 10;
								db.SendMail(message);
								message.Revert();
							}
						}
						if (annexIndex > 0)
						{
							message.AnnexRemark = annexRemark.ToString();
							message.Content = content;
							message.Gold = 0;
							message.Money = 0;
							message.GiftToken = 0;
							message.Receiver = receiver.NickName;
							message.ReceiverID = receiver.ID;
							message.Sender = player.PlayerCharacter.NickName;
							message.SenderID = player.PlayerCharacter.ID;
							message.Title = message.Sender + LanguageMgr.GetTranslation("GoodsPresentHandler.Content", new object[0]) + message.Annex1Name + "]";
							message.Type = 10;
							db.SendMail(message);
							message.Revert();
						}
						if (canNotBuyItems.Count > 0)
						{
							string title = LanguageMgr.GetTranslation("UserBuyItemHandler.ListTitle", new object[0]);
							StringBuilder Content = new StringBuilder();
							if (buyItems.Count > 0)
							{
								Content.Append(LanguageMgr.GetTranslation("UserBuyItemHandler.BuyList", new object[]
								{
									buyItems.Count
								}));
								for (int i = 0; i < buyItems.Count; i++)
								{
									Content.Append(string.Format("{0}{1}", (i == 0) ? "" : ";", buyItems[i].Template.Name));
								}
							}
							if (canNotBuyItems.Count > 0)
							{
								Content.Append(LanguageMgr.GetTranslation("UserBuyItemHandler.NoBuyList", new object[]
								{
									"\n"
								}));
								for (int j = 0; j < canNotBuyItems.Count; j++)
								{
									Content.Append(string.Format("{0}{1}", (j == 0) ? "" : ";", canNotBuyItems[j].Template.Name));
								}
							}
							if (player.SendMail(Content.ToString(), title, null, eMailType.BuyItem))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
							}
						}
						player.OnPaid(totalMoney, 0, 0, 0, "");
						player.Out.SendMailResponse(receiver.ID, eMailRespose.Receiver);
						player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Send);
						buyResult = (buyItems.Count == count);
					}
					else
					{
						buyResult = false;
						eMsg = eMessageType.ERROR;
						msg = "GoodsPresentHandler.NoUser";
					}
				}
				if (GameProperties.LimitShopState)
				{
					this.UpdateLimitShopCount(player, changeLimitShopsID);
				}
				pkg.WriteBoolean(buyResult);
				player.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg, new object[0]));
				player.Out.SendTCP(pkg);
				result = 0;
			}
			return result;
		}
		private void UpdateLimitShopCount(GamePlayer player, Dictionary<int, int> changeLimitShopID)
		{
			GSPacketIn pkg = new GSPacketIn(168);
			pkg.WriteInt(changeLimitShopID.Count);
			foreach (int key in changeLimitShopID.Keys)
			{
				pkg.WriteInt(key);
				pkg.WriteInt(changeLimitShopID[key]);
			}
			player.SendTCP(pkg);
			GameServer.Instance.LoginServer.SendPacket(pkg);
		}
		private void Notice(GamePlayer player, int id, string name)
		{
			int rate = ShopMgr.GetLimitCountByID(id) * 100 / ShopMgr.GetLimitMax(id);
			if (rate == 70 || rate == 50 || rate == 30 || rate == 10)
			{
				GSPacketIn pkg = new GSPacketIn(10);
				pkg.WriteInt(2);
				pkg.WriteString(LanguageMgr.GetTranslation("UserBuyLimitItemNotice.Content", new object[]
				{
					name,
					ShopMgr.GetLimitCountByID(id)
				}));
				GameServer.Instance.LoginServer.SendPacket(pkg);
				GamePlayer[] players = WorldMgr.GetAllPlayers();
				GamePlayer[] array = players;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p != player)
					{
						p.Out.SendTCP(pkg);
					}
				}
			}
		}
	}
}
