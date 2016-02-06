using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(1)]
	public class MoveCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom != null && player.CurrentMarryRoom.RoomState == eRoomState.FREE)
			{
				player.X = packet.ReadInt();
				player.Y = packet.ReadInt();
				player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
