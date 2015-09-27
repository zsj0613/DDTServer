using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(242, "进入礼堂")]
	public class MarryRoomLoginHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			MarryRoom room = null;
			string msg = "";
			int id = packet.ReadInt();
			string pwd = packet.ReadString();
			int sceneID = packet.ReadInt();
			int result;
			if (id != 0)
			{
				room = MarryRoomMgr.GetMarryRoombyID(id, (pwd == null) ? "" : pwd, ref msg);
			}
			else
			{
				if (player.PlayerCharacter.IsCreatedMarryRoom)
				{
					MarryRoom[] rooms = MarryRoomMgr.GetAllMarryRoom();
					MarryRoom[] array = rooms;
					for (int i = 0; i < array.Length; i++)
					{
						MarryRoom r = array[i];
						if (r.Info.GroomID == player.PlayerCharacter.ID || r.Info.BrideID == player.PlayerCharacter.ID)
						{
							room = r;
							break;
						}
					}
				}
				if (room == null && player.PlayerCharacter.SelfMarryRoomID != 0)
				{
					player.Out.SendMarryRoomLogin(player, false);
					MarryRoomInfo info = null;
					using (PlayerBussiness db = new PlayerBussiness())
					{
						info = db.GetMarryRoomInfoSingle(player.PlayerCharacter.SelfMarryRoomID);
					}
					if (info != null)
					{
						player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.RoomExist", new object[]
						{
							info.ServerID,
							player.PlayerCharacter.SelfMarryRoomID
						}));
						result = 0;
						return result;
					}
				}
			}
			if (room != null)
			{
				if (room.CheckUserForbid(player.PlayerCharacter.ID))
				{
					player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MarryRoomLoginHandler.Forbid", new object[0]));
					player.Out.SendMarryRoomLogin(player, false);
					result = 1;
					return result;
				}
				if (room.RoomState == eRoomState.FREE)
				{
					if (room.AddPlayer(player))
					{
						player.MarryMap = sceneID;
						GSPacketIn pkg = player.Out.SendMarryRoomLogin(player, true);
						room.SendMarryRoomInfoUpdateToScenePlayers(room);
						result = 0;
						return result;
					}
				}
				else
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.AlreadyBegin", new object[0]));
				}
				player.Out.SendMarryRoomLogin(player, false);
			}
			else
			{
				player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(string.IsNullOrEmpty(msg) ? "MarryRoomLoginHandler.Failed" : msg, new object[0]));
				player.Out.SendMarryRoomLogin(player, false);
			}
			result = 1;
			return result;
		}
	}
}
