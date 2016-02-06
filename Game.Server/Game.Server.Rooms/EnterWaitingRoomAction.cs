using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class EnterWaitingRoomAction : IAction
	{
		private GamePlayer m_player;
		private int m_hallType;
		public EnterWaitingRoomAction(GamePlayer player, int hallType)
		{
			this.m_player = player;
			this.m_hallType = hallType;
		}
		public void Execute()
		{
			if (this.m_player != null)
			{
				if (this.m_player.CurrentRoom != null)
				{
					RoomMgr.ExitRoom(this.m_player.CurrentRoom, this.m_player);
				}
				BaseWaitingRoom room = RoomMgr.WaitingRoom;
				if (room.AddPlayer(this.m_player))
				{
					List<BaseRoom> list = RoomMgr.GetWaitingRoom(this.m_hallType, 0, 9, 0);
					this.m_player.Out.SendUpdateRoomList(list);
					GamePlayer[] players = room.GetPlayersSafe();
					GamePlayer[] array = players;
					for (int i = 0; i < array.Length; i++)
					{
						GamePlayer p = array[i];
						if (p != this.m_player)
						{
							this.m_player.Out.SendSceneAddPlayer(p);
						}
					}
				}
			}
		}
	}
}
