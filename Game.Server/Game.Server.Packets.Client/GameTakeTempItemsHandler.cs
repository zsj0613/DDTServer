using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
	[PacketHandler(108, "选取")]
	public class GameTakeTempItemsHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			string message = string.Empty;
			bool result = false;
			int place = packet.ReadInt();
			if (place != -1)
			{
				ItemInfo item = player.TempBag.GetItemAt(place);
				if (item != null)
				{
					this.GetItem(player, item, ref message, ref result);
				}
			}
			else
			{
				List<ItemInfo> items = player.TempBag.GetItems();
				if (items.Count > 0)
				{
					foreach (ItemInfo item in items)
					{
						if (!this.GetItem(player, item, ref message, ref result))
						{
							break;
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(message))
			{
				player.Out.SendMessage(eMessageType.ERROR, message);
			}
			if (result)
			{
				packet.ClearContext();
				packet.WriteBoolean(true);
				player.Out.SendTCP(packet);
			}
			player.SaveIntoDatabase();
			return 0;
		}
		private bool GetItem(GamePlayer player, ItemInfo item, ref string message, ref bool result)
		{
			bool result2;
			if (item == null)
			{
				result2 = false;
			}
			else
			{
				PlayerInventory bag = player.GetItemInventory(item.Template);
				if (bag.StackItemToAnother(item) || bag.AddItem(item))
				{
					result = true;
					player.TempBag.TakeOutItem(item);
					result2 = true;
				}
				else
				{
					result = false;
					bag.UpdateChangedPlaces();
					message = LanguageMgr.GetTranslation(item.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("GameTakeTempItemsHandler.Msg", new object[0]);
					result2 = false;
				}
			}
			return result2;
		}
	}
}
