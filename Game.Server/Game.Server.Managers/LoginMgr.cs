using Bussiness;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Game.Server.Managers
{
	public class LoginMgr
	{
		private static Dictionary<int, GameClient> _players = new Dictionary<int, GameClient>();
		private static object _locker = new object();
		public static void Add(int player, GameClient server)
		{
			GameClient temp = null;
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(player))
				{
					GameClient client = LoginMgr._players[player];
					if (client != null)
					{
						temp = client;
					}
					LoginMgr._players[player] = server;
				}
				else
				{
					LoginMgr._players.Add(player, server);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			if (temp != null)
			{
				temp.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext", new object[0]));
				temp.Disconnect();
			}
		}
		public static void Remove(int player)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(player))
				{
					LoginMgr._players.Remove(player);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
		}
		public static GamePlayer LoginClient(int playerId)
		{
			GameClient client = null;
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(playerId))
				{
					client = LoginMgr._players[playerId];
					LoginMgr._players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			GamePlayer result;
			if (client != null)
			{
				result = client.Player;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static void ClearLoginPlayer(int playerId)
		{
			GameClient client = null;
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(playerId))
				{
					client = LoginMgr._players[playerId];
					LoginMgr._players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			if (client != null)
			{
				client.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext", new object[0]));
				client.Disconnect();
			}
		}
		public static void ClearLoginPlayer(int playerId, GameClient client)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			try
			{
				if (LoginMgr._players.ContainsKey(playerId) && LoginMgr._players[playerId] == client)
				{
					LoginMgr._players.Remove(playerId);
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
		}
		public static bool ContainsUser(int playerId)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			bool result;
			try
			{
				if (LoginMgr._players.ContainsKey(playerId) && LoginMgr._players[playerId].IsConnected)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
			return result;
		}
		public static bool ContainsUser(string account)
		{
			object locker;
			Monitor.Enter(locker = LoginMgr._locker);
			bool result;
			try
			{
				foreach (GameClient client in LoginMgr._players.Values)
				{
					if (client != null && client.Player != null && client.Player.Account == account)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			finally
			{
				Monitor.Exit(locker);
			}
			return result;
		}
	}
}
