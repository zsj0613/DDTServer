using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SpaRooms
{
	public interface ISpaProcessor
	{
		void OnGameData(SpaRoom game, GamePlayer player, GSPacketIn packet);
		void OnTick(SpaRoom room);
	}
}
