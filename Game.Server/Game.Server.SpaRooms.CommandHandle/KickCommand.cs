using Game.Base.Packets;
using Game.Server.GameObjects;
using Lsj.Util.Logs;
using System;
using System.Reflection;
namespace Game.Server.SpaRooms.CommandHandle
{
	[SpaCommandAttbute(9)]
	public class KickCommand : ISpaCommandHandler
	{
		protected static LogProvider log => LogProvider.Default;
		public bool HandleCommand(GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentSpaRoom != null)
			{
				if (player.PlayerCharacter.ID == player.CurrentSpaRoom.Spa_Room_Info.PlayerID)
				{
					int userID = packet.ReadInt();
					player.CurrentSpaRoom.KickPlayerByUserID(player, userID);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
