using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(49, "改变物品位置")]
	public class UserChangeItemPlaceHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			eBageType bagType = (eBageType)packet.ReadByte();
			int place = packet.ReadInt();
			eBageType tobagType = (eBageType)packet.ReadByte();
			int result;
			if (tobagType == eBageType.TempBag)
			{
				GameServer.log.Error("User want to put item into temp bag!");
				result = 0;
			}
			else
			{
				int toplace = packet.ReadInt();
				int count = packet.ReadInt();
				PlayerInventory bag = player.GetInventory(bagType);
				PlayerInventory tobag = player.GetInventory(tobagType);
				if (tobagType == eBageType.TempBag)
				{
					result = 0;
				}
				else
				{
					bag.BeginChanges();
					tobag.BeginChanges();
					try
					{
						if (place != -1)
						{
							ItemInfo item = bag.GetItemAt(place);
							if (tobagType == eBageType.Bank)
							{
								ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
								if (info != null)
								{
									tobag.Capalility = info.StoreLevel * 10;
								}
							}
							if (toplace == -1)
							{
								if (tobag.StackItemToAnother(item) || tobag.AddItem(item))
								{
									bag.TakeOutItem(item);
								}
								else
								{
									player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
								}
							}
							else
							{
								if (bagType == tobagType)
								{
									bag.MoveItem(place, toplace, count);
								}
								else
								{
									if (bagType == eBageType.HideBag)
									{
										this.MoveFromHide(player, bag, item, toplace, tobag, count);
									}
									else
									{
										if (tobagType == eBageType.HideBag)
										{
											this.MoveToHide(player, bag, item, toplace, tobag, count);
										}
										else
										{
											if (bagType == eBageType.Bank)
											{
												UserChangeItemPlaceHandler.MoveFromBank(player, place, toplace, bag, tobag, item);
											}
											else
											{
												if (tobagType == eBageType.Bank)
												{
													UserChangeItemPlaceHandler.MoveToBank(place, toplace, bag, tobag, item);
												}
												else
												{
													if (tobag.AddItemTo(item, toplace))
													{
														bag.TakeOutItem(item);
													}
												}
											}
										}
									}
								}
							}
						}
						else
						{
							if (toplace != -1)
							{
								bag.RemoveItemAt(toplace, eItemRemoveType.Delete);
							}
						}
					}
					finally
					{
						bag.CommitChanges();
						tobag.CommitChanges();
					}
					result = 0;
				}
			}
			return result;
		}
		private static void MoveFromBank(GamePlayer player, int place, int toplace, PlayerInventory bag, PlayerInventory tobag, ItemInfo item)
		{
			if (item != null)
			{
				PlayerInventory tb = player.GetItemInventory(item.Template);
				if (tb == tobag)
				{
					ItemInfo toitem = tb.GetItemAt(toplace);
					if (toitem == null)
					{
						if (tb.AddItemTo(item, toplace))
						{
							bag.TakeOutItem(item);
						}
					}
					else
					{
						if (item.CanStackedTo(toitem) && item.Count + toitem.Count <= item.Template.MaxCount)
						{
							if (tb.AddCountToStack(toitem, item.Count))
							{
								bag.RemoveItem(item, eItemRemoveType.Stack);
							}
						}
						else
						{
							tb.TakeOutItem(toitem);
							bag.TakeOutItem(item);
							tb.AddItemTo(item, toplace);
							bag.AddItemTo(toitem, place);
						}
					}
				}
				else
				{
					if (tb.AddItem(item))
					{
						bag.TakeOutItem(item);
					}
				}
			}
		}
		private static void MoveToBank(int place, int toplace, PlayerInventory bag, PlayerInventory bank, ItemInfo item)
		{
			if (bag != null && item != null && bag != null)
			{
				ItemInfo toitem = bank.GetItemAt(toplace);
				if (toitem != null)
				{
					if (item.CanStackedTo(toitem) && item.Count + toitem.Count <= item.Template.MaxCount)
					{
						if (bank.AddCountToStack(toitem, item.Count))
						{
							bag.RemoveItem(item, eItemRemoveType.Stack);
						}
					}
					else
					{
						if (toitem.Template.BagType == (eBageType)bag.BagType)
						{
							bag.TakeOutItem(item);
							bank.TakeOutItem(toitem);
							bag.AddItemTo(toitem, place);
							bank.AddItemTo(item, toplace);
						}
					}
				}
				else
				{
					if (bank.AddItemTo(item, toplace))
					{
						bag.TakeOutItem(item);
					}
				}
			}
		}
		public void MoveToHide(GamePlayer player, PlayerInventory bag, ItemInfo item, int toSlot, PlayerInventory hideBag, int count)
		{
			if (player != null && bag != null && item != null && hideBag != null)
			{
				int oldplace = item.Place;
				ItemInfo toItem = hideBag.GetItemAt(toSlot);
				if (toItem != null)
				{
					if (toItem.CanStackedTo(item))
					{
						return;
					}
					if (item.Count == 1 && item.BagType == toItem.BagType)
					{
						bag.TakeOutItem(item);
						hideBag.TakeOutItem(toItem);
						bag.AddItemTo(toItem, oldplace);
						hideBag.AddItemTo(item, toSlot);
						return;
					}
					string key = string.Format("temp_place_{0}", toItem.ItemID);
					PlayerInventory tb = player.GetItemInventory(toItem.Template);
					if (player.TempProperties.ContainsKey(key) && tb.BagType == 0)
					{
						int tempSlot = (int)player.TempProperties[key];
						player.TempProperties.Remove(key);
						if (tb.AddItemTo(toItem, tempSlot))
						{
							hideBag.TakeOutItem(toItem);
						}
					}
					else
					{
						if (tb.StackItemToAnother(toItem))
						{
							hideBag.RemoveItem(toItem, eItemRemoveType.Stack);
						}
						else
						{
							if (tb.AddItem(toItem))
							{
								hideBag.TakeOutItem(toItem);
							}
							else
							{
								player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]));
							}
						}
					}
				}
				if (hideBag.IsEmpty(toSlot))
				{
					if (item.Count == 1)
					{
						if (hideBag.AddItemTo(item, toSlot))
						{
							bag.TakeOutItem(item);
							if (item.Template.BagType == eBageType.MainBag)
							{
								string key = string.Format("temp_place_{0}", item.ItemID);
								if (player.TempProperties.ContainsKey(key))
								{
									player.TempProperties[key] = oldplace;
								}
								else
								{
									player.TempProperties.Add(key, oldplace);
								}
							}
						}
					}
					else
					{
						ItemInfo newItem = item.Clone();
						newItem.Count = 1;
						if (bag.RemoveCountFromStack(item, 1, eItemRemoveType.Stack))
						{
							if (!hideBag.AddItemTo(newItem, toSlot))
							{
								bag.AddCountToStack(item, 1);
							}
						}
					}
				}
			}
		}
		public void MoveFromHide(GamePlayer player, PlayerInventory hideBag, ItemInfo item, int toSlot, PlayerInventory bag, int count)
		{
			if (player != null && item != null && hideBag != null && bag != null)
			{
				if (item.Template.BagType == (eBageType)bag.BagType)
				{
					if (toSlot < bag.BeginSlot || toSlot > bag.Capalility)
					{
						if (bag.StackItemToAnother(item))
						{
							hideBag.RemoveItem(item, eItemRemoveType.Stack);
							return;
						}
						string key = string.Format("temp_place_{0}", item.ItemID);
						if (player.TempProperties.ContainsKey(key))
						{
							toSlot = (int)hideBag.Player.TempProperties[key];
							hideBag.Player.TempProperties.Remove(key);
						}
						else
						{
							toSlot = bag.FindFirstEmptySlot();
						}
					}
					if (bag.AddItemTo(item, toSlot))
					{
						hideBag.TakeOutItem(item);
					}
					else
					{
						hideBag.SaveToDatabase();
						player.SendItemToMail(item, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full", new object[0]), eMailType.ItemOverdue);
						player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
			}
		}
	}
}
