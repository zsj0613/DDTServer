using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(52, "装备物品")]
	public class UserChainItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			return 0;
		}
	}
}
