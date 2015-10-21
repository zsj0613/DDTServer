using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using System;
using System.Reflection;

namespace Game.Server.SpaRooms.CommandHandle
{
	[SpaCommandAttbute(6)]
	public class UpdateRoomInfoCommand : ISpaCommandHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public bool HandleCommand(GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentSpaRoom != null)
			{
				if (player.PlayerCharacter.ID == player.CurrentSpaRoom.Spa_Room_Info.PlayerID)
				{
					SpaRoom room = player.CurrentSpaRoom;
					if (room.Spa_Room_Info.RoomType != 1 && room.Spa_Room_Info.RoomType != 2)
					{
						room.Spa_Room_Info.RoomName = packet.ReadString();
						room.Spa_Room_Info.Pwd = packet.ReadString();
						room.Spa_Room_Info.RoomIntroduction = packet.ReadString();
						using (PlayerBussiness db = new PlayerBussiness())
						{
							db.UpdateSpaRoomInfo(room.Spa_Room_Info);
						}
						room.SendSpaRoomInfoUpdateToSpaScenePlayers(room);
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomInfoUpdateHandler.Successed", new object[0]));
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}
	}
}
