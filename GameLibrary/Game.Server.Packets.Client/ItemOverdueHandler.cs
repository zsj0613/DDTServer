using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(77, "物品过期")]
	public class ItemOverdueHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.CurrentRoom != null && player.CurrentRoom.IsPlaying)
			{
				result = 0;
			}
			else
			{
				int bagType = (int)packet.ReadByte();
				int index = packet.ReadInt();
				try
				{
					PlayerInventory bag = player.GetInventory((eBageType)bagType);
					ItemInfo item = bag.GetItemAt(index);
					if (item != null && !item.IsValidItem())
					{
						if (bagType == 0 && index <= 30)
						{
							int place = bag.FindFirstEmptySlot();
							if (place == -1 || !bag.MoveItem(item.Place, place, item.Count))
							{
								player.SendItemToMail(item, LanguageMgr.GetTranslation("ItemOverdueHandler.Content", new object[0]), LanguageMgr.GetTranslation("ItemOverdueHandler.Title", new object[0]), eMailType.ItemOverdue);
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
							}
						}
						else
						{
							bag.UpdateItem(item);
						}
					}
				}
				catch
				{
				}
				finally
				{
					player.MainBag.UpdatePlayerProperties();
				}
				result = 0;
			}
			return result;
		}
	}
}
