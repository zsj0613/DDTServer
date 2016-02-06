using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.SpaRooms;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Game.Server.Managers
{
	public class SpaRoomMgr
	{
		private static LogProvider log => LogProvider.Default;
		protected static ReaderWriterLock _locker = new ReaderWriterLock();
		protected static Dictionary<int, SpaRoom> _Rooms;
		protected static Dictionary<int, SpaRoom> _PubGoldRooms;
		protected static Dictionary<int, SpaRoom> _PubMoneyRooms;
		protected static Dictionary<int, int> _RoomsNum;
		public static int pubGoldRoomCount;
		public static int pubMoneyRoomCount;
		private static bool needToCreatePubRoom;
		private static int pubGoldRoom_Gold;
		private static int pubMoneyRoom_Money;
		public static int pubGoldRoom_MinLimit = 60;
		public static int pubMoneyRoom_MinLimit = 60;
		private static int pubGoldRoom_MaxCount;
		private static int pubMoneyRoom_MaxCount;
		public static int priRoomInit_MinLimit;
		public static int priRoomContinue_MinLimit;
		protected static SpaLogicProcessor _processor = new SpaLogicProcessor();
		public static bool Init()
		{
			bool result;
			if (SpaRoomMgr.InitSpaData())
			{
				try
				{
					SpaRoomMgr.CheckRoomStatus(SpaRoomMgr.needToCreatePubRoom);
					result = true;
					return result;
				}
				catch (Exception ex)
				{
					SpaRoomMgr.log.Error(ex.Message);
				}
			}
			result = false;
			return result;
		}
		private static bool InitSpaData()
		{
			bool result;
			if (SpaRoomMgr.LoadSpaPropertyFromDb())
			{
				SpaRoomMgr._Rooms = new Dictionary<int, SpaRoom>();
				SpaRoomMgr._PubGoldRooms = new Dictionary<int, SpaRoom>();
				SpaRoomMgr._PubMoneyRooms = new Dictionary<int, SpaRoom>();
				SpaRoomMgr._RoomsNum = new Dictionary<int, int>();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		private static bool LoadSpaPropertyFromDb()
		{
			string[] array = new string[]
			{
				"8",
				"0"
			};
			array = new string[]
			{
				"60",
				"60"
			};
			array = new string[]
			{
				"10000",
				"200"
			};
			array = new string[]
			{
				"30",
				"30"
			};
            string[] _SpaPubRoomCount = { "8", "0" };
			string[] _SpaPubRoomLoginPay = { "800", "1600" };
            string[] _SpaPubRoomMinLimit = { "60", "60" };
            string[] _SpaPubRoomPlayerMaxCount = { "10", "10" };
            SpaRoomMgr.priRoomInit_MinLimit = 60;
			SpaRoomMgr.priRoomContinue_MinLimit = 60;
			string[] pubRoomsServerIdTemp = "1|2|3|4|5|6|7|8|9|10".Split(new char[]
			{
				'|'
			});
			string[] array2 = pubRoomsServerIdTemp;
			for (int i = 0; i < array2.Length; i++)
			{
				string str = array2[i];
				SpaRoomMgr.needToCreatePubRoom = false;
				if (str == GameServer.Instance.Configuration.ServerID.ToString())
				{
					SpaRoomMgr.needToCreatePubRoom = true;
					break;
				}
			}
			bool result;
			if (_SpaPubRoomCount.Length < 2)
			{
				
					SpaRoomMgr.log.Error("SpaPubRoomCount Init Wrong");
				
				result = false;
			}
			else
			{
				if (_SpaPubRoomLoginPay.Length < 2)
				{
					
						SpaRoomMgr.log.Error("SpaPubRoomLoginPay Init Wrong");
					
					result = false;
				}
				else
				{
					if (_SpaPubRoomMinLimit.Length < 2)
					{
						
							SpaRoomMgr.log.Error("SpaPubRoomMinLimit Init Wrong");
						
						result = false;
					}
					else
					{
						if (_SpaPubRoomPlayerMaxCount.Length < 2)
						{
							
								SpaRoomMgr.log.Error("SpaPubRoomPlayerMaxCount Init Wrong");
							
							result = false;
						}
						else
						{
							
							{
								if (int.Parse(_SpaPubRoomCount[0]) >= 0)
								{
									SpaRoomMgr.pubGoldRoomCount = int.Parse(_SpaPubRoomCount[0]);
								}
								if (int.Parse(_SpaPubRoomCount[1]) >= 0)
								{
									SpaRoomMgr.pubMoneyRoomCount = int.Parse(_SpaPubRoomCount[1]);
								}
								if (int.Parse(_SpaPubRoomLoginPay[0]) >= 0)
								{
									SpaRoomMgr.pubGoldRoom_Gold = int.Parse(_SpaPubRoomLoginPay[0]);
								}
								if (int.Parse(_SpaPubRoomLoginPay[1]) >= 0)
								{
									SpaRoomMgr.pubMoneyRoom_Money = int.Parse(_SpaPubRoomLoginPay[1]);
								}
								if (int.Parse(_SpaPubRoomMinLimit[0]) >= 0)
								{
									SpaRoomMgr.pubGoldRoom_MinLimit = int.Parse(_SpaPubRoomMinLimit[0]);
								}
								if (int.Parse(_SpaPubRoomMinLimit[1]) >= 0)
								{
									SpaRoomMgr.pubMoneyRoom_MinLimit = int.Parse(_SpaPubRoomMinLimit[1]);
								}
								if (int.Parse(_SpaPubRoomPlayerMaxCount[0]) >= 0)
								{
									SpaRoomMgr.pubGoldRoom_MaxCount = int.Parse(_SpaPubRoomPlayerMaxCount[0]);
								}
								if (int.Parse(_SpaPubRoomPlayerMaxCount[1]) >= 0)
								{
									SpaRoomMgr.pubMoneyRoom_MaxCount = int.Parse(_SpaPubRoomPlayerMaxCount[1]);
								}
								result = true;
							}
						}
					}
				}
			}
			return result;
		}
		private static void CheckRoomStatus(bool needToCreatePubRoom)
		{
			using (PlayerBussiness db = new PlayerBussiness())
			{
				SpaRoomInfo[] roomInfos = db.GetSpaRoomInfo();
				int pubGoldRoomCurrentCount = 0;
				int pubMoneyRoomsCurrentCount = 0;
				SpaRoomInfo[] array = roomInfos;
				for (int i = 0; i < array.Length; i++)
				{
					SpaRoomInfo roomInfo = array[i];
					if (roomInfo.ServerID == GameServer.Instance.Configuration.ServerID)
					{
						int timeLeft;
						if (roomInfo.AvailTime > 0)
						{
							if (roomInfo.RoomType != 1 && roomInfo.RoomType != 2 && DateTime.Compare(roomInfo.BreakTime, roomInfo.BeginTime) > 0)
							{
								TimeSpan usedTime = roomInfo.BreakTime - roomInfo.BeginTime;
								timeLeft = roomInfo.AvailTime - (int)usedTime.TotalMinutes;
							}
							else
							{
								TimeSpan usedTime = DateTime.Now - roomInfo.BeginTime;
								timeLeft = roomInfo.AvailTime - (int)usedTime.TotalMinutes;
							}
						}
						else
						{
							if (roomInfo.RoomType != 1 && roomInfo.RoomType != 2)
							{
								SpaRoomMgr.log.Error(string.Format("SpaRoom ===== Room.ID : {0}, Room.RoomType : {1}, Room.AvailTime : {2}", roomInfo.RoomID, roomInfo.RoomType, roomInfo.AvailTime));
							}
							timeLeft = 0;
						}
						if (!needToCreatePubRoom)
						{
							if (roomInfo.RoomType == 1 || roomInfo.RoomType == 2)
							{
								db.DisposeSpaRoomInfo(roomInfo.RoomID);
								goto IL_21F;
							}
						}
						if (timeLeft >= 0)
						{
							if (roomInfo.RoomType == 1)
							{
								if (pubGoldRoomCurrentCount < SpaRoomMgr.pubGoldRoomCount)
								{
									SpaRoomMgr.CreateSpaRoomFromDB(roomInfo, timeLeft);
									pubGoldRoomCurrentCount++;
								}
							}
							else
							{
								if (roomInfo.RoomType == 2)
								{
									if (pubGoldRoomCurrentCount < SpaRoomMgr.pubMoneyRoomCount)
									{
										SpaRoomMgr.CreateSpaRoomFromDB(roomInfo, timeLeft);
										pubMoneyRoomsCurrentCount++;
									}
								}
								else
								{
									SpaRoomMgr.CreateSpaRoomFromDB(roomInfo, timeLeft);
								}
							}
						}
						else
						{
							db.DisposeSpaRoomInfo(roomInfo.RoomID);
						}
					}
					IL_21F:;
				}
				if (needToCreatePubRoom)
				{
					if (SpaRoomMgr._PubGoldRooms.Count<KeyValuePair<int, SpaRoom>>() < SpaRoomMgr.pubGoldRoomCount)
					{
						SpaRoomMgr.CreatePubRoom(1, SpaRoomMgr.pubGoldRoomCount - SpaRoomMgr._PubGoldRooms.Count<KeyValuePair<int, SpaRoom>>());
					}
					if (SpaRoomMgr._PubMoneyRooms.Count<KeyValuePair<int, SpaRoom>>() < SpaRoomMgr.pubMoneyRoomCount)
					{
						SpaRoomMgr.CreatePubRoom(2, SpaRoomMgr.pubMoneyRoomCount - SpaRoomMgr._PubMoneyRooms.Count<KeyValuePair<int, SpaRoom>>());
					}
				}
			}
		}
		public static SpaRoom CreateSpaRoomFromDB(SpaRoomInfo roomInfo, int timeLeft)
		{
			SpaRoomMgr._locker.AcquireWriterLock(1000);
			SpaRoom result;
			try
			{
				SpaRoom room = new SpaRoom(roomInfo, SpaRoomMgr._processor);
				if (room != null)
				{
					SpaRoomMgr._RoomsNum.Add(room.Spa_Room_Info.RoomID, room.Spa_Room_Info.RoomNumber);
					if (roomInfo.RoomType == 1)
					{
						SpaRoomMgr._PubGoldRooms.Add(room.Spa_Room_Info.RoomID, room);
						SpaRoomMgr._Rooms.Add(room.Spa_Room_Info.RoomID, room);
						room.BeginTimerForPubRoom();
						result = room;
						return result;
					}
					if (roomInfo.RoomType == 2)
					{
						SpaRoomMgr._PubMoneyRooms.Add(room.Spa_Room_Info.RoomID, room);
						SpaRoomMgr._Rooms.Add(room.Spa_Room_Info.RoomID, room);
						room.BeginTimerForPubRoom();
						result = room;
						return result;
					}
					SpaRoomMgr._Rooms.Add(room.Spa_Room_Info.RoomID, room);
					room.BeginTimer(timeLeft);
					result = room;
					return result;
				}
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseWriterLock();
			}
			result = null;
			return result;
		}
		private static void CreatePubRoom(int roomType, int roomCount)
		{
			for (int i = 0; i < roomCount; i++)
			{
				SpaRoom pubRoom = null;
				SpaRoomMgr._locker.AcquireWriterLock(1000);
				try
				{
					SpaRoomInfo pubRoomInfo = new SpaRoomInfo();
					DateTime beginTime = DateTime.Now;
					pubRoomInfo.AvailTime = 0;
					pubRoomInfo.BeginTime = beginTime;
					pubRoomInfo.BreakTime = beginTime;
					pubRoomInfo.RoomIntroduction = LanguageMgr.GetTranslation("SpaRoom.PubRoomIntroduction", new object[0]);
					pubRoomInfo.RoomNumber = SpaRoomMgr.FindRoomNumber();
					pubRoomInfo.ServerID = GameServer.Instance.Configuration.ServerID;
					pubRoomInfo.RoomType = roomType;
					pubRoomInfo.Pwd = "";
					if (roomType == 1)
					{
						pubRoomInfo.MaxCount = SpaRoomMgr.pubGoldRoom_MaxCount;
						pubRoomInfo.RoomName = LanguageMgr.GetTranslation("SpaRoom.PubGoldRoomName", new object[0]);
					}
					if (roomType == 2)
					{
						pubRoomInfo.MaxCount = SpaRoomMgr.pubMoneyRoom_MaxCount;
						pubRoomInfo.RoomName = LanguageMgr.GetTranslation("SpaRoom.PubMoneyRoomName", new object[0]);
					}
					using (PlayerBussiness db = new PlayerBussiness())
					{
						if (db.InsertSpaPubRoomInfo(pubRoomInfo))
						{
							pubRoom = new SpaRoom(pubRoomInfo, SpaRoomMgr._processor);
						}
					}
					if (pubRoomInfo != null)
					{
						SpaRoomMgr._RoomsNum.Add(pubRoom.Spa_Room_Info.RoomID, pubRoom.Spa_Room_Info.RoomNumber);
						SpaRoomMgr._Rooms.Add(pubRoom.Spa_Room_Info.RoomID, pubRoom);
						if (roomType == 1)
						{
							SpaRoomMgr._PubGoldRooms.Add(pubRoom.Spa_Room_Info.RoomID, pubRoom);
						}
						if (roomType == 2)
						{
							SpaRoomMgr._PubMoneyRooms.Add(pubRoom.Spa_Room_Info.RoomID, pubRoom);
						}
					}
				}
				finally
				{
					SpaRoomMgr._locker.ReleaseWriterLock();
				}
				if (pubRoom != null)
				{
					pubRoom.BeginTimerForPubRoom();
				}
			}
		}
		public static SpaRoom[] GetAllSpaRoom()
		{
			SpaRoom[] list = null;
			SpaRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				list = new SpaRoom[SpaRoomMgr._Rooms.Count];
				SpaRoomMgr._Rooms.Values.CopyTo(list, 0);
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseReaderLock();
			}
			return list;
		}
		public static SpaRoom[] GetAllSpaPubGoldRoom()
		{
			SpaRoom[] list = null;
			SpaRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				list = new SpaRoom[SpaRoomMgr._PubGoldRooms.Count];
				SpaRoomMgr._PubGoldRooms.Values.CopyTo(list, 0);
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseReaderLock();
			}
			return list;
		}
		public static SpaRoom[] GetAllSpaPubMoneyRoom()
		{
			SpaRoom[] list = null;
			SpaRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				list = new SpaRoom[SpaRoomMgr._PubMoneyRooms.Count];
				SpaRoomMgr._PubMoneyRooms.Values.CopyTo(list, 0);
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseReaderLock();
			}
			return list;
		}
		public static SpaRoom GetSpaRoombyID(int id, string pwd, ref string msg)
		{
			SpaRoom room = null;
			SpaRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				if (id > 0)
				{
					if (SpaRoomMgr._Rooms.Keys.Contains(id))
					{
						if (SpaRoomMgr._Rooms[id].Spa_Room_Info.Pwd != pwd)
						{
							msg = "SpaRoomLoginHandler.Failed3";
						}
						else
						{
							room = SpaRoomMgr._Rooms[id];
						}
					}
				}
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseReaderLock();
			}
			return room;
		}
		public static void RemoveSpaRoom(SpaRoom room)
		{
			SpaRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				if (SpaRoomMgr._Rooms.Keys.Contains(room.Spa_Room_Info.RoomID))
				{
					SpaRoomMgr._Rooms.Remove(room.Spa_Room_Info.RoomID);
					SpaRoomMgr._RoomsNum.Remove(room.Spa_Room_Info.RoomID);
				}
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseReaderLock();
			}
		}
		public static bool UpdateBreakTimeWhereSpaServerStop()
		{
			bool result;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				result = db.UpdateBreakTimeWhereSpaServerStop();
			}
			return result;
		}
		public static bool SpaPubRoomPay(GamePlayer player, SpaRoom room)
		{
			bool result;
			if (room.Spa_Room_Info.RoomType == 1 && !player.PlayerCharacter.IsInSpaPubGoldToday)
			{
				if (player.PlayerCharacter.Gold < SpaRoomMgr.pubGoldRoom_Gold)
				{
					player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed1", new object[0]));
					result = false;
					return result;
				}
				player.RemoveGold(SpaRoomMgr.pubGoldRoom_Gold);
			}
			if (room.Spa_Room_Info.RoomType == 2 && !player.PlayerCharacter.IsInSpaPubMoneyToday)
			{
				if (player.PlayerCharacter.Money < SpaRoomMgr.pubMoneyRoom_Money)
				{
					player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed2", new object[0]));
					result = false;
					return result;
				}
				player.RemoveMoney(SpaRoomMgr.pubMoneyRoom_Money, LogMoneyType.Spa, LogMoneyType.Spa_Room_Login);
			}
			result = true;
			return result;
		}
		public static bool LoginSpaRoom(GamePlayer player, SpaRoom room, string msg)
		{
			bool result;
			if (player.CurrentSpaRoom != null)
			{
				if (player.CurrentSpaRoom == room)
				{
					result = false;
					return result;
				}
				SpaRoomMgr.log.Error(string.Format("SpaRoomMgr ====== player.nickname : {0}, room.roomID : {1}.player is in this room,but be logining.", player.PlayerCharacter.NickName, player.CurrentSpaRoom.Spa_Room_Info.RoomID));
				player.CurrentSpaRoom.RemovePlayer(player);
			}
			if (room != null)
			{
				if (room.Count < room.Spa_Room_Info.MaxCount)
				{
					if (SpaRoomMgr.SpaPubRoomPay(player, room))
					{
						if (room.AddPlayer(player))
						{
							player.Out.SendSpaRoomLogin(player);
							room.SendSpaRoomInfoUpdateToSpaScenePlayers(room);
							result = true;
							return result;
						}
					}
					result = false;
					return result;
				}
				player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("SpaRoom.Msg1", new object[0]));
			}
			else
			{
				player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(string.IsNullOrEmpty(msg) ? "SpaRoomLoginHandler.Failed4" : msg, new object[0]));
			}
			result = false;
			return result;
		}
		public static bool QuickLoginSpaRoom(GamePlayer player)
		{
			bool result;
			if (player.CurrentSpaRoom != null)
			{
				result = false;
			}
			else
			{
				SpaRoom roomRondom = SpaRoomMgr.FindSpaRoomRandom(player);
				if (roomRondom != null)
				{
					if (roomRondom.Spa_Room_Info.RoomType == 1 && !player.PlayerCharacter.IsInSpaPubGoldToday)
					{
						player.Out.SendSpaRoomLoginRemind(roomRondom);
						result = true;
						return result;
					}
					if (roomRondom.Spa_Room_Info.RoomType == 2 && !player.PlayerCharacter.IsInSpaPubMoneyToday)
					{
						player.Out.SendSpaRoomLoginRemind(roomRondom);
						result = true;
						return result;
					}
					if (SpaRoomMgr.LoginSpaRoom(player, roomRondom, null))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}
		public static SpaRoom FindSpaRoomRandom(GamePlayer player)
		{
			SpaRoom targetSpaRoom = null;
			SpaRoom[] spaRoomsTemp = SpaRoomMgr.GetAllSpaRoom();
			SpaRoom result;
			if (player.PlayerCharacter.SpaPubGoldRoomLimit <= 0)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomLoginHandler.Failed", new object[0]));
				result = null;
			}
			else
			{
				SpaRoom[] array = spaRoomsTemp;
				int i = 0;
				while (i < array.Length)
				{
					SpaRoom r = array[i];
					if (r.Count < r.Spa_Room_Info.MaxCount)
					{
						if (r.Spa_Room_Info.PlayerID == player.PlayerCharacter.ID)
						{
							targetSpaRoom = r;
						}
					}
					//IL_9A:
					i++;
					continue;
					//goto IL_9A;
				}
				if (targetSpaRoom == null)
				{
					array = spaRoomsTemp;
					for (i = 0; i < array.Length; i++)
					{
						SpaRoom r2 = array[i];
						if (r2.Count < r2.Spa_Room_Info.MaxCount && r2.Spa_Room_Info.Pwd == "")
						{
							if (r2.Spa_Room_Info.RoomType != 1 && r2.Spa_Room_Info.RoomType != 2)
							{
								targetSpaRoom = r2;
								break;
							}
						}
					}
				}
				if (targetSpaRoom == null)
				{
					SpaRoom[] spaPubGoldRoomsTemp = SpaRoomMgr.GetAllSpaPubGoldRoom();
					array = spaPubGoldRoomsTemp;
					for (i = 0; i < array.Length; i++)
					{
						SpaRoom r3 = array[i];
						if (r3.Count < r3.Spa_Room_Info.MaxCount)
						{
							if (player.PlayerCharacter.SpaPubGoldRoomLimit > 0)
							{
								targetSpaRoom = r3;
								break;
							}
						}
					}
				}
				if (targetSpaRoom == null)
				{
					SpaRoom[] spaPubMoneyRoomsTemp = SpaRoomMgr.GetAllSpaPubMoneyRoom();
					array = spaPubMoneyRoomsTemp;
					for (i = 0; i < array.Length; i++)
					{
						SpaRoom r4 = array[i];
						if (r4.Count < r4.Spa_Room_Info.MaxCount)
						{
							if (player.PlayerCharacter.SpaPubMoneyRoomLimit > 0)
							{
								targetSpaRoom = r4;
								break;
							}
						}
					}
				}
				if (targetSpaRoom == null)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomQuickLoginHandler.Failed", new object[0]));
				}
				result = targetSpaRoom;
			}
			return result;
		}
		public static SpaRoom CreateSpaRoom(GamePlayer player, SpaRoomInfo info)
		{
			SpaRoom room = null;
			SpaRoomMgr._locker.AcquireWriterLock(1000);
			try
			{
				DateTime beginTime = DateTime.Now;
				info.PlayerID = player.PlayerCharacter.ID;
				info.PlayerName = player.PlayerCharacter.NickName;
				info.BeginTime = beginTime;
				info.BreakTime = beginTime;
				info.ServerID = GameServer.Instance.Configuration.ServerID;
				info.RoomNumber = SpaRoomMgr.FindRoomNumber();
				using (PlayerBussiness db = new PlayerBussiness())
				{
					if (db.InsertSpaRoomInfo(info))
					{
						room = new SpaRoom(info, SpaRoomMgr._processor);
					}
				}
				if (room != null)
				{
					SpaRoomMgr._RoomsNum.Add(room.Spa_Room_Info.RoomID, room.Spa_Room_Info.RoomNumber);
					SpaRoomMgr._Rooms.Add(room.Spa_Room_Info.RoomID, room);
				}
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseWriterLock();
			}
			SpaRoom result;
			if (room != null)
			{
				if (room.AddPlayer(player))
				{
					room.BeginTimer(room.Spa_Room_Info.AvailTime);
					result = room;
					return result;
				}
			}
			result = null;
			return result;
		}
		public static int[] GetAllRoomNum()
		{
			int[] list = null;
			SpaRoomMgr._locker.AcquireReaderLock(1000);
			try
			{
				list = new int[SpaRoomMgr._Rooms.Count];
				SpaRoomMgr._RoomsNum.Values.CopyTo(list, 0);
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseReaderLock();
			}
			return list;
		}
		private static int FindRoomNumber()
		{
			SpaRoomMgr._locker.AcquireWriterLock(1000);
			int result;
			try
			{
				int maxNum = 0;
				int[] list = SpaRoomMgr.GetAllRoomNum();
				for (int i = 0; i < list.Length; i++)
				{
					if (list[i] != i + 1)
					{
						if (!list.Contains(i + 1))
						{
							result = i + 1;
							return result;
						}
					}
					if (list[i] > maxNum)
					{
						maxNum = list[i];
					}
				}
				result = maxNum + 1;
			}
			finally
			{
				SpaRoomMgr._locker.ReleaseReaderLock();
			}
			return result;
		}
	}
}
