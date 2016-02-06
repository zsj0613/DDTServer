using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SpaRooms.CommandHandle
{
	public interface ISpaCommandHandler
	{
		bool HandleCommand(GamePlayer player, GSPacketIn packet);
	}
}
