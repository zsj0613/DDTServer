using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(190, "快速进入温泉房间")]
	public class SpaRoomQuickLoginHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (SpaRoomMgr.QuickLoginSpaRoom(player))
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}
	}
}
