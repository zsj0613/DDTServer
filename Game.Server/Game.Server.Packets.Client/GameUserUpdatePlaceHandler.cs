using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(100, "改变房间位置的状态")]
	public class GameUserUpdatePlaceHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null && player == player.CurrentRoom.Host)
			{
				RoomMgr.UpdateRoomPos(player.CurrentRoom, (int)packet.ReadByte(), packet.ReadBoolean());
			}
			return 0;
		}
	}
}
