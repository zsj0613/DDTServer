using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(48, "出售商品")]
	public class UserSellItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			return 0;
		}
	}
}
