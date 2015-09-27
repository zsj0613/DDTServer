using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(102, "用户选队")]
	public class GameUserTeamHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.CurrentRoom == null || player.CurrentRoom.RoomType == eRoomType.Match)
			{
				result = 0;
			}
			else
			{
				RoomMgr.SwitchTeam(player);
				result = 0;
			}
			return result;
		}
	}
}
