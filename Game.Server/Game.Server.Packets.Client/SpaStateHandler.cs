using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.SpaRooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(201, "进入房间场景切换")]
	public class SpaStateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int stateID = packet.ReadInt();
			SpaRoom room = player.CurrentSpaRoom;
			switch (stateID)
			{
			case 0:
				if (room != null)
				{
					GamePlayer[] list = player.CurrentSpaRoom.GetAllPlayers();
					player.Out.SendSpaRoomAddGuest(player);
					GamePlayer[] array = list;
					for (int i = 0; i < array.Length; i++)
					{
						GamePlayer p = array[i];
						if (p != player)
						{
							p.Out.SendSpaRoomAddGuest(player);
							player.Out.SendSpaRoomAddGuest(p);
						}
					}
					if (room.Spa_Room_Info.RoomType != 1 && room.Spa_Room_Info.RoomType != 2)
					{
						if (room.Spa_Room_Info.PlayerID == player.PlayerCharacter.ID && room.RoomLeftMin <= room.RoomContinueRemindTime)
						{
							GSPacketIn pkg2 = new GSPacketIn(191, player.PlayerCharacter.ID);
							pkg2.WriteByte(3);
							player.Out.SendTCP(pkg2);
						}
					}
				}
				break;
			}
			return 0;
		}
	}
}
