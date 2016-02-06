using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SpaRooms.CommandHandle
{
	[SpaCommandAttbute(1)]
	public class MoveCommand : ISpaCommandHandler
	{
		public bool HandleCommand(GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentSpaRoom != null)
			{
				string str = packet.ReadString();
				string[] allPoints = str.Split(new char[]
				{
					','
				});
				player.LastPosX = Convert.ToInt32(allPoints[allPoints.Length - 2]);
				player.LastPosY = Convert.ToInt32(allPoints[allPoints.Length - 1]);
				int id = packet.ReadInt();
				if (id == player.PlayerCharacter.ID)
				{
					player.Spa_X = packet.ReadInt();
					player.Spa_Y = packet.ReadInt();
					int Spa_Target_Area = packet.ReadInt();
					player.Spa_Player_Direction = packet.ReadInt();
					player.CurrentSpaRoom.ReturnPacket(player, packet);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
