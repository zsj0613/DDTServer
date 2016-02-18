using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using Lsj.Util.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
namespace Game.Server.Battle
{
	public class BattleServer
	{
		public static LogProvider log => LogProvider.Default;
		private int m_serverId;
		private FightServerConnector m_server;
		private Dictionary<int, BaseRoom> m_rooms;
		private string m_ip;
		private int m_port;
		private string m_loginKey;
		public int RetryCount = 0;
		public DateTime LastRetryTime = DateTime.Now;
		public int ServerType;
		public int RoomCount;
		public int WaitingRoomCount;
		public bool IsOpen;
        public event EventHandler Disconnected;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.Disconnected = (EventHandler)Delegate.Combine(this.Disconnected, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.Disconnected = (EventHandler)Delegate.Remove(this.Disconnected, value);
        //    }
        //}
		public FightServerConnector Server
		{
			get
			{
				return this.m_server;
			}
		}
		public int ServerId
		{
			get
			{
				return this.m_serverId;
			}
		}
		public FightServerConnector Connector
		{
			get
			{
				return this.m_server;
			}
		}
		public bool IsActive
		{
			get
			{
				return this.m_server.IsConnected;
			}
		}
		public string Ip
		{
			get
			{
				return this.m_ip;
			}
		}
		public int Port
		{
			get
			{
				return this.m_port;
			}
		}
		public string LoginKey
		{
			get
			{
				return this.m_loginKey;
			}
		}
        public bool IsArea
        {
            get;
            private set;
        }
		public BattleServer(int serverId, string ip, int port, string loginKey, bool isOpen,bool IsArea)
		{
			this.m_serverId = serverId;
			this.m_ip = ip;
			this.m_port = port;
			this.m_loginKey = loginKey;
			this.IsOpen = isOpen;
            this.IsArea = IsArea;
			this.m_server = new FightServerConnector(this, ip, port, loginKey);
			this.m_rooms = new Dictionary<int, BaseRoom>();
			this.m_server.Disconnected += new ClientEventHandle(this.m_server_Disconnected);
			this.m_server.Connected += new ClientEventHandle(this.m_server_Connected);
		}
		public BattleServer Clone()
		{
			return new BattleServer(this.m_serverId, this.m_ip, this.m_port, this.m_loginKey, this.IsOpen,this.IsArea);
		}
		public void Start()
		{
			if (!this.m_server.IsConnected)
			{
				try
				{
					if (!this.m_server.Connect())
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.InvokeDisconnect));
					}
				}
				catch (Exception ex)
				{
					BattleServer.log.ErrorFormat("Batter server {0}:{1} can't connected!", this.Ip, this.Port);
					BattleServer.log.Error(ex.Message);
				}
			}
		}
		public void Stop()
		{
			if (this.m_server.IsConnected)
			{
				try
				{
					this.m_server.Disconnect();
				}
				catch (Exception ex)
				{
					BattleServer.log.ErrorFormat("Stop battle server error:{0}", ex.Message);
				}
			}
		}
		public void m_server_Connected(BaseClient client)
		{
		}
		private void InvokeDisconnect(object state)
		{
			this.m_server_Disconnected(this.m_server);
		}
		public void m_server_Disconnected(BaseClient client)
		{
			this.RemoveAllRoom();
			if (this.Disconnected != null)
			{
				this.Disconnected(this, null);
			}
		}
		public BaseRoom FindRoom(int roomId)
		{
			BaseRoom room = null;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(roomId))
				{
					room = this.m_rooms[roomId];
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			return room;
		}
		public bool AddRoom(BaseRoom room)
		{
			bool result = false;
			BaseRoom old = null;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(room.RoomId))
				{
					old = this.m_rooms[room.RoomId];
					this.m_rooms.Remove(room.RoomId);
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (old != null && old.Game != null)
			{
				old.Game.Stop();
			}
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (!this.m_rooms.ContainsKey(room.RoomId))
				{
					this.m_rooms.Add(room.RoomId, room);
					result = true;
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (result)
			{
				this.m_server.SendAddRoom(room);
			}
			return result;
		}
		public bool RemoveRoom(BaseRoom room)
		{
			bool result = false;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				result = this.m_rooms.ContainsKey(room.RoomId);
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (result)
			{
				this.m_server.SendRemoveRoom(room);
			}
			return result;
		}
		public void RemoveRoomImp(int roomId, int fightRoomId)
		{
			BaseRoom room = null;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(roomId) && this.m_rooms[roomId].FightRoomID == fightRoomId)
				{
					room = this.m_rooms[roomId];
					this.m_rooms.Remove(roomId);
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (room != null)
			{
				if (room.IsPlaying && room.Game == null)
				{
					RoomMgr.CancelPickup(this, room);
				}
				else
				{
					RoomMgr.StopProxyGame(room);
				}
			}
		}
		public void RandomNPC(int roomId, int fightRoomId)
		{
			BaseRoom room = null;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(roomId) && this.m_rooms[roomId].FightRoomID == fightRoomId)
				{
					room = this.m_rooms[roomId];
					this.m_rooms.Remove(roomId);
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (room != null)
			{
				RoomMgr.AddAction(new PickUpNPCAction(room));
			}
		}
		public void UpdateRoomId(int roomId, int fightRoomId)
		{
			BaseRoom room = this.FindRoom(roomId);
			if (room != null)
			{
				room.FightRoomID = fightRoomId;
			}
		}
		public void StartGame(int roomId, ProxyGame game)
		{
			BaseRoom room = this.FindRoom(roomId);
			if (room != null)
			{
				RoomMgr.StartProxyGame(room, game);
			}
		}
		public void StopGame(int roomId, int gameId, int fightRoomId)
		{
			BaseRoom room = this.FindRoom(roomId);
			if (room != null && fightRoomId == room.FightRoomID)
			{
				RoomMgr.StopProxyGame(room);
				Dictionary<int, BaseRoom> rooms;
				Monitor.Enter(rooms = this.m_rooms);
				try
				{
					this.m_rooms.Remove(roomId);
				}
				finally
				{
					Monitor.Exit(rooms);
				}
			}
		}
		public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
		{
			BaseRoom room = this.FindRoom(roomId);
			if (room != null)
			{
				if (exceptId != 0)
				{
					GamePlayer player = WorldMgr.GetPlayerById(exceptId);
					if (player != null)
					{
						if (player.GamePlayerId == exceptGameId)
						{
							room.SendToAll(pkg, player);
						}
						else
						{
							room.SendToAll(pkg);
						}
					}
				}
				else
				{
					room.SendToAll(pkg);
				}
			}
		}
		public void SendToUser(int playerid, GSPacketIn pkg, int gameId)
		{
			GamePlayer player = WorldMgr.GetPlayerById(playerid);
			if (player != null)
			{
				player.SendTCP(pkg);
			}
		}
		public void UpdatePlayerGameId(int playerid, int gamePlayerId)
		{
			GamePlayer player = WorldMgr.GetPlayerById(playerid);
			if (player != null)
			{
				player.GamePlayerId = gamePlayerId;
			}
		}
		public void RemoveAllRoom()
		{
			BaseRoom[] list = null;
			Dictionary<int, BaseRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				list = this.m_rooms.Values.ToArray<BaseRoom>();
				this.m_rooms.Clear();
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			BaseRoom[] array = list;
			for (int i = 0; i < array.Length; i++)
			{
				BaseRoom rm = array[i];
				if (rm != null)
				{
					if (rm.IsPlaying && rm.Game == null)
					{
						RoomMgr.CancelPickup(this, rm);
						rm.RemoveAllPlayer();
					}
					else
					{
						rm.RemoveAllPlayer();
						RoomMgr.StopProxyGame(rm);
					}
				}
			}
		}
		public void UpdateServerProperties()
		{
			this.m_server.SendServerProperties();
		}
		public override string ToString()
		{
			return string.Format("ServerID:{0},Ip:{1},Port:{2},IsConnected:{3},IsOpen:{4},RoomCount:{5}", new object[]
			{
				this.m_serverId,
				this.m_server.RemoteEP.Address,
				this.m_server.RemoteEP.Port,
				this.m_server.IsConnected,
				this.IsOpen,
				this.m_rooms.Count
			});
		}
	}
}
