using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(10)]
	public class Position : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom != null)
			{
				player.X = packet.ReadInt();
				player.Y = packet.ReadInt();
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
