using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SpaRooms;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(202, "进入温泉房间")]
	public class SpaRoomLoginHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			SpaRoom room = null;
			string msg = "";
			int id = packet.ReadInt();
			string pwd = packet.ReadString();
			if (id != 0)
			{
				room = SpaRoomMgr.GetSpaRoombyID(id, (pwd == null) ? "" : pwd, ref msg);
			}
			int result;
			if (room != null)
			{
				if (room.Spa_Room_Info.RoomType == 1 && player.PlayerCharacter.SpaPubGoldRoomLimit <= 0)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed", new object[0]));
					result = 0;
					return result;
				}
				if (room.Spa_Room_Info.RoomType == 2 && player.PlayerCharacter.SpaPubMoneyRoomLimit <= 0)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed", new object[0]));
					result = 0;
					return result;
				}
			}
			if (SpaRoomMgr.LoginSpaRoom(player, room, msg))
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
