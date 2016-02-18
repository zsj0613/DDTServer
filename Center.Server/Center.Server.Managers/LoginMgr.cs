using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace Center.Server.Managers
{
	public class LoginMgr
	{
		private static Dictionary<int, Player> m_players = new Dictionary<int, Player>();
		private static object syc_obj = new object();
		public static void CreatePlayer(Player player)
		{
			Player older = null;
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				player.LastTime = DateTime.Now.Ticks;
				if (LoginMgr.m_players.ContainsKey(player.Id))
				{
					older = LoginMgr.m_players[player.Id];
					player.State = older.State;
					player.CurrentServer = older.CurrentServer;
					LoginMgr.m_players[player.Id] = player;
				}
				else
				{
					older = LoginMgr.GetPlayerByName(player.Name);
					if (older != null && LoginMgr.m_players.ContainsKey(older.Id))
					{
						LoginMgr.m_players.Remove(older.Id);
					}
					player.State = ePlayerState.NotLogin;
					LoginMgr.m_players.Add(player.Id, player);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			if (older != null && older.CurrentServer != null)
			{
				older.CurrentServer.SendKitoffUser(older.Id);
			}
		}
		public static bool TryLoginPlayer(int id, ServerClient server)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			bool result;
			try
			{
				if (LoginMgr.m_players.ContainsKey(id))
				{
					Player player = LoginMgr.m_players[id];
					if (player.CurrentServer == null)
					{
						player.CurrentServer = server;
						player.State = ePlayerState.Logining;
						result = true;
					}
					else
					{
						if (player.State == ePlayerState.Play)
						{
							player.CurrentServer.SendKitoffUser(id);
						}
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static void PlayerLogined(int id, ServerClient server)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				if (LoginMgr.m_players.ContainsKey(id))
				{
					Player player = LoginMgr.m_players[id];
					if (player != null)
					{
						player.CurrentServer = server;
						player.State = ePlayerState.Play;
					}
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static void PlayerLoginOut(int id, ServerClient server)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				if (LoginMgr.m_players.ContainsKey(id))
				{
					Player player = LoginMgr.m_players[id];
					if (player != null && player.CurrentServer == server)
					{
						player.CurrentServer = null;
						player.State = ePlayerState.NotLogin;
					}
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static Player GetPlayerByName(string name)
		{
			Player[] list = LoginMgr.GetAllPlayer();
			Player result;
			if (list != null)
			{
				Player[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					Player p = array[i];
					if (p.Name == name)
					{
						result = p;
						return result;
					}
				}
			}
			result = null;
			return result;
		}
		public static Player[] GetAllPlayer()
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			Player[] result;
			try
			{
				result = LoginMgr.m_players.Values.ToArray<Player>();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			return result;
		}
		public static void RemovePlayer(int playerId)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				if (LoginMgr.m_players.ContainsKey(playerId))
				{
					LoginMgr.m_players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static void RemovePlayer(List<Player> players)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			try
			{
				foreach (Player p in players)
				{
					LoginMgr.m_players.Remove(p.Id);
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
		}
		public static Player GetPlayer(int playerId)
		{
			object obj;
			Monitor.Enter(obj = LoginMgr.syc_obj);
			Player result;
			try
			{
				if (LoginMgr.m_players.ContainsKey(playerId))
				{
					result = LoginMgr.m_players[playerId];
					return result;
				}
			}
			finally
			{
				Monitor.Exit(obj);
			}
			result = null;
			return result;
		}
		public static ServerClient GetServerClient(int playerId)
		{
			Player player = LoginMgr.GetPlayer(playerId);
			ServerClient result;
			if (player != null)
			{
				result = player.CurrentServer;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static int GetOnlineCount()
		{
			Player[] list = LoginMgr.GetAllPlayer();
			int count = 0;
			Player[] array = list;
			for (int i = 0; i < array.Length; i++)
			{
				Player p = array[i];
				if (p.State != ePlayerState.NotLogin)
				{
					count++;
				}
			}
			return count;
		}
		public static Dictionary<int, int> GetOnlineForLine()
		{
			Dictionary<int, int> lines = new Dictionary<int, int>();
			Player[] list = LoginMgr.GetAllPlayer();
			Player[] array = list;
			for (int i = 0; i < array.Length; i++)
			{
				Player p = array[i];
				if (p.CurrentServer != null)
				{
					if (lines.ContainsKey(p.CurrentServer.Info.ID))
					{
						Dictionary<int, int> dictionary;
						int iD;
						(dictionary = lines)[iD = p.CurrentServer.Info.ID] = dictionary[iD] + 1;
					}
					else
					{
						lines.Add(p.CurrentServer.Info.ID, 1);
					}
				}
			}
			return lines;
		}
		public static List<Player> GetServerPlayers(ServerClient server)
		{
			List<Player> list = new List<Player>();
			Player[] players = LoginMgr.GetAllPlayer();
			Player[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				Player p = array[i];
				if (p.CurrentServer == server)
				{
					list.Add(p);
				}
			}
			return list;
		}
	}
}
