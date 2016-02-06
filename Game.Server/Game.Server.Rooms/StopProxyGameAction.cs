using System;
namespace Game.Server.Rooms
{
	public class StopProxyGameAction : IAction
	{
		private BaseRoom m_room;
		public StopProxyGameAction(BaseRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			if (this.m_room.Game != null)
			{
				this.m_room.Game.Stop();
			}
		}
	}
}
