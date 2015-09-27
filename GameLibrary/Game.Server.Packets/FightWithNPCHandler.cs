using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using System;
namespace Game.Server.Packets
{
	[PacketHandler(50, "撮合NPC")]
	public class FightWithNPCHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			return 0;
		}
	}
}
