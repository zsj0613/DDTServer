using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(98, "踢出房间")]
	public class GameUserKickHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null && player == player.CurrentRoom.Host)
			{
				RoomMgr.KickPlayer(player.CurrentRoom, packet.ReadByte());
			}
			return 0;
		}
	}
}
