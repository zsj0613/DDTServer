using Bussiness;
using Fighting.Server.GameObjects;
using Fighting.Server.Rooms;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Game.Language;
namespace Fighting.Server.Games
{
	public class GameMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static readonly long THREAD_INTERVAL = 40L;
		public static int DELAY_TIME = 0;
		private static Dictionary<int, BaseGame> m_games;
		private static Thread m_thread;
		private static bool m_running;
		private static int m_serverId;
		private static int m_boxBroadcastLevel;
		private static int m_gameId;
		private static readonly int CLEAR_GAME_INTERVAL = 60000;
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
			GameMgr.m_games = new Dictionary<int, BaseGame>();
			GameMgr.m_serverId = serverId;
			GameMgr.m_boxBroadcastLevel = boxBroadcastLevel;
			GameMgr.m_gameId = 0;
			return true;
		}
		public static void Start()
		{
			if (!GameMgr.m_running)
			{
				GameMgr.m_running = true;
				GameMgr.m_thread.Start();
			}
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
			long balance = 0L;
			GameMgr.m_clearGamesTimer = TickHelper.GetTickCount();
			while (GameMgr.m_running)
			{
				long start = TickHelper.GetTickCount();
				try
				{
					GameMgr.UpdateGames(start);
					GameMgr.ClearStoppedGames(start);
				}
				catch (Exception ex)
				{
					GameMgr.log.Error("Room Mgr Thread Error:", ex);
				}
				long end = TickHelper.GetTickCount();
				balance += GameMgr.THREAD_INTERVAL - (end - start);
				if (balance > 0L)
				{
					Thread.Sleep((int)balance);
					balance = 0L;
				}
				else
				{
					if (balance < -1000L)
					{
						GameMgr.log.WarnFormat("Room Mgr is delay {0} ms!", balance);
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
		private static void UpdateGames(long tick)
		{
			IList games = GameMgr.GetGames();
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
			}
		}
		private static void ClearStoppedGames(long tick)
		{
			if (GameMgr.m_clearGamesTimer <= tick)
			{
				GameMgr.m_clearGamesTimer += (long)GameMgr.CLEAR_GAME_INTERVAL;
				ArrayList temp = new ArrayList();
				Dictionary<int, BaseGame> games;
				Monitor.Enter(games = GameMgr.m_games);
				try
				{
					foreach (BaseGame g in GameMgr.m_games.Values)
					{
						if (g.GameState == eGameState.Stopped)
						{
							temp.Add(g);
						}
					}
					foreach (BaseGame g in temp)
					{
						GameMgr.m_games.Remove(g.Id);
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
				finally
				{
					Monitor.Exit(games);
				}
			}
		}
		public static List<BaseGame> GetGames()
		{
			List<BaseGame> temp = new List<BaseGame>();
			Dictionary<int, BaseGame> games;
			Monitor.Enter(games = GameMgr.m_games);
			try
			{
				temp.AddRange(GameMgr.m_games.Values);
			}
			finally
			{
				Monitor.Exit(games);
			}
			return temp;
		}
		public static BaseGame FindGame(int id)
		{
			Dictionary<int, BaseGame> games;
			Monitor.Enter(games = GameMgr.m_games);
			BaseGame result;
			try
			{
				if (GameMgr.m_games.ContainsKey(id))
				{
					result = GameMgr.m_games[id];
					return result;
				}
			}
			finally
			{
				Monitor.Exit(games);
			}
			result = null;
			return result;
		}
		public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
		{
			BattleGame result;
			try
			{
				int index = MapMgr.GetMapIndex(mapIndex, (byte)roomType, GameMgr.m_serverId);
				Map map = MapMgr.AllocateMapInstance(index);
				if (map != null)
				{
					BattleGame game = new BattleGame(GameMgr.m_gameId++, red, roomRed, blue, roomBlue, map, roomType, gameType, timeType, RateMgr.GetNpcID());
					game.GameOverLog += new BaseGame.GameOverLogEventHandle(roomRed.LogFight);
					Dictionary<int, BaseGame> games;
					Monitor.Enter(games = GameMgr.m_games);
					try
					{
						GameMgr.m_games.Add(game.Id, game);
					}
					finally
					{
						Monitor.Exit(games);
					}
					game.Prepare();
					GameMgr.SendStartMessage(game);
					GameMgr.SendBufferList(game);
					GameMgr.UpdatePlayerGameId(game);
					result = game;
				}
				else
				{
					result = null;
				}
			}
			catch (Exception e)
			{
				GameMgr.log.Error("Create battle game error:", e);
				result = null;
			}
			return result;
		}
		private static void UpdatePlayerGameId(BattleGame game)
		{
			foreach (Player p in game.GetAllFightPlayers())
			{
				if (p.PlayerDetail is ProxyPlayer)
				{
					(p.PlayerDetail as ProxyPlayer).GameId = game.Id;
				}
			}
		}
		private static void SendStartMessage(BattleGame game)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(2);
			if (game.GameType == eGameType.Free)
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.free", new object[0]));
			}
			else
			{
				pkg.WriteString(LanguageMgr.GetTranslation("StartMessage.Consortia", new object[0]));
			}
			game.SendToAll(pkg, null);
		}
		private static void SendBufferList(BattleGame game)
		{
			foreach (Player p in game.GetAllFightPlayers())
			{
				List<BufferInfo> infos = (p.PlayerDetail as ProxyPlayer).Buffers;
				GSPacketIn pkg = new GSPacketIn(186, p.PlayerDetail.PlayerCharacter.ID);
				pkg.Parameter1 = p.Id;
				pkg.WriteInt(infos.Count);
				foreach (BufferInfo info in infos)
				{
					pkg.WriteInt(info.Type);
					pkg.WriteBoolean(info.IsExist);
					pkg.WriteDateTime(info.BeginDate);
					pkg.WriteInt(info.ValidDate);
					pkg.WriteInt(info.Value);
				}
				game.SendToAll(pkg);
			}
		}
	}
}
