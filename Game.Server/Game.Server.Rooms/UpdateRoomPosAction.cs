using System;
namespace Game.Server.Rooms
{
	public class UpdateRoomPosAction : IAction
	{
		private BaseRoom m_room;
		private int m_pos;
		private bool m_isOpened;
		public UpdateRoomPosAction(BaseRoom room, int pos, bool isOpened)
		{
			this.m_room = room;
			this.m_pos = pos;
			this.m_isOpened = isOpened;
		}
		public void Execute()
		{
			if (this.m_room.PlayerCount > 0 && this.m_room.UpdatePosUnsafe(this.m_pos, this.m_isOpened))
			{
				RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
			}
		}
	}
}
