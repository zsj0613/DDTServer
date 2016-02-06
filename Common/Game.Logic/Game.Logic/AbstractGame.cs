using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Game.Logic
{
	public class AbstractGame
	{		
		protected eMapType m_mapType;		
		private int m_disposed = 0;
		public event GameEventHandle RoomRest;
		public event GameEventHandle GameStarted;
        public event GameEventHandle GameStopped;
		public event GameEventHandle GameOverred;
		
		public int Id
		{
			get
			{
				return this.m_id;
			}
		}
		private int m_id;
		 
		
		public eRoomType RoomType
		{
			get
			{
				return this.m_roomType;
			}
		}
		protected eRoomType m_roomType;
		
		public eGameType GameType
		{
			get
			{
				return this.m_gameType;
			}
		}
		protected eGameType m_gameType;
		
		public int TimeType
		{
			get
			{
				return this.m_timeType;
			}
		}
		protected int m_timeType;
		
		
		public AbstractGame(int id, eRoomType roomType, eGameType gameType, int timeType)
		{
			this.m_id = id;
			this.m_roomType = roomType;
			this.m_gameType = gameType;
			this.m_timeType = timeType;
			switch (this.m_roomType)
			{
			   case eRoomType.Match:
				   this.m_mapType = eMapType.PairUp;
				   break;
			   case eRoomType.Freedom:
				   this.m_mapType = eMapType.Normal;
				   break;
			   default:
				   this.m_mapType = eMapType.Normal;
			   	break;
			}
		}
		public virtual void Start()
		{
			this.OnGameStarted();
		}
		public virtual void Stop()
		{
			this.OnGameStopped();
		}
		public virtual bool CanAddPlayer()
		{
			return false;
		}
		public virtual void Pause(int time)
		{
		}
		public virtual void Resume()
		{
		}
		public virtual void ProcessData(GSPacketIn pkg)
		{
		}
		public virtual Player AddPlayer(IGamePlayer player)
		{
			return null;
		}
		public virtual Player RemovePlayer(IGamePlayer player, bool IsKick)
		{
			return null;
		}
		public void SendToAll(GSPacketIn pkg)
		{
			this.SendToAll(pkg, null);
		}
		public virtual void SendToAll(GSPacketIn pkg, IGamePlayer except)
		{
		}
		
		public void Dispose()
		{
			int disposed = Interlocked.Exchange(ref this.m_disposed, 1);
			if (disposed == 0)
			{
				this.Dispose(true);
			}
		}
		protected virtual void Dispose(bool disposing)
		{		   
		}
		
		protected void OnRest()
		{
			if (this.RoomRest != null)
			{
				this.RoomRest(this);
			}
		}
		protected void OnGameOverred()
		{
			if (this.GameOverred != null)
			{
				this.GameOverred(this);
			}
		}
		protected void OnGameStarted()
		{
			if (this.GameStarted != null)
			{
				this.GameStarted(this);
			}
		}
		protected void OnGameStopped()
		{
			if (this.GameStopped != null)
			{
				this.GameStopped(this);
			}
		}
	}
}