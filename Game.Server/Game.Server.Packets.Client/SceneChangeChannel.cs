using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(24, "用户改变频道")]
	public class SceneChangeChannel : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			byte type = packet.ReadByte();
			return 0;
		}
	}
}
