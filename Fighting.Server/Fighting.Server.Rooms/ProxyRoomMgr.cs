using Fighting.Server.Games;
using Game.Base.Packets;
using Game.Logic;
using Lsj.Util.Logs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Fighting.Server.Rooms
{
	public class ProxyRoomMgr
	{
		private static LogProvider log => LogProvider.Default;
		public static readonly int THREAD_INTERVAL = 40;
		public static int DELAY_TIME = 0;
		public static readonly int PICK_UP_INTERVAL = 20000;
		public static readonly int CLEAR_ROOM_INTERVAL = 1000;
		private static bool m_running = false;
		private static int m_serverId = 1;
		private static Queue<IAction> m_actionQueue = new Queue<IAction>();
		private static Thread m_thread;
		private static Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();
		private static int RoomIndex = 0;
		public static bool ShowTick = false;
		public static List<int> m_FreeCount = new List<int>();
		public static List<int> m_GuidCount = new List<int>();
		private static long m_nextPickTick = 0L;
		private static long m_nextClearTick = 0L;
		public static Thread Thread
		{
			get
			{
				return ProxyRoomMgr.m_thread;
			}
		}
		public static bool Setup()
		{
			ProxyRoomMgr.m_thread = new Thread(new ThreadStart(ProxyRoomMgr.RoomThread));
			return true;
		}
		public static void Start()
		{
			if (!ProxyRoomMgr.m_running)
			{
				ProxyRoomMgr.m_running = true;
				ProxyRoomMgr.m_thread.Start();
			}
		}
		public static void Stop()
		{
			if (ProxyRoomMgr.m_running)
			{
				ProxyRoomMgr.m_running = false;
				ProxyRoomMgr.m_thread.Join();
			}
		}
		public static void AddAction(IAction action)
		{
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = ProxyRoomMgr.m_actionQueue);
			try
			{
				ProxyRoomMgr.m_actionQueue.Enqueue(action);
			}
			finally
			{
				Monitor.Exit(actionQueue);
			}
		}
		private static void RoomThread()
		{
			long balance = 0L;
			ProxyRoomMgr.m_nextClearTick = TickHelper.GetTickCount();
			ProxyRoomMgr.m_nextPickTick = TickHelper.GetTickCount();
			while (ProxyRoomMgr.m_running)
			{
				long start = TickHelper.GetTickCount();
				try
				{
					ProxyRoomMgr.ExecuteActions();
					if (ProxyRoomMgr.m_nextPickTick <= start)
					{
						ProxyRoomMgr.m_nextPickTick += (long)ProxyRoomMgr.PICK_UP_INTERVAL;
						ProxyRoomMgr.PickUpRooms(start);
					}
					if (ProxyRoomMgr.m_nextClearTick <= start)
					{
						ProxyRoomMgr.m_nextClearTick += (long)ProxyRoomMgr.CLEAR_ROOM_INTERVAL;
						ProxyRoomMgr.ClearRooms(start);
					}
				}
				catch (Exception ex)
				{
					ProxyRoomMgr.log.Error("Room Mgr Thread Error:", ex);
				}
				long end = TickHelper.GetTickCount();
				balance += (long)ProxyRoomMgr.THREAD_INTERVAL - (end - start);
				if (balance > 0L)
				{
					Thread.Sleep((int)balance);
					balance = 0L;
				}
				else
				{
					if (balance < -1000L)
					{
						ProxyRoomMgr.log.WarnFormat("Room Mgr is delay {0} ms!", balance);
						balance += 1000L;
					}
				}
				if (ProxyRoomMgr.DELAY_TIME > 0)
				{
					ProxyRoomMgr.log.ErrorFormat("Delay for {0} ms!", ProxyRoomMgr.DELAY_TIME);
					Thread.Sleep(ProxyRoomMgr.DELAY_TIME);
				}
			}
		}
		private static void ExecuteActions()
		{
			IAction[] actions = null;
			Queue<IAction> actionQueue;
			Monitor.Enter(actionQueue = ProxyRoomMgr.m_actionQueue);
			try
			{
				if (ProxyRoomMgr.m_actionQueue.Count > 0)
				{
					actions = new IAction[ProxyRoomMgr.m_actionQueue.Count];
					ProxyRoomMgr.m_actionQueue.CopyTo(actions, 0);
					ProxyRoomMgr.m_actionQueue.Clear();
				}
			}
			finally
			{
				Monitor.Exit(actionQueue);
			}
			if (actions != null)
			{
				IAction[] array = actions;
				for (int i = 0; i < array.Length; i++)
				{
					IAction ac = array[i];
					try
					{
						ac.Execute();
					}
					catch (Exception ex)
					{
						ProxyRoomMgr.log.Error("RoomMgr execute action error:", ex);
					}
				}
			}
		}
		private static void PickUpRooms(long tick)
		{
			List<ProxyRoom> rooms = ProxyRoomMgr.GetWaitMatchRoomUnsafe();
			rooms.Sort();
			int pairs = 0;
			long begin = TickHelper.GetTickCount();
			if (ProxyRoomMgr.ShowTick)
			{
				log.DebugFormat("-----begin pickup----tick:{0}-----rooms:{1}", begin, rooms.Count);
			}
			foreach (ProxyRoom red in rooms)
			{
				red.PickUpCount++;
				int maxScore = -2147483648;
				ProxyRoom matchRoom = null;
				if (!red.IsPlaying)
				{
					if (red.GameType == eGameType.ALL)
					{
						foreach (ProxyRoom blue in rooms)
						{
                            //判断是不是同一个公会或不是同一个区
							if (blue.GuildId != red.GuildId || blue.AreaID != red.AreaID)
							{
								if (blue != red && !blue.IsPlaying && blue.PlayerCount == red.PlayerCount)
								{
									int score = ProxyRoomMgr.CalculateScore(red, blue);
									if (score > maxScore || matchRoom == null)
									{
                                        if (red.AreaID == blue.AreaID || red.IsArea && blue.IsArea)
                                        {
                                            maxScore = score;
                                            matchRoom = blue;
                                        }
									}
								}
							}
						}
					}
					else
					{
						if (red.GameType == eGameType.Guild)
						{
							foreach (ProxyRoom blue in rooms)
							{
								if (blue.GuildId == 0 || blue.AreaID != red.AreaID || blue.GuildId != red.GuildId)
								{
									if (blue != red && blue.GameType != eGameType.Free && !blue.IsPlaying && blue.PlayerCount == red.PlayerCount)
									{
										int score = ProxyRoomMgr.CalculateScore(red, blue);
										if (score >= maxScore || matchRoom == null)
										{
                                            if (red.AreaID == blue.AreaID || red.IsArea && blue.IsArea)
                                            {
                                                maxScore = score;
                                                matchRoom = blue;
                                            }
                                        }
									}
								}
							}
						}
						else
						{
							foreach (ProxyRoom blue in rooms)
							{
								if (blue.GuildId == 0 || blue.GuildId != red.GuildId || blue.AreaID != red.AreaID)
								{
									if (blue != red && blue.GameType != eGameType.Guild && !blue.IsPlaying && blue.PlayerCount == red.PlayerCount)
									{
										int score = ProxyRoomMgr.CalculateScore(red, blue);
										if (score >= maxScore || matchRoom == null)
										{
                                            if (red.AreaID == blue.AreaID || red.IsArea && blue.IsArea)
                                            {
                                                maxScore = score;
                                                matchRoom = blue;
                                            }
                                        }
									}
								}
							}
						}
					}
					if (matchRoom != null)
					{
						if (red.PickUpCount >= 2)
						{
							pairs++;
							ProxyRoomMgr.StartMatchGame(red, matchRoom);
						}
					}
				}
			}
			ProxyRoomMgr.SendInfoToAllGameServer();
			if (ProxyRoomMgr.ShowTick)
			{
				long end = TickHelper.GetTickCount();
				Console.WriteLine("-----end pickup----tick:{0}-----spends:{1} --- pairs:{2}", end, end - begin, pairs);
			}
		}
		private static int CalculateScore(ProxyRoom red, ProxyRoom blue)
		{
			int gameType = (int)blue.GameType;
			int power = Math.Abs(red.FightPower - blue.FightPower);
			int level = Math.Abs(red.AvgLevel - blue.AvgLevel);
			return gameType * 100000 - power;
		}
		private static void ClearRooms(long tick)
		{
			List<ProxyRoom> list = new List<ProxyRoom>();
			foreach (ProxyRoom rm in ProxyRoomMgr.m_rooms.Values)
			{
				if (!rm.IsPlaying && rm.Game != null)
				{
					list.Add(rm);
				}
			}
			foreach (ProxyRoom rm in list)
			{
				ProxyRoomMgr.m_rooms.Remove(rm.RoomId);
				try
				{
					rm.Dispose();
				}
				catch (Exception ex)
				{
					ProxyRoomMgr.log.Error("Room dispose error:", ex);
				}
			}
		}
		private static void StartMatchGame(ProxyRoom red, ProxyRoom blue)
		{
			try
			{
				int mapId = MapMgr.GetMapIndex((red.AvgLevel + blue.AvgLevel) / 2, ProxyRoomMgr.m_serverId);
				eGameType gameType = eGameType.Free;
				if (red.GameType == blue.GameType)
				{
					gameType = red.GameType;
				}
				else
				{
					if ((red.GameType == eGameType.ALL && blue.GameType == eGameType.Guild) || (blue.GameType == eGameType.ALL && red.GameType == eGameType.Guild))
					{
						gameType = eGameType.Guild;
					}
				}
				BaseGame game = GameMgr.StartBattleGame(red.GetPlayers(), red, blue.GetPlayers(), blue, mapId, eRoomType.Match, gameType, 3);
				if (game != null)
				{
					blue.StartGame(game);
					red.StartGame(game);
				}
			}
			catch (Exception e)
			{
				ProxyRoomMgr.log.Error("Start  Match Game Error:", e);
			}
		}
		public static bool AddRoomUnsafe(ProxyRoom room)
		{
			bool result;
			if (!ProxyRoomMgr.m_rooms.ContainsKey(room.RoomId))
			{
				ProxyRoomMgr.m_rooms.Add(room.RoomId, room);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public static bool RemoveRoomUnsafe(int roomId)
		{
			bool result;
			if (ProxyRoomMgr.m_rooms.ContainsKey(roomId))
			{
				ProxyRoomMgr.m_rooms.Remove(roomId);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public static ProxyRoom GetRoomUnsafe(int roomId)
		{
			ProxyRoom result;
			if (ProxyRoomMgr.m_rooms.ContainsKey(roomId))
			{
				result = ProxyRoomMgr.m_rooms[roomId];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static ProxyRoom[] GetAllRoom()
		{
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = ProxyRoomMgr.m_rooms);
			ProxyRoom[] allRoomUnsafe;
			try
			{
				allRoomUnsafe = ProxyRoomMgr.GetAllRoomUnsafe();
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return allRoomUnsafe;
		}
		public static int GetAllRoomCount()
		{
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = ProxyRoomMgr.m_rooms);
			int count;
			try
			{
				count = ProxyRoomMgr.m_rooms.Count;
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return count;
		}
		public static ProxyRoom[] GetAllRoomUnsafe()
		{
			ProxyRoom[] list = new ProxyRoom[ProxyRoomMgr.m_rooms.Values.Count];
			ProxyRoomMgr.m_rooms.Values.CopyTo(list, 0);
			return list;
		}
		public static List<ProxyRoom> GetWaitMatchRoom()
		{
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = ProxyRoomMgr.m_rooms);
			List<ProxyRoom> waitMatchRoomUnsafe;
			try
			{
				waitMatchRoomUnsafe = ProxyRoomMgr.GetWaitMatchRoomUnsafe();
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return waitMatchRoomUnsafe;
		}
		public static int GetWaitingRoomCount()
		{
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = ProxyRoomMgr.m_rooms);
			int result;
			try
			{
				int count = 0;
				foreach (ProxyRoom room in ProxyRoomMgr.m_rooms.Values)
				{
					if (!room.IsPlaying && room.Game == null)
					{
						count++;
					}
				}
				result = count;
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return result;
		}
		public static List<ProxyRoom> GetWaitMatchRoomUnsafe()
		{
			List<ProxyRoom> list = new List<ProxyRoom>();
			foreach (ProxyRoom room in ProxyRoomMgr.m_rooms.Values)
			{
				if (!room.IsPlaying && room.Game == null && room.PlayerCount > 0)
				{
					list.Add(room);
				}
			}
			return list;
		}
		public static int NextRoomId()
		{
			return Interlocked.Increment(ref ProxyRoomMgr.RoomIndex);
		}
		public static void AddRoom(ProxyRoom room)
		{
			ProxyRoomMgr.AddAction(new AddRoomAction(room));
		}
		public static void RemoveRoom(ProxyRoom room)
		{
			ProxyRoomMgr.AddAction(new RemoveRoomAction(room));
		}
		public static void FightWithNPC(ProxyRoom room)
		{
			ProxyRoomMgr.AddAction(new RandomNPCAction(room));
		}
		public static void SendInfoToAllGameServer()
		{
			GSPacketIn pkg = null;
			ServerClient[] clientlist = FightServer.Instance.GetAllClients();
			ServerClient[] array = clientlist;
			for (int i = 0; i < array.Length; i++)
			{
				ServerClient client = array[i];
				if (pkg == null)
				{
					pkg = client.SendInfoToGameServer(ProxyRoomMgr.GetAllRoomCount(), ProxyRoomMgr.GetWaitingRoomCount(), FightServer.Instance.Configuration.ServerType);
				}
				else
				{
					client.SendTCP(pkg);
				}
			}
		}
	}
}
