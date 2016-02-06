using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(249, "礼堂数据")]
	public class MarryDataHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom != null)
			{
				player.CurrentMarryRoom.ProcessData(player, packet);
			}
			return 0;
		}
	}
}
