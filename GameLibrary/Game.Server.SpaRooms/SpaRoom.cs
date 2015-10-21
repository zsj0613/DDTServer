using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Game.Server.SpaRooms
{
	public class SpaRoom
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static object _syncStop = new object();
		private ISpaProcessor _processor;
		private List<GamePlayer> _guestsList;
		public SpaRoomInfo Spa_Room_Info;
		private Timer _roomTimer;
		private Timer _minTimer;
		protected int guestGpUpInterval = 60000;
		private int _roomLeftMin;
		private int _roomContinueRemindTime;
		private DateTime _zeroToday;
		public int RoomLeftMin
		{
			get
			{
				return this._roomLeftMin;
			}
		}
		public int RoomContinueRemindTime
		{
			get
			{
				return this._roomContinueRemindTime;
			}
		}
		public int Count
		{
			get
			{
				return this._guestsList.Count<GamePlayer>();
			}
		}
		public DateTime ZeroToday
		{
			get
			{
				return this._zeroToday;
			}
		}
		public SpaRoom(SpaRoomInfo info, ISpaProcessor processor)
		{
			this.Spa_Room_Info = info;
			this._processor = processor;
			this._guestsList = new List<GamePlayer>();
			this._roomContinueRemindTime = 10;
		}
		public bool AddPlayer(GamePlayer player)
		{
			object syncStop;
			Monitor.Enter(syncStop = SpaRoom._syncStop);
			bool result;
			try
			{
				if (player.CurrentRoom != null)
				{
					result = false;
					return result;
				}
				if (player.CurrentSpaRoom != null)
				{
					SpaRoom.log.Error(string.Format("SpaRoom ====== player.nickname : {0}, room.roomID : {1}.player is in this room,but be adding.", player.PlayerCharacter.NickName, player.CurrentSpaRoom.Spa_Room_Info.RoomID));
					result = false;
					return result;
				}
				if (this.CheckIsExitPlayer(player))
				{
					result = false;
					return result;
				}
				if (this._guestsList.Count >= this.Spa_Room_Info.MaxCount)
				{
					player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("SpaRoom.Msg1", new object[0]));
					result = false;
					return result;
				}
				this._guestsList.Add(player);
				this.InitNewPlayer(player);
				player.OnPlayerSpa();
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
		private void InitNewPlayer(GamePlayer player)
		{
			player.CurrentSpaRoom = this;
			player.Spa_X = 480;
			player.Spa_Y = 560;
			player.LastPosX = 480;
			player.LastPosY = 560;
			player.Spa_Player_Direction = 3;
			player.DayOrNightInSpa = DateTime.Now;
			player.SpaRoomEnterDate = DateTime.Now;
			player.SpaRoomAddGPTotal = 0;
			if (this.Spa_Room_Info.RoomType == 1 || this.Spa_Room_Info.RoomType == 2)
			{
				player.Spa_Day_Alter_Continue = true;
				if (this.Spa_Room_Info.RoomType == 1)
				{
					player.UpdateIsInSpaPubGoldToday(true);
				}
				else
				{
					player.UpdateIsInSpaPubMoneyToday(true);
				}
			}
		}
		private bool CheckIsExitPlayer(GamePlayer player)
		{
			GamePlayer p = this.GetPlayerByUserID(player.PlayerCharacter.ID);
			bool result;
			if (p != null)
			{
				SpaRoom.log.Error(string.Format("SpaRoom ====== player.nickname : {0}, room.roomID : {1}.player is in this room,but be adding.", player.PlayerCharacter.NickName, this.Spa_Room_Info.RoomID));
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public void RemovePlayer(GamePlayer player)
		{
			object syncStop;
			Monitor.Enter(syncStop = SpaRoom._syncStop);
			try
			{
				if (this._guestsList.Contains(player))
				{
					this._guestsList.Remove(player);
					string msg = null;
					if (player.SpaRoomAddGPTotal > 0)
					{
						msg = LanguageMgr.GetTranslation("SpaRoom.Msg2", new object[]
						{
							player.SpaRoomAddGPTotal
						});
					}
					if (this.Spa_Room_Info.RoomType == 1 && player.PlayerCharacter.SpaPubGoldRoomLimit <= 0)
					{
						player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("SpaRoom.TimeOver", new object[0]));
					}
					if (this.Spa_Room_Info.RoomType == 2 && player.PlayerCharacter.SpaPubMoneyRoomLimit <= 0)
					{
						player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("SpaRoom.TimeOver", new object[0]));
					}
					player.Out.SendPlayerLeaveSpaRoom(player, msg);
					GamePlayer[] roomPlayers = player.CurrentSpaRoom.GetAllPlayers();
					if (roomPlayers != null)
					{
						GamePlayer[] array = roomPlayers;
						for (int i = 0; i < array.Length; i++)
						{
							GamePlayer p = array[i];
							p.Out.SendSpaRoomRemoveGuest(player);
						}
					}
					player.CurrentSpaRoom = null;
					this.SendSpaRoomInfoUpdateToSpaScenePlayers(this);
					player.UpdateLastSpaDate(DateTime.Now);
					player.SpaRoomAddGPTotal = 0;
				}
				else
				{
					SpaRoom.log.Error(string.Format("SpaRoom ====== player.nickname : {0}, room.roomID : {1}.player is not in this room,but be removing.", player.PlayerCharacter.NickName, this.Spa_Room_Info.RoomID));
				}
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public void RemovePlayerSpecial(GamePlayer player)
		{
			if (this._guestsList.Contains(player))
			{
				this._guestsList.Remove(player);
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("SpaRoom.ZeroKick", new object[0]));
				string msg = null;
				if (player.SpaRoomAddGPTotal > 0)
				{
					msg = LanguageMgr.GetTranslation("SpaRoom.Msg2", new object[]
					{
						player.SpaRoomAddGPTotal
					});
				}
				player.Out.SendPlayerLeaveSpaRoom(player, msg);
				GamePlayer[] roomPlayers = player.CurrentSpaRoom.GetAllPlayers();
				if (roomPlayers != null)
				{
					GamePlayer[] array = roomPlayers;
					for (int i = 0; i < array.Length; i++)
					{
						GamePlayer p = array[i];
						p.Out.SendSpaRoomRemoveGuest(player);
					}
				}
				player.CurrentSpaRoom = null;
				this.SendSpaRoomInfoUpdateToSpaScenePlayers(this);
				//DateTime arg_E4_0 = this.ZeroToday;
				//bool flag = 1 == 0;
				player.UpdateLastSpaDate(this.ZeroToday);
				player.SpaRoomAddGPTotal = 0;
			}
			else
			{
				SpaRoom.log.Error(string.Format("SpaRoom ====== player.nickname : {0}, room.roomID : {1}.player is not in this room,but be removing in zero.", player.PlayerCharacter.NickName, this.Spa_Room_Info.RoomID));
			}
		}
		public void KickAllPlayer()
		{
			GamePlayer[] players = this.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				this.RemovePlayer(p);
				p.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("SpaRoom.TimeOver", new object[0]));
			}
		}
		public bool KickPlayerByUserID(GamePlayer player, int userID)
		{
			GamePlayer kickPlayer = this.GetPlayerByUserID(userID);
			bool result;
			if (kickPlayer != null && kickPlayer.PlayerCharacter.ID != player.CurrentSpaRoom.Spa_Room_Info.PlayerID)
			{
				this.RemovePlayer(kickPlayer);
				kickPlayer.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom", new object[0]));
				GSPacketIn pkg = player.Out.SendMessage(eMessageType.ChatERROR, kickPlayer.PlayerCharacter.NickName + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2", new object[0]));
				player.CurrentSpaRoom.SendToRoomPlayerExceptSelf(pkg, player);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public GamePlayer GetPlayerByUserID(int userID)
		{
			object syncStop;
			Monitor.Enter(syncStop = SpaRoom._syncStop);
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
		public GamePlayer[] GetAllPlayers()
		{
			object syncStop;
			Monitor.Enter(syncStop = SpaRoom._syncStop);
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
		public void BeginTimer(int interval)
		{
			this._roomLeftMin = interval;
			interval = interval * 60 * 1000;
			if (this.Spa_Room_Info.RoomType == 1 || this.Spa_Room_Info.RoomType == 2)
			{
				if (SpaRoom.log.IsErrorEnabled)
				{
					SpaRoom.log.Error("SpaPubRooms start the minTimer!----- Wrong");
				}
			}
			else
			{
				if (this._roomTimer == null)
				{
					this._roomTimer = new Timer(new TimerCallback(this.OnTick), null, interval, interval);
				}
				else
				{
					this._roomTimer.Change(interval, interval);
				}
				if (this._minTimer == null)
				{
					this._minTimer = new Timer(new TimerCallback(this.OnTickForGpUpLimit), null, this.guestGpUpInterval, this.guestGpUpInterval);
				}
				else
				{
					this._minTimer.Change(this.guestGpUpInterval, this.guestGpUpInterval);
				}
			}
		}
		public void BeginTimerForPubRoom()
		{
			if (this.Spa_Room_Info.RoomType == 1 || this.Spa_Room_Info.RoomType == 2)
			{
				if (this._minTimer == null)
				{
					this._minTimer = new Timer(new TimerCallback(this.OnTickForGpUpLimit), null, this.guestGpUpInterval, this.guestGpUpInterval);
				}
				else
				{
					this._minTimer.Change(this.guestGpUpInterval, this.guestGpUpInterval);
				}
			}
			else
			{
				if (SpaRoom.log.IsErrorEnabled)
				{
					SpaRoom.log.Error("SpaPriRooms start the SpaPubRooms' Timer!----- Wrong");
				}
			}
		}
		public void StopTimer()
		{
			if (this._roomTimer != null)
			{
				this._roomTimer.Change(-1, -1);
				this._roomTimer.Dispose();
				this._roomTimer = null;
			}
			this._minTimer.Change(-1, -1);
			this.StopTimerForGpUpLimit();
		}
		protected void OnTick(object obj)
		{
			this._processor.OnTick(this);
		}
		protected void OnTickForGpUpLimit(object obj)
		{
			try
			{
				DateTime currenTime = DateTime.Now;
				int leftTimeToClient = 0;
				if (this.Spa_Room_Info.RoomType != 1 && this.Spa_Room_Info.RoomType != 2)
				{
					this._roomLeftMin--;
				}
				GamePlayer[] currentSpaRoomPlayers = this.GetAllPlayers();
				GamePlayer[] array = currentSpaRoomPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					p.DayOrNightInSpa = currenTime;
					TimeSpan usedTime = currenTime - p.SpaRoomEnterDate;
					if (usedTime.TotalMinutes > 0.0)
					{
						if (this._minTimer != null)
						{
							if (this.Spa_Room_Info.RoomType == 1 || this.Spa_Room_Info.RoomType == 2)
							{
								if (currenTime.Hour == 0 && currenTime.Minute == 0)
								{
									p.UpdateIsInSpaPubGoldToday(false);
									p.UpdateIsInSpaPubMoneyToday(false);
									p.Spa_Day_Alter_Continue = false;
									this._zeroToday = currenTime;
									this.AddSpaGpByMin(p);
									p.UpdateSpaPubGoldRoomLimit(SpaRoomMgr.pubGoldRoom_MinLimit);
									p.UpdateSpaPubMoneyRoomLimit(SpaRoomMgr.pubMoneyRoom_MinLimit);
									p.SpaRoomEnterDate = currenTime;
									if (this.Spa_Room_Info.RoomType == 1)
									{
										leftTimeToClient = SpaRoomMgr.pubGoldRoom_MinLimit;
									}
									else
									{
										leftTimeToClient = SpaRoomMgr.pubMoneyRoom_MinLimit;
									}
									p.Out.SendSpaRoomInfoPerMin(p, leftTimeToClient);
									p.Out.SendIsContinueNextDay(p);
									goto IL_3DD;
								}
								if (currenTime.Hour == 0 && currenTime.Minute == 1)
								{
									if (!p.Spa_Day_Alter_Continue)
									{
										this.RemovePlayerSpecial(p);
										goto IL_3DD;
									}
								}
								if (this.Spa_Room_Info.RoomType == 1)
								{
									if ((int)usedTime.TotalMinutes > SpaRoomMgr.pubGoldRoom_MinLimit || p.PlayerCharacter.SpaPubGoldRoomLimit <= 1)
									{
										p.RemoveSpaPubGoldRoomLimit(1);
										leftTimeToClient = p.PlayerCharacter.SpaPubGoldRoomLimit;
										this.AddSpaGpByMin(p);
										p.Out.SendSpaRoomInfoPerMin(p, leftTimeToClient);
										this.RemovePlayer(p);
										goto IL_3DD;
									}
									p.RemoveSpaPubGoldRoomLimit(1);
									leftTimeToClient = p.PlayerCharacter.SpaPubGoldRoomLimit;
								}
								if (this.Spa_Room_Info.RoomType == 2)
								{
									if ((int)usedTime.TotalMinutes > SpaRoomMgr.pubMoneyRoom_MinLimit || p.PlayerCharacter.SpaPubMoneyRoomLimit <= 1)
									{
										p.RemoveSpaPubMoneyRoomLimit(1);
										leftTimeToClient = p.PlayerCharacter.SpaPubMoneyRoomLimit;
										this.AddSpaGpByMin(p);
										p.Out.SendSpaRoomInfoPerMin(p, leftTimeToClient);
										this.RemovePlayer(p);
										goto IL_3DD;
									}
									p.RemoveSpaPubMoneyRoomLimit(1);
									leftTimeToClient = p.PlayerCharacter.SpaPubMoneyRoomLimit;
								}
							}
							this.AddSpaGpByMin(p);
							if (this.Spa_Room_Info.RoomType != 1 && this.Spa_Room_Info.RoomType != 2)
							{
								leftTimeToClient = this._roomLeftMin;
								if (this._roomLeftMin == this.RoomContinueRemindTime)
								{
									if (p.PlayerCharacter.ID == this.Spa_Room_Info.PlayerID)
									{
										GSPacketIn pkg = new GSPacketIn(191, p.PlayerCharacter.ID);
										pkg.WriteByte(3);
										p.Out.SendTCP(pkg);
									}
								}
								if (this._roomLeftMin <= 0)
								{
									this._processor.OnTick(this);
								}
							}
							p.Out.SendSpaRoomInfoPerMin(p, leftTimeToClient);
						}
						else
						{
							SpaRoom.log.Error("SpaRoom ====== Min Timer Lost.");
						}
					}
					IL_3DD:;
				}
			}
			catch (Exception ex)
			{
				if (SpaRoom.log.IsErrorEnabled)
				{
					SpaRoom.log.Error("OnTickForSpaMinTimer", ex);
				}
			}
		}
		private void AddSpaGpByMin(GamePlayer player)
		{
			int priGrade = player.PlayerCharacter.Grade;
			int spaGpAddTemp;
			if (this.Spa_Room_Info.RoomType == 1)
			{
				spaGpAddTemp = LevelMgr.GetSpaGoldGP(priGrade);
				spaGpAddTemp /= 60;
			}
			else
			{
				spaGpAddTemp = LevelMgr.GetSpaMoneyGP(priGrade);
				spaGpAddTemp /= 60;
			}
			if (player.PlayerCharacter.Grade >= 40)
			{
				spaGpAddTemp = 0;
			}
			player.SpaRoomAddGPTotal += spaGpAddTemp;
			if (spaGpAddTemp >= 0)
			{
				player.AddGP(spaGpAddTemp);
			}
			else
			{
				player.AddGP(0);
			}
		}
		public void StopTimerForGpUpLimit()
		{
			if (this._minTimer != null)
			{
				this._minTimer.Dispose();
				this._minTimer = null;
			}
		}
		public bool RoomContinuation(int time, GamePlayer player)
		{
			this._roomLeftMin += time;
			this.Spa_Room_Info.AvailTime += time;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				db.UpdateSpaRoomInfo(this.Spa_Room_Info);
			}
			bool result;
			if (this.Spa_Room_Info.RoomType != 1 && this.Spa_Room_Info.RoomType != 2)
			{
				Console.WriteLine("now continue the priRoom roomTimer.the time interval is {0}", this._roomLeftMin);
				int roomLeftTime = this._roomLeftMin * 60000;
				if (this._roomTimer == null)
				{
					this._roomTimer = new Timer(new TimerCallback(this.OnTick), null, roomLeftTime, roomLeftTime);
				}
				else
				{
					this._roomTimer.Change(roomLeftTime, roomLeftTime);
				}
				GSPacketIn pkg = new GSPacketIn(191, player.PlayerCharacter.ID);
				pkg.WriteByte(7);
				pkg.WriteInt(this.RoomLeftMin);
				this.SendToRoomPlayer(pkg);
				this.SendSpaRoomInfoUpdateToSpaScenePlayers(this);
				result = true;
			}
			else
			{
				if (SpaRoom.log.IsErrorEnabled)
				{
					SpaRoom.log.Error("SpaPubRoom shouldn't continue roomTimer");
				}
				result = false;
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
		public void SendToRoomPlayerExceptSelf(GSPacketIn packet, GamePlayer self)
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
		public void SendToRoomplayerAddplayer(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(198);
			pkg.WriteInt(player.PlayerCharacter.ID);
			pkg.WriteInt(player.PlayerCharacter.Grade);
			pkg.WriteInt(player.PlayerCharacter.Hide);
			pkg.WriteInt(player.PlayerCharacter.Repute);
			pkg.WriteString(player.PlayerCharacter.NickName);
			pkg.WriteBoolean(player.PlayerCharacter.Sex);
			pkg.WriteString(player.PlayerCharacter.Style);
			pkg.WriteString(player.PlayerCharacter.Colors);
			pkg.WriteString(player.PlayerCharacter.Skin);
			pkg.WriteInt(player.LastPosX);
			pkg.WriteInt(player.LastPosY);
			pkg.WriteInt(player.FightPower);
			pkg.WriteInt(player.PlayerCharacter.Win);
			pkg.WriteInt(player.PlayerCharacter.Total);
			pkg.WriteInt(player.Spa_Player_Direction);
			this.SendToRoomPlayerExceptSelf(pkg, player);
		}
		public void SendToRoomplayerAddplayer(GamePlayer player, GSPacketIn pkg)
		{
			this.SendToRoomPlayerExceptSelf(pkg, player);
		}
		public void SendToScenePlayer(GSPacketIn packet)
		{
			WorldMgr.SpaScene.SendToALL(packet);
		}
		public void SendToScenePlayer(GSPacketIn packet, GamePlayer player)
		{
			WorldMgr.SpaScene.SendToALL(packet, player);
		}
		public void ProcessData(GamePlayer player, GSPacketIn data)
		{
			object syncStop;
			Monitor.Enter(syncStop = SpaRoom._syncStop);
			try
			{
				this._processor.OnGameData(this, player, data);
			}
			finally
			{
				Monitor.Exit(syncStop);
			}
		}
		public GSPacketIn SendSpaRoomInfoUpdateToSpaScenePlayers(SpaRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(173);
			pkg.WriteInt(room.Spa_Room_Info.RoomNumber);
			pkg.WriteInt(room.Spa_Room_Info.RoomID);
			pkg.WriteString(room.Spa_Room_Info.RoomName);
			pkg.WriteString(room.Spa_Room_Info.Pwd);
			pkg.WriteInt(room.RoomLeftMin);
			pkg.WriteInt(room.Count);
			pkg.WriteInt(room.Spa_Room_Info.PlayerID);
			pkg.WriteString(room.Spa_Room_Info.PlayerName);
			pkg.WriteDateTime(room.Spa_Room_Info.BeginTime);
			pkg.WriteString(room.Spa_Room_Info.RoomIntroduction);
			pkg.WriteInt(room.Spa_Room_Info.RoomType);
			pkg.WriteInt(room.Spa_Room_Info.MaxCount);
			this.SendToScenePlayer(pkg);
			return pkg;
		}
		public void ReturnPacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClientID = player.PlayerCharacter.ID;
			this.SendToRoomPlayerExceptSelf(pkg, player);
		}
	}
}
