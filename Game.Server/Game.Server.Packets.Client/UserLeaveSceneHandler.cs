using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(21, "场景用户离开")]
	public class UserLeaveSceneHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			RoomMgr.ExitWaitingRoom(player);
			return 0;
		}
	}
}
