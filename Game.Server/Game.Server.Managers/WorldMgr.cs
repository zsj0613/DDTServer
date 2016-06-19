using Bussiness;
using Game.Base.Packets;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
namespace Game.Server.Managers
{
	public sealed class WorldMgr
	{
		private static LogProvider log => LogProvider.Default;
		private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();
		private static Dictionary<int, GamePlayer> m_players = new Dictionary<int, GamePlayer>();
		public static Scene _marryScene;
		public static Scene _spaScene;
		private static RSACryptoServiceProvider m_rsa;
		public static Scene MarryScene
		{
			get
			{
				return WorldMgr._marryScene;
			}
		}
		public static Scene SpaScene
		{
			get
			{
				return WorldMgr._spaScene;
			}
		}
		public static RSACryptoServiceProvider RsaCryptor
		{
			get
			{
				return WorldMgr.m_rsa;
			}
		}
		public static bool Init()
		{
			bool result = false;
			try
			{
				WorldMgr.m_rsa = new RSACryptoServiceProvider();
				WorldMgr.m_rsa.FromXmlString(GameServer.Instance.Config.PrivateKey);
				WorldMgr.m_players.Clear();
				using (ServiceBussiness db = new ServiceBussiness())
				{
					ServerInfo info = db.GetServiceSingle(GameServer.Instance.Config.ServerID);
					if (info != null)
					{
						WorldMgr._marryScene = new Scene(info);
						WorldMgr._spaScene = new Scene(info);
						result = true;
					}
				}
			}
			catch (Exception e)
			{
				WorldMgr.log.Error("WordMgr Init", e);
			}
			return result;
		}
		public static bool AddPlayer(int playerId, GamePlayer player)
		{
			WorldMgr.m_clientLocker.AcquireWriterLock(-1);
			bool result;
			try
			{
				if (WorldMgr.m_players.ContainsKey(playerId))
				{
					result = false;
					return result;
				}
				WorldMgr.m_players.Add(playerId, player);
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseWriterLock();
			}
			result = true;
			return result;
		}
		public static bool RemovePlayer(int playerId)
		{
			WorldMgr.m_clientLocker.AcquireWriterLock(-1);
			GamePlayer player = null;
			try
			{
				if (WorldMgr.m_players.ContainsKey(playerId))
				{
					player = WorldMgr.m_players[playerId];
					WorldMgr.m_players.Remove(playerId);
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseWriterLock();
			}
			bool result;
			if (player != null)
			{
				GameServer.Instance.LoginServer.SendUserOffline(playerId, player.PlayerCharacter.ConsortiaID);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public static GamePlayer GetPlayerById(int playerId)
		{
			GamePlayer player = null;
			WorldMgr.m_clientLocker.AcquireReaderLock(-1);
			try
			{
				if (WorldMgr.m_players.ContainsKey(playerId))
				{
					player = WorldMgr.m_players[playerId];
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseReaderLock();
			}
			return player;
		}
		public static GamePlayer GetClientByPlayerNickName(string nickName)
		{
			GamePlayer[] list = WorldMgr.GetAllPlayers();
			GamePlayer[] array = list;
			GamePlayer result;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer client = array[i];
				if (client.PlayerCharacter.NickName == nickName)
				{
					result = client;
					return result;
				}
			}
			result = null;
			return result;
		}
		public static GamePlayer[] GetAllPlayers()
		{
			List<GamePlayer> list = new List<GamePlayer>();
			WorldMgr.m_clientLocker.AcquireReaderLock(-1);
			try
			{
				foreach (GamePlayer p in WorldMgr.m_players.Values)
				{
					if (p != null && p.PlayerCharacter != null)
					{
						list.Add(p);
					}
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseReaderLock();
			}
			return list.ToArray();
		}
		public static GamePlayer[] GetAllPlayersNoGame()
		{
			List<GamePlayer> list = new List<GamePlayer>();
			WorldMgr.m_clientLocker.AcquireReaderLock(-1);
			try
			{
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					GamePlayer p = allPlayers[i];
					if (p.CurrentRoom == null)
					{
						list.Add(p);
					}
				}
			}
			finally
			{
				WorldMgr.m_clientLocker.ReleaseReaderLock();
			}
			return list.ToArray();
		}
		public static void SendToAll(GSPacketIn pkg)
		{
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(pkg);
			}
		}
		public static GSPacketIn SendSysNotice(string msg, ItemInfo item, GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(10);
			pkg.WriteInt(1);
			int count = 0;
			for (int i = 0; i < msg.Length; i++)
			{
				char ch = msg[i];
				if (ch == '|')
				{
					pkg.WriteString(msg.Remove(count, 1));
					pkg.WriteByte(1);
					pkg.WriteInt(count);
					pkg.WriteInt(item.TemplateID);
					pkg.WriteInt(item.ItemID);
				}
				count++;
			}
			WorldMgr.SendToAll(pkg);
			return pkg;
		}

        public static string GetPlayerStringByPlayerNickName(string nickName)
		{
			GamePlayer[] list = WorldMgr.GetAllPlayers();
			GamePlayer[] array = list;
			string result;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer client = array[i];
				if (client.PlayerCharacter.NickName == nickName)
				{
					result = client.ToString();
					return result;
				}
			}
			result = nickName + " is not online!";
			return result;
		}
		public static void OnPlayerOffline(int playerid, int consortiaID)
		{
			WorldMgr.ChangePlayerState(playerid, false, consortiaID);
		}
		public static void OnPlayerOnline(int playerid, int consortiaID, int areaid, bool isSend)
		{
			WorldMgr.ChangePlayerState(playerid, true, consortiaID);
			if (isSend)
			{
				List<BattleServer> servers = BattleMgr.GetAllBattles();
				foreach (BattleServer server in servers)
				{
					if (server != null && server.IsOpen)
					{
						server.Connector.SendKitOffPlayer(playerid, areaid);
					}
				}
			}
		}
		public static void ChangePlayerState(int playerID, bool state, int consortiaID)
		{
			GSPacketIn pkg = null;
			GamePlayer[] list = WorldMgr.GetAllPlayers();
			GamePlayer[] array = list;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer client = array[i];
				if ((client.Friends != null && client.Friends.ContainsKey(playerID) && client.Friends[playerID] == 0) || (client.PlayerCharacter.ConsortiaID != 0 && client.PlayerCharacter.ConsortiaID == consortiaID))
				{
					if (pkg == null)
					{
						pkg = client.Out.SendFriendState(playerID, state);
					}
					else
					{
						client.Out.SendTCP(pkg);
					}
				}
			}
		}
	}
}
