using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(15, "New User Answer Question")]
	public class UserAnswerHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			if (id == 7)
			{
			}
			client.Player.UpdateAnswerSite(id);
			return 1;
		}
	}
}
