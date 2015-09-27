using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(137, "添加职务")]
	public class ConsortiaDutyAddHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer client, GSPacketIn packet)
		{
			return 0;
		}
	}
}
