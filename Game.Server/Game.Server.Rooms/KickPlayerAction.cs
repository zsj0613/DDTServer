using System;
namespace Game.Server.Rooms
{
	public class KickPlayerAction : IAction
	{
		private BaseRoom m_room;
		private int m_place;
		public KickPlayerAction(BaseRoom room, int place)
		{
			this.m_room = room;
			this.m_place = place;
		}
		public void Execute()
		{
			this.m_room.RemovePlayerAtUnsafe(this.m_place);
		}
	}
}
