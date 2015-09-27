using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(16, "Player enter scene.")]
	public class UserEnterSceneHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int hallType = packet.ReadInt();
			RoomMgr.EnterWaitingRoom(player, hallType);
			return 1;
		}
	}
}
