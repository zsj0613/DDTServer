using Game.Logic;
using Game.Logic.Phy.Maps;

using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Games
{
	public class GameMgr
	{
		private static LogProvider log => LogProvider.Default;
		public static readonly long THREAD_INTERVAL = 40L;
		public static int DELAY_TIME = 0;
		private static List<BaseGame> m_games;
		private static Thread m_thread;
		private static bool m_running;
		private static int m_serverId;
		private static int m_boxBroadcastLevel;
		private static int m_gameId;
		private static readonly int CLEAR_GAME_INTERVAL = 5000;
		private static long m_clearGamesTimer;
		public static Thread Thread
		{
			get
			{
				return GameMgr.m_thread;
			}
		}
		public static int BoxBroadcastLevel
		{
			get
			{
				return GameMgr.m_boxBroadcastLevel;
			}
		}
		public static bool Setup(int serverId, int boxBroadcastLevel)
		{
			GameMgr.m_thread = new Thread(new ThreadStart(GameMgr.GameThread));
			GameMgr.m_games = new List<BaseGame>();
			GameMgr.m_serverId = serverId;
			GameMgr.m_boxBroadcastLevel = boxBroadcastLevel;
			GameMgr.m_gameId = 0;
			return true;
		}
		public static bool Start()
		{
			if (!GameMgr.m_running)
			{
				GameMgr.m_running = true;
				GameMgr.m_thread.Start();
			}
			return true;
		}
		public static void Stop()
		{
			if (GameMgr.m_running)
			{
				GameMgr.m_running = false;
				GameMgr.m_thread.Join();
			}
		}
		private static void GameThread()
		{
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			long balance = 0L;
			GameMgr.m_clearGamesTimer = TickHelper.GetTickCount();
			while (GameMgr.m_running)
			{
				long start = TickHelper.GetTickCount();
				int gameCount = 0;
				try
				{
					gameCount = GameMgr.UpdateGames(start);
					if (GameMgr.m_clearGamesTimer <= start)
					{
						GameMgr.m_clearGamesTimer += (long)GameMgr.CLEAR_GAME_INTERVAL;
						ArrayList temp = new ArrayList();
						foreach (BaseGame g in GameMgr.m_games)
						{
							if (g.GameState == eGameState.Stopped)
							{
								temp.Add(g);
							}
						}
						foreach (BaseGame g in temp)
						{
							GameMgr.m_games.Remove(g);
						}
						ThreadPool.QueueUserWorkItem(new WaitCallback(GameMgr.ClearStoppedGames), temp);
					}
				}
				catch (Exception ex)
				{
					GameMgr.log.Error("Game Mgr Thread Error:", ex);
				}
				long end = TickHelper.GetTickCount();
				balance += GameMgr.THREAD_INTERVAL - (end - start);
				if (end - start > GameMgr.THREAD_INTERVAL * 2L)
				{
					GameMgr.log.WarnFormat("Game Mgr spent too much times: {0} ms, count:{1}", end - start, gameCount);
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
				if (GameMgr.DELAY_TIME > 0)
				{
					GameMgr.log.ErrorFormat("Delay for {0} ms!", GameMgr.DELAY_TIME);
					Thread.Sleep(GameMgr.DELAY_TIME);
				}
			}
		}
		private static void ClearStoppedGames(object state)
		{
			ArrayList temp = state as ArrayList;
			foreach (BaseGame g in temp)
			{
				try
				{
					g.Dispose();
				}
				catch (Exception ex)
				{
					GameMgr.log.Error("game dispose error:", ex);
				}
			}
		}
		private static int UpdateGames(long tick)
		{
			IList games = GameMgr.GetAllGame();
			int result;
			if (games != null)
			{
				foreach (BaseGame g in games)
				{
					try
					{
						g.Update(tick);
					}
					catch (Exception ex)
					{
						GameMgr.log.Error("Game  updated error:", ex);
					}
				}
				result = games.Count;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public static List<BaseGame> GetAllGame()
		{
			List<BaseGame> list = new List<BaseGame>();
			List<BaseGame> games;
			Monitor.Enter(games = GameMgr.m_games);
			try
			{
				list.AddRange(GameMgr.m_games);
			}
			finally
			{
				Monitor.Exit(games);
			}
			return list;
		}
		public static BaseGame StartPVPGame(int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			BaseGame result;
			try
			{
				int totalLevel = 0;
				foreach (IGamePlayer player in red)
				{
					totalLevel += player.PlayerCharacter.Grade;
				}
				foreach (IGamePlayer player in blue)
				{
					totalLevel += player.PlayerCharacter.Grade;
				}
				int averageLevel = totalLevel / (red.Count + blue.Count);
				int mapId = MapMgr.GetMapIndex(averageLevel, GameMgr.m_serverId);
				if (mapIndex == 0)
				{
					mapIndex = mapId;
				}
				Map map = MapMgr.AllocateMapInstance(mapIndex);
				if (map != null)
				{
					PVPGame game = new PVPGame(GameMgr.m_gameId++, roomId, red, blue, map, roomType, gameType, timeType, 0);
					//game.GameOverLog += new BaseGame.GameOverLogEventHandle(LogMgr.LogFightAdd);
					List<BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(game);
					}
					finally
					{
						Monitor.Exit(games);
					}
					game.Prepare();
					result = game;
				}
				else
				{
					result = null;
				}
			}
			catch (Exception e)
			{
				GameMgr.log.Error("Create game error:", e);
				result = null;
			}
			return result;
		}
		public static BaseGame StartPVEGame(int roomId, List<IGamePlayer> players, int copyId, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int levelLimits)
		{
			BaseGame result;
			try
			{
				PveInfo info;
				if (copyId == 0 || copyId == 10000)
				{
					info = PveInfoMgr.GetPveInfoByType(roomType, levelLimits);
				}
				else
				{
					info = PveInfoMgr.GetPveInfoById(copyId);
				}
				if (info != null)
				{
					PVEGame game = new PVEGame(GameMgr.m_gameId++, roomId, info, players, null, roomType, gameType, timeType, hardLevel);
					List<BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(game);
					}
					finally
					{
						Monitor.Exit(games);
					}
					game.Prepare();
					result = game;
				}
				else
				{
					result = null;
				}
			}
			catch (Exception e)
			{
				GameMgr.log.Error("Create game error:", e);
				result = null;
			}
			return result;
		}
	}
}
