using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using System;
using System.Reflection;
namespace Game.Server.SpaRooms.CommandHandle
{
	[SpaCommandAttbute(9)]
	public class KickCommand : ISpaCommandHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
