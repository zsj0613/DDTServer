using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(244, "玩家退出礼堂")]
	public class UserLeaveMarryRoom : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.IsInMarryRoom)
			{
				player.CurrentMarryRoom.RemovePlayer(player);
			}
			return 0;
		}
	}
}
