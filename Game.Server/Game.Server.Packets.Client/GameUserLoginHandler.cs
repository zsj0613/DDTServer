using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(81, "进入游戏")]
	public class GameUserLoginHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			bool isInvite = packet.ReadBoolean();
			int hallType = packet.ReadInt();
			int type = packet.ReadInt();
			int roomId = -1;
			string pwd = null;
			int num = type;
			if (num == -1)
			{
				roomId = packet.ReadInt();
				pwd = packet.ReadString();
			}
			RoomMgr.EnterRoom(player, roomId, pwd, type, hallType, isInvite);
			return 0;
		}
	}
}
