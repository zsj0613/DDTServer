using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.SpaRooms.CommandHandle
{
	[SpaCommandAttbute(10)]
	public class IsContinueNextDay : ISpaCommandHandler
	{
		public bool HandleCommand(GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentSpaRoom == null)
			{
				result = false;
			}
			else
			{
				SpaRoom room = player.CurrentSpaRoom;
				player.Spa_Day_Alter_Continue = packet.ReadBoolean();
				if (player.Spa_Day_Alter_Continue)
				{
					if (SpaRoomMgr.SpaPubRoomPay(player, room))
					{
						if (room.Spa_Room_Info.RoomType == 1)
						{
							player.UpdateIsInSpaPubGoldToday(true);
						}
						if (room.Spa_Room_Info.RoomType == 2)
						{
							player.UpdateIsInSpaPubMoneyToday(true);
						}
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					player.CurrentSpaRoom.RemovePlayerSpecial(player);
					result = true;
				}
			}
			return result;
		}
	}
}
