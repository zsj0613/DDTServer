using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;

using SqlDataProvider.Data;
using System;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(61, "物品转移")]
	public class ItemTransferHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			StringBuilder str = new StringBuilder();
			int mustGold = 10000;
			ItemInfo fromItem = player.HideBag.GetItemAt(0);
			ItemInfo toItem = player.HideBag.GetItemAt(1);
			int result;
			if (fromItem.StrengthenLevel < toItem.StrengthenLevel)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTransferHandler.NoCondition1", new object[0]));
				result = 0;
			}
			else
			{
				if (fromItem != null && toItem != null && fromItem.Template.CategoryID == toItem.Template.CategoryID && fromItem.Template.CategoryID < 10 && toItem.Count == 1 && fromItem.Count == 1 && fromItem.IsValidItem() && toItem.IsValidItem())
				{
					if (fromItem.StrengthenLevel == 0 && fromItem.DefendCompose == 0 && fromItem.LuckCompose == 0 && fromItem.AgilityCompose == 0 && fromItem.AttackCompose == 0)
					{
						result = 1;
						return result;
					}
					if (player.PlayerCharacter.Gold < mustGold)
					{
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nogold", new object[0]));
						result = 1;
						return result;
					}
					player.BeginChanges();
					player.MainBag.BeginChanges();
					try
					{
						player.RemoveGold(mustGold);
						str.Append(string.Concat(new object[]
						{
							fromItem.ItemID,
							":",
							fromItem.TemplateID,
							","
						}));
						str.Append(fromItem.StrengthenLevel + ",");
						str.Append(fromItem.AttackCompose + ",");
						str.Append(fromItem.DefendCompose + ",");
						str.Append(fromItem.LuckCompose + ",");
						str.Append(fromItem.AgilityCompose + ",");
						StrengthenGoodsInfo newGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(fromItem.StrengthenLevel, toItem.TemplateID);
						ItemInfo toNewItem = null;
						if (newGoodsInfo != null)
						{
							toNewItem = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(newGoodsInfo.GainEquip), 1, 115);
							if (toNewItem == null)
							{
								result = 0;
								return result;
							}
						}
						StrengthenGoodsInfo oldGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(fromItem.TemplateID);
						ItemInfo fromOldItem = null;
						if (oldGoodsInfo != null)
						{
							fromOldItem = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(oldGoodsInfo.OrginEquip), 1, 115);
							if (fromOldItem == null)
							{
								result = 0;
								return result;
							}
						}
						if (toNewItem != null)
						{
							StrengthenMgr.InheritProperty(toItem, toNewItem);
							toNewItem.StrengthenLevel = fromItem.StrengthenLevel;
							toNewItem.AttackCompose = fromItem.AttackCompose;
							toNewItem.DefendCompose = fromItem.DefendCompose;
							toNewItem.LuckCompose = fromItem.LuckCompose;
							toNewItem.AgilityCompose = fromItem.AgilityCompose;
							toNewItem.ValidDate = toItem.ValidDate;
							toNewItem.IsBinds = true;
							player.HideBag.RemoveItem(toItem, eItemRemoveType.ItemTransfer);
							player.HideBag.AddItemTo(toNewItem, 1);
							toItem = toNewItem;
						}
						else
						{
							toItem.StrengthenLevel = fromItem.StrengthenLevel;
							toItem.AttackCompose = fromItem.AttackCompose;
							toItem.DefendCompose = fromItem.DefendCompose;
							toItem.LuckCompose = fromItem.LuckCompose;
							toItem.AgilityCompose = fromItem.AgilityCompose;
							toItem.IsBinds = true;
						}
						if (fromOldItem != null)
						{
							fromItem.StrengthenLevel = 0;
							fromItem.AttackCompose = 0;
							fromItem.DefendCompose = 0;
							fromItem.LuckCompose = 0;
							fromItem.AgilityCompose = 0;
							StrengthenMgr.InheritProperty(fromItem, fromOldItem);
							fromOldItem.ValidDate = fromItem.ValidDate;
							player.HideBag.RemoveItem(fromItem, eItemRemoveType.ItemTransfer);
							player.HideBag.AddItemTo(fromOldItem, 0);
							fromOldItem.IsBinds = true;
							fromItem = fromOldItem;
						}
						else
						{
							fromItem.StrengthenLevel = 0;
							fromItem.AttackCompose = 0;
							fromItem.DefendCompose = 0;
							fromItem.LuckCompose = 0;
							fromItem.AgilityCompose = 0;
							fromItem.IsBinds = true;
						}
						player.HideBag.UpdateItem(fromItem);
						player.HideBag.UpdateItem(toItem);
						str.Append(string.Concat(new object[]
						{
							toItem.ItemID,
							":",
							toItem.TemplateID,
							","
						}));
						str.Append(toItem.StrengthenLevel + ",");
						str.Append(toItem.AttackCompose + ",");
						str.Append(toItem.DefendCompose + ",");
						str.Append(toItem.LuckCompose + ",");
						str.Append(toItem.AgilityCompose);
						pkg.WriteByte(0);
						player.Out.SendTCP(pkg);
						//LogMgr.LogItemAdd(player.PlayerCharacter.ID, LogItemType.Move, str.ToString(), toItem, null, 1);
					}
					finally
					{
						player.CommitChanges();
						player.MainBag.CommitChanges();
					}
				}
				else
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTransferHandler.NoCondition", new object[0]));
				}
				result = 0;
			}
			return result;
		}
	}
}
