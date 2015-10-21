using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Game.Server.SceneMarryRooms
{
	public class MarryRoom
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static object _syncStop = new object();
		private List<GamePlayer> _guestsList;
		private IMarryProcessor _processor;
		private int _count;
		public MarryRoomInfo Info;
		private eRoomState _roomState;
		private Timer _timer;
		private Timer _timerForHymeneal;
		private List<int> _userForbid;
		private List<int> _userRemoveList;
		public eRoomState RoomState
		{
			get
			{
				return this._roomState;
			}
			set
			{
				if (this._roomState != value)
				{
					this._roomState = value;
					this.SendMarryRoomInfoUpdateToScenePlayers(this);
				}
			}
		}
		public int Count
		{
			get
			{
				return this._count;
			}
		}
		public MarryRoom(MarryRoomInfo info, IMarryProcessor processor)
		{
			this.Info = info;
			this._processor = processor;
			this._guestsList = new List<GamePlayer>();
			this._count = 0;
			this._roomState = eRoomState.FREE;
			this._userForbid = new List<int>();
			this._userRemoveList = new List<int>();
		}
		public bool AddPlayer(GamePlayer player)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			bool result;
			try
			{
				if (player.CurrentRoom != null || player.IsInMarryRoom)
				{
					result = false;
					return result;
				}
				GamePlayer Groom = WorldMgr.GetPlayerById(this.Info.GroomID);
				GamePlayer Bride = WorldMgr.GetPlayerById(this.Info.BrideID);
				int maxcount;
				if (this._guestsList.Contains(Groom) && this._guestsList.Contains(Bride))
				{
					maxcount = this.Info.MaxCount;
				}
				else
				{
					maxcount = this.Info.MaxCount - 2;
				}
				if (this._guestsList.Count > maxcount)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoom.Msg1", new object[0]));
					result = false;
					return result;
				}
				this._count++;
				this._guestsList.Add(player);
				player.CurrentMarryRoom = this;
				player.MarryMap = 1;
				if (player.CurrentRoom != null)
				{
					player.CurrentRoom.RemovePlayerUnsafe(player);
				}
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			result = true;
			return result;
		}
		public void RemovePlayer(GamePlayer player)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				if (this.RoomState == eRoomState.FREE)
				{
					this._count--;
					this._guestsList.Remove(player);
					GSPacketIn pkg = player.Out.SendPlayerLeaveMarryRoom(player);
					player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(pkg, player);
					player.CurrentMarryRoom = null;
					player.MarryMap = 0;
				}
				else
				{
					if (this.RoomState == eRoomState.Hymeneal)
					{
						this._userRemoveList.Add(player.PlayerCharacter.ID);
						this._count--;
						this._guestsList.Remove(player);
						GSPacketIn pkg = player.Out.SendPlayerLeaveMarryRoom(player);
						player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(pkg, player);
						player.CurrentMarryRoom = null;
					}
				}
				this.SendMarryRoomInfoUpdateToScenePlayers(this);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public void BeginTimer(int interval)
		{
			if (this._timer == null)
			{
				this._timer = new Timer(new TimerCallback(this.OnTick), null, interval, interval);
			}
			else
			{
				this._timer.Change(interval, interval);
			}
		}
		protected void OnTick(object obj)
		{
			this._processor.OnTick(this);
		}
		public void StopTimer()
		{
			if (this._timer != null)
			{
				this._timer.Dispose();
				this._timer = null;
			}
		}
		public void BeginTimerForHymeneal(int interval)
		{
			if (this._timerForHymeneal == null)
			{
				this._timerForHymeneal = new Timer(new TimerCallback(this.OnTickForHymeneal), null, interval, interval);
			}
			else
			{
				this._timerForHymeneal.Change(interval, interval);
			}
		}
		protected void OnTickForHymeneal(object obj)
		{
			try
			{
				this._roomState = eRoomState.FREE;
				GSPacketIn pkg = new GSPacketIn(249);
				pkg.WriteByte(9);
				this.SendToAll(pkg);
				this.StopTimerForHymeneal();
				this.SendUserRemoveLate();
				this.SendMarryRoomInfoUpdateToScenePlayers(this);
			}
			catch (Exception ex)
			{
				if (MarryRoom.log.IsErrorEnabled)
				{
					MarryRoom.log.Error("OnTickForHymeneal", ex);
				}
			}
		}
		public void StopTimerForHymeneal()
		{
			if (this._timerForHymeneal != null)
			{
				this._timerForHymeneal.Dispose();
				this._timerForHymeneal = null;
			}
		}
		public GamePlayer[] GetAllPlayers()
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			GamePlayer[] result;
			try
			{
				result = this._guestsList.ToArray();
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			return result;
		}
		public void SendToRoomPlayer(GSPacketIn packet)
		{
			GamePlayer[] player = this.GetAllPlayers();
			if (player != null)
			{
				GamePlayer[] array = player;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					p.Out.SendTCP(packet);
				}
			}
		}
		public void SendToAll(GSPacketIn packet)
		{
			this.SendToAll(packet, null, false);
		}
		public void SendToAll(GSPacketIn packet, GamePlayer self, bool isChat)
		{
			GamePlayer[] player = this.GetAllPlayers();
			if (player != null)
			{
				GamePlayer[] array = player;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (!isChat || !p.IsBlackFriend(self.PlayerCharacter.ID))
					{
						p.Out.SendTCP(packet);
					}
				}
			}
		}
		public void SendToAllForScene(GSPacketIn packet, int sceneID)
		{
			GamePlayer[] player = this.GetAllPlayers();
			if (player != null)
			{
				GamePlayer[] array = player;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p.MarryMap == sceneID)
					{
						p.Out.SendTCP(packet);
					}
				}
			}
		}
		public void SendToPlayerExceptSelf(GSPacketIn packet, GamePlayer self)
		{
			GamePlayer[] player = this.GetAllPlayers();
			if (player != null)
			{
				GamePlayer[] array = player;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p != self)
					{
						p.Out.SendTCP(packet);
					}
				}
			}
		}
		public void SendToPlayerExceptSelfForScene(GSPacketIn packet, GamePlayer self)
		{
			GamePlayer[] player = this.GetAllPlayers();
			if (player != null)
			{
				GamePlayer[] array = player;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p != self)
					{
						if (p.MarryMap == self.MarryMap)
						{
							p.Out.SendTCP(packet);
						}
					}
				}
			}
		}
		public void SendToScenePlayer(GSPacketIn packet)
		{
			WorldMgr.MarryScene.SendToALL(packet);
		}
		public void ProcessData(GamePlayer player, GSPacketIn data)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				this._processor.OnGameData(this, player, data);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public void ReturnPacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClientID = player.PlayerCharacter.ID;
			this.SendToPlayerExceptSelf(pkg, player);
		}
		public void ReturnPacketForScene(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClientID = player.PlayerCharacter.ID;
			this.SendToPlayerExceptSelfForScene(pkg, player);
		}
		public bool KickPlayerByUserID(GamePlayer player, int userID)
		{
			GamePlayer kickPlayer = this.GetPlayerByUserID(userID);
			bool result;
			if (kickPlayer != null && kickPlayer.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && kickPlayer.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				this.RemovePlayer(kickPlayer);
				kickPlayer.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom", new object[0]));
				GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatERROR, kickPlayer.PlayerCharacter.NickName + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2", new object[0]));
				player.CurrentMarryRoom.SendToPlayerExceptSelf(msg, player);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public void KickAllPlayer()
		{
			GamePlayer[] players = this.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				this.RemovePlayer(p);
				p.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoom.TimeOver", new object[0]));
			}
		}
		public void KillAllPlayer()
		{
			GamePlayer[] players = this.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				this.RemovePlayer(p);
				p.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoom.Close", new object[0]));
			}
		}
		public GamePlayer GetPlayerByUserID(int userID)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			GamePlayer result;
			try
			{
				foreach (GamePlayer p in this._guestsList)
				{
					if (p.PlayerCharacter.ID == userID)
					{
						result = p;
						return result;
					}
				}
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			result = null;
			return result;
		}
		public void RoomContinuation(int time)
		{
			TimeSpan timeLeft = DateTime.Now - this.Info.BeginTime;
			int newTime = this.Info.AvailTime * 60 - timeLeft.Minutes + time * 60;
			this.Info.AvailTime += time;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				db.UpdateMarryRoomInfo(this.Info);
			}
			this.BeginTimer(60000 * newTime);
		}
		public void SetUserForbid(int userID)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				this._userForbid.Add(userID);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public bool CheckUserForbid(int userID)
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			bool result;
			try
			{
				result = this._userForbid.Contains(userID);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
			return result;
		}
		public void SendUserRemoveLate()
		{
			object syncStop;
			Monitor.Enter(syncStop = MarryRoom._syncStop);
			try
			{
				foreach (int userID in this._userRemoveList)
				{
					GSPacketIn pkg = new GSPacketIn(244, userID);
					this.SendToAllForScene(pkg, 1);
				}
				this._userRemoveList.Clear();
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public GSPacketIn SendMarryRoomInfoUpdateToScenePlayers(MarryRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(255);
			bool result = room != null;
			pkg.WriteBoolean(result);
			if (result)
			{
				pkg.WriteInt(room.Info.ID);
				pkg.WriteBoolean(room.Info.IsHymeneal);
				pkg.WriteString(room.Info.Name);
				pkg.WriteBoolean(!(room.Info.Pwd == ""));
				pkg.WriteInt(room.Info.MapIndex);
				pkg.WriteInt(room.Info.AvailTime);
				pkg.WriteInt(room.Count);
				pkg.WriteInt(room.Info.PlayerID);
				pkg.WriteString(room.Info.PlayerName);
				pkg.WriteInt(room.Info.GroomID);
				pkg.WriteString(room.Info.GroomName);
				pkg.WriteInt(room.Info.BrideID);
				pkg.WriteString(room.Info.BrideName);
				pkg.WriteDateTime(room.Info.BeginTime);
				pkg.WriteByte((byte)room.RoomState);
				pkg.WriteString(room.Info.RoomIntroduction);
			}
			this.SendToScenePlayer(pkg);
			return pkg;
		}
	}
}
