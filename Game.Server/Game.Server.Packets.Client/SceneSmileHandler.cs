using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(20, "用户场景表情")]
	public class SceneSmileHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			packet.ClientID = player.PlayerCharacter.ID;
			if (player.CurrentSpaRoom != null)
			{
				player.CurrentSpaRoom.SendToRoomPlayer(packet);
			}
			else
			{
				if (player.CurrentRoom != null)
				{
					if (player.CurrentRoom.Game != null)
					{
						player.CurrentRoom.Game.SendToAll(packet);
					}
					else
					{
						player.CurrentRoom.SendToAll(packet);
					}
				}
				else
				{
					if (player.CurrentMarryRoom != null)
					{
						player.CurrentMarryRoom.SendToAllForScene(packet, player.MarryMap);
					}
					else
					{
						RoomMgr.WaitingRoom.SendToALL(packet);
					}
				}
			}
			return 1;
		}
	}
}
