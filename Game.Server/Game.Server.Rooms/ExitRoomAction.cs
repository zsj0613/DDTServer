using Game.Server.GameObjects;
using System;
namespace Game.Server.Rooms
{
	public class ExitRoomAction : IAction
	{
		private BaseRoom m_room;
		private GamePlayer m_player;
		public ExitRoomAction(BaseRoom room, GamePlayer player)
		{
			this.m_room = room;
			this.m_player = player;
		}
		public void Execute()
		{
			this.m_room.RemovePlayerUnsafe(this.m_player);
			if (this.m_room.PlayerCount == 0)
			{
				this.m_room.Stop();
			}
		}
	}
}
