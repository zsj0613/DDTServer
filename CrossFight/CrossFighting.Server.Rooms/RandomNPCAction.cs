using System;
namespace CrossFighting.Server.Rooms
{
	public class RandomNPCAction : IAction
	{
		private ProxyRoom m_room;
		public RandomNPCAction(ProxyRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			ProxyRoomMgr.RemoveRoomUnsafe(this.m_room.RoomId);
			this.m_room.CancelPickUp();
		}
	}
}
