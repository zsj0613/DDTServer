using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(47, "解除物品")]
	public class UserUnchainItemHandler : AbstractPlayerPacketHandler
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
				int start = packet.ReadInt();
				int place = player.MainBag.FindFirstEmptySlot(31);
				player.MainBag.MoveItem(start, place, 0);
				result = 0;
			}
			return result;
		}
	}
}
