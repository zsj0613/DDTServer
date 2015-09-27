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
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(44, "购买物品")]
	public class UserBuyItemHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int totalGold = 0;
			int totalMoney = 0;
			int totalOffer = 0;
			int totalGifttoken = 0;
			int buyItemCount = 0;
			eMessageType eMsg = eMessageType.Normal;
			string msg = "UserBuyItemHandler.Success";
			GSPacketIn pkg = new GSPacketIn(44);
			List<ItemInfo> buyItems = new List<ItemInfo>();
			List<ItemInfo> canNotBuyItems = new List<ItemInfo>();
			List<int> needitemsinfo = new List<int>();
			Dictionary<int, int> changeLimitShopsID = new Dictionary<int, int>();
			List<bool> dresses = new List<bool>();
			List<int> places = new List<int>();
			List<ItemJoinShopInfo> log = new List<ItemJoinShopInfo>();
			Dictionary<int, int> playerPayGoods = new Dictionary<int, int>();
			int result2;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result2 = 1;
			}
			else
			{
				ConsortiaInfo consotia = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
				int count = packet.ReadInt();
				for (int i = 0; i < count; i++)
				{
					int goodsId = packet.ReadInt();
					int type = packet.ReadInt();
					string color = packet.ReadString();
					bool isDress = packet.ReadBoolean();
					string skin = packet.ReadString();
					int place = packet.ReadInt();
					bool isBind = false;
					int gold = 0;
					int money = 0;
					int offer = 0;
					int gifttoken = 0;
					ShopItemInfo shopItem = ShopMgr.GetShopItemInfoById(goodsId);
					if (shopItem != null && shopItem.ShopID != 21 && shopItem.ShopID != 22)
					{
						if (shopItem.ShopID == 2 || !ShopMgr.CanBuy(shopItem.ShopID, (consotia == null) ? 1 : consotia.ShopLevel, ref isBind, player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.Riches))
						{
							player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailbyPermission", new object[0]));
							result2 = 1;
							return result2;
						}
						List<int> needsItems = ShopMgr.GetShopItemBuyConditions(shopItem, type, ref gold, ref money, ref offer, ref gifttoken);
						for (int j = 0; j < needsItems.Count; j += 2)
						{
							if (!playerPayGoods.ContainsKey(needsItems[j]))
							{
								playerPayGoods.Add(needsItems[j], player.GetItemCount(needsItems[j]));
							}
						}
						bool result = true;
						string needsItemsLog = "";
						for (int j = 0; j < needsItems.Count; j += 2)
						{
							if (playerPayGoods.ContainsKey(needsItems[j]) && playerPayGoods[needsItems[j]] >= needsItems[j + 1])
							{
								foreach (int a in needsItems)
								{
									needitemsinfo.Add(a);
								}
								string text = needsItemsLog;
								needsItemsLog = string.Concat(new string[]
								{
									text,
									needsItems[j].ToString(),
									":",
									needsItems[j + 1].ToString(),
									"|"
								});
								Dictionary<int, int> dictionary;
								int key;
								(dictionary = playerPayGoods)[key = needsItems[j]] = dictionary[key] - needsItems[j + 1];
							}
							else
							{
								result = false;
							}
						}
						ItemInfo item = ShopMgr.CreateItem(shopItem, 102, type, color, skin, isBind);
						int limitCount = ShopMgr.GetLimitCountByID(goodsId);
						if (limitCount == -1 || limitCount > 0 || !GameProperties.LimitShopState)
						{
							if (result && player.PlayerCharacter.Gold >= gold + totalGold && player.PlayerCharacter.Money >= money + totalMoney && player.PlayerCharacter.Offer >= offer + totalOffer && player.PlayerCharacter.GiftToken >= gifttoken + totalGifttoken)
							{
								totalGold += gold;
								totalMoney += money;
								totalOffer += offer;
								totalGifttoken += gifttoken;
								buyItemCount++;
								buyItems.Add(item);
								dresses.Add(isDress);
								places.Add(place);
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
								log.Add(new ItemJoinShopInfo(item.TemplateID, item.Count, money, gold, gifttoken, offer, needsItemsLog));
							}
							else
							{
								canNotBuyItems.Add(item);
							}
						}
					}
				}
				if (buyItems.Count == 0)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.Failed", new object[0]));
					result2 = 1;
				}
				else
				{
					int buyFrom = packet.ReadInt();
					player.RemoveMoney(totalMoney, LogMoneyType.Shop, LogMoneyType.Shop_Buy);
					player.RemoveGold(totalGold);
					player.RemoveOffer(totalOffer);
					player.RemoveGiftToken(totalGifttoken);
					StringBuilder allPayItemsStr = new StringBuilder();
					for (int j = 0; j < needitemsinfo.Count; j += 2)
					{
						player.RemoveTemplate(needitemsinfo[j], needitemsinfo[j + 1], eItemRemoveType.Shopping);
						allPayItemsStr.Append(needitemsinfo[j]).Append(",");
					}
					foreach (ItemJoinShopInfo templog in log)
					{
						//LogMgr.LogMoneyAdd(LogMoneyType.Shop, LogMoneyType.Shop_Buy, player.PlayerCharacter.ID, templog.Moneys, templog.Gold, templog.GiftToken, templog.Offer, templog.OtherPay, templog.TemplateID, 4, templog.Data);
					}
					List<ItemInfo> lastItems = new List<ItemInfo>();
					player.StackItem(ref buyItems);
					for (int i = 0; i < buyItems.Count; i++)
					{
						switch (buyFrom)
						{
						case 0:
						case 3:
							if (player.AddItem(buyItems[i]))
							{
								if (dresses[i])
								{
									player.EquipItem(buyItems[i], places[i]);
									msg = "UserBuyItemHandler.Save";
								}
							}
							else
							{
								lastItems.Add(buyItems[i]);
							}
							break;
						case 1:
							if (!UserBuyItemHandler.AddStrengthItemsToHideBag(player, buyItems[i]))
							{
								lastItems.Add(buyItems[i]);
							}
							break;
						case 2:
							if (!UserBuyItemHandler.AddComposeItemsToHideBag(player, buyItems[i]))
							{
								lastItems.Add(buyItems[i]);
							}
							break;
						case 4:
							if (!UserBuyItemHandler.AddFusionItemsToHideBag(player, buyItems[i]))
							{
								lastItems.Add(buyItems[i]);
							}
							break;
						}
					}
					bool sentMail = false;
					if (lastItems.Count > 0)
					{
						string title = "";
						sentMail = player.SendItemsToMail(lastItems, "", title, eMailType.BuyItem);
						msg = "UserBuyItemHandler.Mail";
					}
					if (canNotBuyItems.Count > 0)
					{
						string title = LanguageMgr.GetTranslation("UserBuyItemHandler.ListTitle", new object[0]);
						StringBuilder content = new StringBuilder();
						if (buyItems.Count > 0)
						{
							content.Append(LanguageMgr.GetTranslation("UserBuyItemHandler.BuyList", new object[]
							{
								buyItems.Count
							}));
							for (int i = 0; i < buyItems.Count; i++)
							{
								content.Append(string.Format("{0}{1}", (i == 0) ? "" : ";", buyItems[i].Template.Name));
							}
						}
						if (canNotBuyItems.Count > 0)
						{
							content.Append(LanguageMgr.GetTranslation("UserBuyItemHandler.NoBuyList", new object[]
							{
								"\n"
							}));
							for (int j = 0; j < canNotBuyItems.Count; j++)
							{
								content.Append(string.Format("{0}{1}", (j == 0) ? "" : ";", canNotBuyItems[j].Template.Name));
							}
						}
						sentMail = player.SendMail(content.ToString(), title, null, eMailType.BuyItem);
					}
					if (sentMail)
					{
						player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
					player.OnPaid(totalMoney, totalGold, totalOffer, totalGifttoken, allPayItemsStr.ToString());
					int buyResult = 0;
					if (buyItemCount == count)
					{
						buyResult = (sentMail ? 2 : 1);
					}
					pkg.WriteInt(buyResult);
					pkg.WriteInt(buyFrom);
					player.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg, new object[0]));
					player.Out.SendTCP(pkg);
					player.SaveIntoDatabase();
					if (GameProperties.LimitShopState)
					{
						this.UpdateLimitShopCount(player, changeLimitShopsID);
					}
					result2 = 0;
				}
			}
			return result2;
		}
		private static bool AddFusionItemsToHideBag(GamePlayer player, ItemInfo item)
		{
			bool result;
			if (player.HideBag.IsEmpty(0))
			{
				if (item.Count > 1)
				{
					ItemInfo copyitem = item.Clone();
					copyitem.Count = 1;
					item.Count--;
					result = (player.HideBag.AddItemTo(copyitem, 0) && player.AddItem(item));
				}
				else
				{
					result = player.HideBag.AddItemTo(item, 0);
				}
			}
			else
			{
				result = player.AddItem(item);
			}
			return result;
		}
		private static bool AddComposeItemsToHideBag(GamePlayer player, ItemInfo item)
		{
			int place = -1;
			if (item.TemplateID >= 11001 && item.TemplateID <= 11016)
			{
				place = 2;
			}
			if (item.TemplateID == 11018)
			{
				place = 0;
			}
			bool result;
			if (place != -1 && player.HideBag.IsEmpty(place))
			{
				if (item.Count > 1)
				{
					ItemInfo copyitem = item.Clone();
					copyitem.Count = 1;
					item.Count--;
					result = (player.HideBag.AddItemTo(copyitem, place) && player.AddItem(item));
				}
				else
				{
					result = player.HideBag.AddItemTo(item, place);
				}
			}
			else
			{
				result = player.AddItem(item);
			}
			return result;
		}
		private static bool AddStrengthItemsToHideBag(GamePlayer player, ItemInfo item)
		{
			int place = -1;
			int templateID = item.TemplateID;
			switch (templateID)
			{
			case 11018:
				place = 4;
				break;
			case 11019:
				break;
			case 11020:
				place = 3;
				break;
			default:
				if (templateID == 11023)
				{
					for (int i = 0; i < 3; i++)
					{
						if (player.HideBag.IsEmpty(i) && item.Count >= 1)
						{
							ItemInfo copyitem = item.Clone();
							copyitem.Count = 1;
							player.HideBag.AddItemTo(copyitem, i);
							item.Count--;
						}
					}
					place = -1;
				}
				break;
			}
			bool result;
			if (item.Count <= 0)
			{
				result = true;
			}
			else
			{
				if (place != -1 && player.HideBag.IsEmpty(place))
				{
					if (item.Count > 1)
					{
						ItemInfo copyitem = item.Clone();
						copyitem.Count = 1;
						item.Count--;
						result = (player.HideBag.AddItemTo(copyitem, place) && player.AddItem(item));
					}
					else
					{
						result = player.HideBag.AddItemTo(item, place);
					}
				}
				else
				{
					result = player.AddItem(item);
				}
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
			float rate = (float)ShopMgr.GetLimitCountByID(id) / (float)ShopMgr.GetLimitMax(id);
			int index;
			if (rate <= 0.7f && rate > 0.5f)
			{
				index = 0;
			}
			else
			{
				if (rate <= 0.5f && rate > 0.3f)
				{
					index = 1;
				}
				else
				{
					if (rate <= 0.3f && rate > 0.1f)
					{
						index = 2;
					}
					else
					{
						if (rate > 0.1f || rate <= 0f)
						{
							return;
						}
						index = 3;
					}
				}
			}
			if (ShopMgr.GetIsNotice(id, index) != 0)
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
				ShopMgr.CloseNotice(id, index);
				GSPacketIn pkg2 = new GSPacketIn(204);
				pkg2.WriteInt(id);
				pkg2.WriteInt(index);
				GameServer.Instance.LoginServer.SendPacket(pkg2);
			}
		}
	}
}
