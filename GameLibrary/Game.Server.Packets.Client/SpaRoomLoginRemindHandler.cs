using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SpaRooms;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(212, "温泉房间扣费框弹出控制")]
	public class SpaRoomLoginRemindHandler : AbstractPlayerPacketHandler
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
				if (room.Count >= room.Spa_Room_Info.MaxCount)
				{
					player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("SpaRoom.Msg1", new object[0]));
				}
				else
				{
					if (room.Spa_Room_Info.RoomType == 1)
					{
						if (player.PlayerCharacter.SpaPubGoldRoomLimit <= 0)
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed", new object[0]));
							result = 0;
							return result;
						}
						if (player.PlayerCharacter.IsInSpaPubGoldToday)
						{
							if (SpaRoomMgr.LoginSpaRoom(player, room, msg))
							{
								result = 1;
								return result;
							}
							result = 0;
							return result;
						}
					}
					else
					{
						if (room.Spa_Room_Info.RoomType != 2)
						{
							result = 1;
							return result;
						}
						if (player.PlayerCharacter.SpaPubMoneyRoomLimit <= 0)
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed", new object[0]));
							result = 0;
							return result;
						}
						if (player.PlayerCharacter.IsInSpaPubMoneyToday)
						{
							if (SpaRoomMgr.LoginSpaRoom(player, room, msg))
							{
								result = 1;
								return result;
							}
							result = 0;
							return result;
						}
					}
					player.Out.SendSpaRoomLoginRemind(room);
				}
			}
			result = 0;
			return result;
		}
	}
}
