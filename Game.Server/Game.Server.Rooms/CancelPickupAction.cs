using Game.Server.Battle;
using System;
namespace Game.Server.Rooms
{
	public class CancelPickupAction : IAction
	{
		private BattleServer m_server;
		private BaseRoom m_room;
		public CancelPickupAction(BattleServer server, BaseRoom room)
		{
			this.m_room = room;
			this.m_server = server;
		}
		public void Execute()
		{
			if (this.m_room.Game == null)
			{
				this.m_room.BattleServer = null;
				this.m_room.IsPlaying = false;
				this.m_room.SendCancelPickUp();
				RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
			}
		}
	}
}
