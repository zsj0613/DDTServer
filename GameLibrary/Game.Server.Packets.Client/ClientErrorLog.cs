using Game.Base.Packets;
using log4net;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(8, "客户端日记")]
	public class ClientErrorLog : IPacketHandler
	{
		public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			ClientErrorLog.log.Error("client log:" + packet.ReadString());
			return 0;
		}
	}
}
