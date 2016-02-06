using System;
namespace Fighting.Server.Rooms
{
	public class AddRoomAction : IAction
	{
		private ProxyRoom m_room;
		public AddRoomAction(ProxyRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			ProxyRoomMgr.AddRoomUnsafe(this.m_room);
		}
	}
}
