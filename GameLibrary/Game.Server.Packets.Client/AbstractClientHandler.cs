using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	public class AbstractClientHandler : IPacketHandler
	{
		public virtual int HandlePacket(GameClient client, GSPacketIn packet)
		{
			return 0;
		}
	}
}
