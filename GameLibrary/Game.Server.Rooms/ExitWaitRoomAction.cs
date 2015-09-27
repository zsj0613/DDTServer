using Game.Server.GameObjects;
using System;
namespace Game.Server.Rooms
{
	public class ExitWaitRoomAction : IAction
	{
		private GamePlayer m_player;
		public ExitWaitRoomAction(GamePlayer player)
		{
			this.m_player = player;
		}
		public void Execute()
		{
			BaseWaitingRoom room = RoomMgr.WaitingRoom;
			room.RemovePlayer(this.m_player);
		}
	}
}
