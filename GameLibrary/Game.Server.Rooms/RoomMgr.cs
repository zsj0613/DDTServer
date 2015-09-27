using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Rooms
{
	public class RoomMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static bool m_running;
		public static int DELAY_TIME = 0;
		private static Queue<IAction> m_actionQueue;
		private static Thread m_thread;
		private static BaseRoom[] m_rooms;
		private static BaseWaitingRoom m_waitingRoom;
		private static Random rand = new Random();
		public static int UsingRoomCount;
		public static readonly int THREAD_INTERVAL = 40;
		public static readonly int PICK_UP_INTERVAL = 10000;
		public static readonly int CLEAR_ROOM_INTERVAL = 400;
		private static long m_clearTick = 0L;
		public static int Count = 0;
		public static BaseRoom[] Rooms
		{
			get
			{
				return RoomMgr.m_rooms;
			}
		}
		public static Thread Thread
		{
			get
			{
				return RoomMgr.m_thread;
			}
		}
		public static BaseWaitingRoom WaitingRoom
		{
			get
			{
				return RoomMgr.m_waitingRoom;
			}
		}
		public static bool Setup(int maxRoom)
		{
			maxRoom = ((maxRoom < 1) ? 1 : maxRoom);
			RoomMgr.m_thread = new Thread(new ThreadStart(RoomMgr.RoomThread));
			RoomMgr.m_actionQueue = new Queue<IAction>();
			RoomMgr.m_rooms = new BaseRoom[maxRoom];
			for (int i = 0; i < maxRoom; i++)
			{
				RoomMgr.m_rooms[i] = new BaseRoom(i + 1);
			}
			RoomMgr.m_waitingRoom = new BaseWaitingRoom();
			return true;
		}
		public static void Start()
		{
			if (!RoomMgr.m_running)
			{
				RoomMgr.m_running = true;
				RoomMgr.m_thread.Start();
			}
		}
		public static void Stop()
		{
			if (RoomMgr.m_running)
			{
				RoomMgr.m_running = false;
				RoomMgr.m_thread.Join();
			}
		}
		private static void RoomThread()
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			long balance = 0L;
			RoomMgr.m_clearTick = TickHelper.GetTickCount();
			while (RoomMgr.m_running)
			{
				long start = TickHelper.GetTickCount();
				int count = 0;
				try
				{
					count = RoomMgr.ExecuteActions();
					if (RoomMgr.m_clearTick <= start)
					{
						RoomMgr.m_clearTick += (long)RoomMgr.CLEAR_ROOM_INTERVAL;
						RoomMgr.ClearRooms(start);
					}
				}
				catch (Exception ex)
				{
					RoomMgr.log.Error("Room Mgr Thread Error:", ex);
				}
				long end = TickHelper.GetTickCount();
				balance += (long)RoomMgr.THREAD_INTERVAL - (end - start);
				if (end - start > (long)(RoomMgr.THREAD_INTERVAL * 2))
				{
					RoomMgr.log.WarnFormat("Room Mgr is spent too much times: {0} ms,count:{1}", end - start, count);
				}
				if (balance > 0L)
				{
					Thread.Sleep((int)balance);
					balance = 0L;
				}
				else
				{
					if (balance < -1000L)
					{
						balance += 1000L;
					}
				}
				if (RoomMgr.DELAY_TIME > 0)
				{
					RoomMgr.log.ErrorFormat("Delay for {0} ms!", RoomMgr.DELAY_TIME);
					Thread.Sleep(RoomMgr.DELAY_TIME);
				}
			}
		}
		private static int ExecuteActions()
		{
			IAction[] actions = null;
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = RoomMgr.m_actionQueue);
			try
			{
				if (RoomMgr.m_actionQueue.Count > 0)
				{
					actions = new IAction[RoomMgr.m_actionQueue.Count];
					RoomMgr.m_actionQueue.CopyTo(actions, 0);
					RoomMgr.m_actionQueue.Clear();
				}
			}
			finally
			{
				Monitor.Exit(actionQueue);
			}
			int result;
			if (actions != null)
			{
				IAction[] array = actions;
				for (int i = 0; i < array.Length; i++)
				{
					IAction ac = array[i];
					try
					{
						long begin = TickHelper.GetTickCount();
						ac.Execute();
						long end = TickHelper.GetTickCount();
						if (end - begin > 40L)
						{
							RoomMgr.log.WarnFormat("RoomMgr action spent too much times:{0},{1}ms!", ac.GetType(), end - begin);
						}
					}
					catch (Exception ex)
					{
						RoomMgr.log.Error("RoomMgr execute action error:", ex);
					}
				}
				result = actions.Length;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public static void ClearRooms(long tick)
		{
			BaseRoom[] rooms = RoomMgr.m_rooms;
			for (int i = 0; i < rooms.Length; i++)
			{
				BaseRoom room = rooms[i];
				if (room.IsUsing)
				{
					if (room.PlayerCount == 0)
					{
						room.Stop();
					}
				}
			}
		}
		public static void AddAction(IAction action)
		{
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = RoomMgr.m_actionQueue);
			try
			{
				RoomMgr.m_actionQueue.Enqueue(action);
			}
			finally
			{
				Monitor.Exit(actionQueue);
			}
		}
		public static void CreateRoom(GamePlayer player, string name, string password, eRoomType roomType, byte timeType)
		{
			RoomMgr.AddAction(new CreateRoomAction(player, name, password, roomType, timeType));
		}
		public static void EnterRoom(GamePlayer player, int roomId, string pwd, int type, int hallType, bool isInvite)
		{
			RoomMgr.AddAction(new EnterRoomAction(player, roomId, pwd, type, hallType, isInvite));
		}
		public static void ExitRoom(BaseRoom room, GamePlayer player)
		{
			RoomMgr.AddAction(new ExitRoomAction(room, player));
		}
		public static void StartGame(BaseRoom room)
		{
			RoomMgr.AddAction(new StartGameAction(room));
		}
		public static void UpdatePlayerState(GamePlayer player, byte state)
		{
			RoomMgr.AddAction(new UpdatePlayerStateAction(player, player.CurrentRoom, state));
		}
		public static void UpdateRoomPos(BaseRoom room, int pos, bool isOpened)
		{
			RoomMgr.AddAction(new UpdateRoomPosAction(room, pos, isOpened));
		}
		public static void KickPlayer(BaseRoom baseRoom, byte index)
		{
			RoomMgr.AddAction(new KickPlayerAction(baseRoom, (int)index));
		}
		public static void EnterWaitingRoom(GamePlayer player, int hallType)
		{
			RoomMgr.AddAction(new EnterWaitingRoomAction(player, hallType));
		}
		public static void ExitWaitingRoom(GamePlayer player)
		{
			RoomMgr.AddAction(new ExitWaitRoomAction(player));
		}
		public static void CancelPickup(BattleServer server, BaseRoom room)
		{
			RoomMgr.AddAction(new CancelPickupAction(server, room));
		}
		public static void UpdateRoomGameType(BaseRoom room, GSPacketIn packet, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId, bool isArea)
		{
			RoomMgr.AddAction(new RoomSetupChangeAction(room, packet, roomType, timeMode, hardLevel, levelLimits, mapId, isArea));
		}
		internal static void SwitchTeam(GamePlayer gamePlayer)
		{
			RoomMgr.AddAction(new SwitchTeamAction(gamePlayer));
		}
		public static List<BaseRoom> GetAllUsingRoom()
		{
			List<BaseRoom> list = new List<BaseRoom>();
			BaseRoom[] rooms;
			Monitor.Enter(rooms = RoomMgr.m_rooms);
			try
			{
				BaseRoom[] rooms2 = RoomMgr.m_rooms;
				for (int i = 0; i < rooms2.Length; i++)
				{
					BaseRoom room = rooms2[i];
					if (room.IsUsing)
					{
						list.Add(room);
					}
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return list;
		}
		public static List<BaseRoom> GetWaitingRoom(int hallType, int condition, int count, int roomid)
		{
			if (condition == 0)
			{
				if (hallType == 1)
				{
					condition = 3;
				}
				else
				{
					condition = 1001;
				}
			}
			BaseRoom[] rooms;
			Monitor.Enter(rooms = RoomMgr.m_rooms);
			List<BaseRoom> result;
			try
			{
				List<BaseRoom> list = new List<BaseRoom>();
				if (hallType == 1)
				{
					List<BaseRoom> fightRooms = new List<BaseRoom>();
					BaseRoom[] rooms2 = RoomMgr.m_rooms;
					for (int i = 0; i < rooms2.Length; i++)
					{
						BaseRoom rm = rooms2[i];
						if (rm.IsUsing && (rm.RoomType == eRoomType.Freedom || rm.RoomType == eRoomType.Match) && rm.RoomId != roomid)
						{
							fightRooms.Add(rm);
						}
					}
					if (fightRooms.Count > count)
					{
						if (condition == 3)
						{
							foreach (BaseRoom rm in fightRooms)
							{
								if (!rm.IsPlaying)
								{
									list.Add(rm);
								}
							}
						}
						else
						{
							List<BaseRoom> rms = new List<BaseRoom>();
							foreach (BaseRoom room in fightRooms)
							{
								if (room.RoomType == (eRoomType)(condition - 4))
								{
									rms.Add(room);
								}
							}
							if (rms.Count > count)
							{
								foreach (BaseRoom rm in rms)
								{
									if (!rm.IsPlaying)
									{
										list.Add(rm);
									}
								}
								while (list.Count < count)
								{
									int id = RoomMgr.rand.Next(rms.Count);
									if (!list.Contains(rms[id]))
									{
										list.Add(rms[id]);
									}
								}
							}
							else
							{
								list = rms;
							}
						}
						while (list.Count < count)
						{
							int id = RoomMgr.rand.Next(fightRooms.Count);
							if (!list.Contains(fightRooms[id]))
							{
								list.Add(fightRooms[id]);
							}
						}
					}
					else
					{
						list = fightRooms;
					}
				}
				else
				{
					List<BaseRoom> fightRooms = new List<BaseRoom>();
					BaseRoom[] rooms2 = RoomMgr.m_rooms;
					for (int i = 0; i < rooms2.Length; i++)
					{
						BaseRoom rm = rooms2[i];
						if (rm.IsUsing && (rm.RoomType == eRoomType.Boss || rm.RoomType == eRoomType.Treasure) && rm.RoomId != roomid)
						{
							fightRooms.Add(rm);
						}
					}
					if (fightRooms.Count > count)
					{
						if (condition > 1000)
						{
							if (condition == 1001 || condition == 1004 || condition == 1006)
							{
								foreach (BaseRoom rm in fightRooms)
								{
									if (!rm.IsPlaying)
									{
										list.Add(rm);
									}
								}
							}
							else
							{
								if (condition == 1002 || condition == 1003)
								{
									List<BaseRoom> rms = new List<BaseRoom>();
									foreach (BaseRoom room in fightRooms)
									{
										if (room.RoomType == (eRoomType)(condition - 999))
										{
											rms.Add(room);
										}
									}
									if (rms.Count > count)
									{
										foreach (BaseRoom rm in rms)
										{
											if (!rm.IsPlaying)
											{
												list.Add(rm);
											}
										}
										while (list.Count < count)
										{
											int id = RoomMgr.rand.Next(rms.Count);
											if (!list.Contains(rms[id]))
											{
												list.Add(rms[id]);
											}
										}
									}
									else
									{
										list = rms;
									}
								}
								else
								{
									List<BaseRoom> rms = new List<BaseRoom>();
									foreach (BaseRoom room in fightRooms)
									{
										if (room.HardLevel == (eHardLevel)(condition - 1007) && room.MapId != 10000)
										{
											rms.Add(room);
										}
									}
									if (rms.Count > count)
									{
										foreach (BaseRoom rm in rms)
										{
											if (!rm.IsPlaying)
											{
												list.Add(rm);
											}
										}
										if (list.Count < count)
										{
											int id = RoomMgr.rand.Next(rms.Count);
											if (!list.Contains(rms[id]))
											{
												list.Add(rms[id]);
											}
										}
									}
									else
									{
										list = rms;
									}
								}
							}
						}
						else
						{
							List<BaseRoom> rms = new List<BaseRoom>();
							foreach (BaseRoom room in fightRooms)
							{
								if (room.MapId == condition)
								{
									rms.Add(room);
								}
							}
							if (rms.Count > count)
							{
								foreach (BaseRoom rm in rms)
								{
									if (!rm.IsPlaying)
									{
										list.Add(rm);
									}
								}
								while (list.Count < count)
								{
									int id = RoomMgr.rand.Next(rms.Count);
									if (!list.Contains(rms[id]))
									{
										list.Add(rms[id]);
									}
								}
							}
							else
							{
								list = rms;
							}
						}
						while (list.Count < count)
						{
							int id = RoomMgr.rand.Next(fightRooms.Count);
							if (!list.Contains(fightRooms[id]))
							{
								list.Add(fightRooms[id]);
							}
						}
					}
					else
					{
						list = fightRooms;
					}
				}
				if (list.Count > count)
				{
					List<BaseRoom> removeList = new List<BaseRoom>();
					foreach (BaseRoom rm in list)
					{
						if (rm.NeedPassword)
						{
							removeList.Add(rm);
						}
						if (list.Count - removeList.Count == count)
						{
							break;
						}
					}
					foreach (BaseRoom rm in removeList)
					{
						list.Remove(rm);
					}
					while (list.Count > count)
					{
						int id = RoomMgr.rand.Next(list.Count);
						list.Remove(list[id]);
					}
				}
				result = list;
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return result;
		}
		public static void StartProxyGame(BaseRoom room, ProxyGame game)
		{
			RoomMgr.AddAction(new StartProxyGameAction(room, game));
		}
		public static void StopProxyGame(BaseRoom room)
		{
			RoomMgr.AddAction(new StopProxyGameAction(room));
		}
	}
}
