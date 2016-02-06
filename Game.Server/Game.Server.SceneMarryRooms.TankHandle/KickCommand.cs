using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(7)]
	public class KickCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom != null && player.CurrentMarryRoom.RoomState == eRoomState.FREE)
			{
				if (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID || player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.BrideID)
				{
					int userID = packet.ReadInt();
					player.CurrentMarryRoom.KickPlayerByUserID(player, userID);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
