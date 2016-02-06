using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Lsj.Util.Logs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Game.Server.Rooms
{
	public class BaseRoom
	{
		private static LogProvider log => LogProvider.Default;
		private GamePlayer[] m_places;
		private int[] m_placesState;
		private byte[] m_playerState;
		private int m_playerCount = 0;
		private int m_placesCount = 8;
		private bool m_isUsing = false;
		private GamePlayer m_host;
		private bool m_IsPlaying;
		public int RoomId;
		public int FightRoomID;
		public int GameStyle = 0;
		public string Name;
		public string Password;
		public eRoomType RoomType;
		public eRoomType OldRoomType;
		public eGameType GameType;
		public eGameType OldGameType = eGameType.ALL;
		public eHardLevel HardLevel;
		public bool IsArea;
		public int LevelLimits;
		public byte TimeMode;
		public int MapId;
		public bool PickUpNPC;
		public string m_roundName;
		private int m_avgLevel = 0;
		private AbstractGame m_game;
		public BattleServer BattleServer;
		public bool IsPlaying
		{
			get
			{
				return this.m_IsPlaying;
			}
			set
			{
				this.m_IsPlaying = value;
			}
		}
		public GamePlayer Host
		{
			get
			{
				return this.m_host;
			}
		}
		public int[] PlaceSate
		{
			get
			{
				return this.m_placesState;
			}
		}
		public byte[] PlayerState
		{
			get
			{
				return this.m_playerState;
			}
			set
			{
				this.m_playerState = value;
			}
		}
		public int PlayerCount
		{
			get
			{
				return this.m_playerCount;
			}
		}
		public int PlacesCount
		{
			get
			{
				return this.m_placesCount;
			}
		}
		public int GuildId
		{
			get
			{
				int result;
				if (this.m_host == null || this.m_host.PlayerCharacter == null)
				{
					result = 0;
				}
				else
				{
					result = this.m_host.PlayerCharacter.ConsortiaID;
				}
				return result;
			}
		}
		public bool IsUsing
		{
			get
			{
				return this.m_isUsing;
			}
		}
		public string RoundName
		{
			get
			{
				return this.m_roundName;
			}
			set
			{
				this.m_roundName = value;
			}
		}
		public bool NeedPassword
		{
			get
			{
				return !string.IsNullOrEmpty(this.Password);
			}
		}
		public bool IsEmpty
		{
			get
			{
				return this.m_playerCount == 0;
			}
		}
		public int AvgLevel
		{
			get
			{
				return this.m_avgLevel;
			}
		}
		public AbstractGame Game
		{
			get
			{
				return this.m_game;
			}
		}
		public BaseRoom(int roomId)
		{
			this.RoomId = roomId;
			this.m_places = new GamePlayer[8];
			this.m_placesState = new int[8];
			this.m_playerState = new byte[8];
			this.Reset();
		}
		public void Start()
		{
			if (!this.m_isUsing)
			{
				this.m_isUsing = true;
				this.Reset();
				RoomMgr.UsingRoomCount++;
			}
		}
		public void Stop()
		{
			if (this.m_isUsing)
			{
				this.m_isUsing = false;
				if (this.m_game != null)
				{
					this.m_game.GameStopped -= new GameEventHandle(this.m_game_GameStopped);
					this.m_game = null;
					this.IsPlaying = false;
				}
				RoomMgr.WaitingRoom.SendUpdateRoom(this);
				RoomMgr.UsingRoomCount--;
			}
		}
		private void Reset()
		{
			for (int i = 0; i < 8; i++)
			{
				this.m_places[i] = null;
				this.m_placesState[i] = -1;
				this.m_playerState[i] = 0;
			}
			this.m_host = null;
			this.IsPlaying = false;
			this.m_placesCount = 8;
			this.m_playerCount = 0;
			this.HardLevel = eHardLevel.Simple;
			this.IsArea = false;
		}
		public bool CanStart()
		{
			bool result;
			if (this.RoomType == eRoomType.Freedom)
			{
				int red = 0;
				int blue = 0;
				for (int i = 0; i < 8; i++)
				{
					if (i % 2 == 0)
					{
						if (this.m_playerState[i] > 0)
						{
							red++;
						}
					}
					else
					{
						if (this.m_playerState[i] > 0)
						{
							blue++;
						}
					}
				}
				result = (red > 0 && blue > 0);
			}
			else
			{
				int ready = 0;
				for (int i = 0; i < 8; i++)
				{
					if (this.m_playerState[i] > 0)
					{
						ready++;
					}
				}
				result = (ready == this.m_playerCount);
			}
			return result;
		}
		public bool CanAddPlayer()
		{
			return this.m_playerCount < this.m_placesCount;
		}
		public bool IsGradeAchieved(int copyId, ref int levelLimit)
		{
			bool result;
			if (copyId != 0 && copyId != 10000)
			{
				levelLimit = PveInfoMgr.GetPveInfoById(copyId).LevelLimits;
				result = (this.m_host.PlayerCharacter.Grade >= levelLimit);
			}
			else
			{
				result = true;
			}
			return result;
		}
		public List<GamePlayer> GetPlayers()
		{
			List<GamePlayer> temp = new List<GamePlayer>();
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				for (int i = 0; i < 8; i++)
				{
					if (this.m_places[i] != null)
					{
						temp.Add(this.m_places[i]);
					}
				}
			}
			finally
			{
				Monitor.Exit(places);
			}
			return temp;
		}
		public void SetHost(GamePlayer player)
		{
			if (this.m_host != player)
			{
				if (this.m_host != null)
				{
					this.UpdatePlayerState(player, 0, false);
				}
				this.m_host = player;
				this.UpdatePlayerState(player, 2, true);
			}
		}
		public void UpdateRoom(string name, string pwd, eRoomType roomType, byte timeMode, int mapId)
		{
			this.Name = name;
			this.Password = pwd;
			this.RoomType = roomType;
			this.TimeMode = timeMode;
			this.MapId = mapId;
			this.UpdateRoomGameType();
			if (roomType == eRoomType.Freedom)
			{
				this.m_placesCount = 8;
				this.GameType = eGameType.Training;
			}
			else
			{
				this.m_placesCount = 4;
			}
		}
		public void UpdateRoomGameType()
		{
			switch (this.RoomType)
			{
			    case eRoomType.Match:
				    if (this.IsAllSameGuild())
				    {
					    this.GameType = eGameType.Guild;
				    }
				    else
				    {
					    this.GameType = eGameType.Free;
				    }
				    break;
			    case eRoomType.Freedom:
				    this.GameType = eGameType.Training;
				    break;
			    case eRoomType.Exploration:
				    this.GameType = eGameType.Exploration;
				    break;
			    case eRoomType.Boss:
				    this.GameType = eGameType.Boss;
				    break;
			    case eRoomType.Treasure:
				    this.GameType = eGameType.Treasure;
				    break;
			    case eRoomType.FightLab:
				    this.GameType = eGameType.FightLab;
				    break;
                case eRoomType.WorldBoss:
                    this.GameType = eGameType.WorldBoss;
                    break;
			    default:
				    this.GameType = eGameType.ALL;
				    break;
			}
		}
		public void UpdatePlayerState(GamePlayer player, byte state, bool sendToClient)
		{
			this.m_playerState[player.CurrentRoomIndex] = state;
			if (sendToClient)
			{
				this.SendPlayerState();
			}
		}
		public void ResetPlayerState()
		{
			for (int i = 0; i < this.m_playerState.Length; i++)
			{
				if (this.m_playerState[i] != 2)
				{
					this.m_playerState[i] = 0;
				}
			}
			this.SendPlayerState();
		}
		public void UpdateAvgLevel()
		{
			int sum = 0;
			for (int i = 0; i < 8; i++)
			{
				if (this.m_places[i] != null)
				{
					sum += this.m_places[i].PlayerCharacter.Grade;
				}
			}
			this.m_avgLevel = ((this.m_playerCount > 0) ? (sum / this.m_playerCount) : this.m_playerCount);
		}
		public void SendToAll(GSPacketIn pkg)
		{
			this.SendToAll(pkg, null);
		}
		public void SendToAll(GSPacketIn pkg, GamePlayer except)
		{
			GamePlayer[] temp = null;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				temp = (GamePlayer[])this.m_places.Clone();
			}
			finally
			{
				Monitor.Exit(places);
			}
			if (temp != null)
			{
				for (int i = 0; i < temp.Length; i++)
				{
					if (temp[i] != null && temp[i] != except)
					{
						temp[i].Out.SendTCP(pkg);
					}
				}
			}
		}
		public void SendToTeam(GSPacketIn pkg, int team)
		{
			this.SendToTeam(pkg, team, null);
		}
		public void SendToTeam(GSPacketIn pkg, int team, GamePlayer except)
		{
			GamePlayer[] temp = null;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				temp = (GamePlayer[])this.m_places.Clone();
			}
			finally
			{
				Monitor.Exit(places);
			}
			for (int i = 0; i < temp.Length; i++)
			{
				if (temp[i] != null && temp[i].CurrentRoomTeam == team && temp[i] != except)
				{
					temp[i].Out.SendTCP(pkg);
				}
			}
		}
		public void SendToHost(GSPacketIn pkg)
		{
			GamePlayer[] temp = null;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				temp = (GamePlayer[])this.m_places.Clone();
			}
			finally
			{
				Monitor.Exit(places);
			}
			for (int i = 0; i < temp.Length; i++)
			{
				if (temp[i] != null && temp[i] == this.Host)
				{
					temp[i].Out.SendTCP(pkg);
				}
			}
		}
		public void SendPlayerState()
		{
			GSPacketIn pkg = this.m_host.Out.SendRoomUpdatePlayerStates(this.m_playerState);
			this.SendToAll(pkg, this.m_host);
		}
		public void SendPlaceState()
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendRoomUpdatePlacesStates(this.m_placesState);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public void SendCancelPickUp()
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendRoomPairUpCancel(this);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public void SendStartPickUp()
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendRoomPairUpStart(this);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public void SendMessage(eMessageType type, string msg)
		{
			if (this.m_host != null)
			{
				GSPacketIn pkg = this.m_host.Out.SendMessage(type, msg);
				this.SendToAll(pkg, this.m_host);
			}
		}
		public bool UpdatePosUnsafe(int pos, bool isOpened)
		{
			bool result;
			if (pos < 0 || pos > 7)
			{
				result = false;
			}
			else
			{
				int temp = isOpened ? -1 : 0;
				if (this.m_placesState[pos] != temp)
				{
					if (this.m_places[pos] != null)
					{
						this.RemovePlayerUnsafe(this.m_places[pos]);
					}
					this.m_placesState[pos] = temp;
					this.SendPlaceState();
					if (isOpened)
					{
						this.m_placesCount++;
					}
					else
					{
						this.m_placesCount--;
					}
					this.LimitPlaceCount();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		public void LimitPlaceCount()
		{
			if (this.RoomType == eRoomType.Freedom)
			{
				this.m_placesCount = ((this.m_placesCount > 8) ? 8 : this.m_placesCount);
			}
			else
			{
				this.m_placesCount = ((this.m_placesCount > 4) ? 4 : this.m_placesCount);
			}
		}
		public bool IsAllSameGuild()
		{
			int guildId = this.GuildId;
			bool result;
			if (guildId != 0)
			{
				List<GamePlayer> list = this.GetPlayers();
				if (list.Count >= 2)
				{
					foreach (GamePlayer p in list)
					{
						if (p.PlayerCharacter.ConsortiaID != guildId || p.PlayerCharacter.ConsortiaLevel < 3)
						{
							result = false;
							return result;
						}
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}
		public void UpdateGameStyle()
		{
			if (this.m_host != null && this.RoomType == eRoomType.Match)
			{
				if (this.IsAllSameGuild())
				{
					if (this.OldGameType == eGameType.ALL)
					{
						this.GameStyle = 1;
						this.GameType = eGameType.Guild;
						GSPacketIn pkg = this.m_host.Out.SendRoomType(this.m_host, this);
						this.SendToAll(pkg);
					}
					else
					{
						this.OldGameType = eGameType.ALL;
					}
				}
				else
				{
					this.GameStyle = 0;
					this.GameType = eGameType.Free;
					this.OldGameType = eGameType.ALL;
					GSPacketIn pkg = this.m_host.Out.SendRoomType(this.m_host, this);
					this.SendToAll(pkg);
				}
			}
		}
		public bool AddPlayerUnsafe(GamePlayer player)
		{
			int index = -1;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				for (int i = 0; i < 8; i++)
				{
					if (this.m_places[i] == null && this.m_placesState[i] == -1)
					{
						this.m_places[i] = player;
						this.m_placesState[i] = player.PlayerId;
						this.m_playerCount++;
						index = i;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(places);
			}
			if (index != -1)
			{
				player.CurrentRoom = this;
				player.CurrentRoomIndex = index;
				if (this.RoomType == eRoomType.Freedom)
				{
					player.CurrentRoomTeam = index % 2 + 1;
				}
				else
				{
					player.CurrentRoomTeam = 1;
				}
				GSPacketIn pkg = player.Out.SendRoomPlayerAdd(player);
				this.SendToAll(pkg, player);
				GSPacketIn bufferPkg = player.Out.SendBufferList(player, player.BufferList.GetAllBuffer());
				this.SendToAll(bufferPkg, player);
				List<GamePlayer> list = this.GetPlayers();
				foreach (GamePlayer p in list)
				{
					if (p != player)
					{
						player.Out.SendRoomPlayerAdd(p);
						player.Out.SendBufferList(p, p.BufferList.GetAllBuffer());
					}
				}
				if (this.m_host == null)
				{
					this.m_host = player;
					this.UpdatePlayerState(player, 2, true);
				}
				else
				{
					this.UpdatePlayerState(player, 0, true);
				}
				this.SendPlaceState();
				this.UpdateGameStyle();
			}
			return index != -1;
		}
		public bool RemovePlayerUnsafe(GamePlayer player)
		{
			return this.RemovePlayerUnsafe(player, false);
		}
		public bool RemovePlayerUnsafe(GamePlayer player, bool isKick)
		{
			int index = -1;
			GamePlayer[] places;
			Monitor.Enter(places = this.m_places);
			try
			{
				for (int i = 0; i < 8; i++)
				{
					if (this.m_places[i] == player)
					{
						this.m_places[i] = null;
						this.m_playerState[i] = 0;
						this.m_placesState[i] = -1;
						this.m_playerCount--;
						index = i;
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(places);
			}
			if (index != -1)
			{
				this.UpdatePosUnsafe(index, true);
				player.CurrentRoom = null;
				GSPacketIn pkg = player.Out.SendRoomPlayerRemove(player);
				this.SendToAll(pkg);
				if (isKick)
				{
					player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom", new object[0]));
				}
				bool isChangeHost = false;
				if (this.m_host == player)
				{
					if (this.m_playerCount > 0)
					{
						for (int i = 0; i < 8; i++)
						{
							if (this.m_places[i] != null)
							{
								this.SetHost(this.m_places[i]);
								isChangeHost = true;
								break;
							}
						}
					}
					else
					{
						this.m_host = null;
					}
				}
				if (this.IsPlaying)
				{
					if (this.m_game != null)
					{
						if (isChangeHost && this.m_game is PVEGame)
						{
							PVEGame pveGame = this.m_game as PVEGame;
							foreach (Player p in pveGame.Players.Values)
							{
								if (p.PlayerDetail == this.m_host)
								{
									p.Ready = false;
								}
							}
						}
						this.m_game.RemovePlayer(player, isKick);
					}
					if (this.BattleServer != null)
					{
						if (this.m_game != null)
						{
							this.BattleServer.Server.SendPlayerDisconnet(this.Game.Id, player.GamePlayerId, this.RoomId);
							if (this.PlayerCount == 0)
							{
								this.BattleServer.RemoveRoom(this);
							}
							player.RemoveGP(player.PlayerCharacter.Grade * 12);
							if (this.GameType == eGameType.Guild)
							{
								player.RemoveOffer(15);
							}
							else
							{
								if (this.GameType == eGameType.Free)
								{
									player.RemoveOffer(5);
								}
							}
						}
						else
						{
							this.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed", new object[0]));
							RoomMgr.AddAction(new CancelPickupAction(this.BattleServer, this));
							this.BattleServer.RemoveRoom(this);
							this.IsPlaying = false;
						}
					}
				}
				this.UpdateGameStyle();
				if (isChangeHost)
				{
					if (!(this.m_game is PVEGame) || !this.IsPlaying)
					{
						this.ResetRoomSetting();
					}
				}
			}
			return index != -1;
		}
		public void RemovePlayerAtUnsafe(int pos)
		{
			if (pos >= 0 && pos <= 7)
			{
				if (this.m_places[pos] != null)
				{
					if (this.m_places[pos].KickProtect)
					{
						string message = LanguageMgr.GetTranslation("Game.Server.SceneGames.Protect", new object[]
						{
							this.m_places[pos].PlayerCharacter.NickName
						});
						GSPacketIn pkg = new GSPacketIn(3);
						pkg.WriteInt(0);
						pkg.WriteString(message);
						this.SendToHost(pkg);
					}
					else
					{
						this.RemovePlayerUnsafe(this.m_places[pos], true);
					}
				}
			}
		}
		public bool SwitchTeamUnsafe(GamePlayer m_player)
		{
			bool result;
			if (this.RoomType == eRoomType.Match)
			{
				result = false;
			}
			else
			{
				int index = -1;
				GamePlayer[] places;
				Monitor.Enter(places = this.m_places);
				try
				{
					for (int i = (m_player.CurrentRoomIndex + 1) % 2; i < 8; i += 2)
					{
						if (this.m_places[i] == null && this.m_placesState[i] == -1)
						{
							index = i;
							this.m_places[m_player.CurrentRoomIndex] = null;
							this.m_places[i] = m_player;
							this.m_placesState[m_player.CurrentRoomIndex] = -1;
							this.m_placesState[i] = m_player.PlayerId;
							this.m_playerState[i] = this.m_playerState[m_player.CurrentRoomIndex];
							this.m_playerState[m_player.CurrentRoomIndex] = 0;
							break;
						}
					}
				}
				finally
				{
					Monitor.Exit(places);
				}
				if (index != -1)
				{
					m_player.CurrentRoomIndex = index;
					m_player.CurrentRoomTeam = index % 2 + 1;
					GSPacketIn pkg = m_player.Out.SendRoomPlayerChangedTeam(m_player);
					this.SendToAll(pkg, m_player);
					this.SendPlaceState();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		private void ResetRoomSetting()
		{
			if (this.RoomType == eRoomType.Exploration)
			{
				int newLevel = (int)this.GetLevelLimit(this.m_host);
				if (this.LevelLimits > newLevel)
				{
					this.LevelLimits = newLevel;
				}
				this.HardLevel = eHardLevel.Normal;
			}
			else
			{
				if (this.RoomType == eRoomType.Treasure)
				{
					if (!this.m_host.IsPvePermission(this.MapId, this.HardLevel))
					{
						this.HardLevel = this.m_host.GetMaxPvePermission(this.MapId);
					}
				}
				else
				{
					if (this.RoomType == eRoomType.Boss)
					{
						this.HardLevel = eHardLevel.Simple;
					}
				}
			}
			this.UpdatePveRoomTimeMode();
			foreach (GamePlayer gp in this.GetPlayers())
			{
				gp.Out.SendRoomChange(this);
			}
		}
		public void UpdatePveRoomTimeMode()
		{
			if (this.RoomType == eRoomType.Exploration || this.RoomType == eRoomType.Boss || this.RoomType == eRoomType.Treasure || this.RoomType == eRoomType.Training)
			{
				switch (this.HardLevel)
				{
				case eHardLevel.Simple:
					this.TimeMode = 3;
					break;
				case eHardLevel.Normal:
					this.TimeMode = 3;
					break;
				case eHardLevel.Hard:
					this.TimeMode = 2;
					break;
				case eHardLevel.Terror:
					this.TimeMode = 1;
					break;
				}
			}
		}
		public eLevelLimits GetLevelLimit(GamePlayer player)
		{
            return eLevelLimits.None;
		}
		public void StartGame(AbstractGame game)
		{
			if (this.m_game != null)
			{
				List<GamePlayer> list = this.GetPlayers();
				foreach (GamePlayer player in list)
				{
					this.m_game.RemovePlayer(player, false);
				}
				this.m_game_GameStopped(this.m_game);
			}
			this.OldGameType = game.GameType;
			this.m_game = game;
			if (this.m_host != null && this.RoomType == eRoomType.Match)
			{
				if (this.IsAllSameGuild())
				{
					this.GameType = this.OldGameType;
					GSPacketIn pkg = this.m_host.Out.SendRoomType(this.m_host, this);
					this.SendToAll(pkg);
				}
			}
			this.IsPlaying = true;
			this.m_game.GameStopped += new GameEventHandle(this.m_game_GameStopped);
			if (this.PickUpNPC)
			{
				this.m_game.RoomRest += new GameEventHandle(this.m_game_RoomRest);
			}
		}
		private void m_game_RoomRest(AbstractGame game)
		{
			if (this.m_game != null && this.PickUpNPC)
			{
				this.m_game.RoomRest -= new GameEventHandle(this.m_game_RoomRest);
			}
			this.RoomType = this.OldRoomType;
			this.PickUpNPC = false;
			this.MapId = 0;
		}
		private void m_game_GameStopped(AbstractGame game)
		{
			if (game != null)
			{
				this.m_game.GameStopped -= new GameEventHandle(this.m_game_GameStopped);
				this.m_game = null;
			}
			this.IsPlaying = false;
			this.UpdateGameStyle();
			GamePlayer[] places = this.m_places;
			for (int i = 0; i < places.Length; i++)
			{
				GamePlayer player = places[i];
				if (player != null)
				{
					player.BufferList.Update();
				}
			}
			RoomMgr.WaitingRoom.SendUpdateRoom(this);
		}
		public void ProcessData(GSPacketIn packet)
		{
			if (this.m_game != null)
			{
				this.m_game.ProcessData(packet);
			}
		}
		public override string ToString()
		{
			return string.Format("Id:{0},player:{1},game:{2},isPlaying:{3}", new object[]
			{
				this.RoomId,
				this.PlayerCount,
				this.Game,
				this.IsPlaying
			});
		}
		public void RemoveAllPlayer()
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.m_places[i] != null)
				{
					RoomMgr.AddAction(new ExitRoomAction(this, this.m_places[i]));
					RoomMgr.AddAction(new EnterWaitingRoomAction(this.m_places[i], 1));
				}
			}
		}
	}
}
