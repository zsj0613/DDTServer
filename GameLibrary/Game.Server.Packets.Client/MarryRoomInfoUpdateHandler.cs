using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.SceneMarryRooms;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(253, "更新礼堂信息")]
	internal class MarryRoomInfoUpdateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.CurrentMarryRoom != null && player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.PlayerID)
			{
				string roomName = packet.ReadString();
				bool isPwdChanged = packet.ReadBoolean();
				string pwd = packet.ReadString();
				string introduction = packet.ReadString();
				MarryRoom room = player.CurrentMarryRoom;
				room.Info.RoomIntroduction = introduction;
				room.Info.Name = roomName;
				if (isPwdChanged)
				{
					room.Info.Pwd = pwd;
				}
				using (PlayerBussiness db = new PlayerBussiness())
				{
					db.UpdateMarryRoomInfo(room.Info);
				}
				room.SendMarryRoomInfoUpdateToScenePlayers(room);
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoomInfoUpdateHandler.Successed", new object[0]));
				result = 0;
			}
			else
			{
				result = 1;
			}
			return result;
		}
	}
}
