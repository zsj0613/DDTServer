using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(169, "玩家退出温泉房间")]
	public class UserExitSpaRoom : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.CurrentSpaRoom != null)
			{
				player.CurrentSpaRoom.RemovePlayer(player);
				result = 0;
			}
			else
			{
				result = 1;
			}
			return result;
		}
	}
}
