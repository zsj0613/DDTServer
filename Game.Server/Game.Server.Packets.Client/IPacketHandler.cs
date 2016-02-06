using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	public interface IPacketHandler
	{
		int HandlePacket(GameClient client, GSPacketIn packet);
	}
}
