using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Packets;
using System;
using Game.Language;
namespace Game.Server.Rooms
{
	internal class RoomSetupChangeAction : IAction
	{
		private BaseRoom m_room;
		private eRoomType m_roomType;
		private byte m_timeMode;
		private eHardLevel m_hardLevel;
		private int m_mapId;
		private int m_levelLimits;
		private bool m_isArea;
		private GSPacketIn m_packet;
		public RoomSetupChangeAction(BaseRoom room, GSPacketIn packet, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId, bool isArea)
		{
			this.m_room = room;
			this.m_roomType = roomType;
			this.m_timeMode = timeMode;
			this.m_hardLevel = hardLevel;
			this.m_levelLimits = levelLimits;
			this.m_mapId = mapId;
			this.m_packet = packet;
			this.m_isArea = isArea;
		}
		public void Execute()
		{
			int levelLimites = 0;
			if ((this.m_roomType == eRoomType.Boss || this.m_roomType == eRoomType.Exploration || this.m_roomType == eRoomType.Treasure) && !this.m_room.IsGradeAchieved(this.m_mapId, ref levelLimites))
			{
				this.m_room.Host.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("RoomSetupChangeAction.GradeNotAchieved", new object[]
				{
					levelLimites
				}));
			}
			else
			{
				this.m_room.RoomType = this.m_roomType;
				this.m_room.TimeMode = this.m_timeMode;
				this.m_room.HardLevel = this.m_hardLevel;
				this.m_room.LevelLimits = this.m_levelLimits;
				this.m_room.MapId = this.m_mapId;
				this.m_room.IsArea = this.m_isArea;
				this.m_room.UpdateRoomGameType();
                if(m_packet != null)
				    this.m_room.SendToAll(this.m_packet);
				RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
			}
		}
	}
}
