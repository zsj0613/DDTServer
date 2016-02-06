using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(11)]
	public class GunsaluteCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom != null)
			{
				int userID = packet.ReadInt();
				int templateID = packet.ReadInt();
				ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateID);
				if (template != null)
				{
					if (!player.CurrentMarryRoom.Info.IsGunsaluteUsed && (player.CurrentMarryRoom.Info.GroomID == player.PlayerCharacter.ID || player.CurrentMarryRoom.Info.BrideID == player.PlayerCharacter.ID))
					{
						player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
						player.CurrentMarryRoom.Info.IsGunsaluteUsed = true;
						GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", new object[]
						{
							player.PlayerCharacter.NickName
						}));
						player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(msg, player);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(player.CurrentMarryRoom.Info.GroomID, true, player.CurrentMarryRoom.Info);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(player.CurrentMarryRoom.Info.BrideID, true, player.CurrentMarryRoom.Info);
						using (PlayerBussiness db = new PlayerBussiness())
						{
							db.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
						}
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
