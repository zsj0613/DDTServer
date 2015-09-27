using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(94, "游戏创建")]
	public class GameUserCreateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			byte roomType = packet.ReadByte();
			byte timeType = packet.ReadByte();
			string room = packet.ReadString();
			string pwd = packet.ReadString();
			RoomMgr.CreateRoom(player, room, pwd, (eRoomType)roomType, timeType);
			return 1;
		}
	}
}
