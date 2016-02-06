using Bussiness;
using Game.Server.GameObjects;
using Game.Server.SceneMarryRooms;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Game.Server.Managers
{
	public class MarryRoomMgr
	{
		private static LogProvider log => LogProvider.Default;
		protected static ReaderWriterLock _locker = new ReaderWriterLock();
		protected static Dictionary<int, MarryRoom> _Rooms;
		protected static TankMarryLogicProcessor _processor = new TankMarryLogicProcessor();
		public static bool Init()
		{
			MarryRoomMgr._Rooms = new Dictionary<int, MarryRoom>();
			try
			{
				MarryRoomMgr.CheckRoomStatus();
			}
			catch (Exception ex)
			{
				MarryRoomMgr.log.Error(ex.Message);
			}
			return true;
		}
		private static void CheckRoomStatus()
		{
			using (PlayerBussiness db = new PlayerBussiness())
			{
				MarryRoomInfo[] roomInfos = db.GetMarryRoomInfo();
				MarryRoomInfo[] array = roomInfos;
				for (int i = 0; i < array.Length; i++)
				{
					MarryRoomInfo roomInfo = array[i];
					if (roomInfo.ServerID == GameServer.Instance.Configuration.ServerID)
					{
						TimeSpan usedTime = DateTime.Now - roomInfo.BeginTime;
						int timeLeft = roomInfo.AvailTime * 60 - (int)usedTime.TotalMinutes;
						if (timeLeft > 0)
						{
							MarryRoomMgr.CreateMarryRoomFromDB(roomInfo, timeLeft);
						}
						else
						{
							db.DisposeMarryRoomInfo(roomInfo.ID);
							GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(roomInfo.GroomID);
							GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(roomInfo.BrideID);
							GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(roomInfo.GroomID, false, roomInfo);
							GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(roomInfo.BrideID, false, roomInfo);
						}
					}
				}
			}
		}
		public static MarryRoom[] GetAllMarryRoom()
		{
			MarryRoom[] list = null;
			MarryRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				list = new MarryRoom[MarryRoomMgr._Rooms.Count];
				MarryRoomMgr._Rooms.Values.CopyTo(list, 0);
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseReaderLock();
			}
			return (list == null) ? new MarryRoom[0] : list;
		}
		public static MarryRoom CreateMarryRoom(GamePlayer player, MarryRoomInfo info)
		{
			MarryRoom result;
			if (!player.PlayerCharacter.IsMarried)
			{
				result = null;
			}
			else
			{
				MarryRoom room = null;
				DateTime beginTime = DateTime.Now;
				info.PlayerID = player.PlayerCharacter.ID;
				info.PlayerName = player.PlayerCharacter.NickName;
				if (player.PlayerCharacter.Sex)
				{
					info.GroomID = info.PlayerID;
					info.GroomName = info.PlayerName;
					info.BrideID = player.PlayerCharacter.SpouseID;
					info.BrideName = player.PlayerCharacter.SpouseName;
				}
				else
				{
					info.BrideID = info.PlayerID;
					info.BrideName = info.PlayerName;
					info.GroomID = player.PlayerCharacter.SpouseID;
					info.GroomName = player.PlayerCharacter.SpouseName;
				}
				info.BeginTime = beginTime;
				info.BreakTime = beginTime;
				using (PlayerBussiness db = new PlayerBussiness())
				{
					if (db.InsertMarryRoomInfo(info))
					{
						room = new MarryRoom(info, MarryRoomMgr._processor);
						GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.GroomID);
						GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.BrideID);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.GroomID, true, info);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.BrideID, true, info);
					}
				}
				if (room != null)
				{
					MarryRoomMgr._locker.AcquireWriterLock(1000);
					try
					{
						MarryRoomMgr._Rooms.Add(room.Info.ID, room);
					}
					finally
					{
						MarryRoomMgr._locker.ReleaseWriterLock();
					}
					if (room.AddPlayer(player))
					{
						room.BeginTimer(3600000 * room.Info.AvailTime);
						result = room;
						return result;
					}
				}
				result = null;
			}
			return result;
		}
		public static MarryRoom CreateMarryRoomFromDB(MarryRoomInfo roomInfo, int timeLeft)
		{
			MarryRoomMgr._locker.AcquireWriterLock(1000);
			MarryRoom result;
			try
			{
				MarryRoom room = new MarryRoom(roomInfo, MarryRoomMgr._processor);
				if (room != null)
				{
					MarryRoomMgr._Rooms.Add(room.Info.ID, room);
					room.BeginTimer(60000 * timeLeft);
					result = room;
					return result;
				}
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseWriterLock();
			}
			result = null;
			return result;
		}
		public static MarryRoom GetMarryRoombyID(int id, string pwd, ref string msg)
		{
			MarryRoom room = null;
			MarryRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				if (id > 0)
				{
					if (MarryRoomMgr._Rooms.Keys.Contains(id))
					{
						if (MarryRoomMgr._Rooms[id].Info.Pwd != pwd)
						{
							msg = "Game.Server.Managers.PWDError";
						}
						else
						{
							room = MarryRoomMgr._Rooms[id];
						}
					}
				}
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseReaderLock();
			}
			return room;
		}
		public static bool UpdateBreakTimeWhereServerStop()
		{
			bool result;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				result = db.UpdateBreakTimeWhereServerStop();
			}
			return result;
		}
		public static void RemoveMarryRoom(MarryRoom room)
		{
			MarryRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				if (MarryRoomMgr._Rooms.Keys.Contains(room.Info.ID))
				{
					MarryRoomMgr._Rooms.Remove(room.Info.ID);
				}
			}
			finally
			{
				MarryRoomMgr._locker.ReleaseReaderLock();
			}
		}
	}
}
