using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.SpaRooms
{
	public abstract class AbstractSpaProcessor : ISpaProcessor
	{
		public virtual void OnGameData(SpaRoom game, GamePlayer player, GSPacketIn packet)
		{
		}
		public virtual void OnTick(SpaRoom room)
		{
		}
	}
}
