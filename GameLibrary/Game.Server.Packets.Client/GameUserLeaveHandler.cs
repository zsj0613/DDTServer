using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(83, "用户退出")]
	public class GameUserLeaveHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null)
			{
				RoomMgr.ExitRoom(player.CurrentRoom, player);
			}
			return 0;
		}
	}
}
