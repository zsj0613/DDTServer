using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(191, "温泉房间内命令")]
	public class SpaDataHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentSpaRoom != null)
			{
				player.CurrentSpaRoom.ProcessData(player, packet);
			}
			return 0;
		}
	}
}
