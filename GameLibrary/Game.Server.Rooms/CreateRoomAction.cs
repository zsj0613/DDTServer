using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;

namespace Game.Server.Rooms
{
	public class CreateRoomAction : IAction
	{
		private GamePlayer m_player;
		private string m_name;
		private string m_password;
		private eRoomType m_roomType;
		private byte m_timeType;
		public CreateRoomAction(GamePlayer player, string name, string password, eRoomType roomType, byte timeType)
		{
			this.m_player = player;
			this.m_name = name;
			this.m_password = password;
			this.m_roomType = roomType;
			this.m_timeType = timeType;
		}
		public void Execute()
		{
			if (this.m_player.CurrentRoom != null)
			{
				this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
			}
			if (this.m_player.IsActive)
			{
				BaseRoom[] rooms = RoomMgr.Rooms;
				BaseRoom room = null;
				for (int i = 0; i < rooms.Length; i++)
				{
					if (!rooms[i].IsUsing)
					{
						room = rooms[i];
						break;
					}
				}
				if (room != null)
				{
					RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
					room.Start();
					if (this.m_roomType == eRoomType.Exploration)
					{
						room.HardLevel = eHardLevel.Normal;
						room.LevelLimits = (int)room.GetLevelLimit(this.m_player);
					}
					room.UpdateRoom(this.m_name, this.m_password, this.m_roomType, this.m_timeType, 0);

					if (this.m_roomType == eRoomType.Match)
					{
						if (BattleMgr.IsOpenAreaFight)
						{
							room.IsArea = true;
						}
					}
					GSPacketIn pkg = this.m_player.Out.SendRoomCreate(room);
					room.AddPlayerUnsafe(this.m_player);
					RoomMgr.WaitingRoom.SendUpdateRoom(room);
				}
				else
				{
					Console.WriteLine(LanguageMgr.GetTranslation("CreateRoomAction.MaxRoom", new object[0]));
					this.m_player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CreateRoomAction.MaxRoom", new object[0]));
				}
			}
		}
	}
}
