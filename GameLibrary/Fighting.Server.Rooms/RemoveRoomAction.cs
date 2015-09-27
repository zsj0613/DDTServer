using System;
namespace Fighting.Server.Rooms
{
	public class RemoveRoomAction : IAction
	{
		private ProxyRoom m_room;
		public RemoveRoomAction(ProxyRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			ProxyRoomMgr.RemoveRoomUnsafe(this.m_room.RoomId);
			this.m_room.Dispose();
		}
	}
}
