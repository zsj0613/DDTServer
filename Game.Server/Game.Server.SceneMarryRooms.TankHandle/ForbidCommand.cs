using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(8)]
	public class ForbidCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom != null)
			{
				if (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID || player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.BrideID)
				{
					int userID = packet.ReadInt();
					if (userID != player.CurrentMarryRoom.Info.BrideID && userID != player.CurrentMarryRoom.Info.GroomID)
					{
						player.CurrentMarryRoom.KickPlayerByUserID(player, userID);
						player.CurrentMarryRoom.SetUserForbid(userID);
					}
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
