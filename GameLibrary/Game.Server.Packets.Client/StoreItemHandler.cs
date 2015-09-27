using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(79, "储存物品")]
	public class StoreItemHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.ConsortiaID == 0)
			{
				result = 1;
			}
			else
			{
				int bagType = (int)packet.ReadByte();
				int bagPlace = packet.ReadInt();
				int storePlace = packet.ReadInt();
				if (bagType == 0 && bagPlace < 31)
				{
					result = 1;
				}
				else
				{
					ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
					if (info != null)
					{
						PlayerInventory storeBag = player.StoreBag;
						PlayerInventory toBag = player.GetInventory((eBageType)bagType);
					}
					result = 0;
				}
			}
			return result;
		}
	}
}
