using Game.Base.Packets;
using Lsj.Util.Logs;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler(8, "客户端日记")]
    public class ClientErrorLog : IPacketHandler
    {
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			
			return 0;
		}
	}
}
