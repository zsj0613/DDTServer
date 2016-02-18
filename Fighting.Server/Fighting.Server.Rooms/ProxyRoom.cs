using Game.Base.Packets;
using Game.Logic;
using Lsj.Util.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Fighting.Server.Rooms
{
	public class ProxyRoom : IComparable
	{
		private static LogProvider log => LogProvider.Default;
		private List<IGamePlayer> m_players;
		private int m_roomId;
		private int m_orientRoomId;
		private ServerClient m_client;
		public int PickUpCount;
		public bool IsPlaying;
		public eGameType GameType;
		public int GuildId;
		public int AreaID;
		public string GuildName;
		public int AvgLevel;
		public int FightPower = 0;
		private BaseGame m_game;
		public int RoomId
		{
			get
			{
				return this.m_roomId;
			}
		}
		public ServerClient Client
		{
			get
			{
				return this.m_client;
			}
		}
		public int PlayerCount
		{
			get
			{
				return this.m_players.Count;
			}
		}
		public BaseGame Game
		{
			get
			{
				return this.m_game;
			}
		}

        public bool IsArea
        {
            get;
            private set;
        }

        public ProxyRoom(int roomId, int orientRoomId, IGamePlayer[] players, ServerClient client, int totallevel, int totalFightPower,bool IsArea)
		{
			this.m_roomId = roomId;
			this.m_orientRoomId = orientRoomId;
			this.m_players = new List<IGamePlayer>();
			this.m_players.AddRange(players);
			this.m_client = client;
			this.FightPower = totalFightPower;
			this.AvgLevel = totallevel / players.Count<IGamePlayer>();
			this.PickUpCount = 0;
            this.IsArea = IsArea;
		}
		public void SendToAll(GSPacketIn pkg)
		{
			this.SendToAll(pkg, null);
		}
		public void SendToAll(GSPacketIn pkg, IGamePlayer except)
		{
			this.m_client.SendToRoom(this.m_orientRoomId, pkg, except);
		}
		public List<IGamePlayer> GetPlayers()
		{
			List<IGamePlayer> list = new List<IGamePlayer>();
			List<IGamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				list.AddRange(this.m_players);
			}
			finally
			{
				Monitor.Exit(players);
			}
			return list;
		}
		public IGamePlayer GetPlayer(int playerId)
		{
			List<IGamePlayer> players;
			Monitor.Enter(players = this.m_players);
			IGamePlayer result;
			try
			{
				foreach (IGamePlayer player in this.m_players)
				{
					if (player.PlayerCharacter.ID == playerId)
					{
						result = player;
						return result;
					}
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			result = null;
			return result;
		}
		public bool RemovePlayer(IGamePlayer player)
		{
			bool result = false;
			List<IGamePlayer> players;
			Monitor.Enter(players = this.m_players);
			try
			{
				if (this.m_players.Remove(player))
				{
					result = true;
				}
			}
			finally
			{
				Monitor.Exit(players);
			}
			if (this.PlayerCount == 0)
			{
				ProxyRoomMgr.RemoveRoom(this);
				this.Dispose();
			}
			else
			{
				this.m_client.SendRemovePlayer(player.PlayerCharacter.ID, this.m_orientRoomId);
			}
			return result;
		}
		public void StartGame(BaseGame game)
		{
			this.IsPlaying = true;
			this.m_game = game;
			game.GameStopped += new GameEventHandle(this.game_GameStopped);
			this.m_client.SendStartGame(this.m_orientRoomId, game);
		}
		private void game_GameStopped(AbstractGame game)
		{
			this.m_game.GameStopped -= new GameEventHandle(this.game_GameStopped);
			this.IsPlaying = false;
			this.m_client.SendStopGame(this.m_orientRoomId, this.m_game.Id, this.RoomId);
		}
		public void Dispose()
		{
			this.m_client.RemoveRoom(this.m_orientRoomId, this);
		}
		public void CancelPickUp()
		{
			this.m_client.PickUpNPC(this.m_orientRoomId, this);
		}
		public void SendCanPickUpNpc()
		{
			GSPacketIn pkg = new GSPacketIn(50);
			pkg.WriteBoolean(true);
			this.SendToAll(pkg);
		}
		public override string ToString()
		{
			return string.Format("RoomId:{0} PlayerCount:{1},IsPlaying:{2},GuildId:{3},GameType:{4},Game:{5}", new object[]
			{
				this.m_roomId,
				this.m_players.Count,
				this.IsPlaying,
				this.GuildId,
				this.GameType,
				this.Game
			});
		}
		public void LogFight(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
		{
			this.m_client.SendLogFight(_roomId, _roomType, _fightType, _changeTeam, _playBegin, _playEnd, _userCount, _mapId, _teamA, _teamB, _playResult, _winTeam, BossWar);
		}
		public int CompareTo(object obj)
		{
			return this.m_roomId.CompareTo(((ProxyRoom)obj).m_roomId);
		}
	}
}
