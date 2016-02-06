using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(109, "玩家刷新房间列表")]
	public class UserUpdateRoomListHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom == null)
			{
				int hallType = packet.ReadInt();
				int condition = packet.ReadInt();
				Random rand = new Random();
				BaseRoom[] rooms = RoomMgr.Rooms;
				List<BaseRoom> list = RoomMgr.GetWaitingRoom(hallType, condition, 9, 0);
				player.Out.SendUpdateRoomList(list);
			}
			return 0;
		}
	}
}
