using Bussiness;
using Bussiness.Managers;
using Fighting.Server.GameObjects;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Fighting.Server
{
	public class ServerClient : BaseClient
	{
        private new static LogProvider log => FightServer.log;
		private RSACryptoServiceProvider m_rsa;
		private FightServer m_svr;
		private Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();
		protected override void OnConnect()
		{
			base.OnConnect();
			this.m_rsa = new RSACryptoServiceProvider();
			RSAParameters para = this.m_rsa.ExportParameters(false);
			this.SendRSAKey(para.Modulus, para.Exponent);
			this.SendInfoToGameServer(ProxyRoomMgr.GetAllRoomCount(), ProxyRoomMgr.GetWaitingRoomCount(), FightServer.Instance.Configuration.ServerType);
		}
		protected override void OnDisconnect()
		{
			base.OnDisconnect();
			this.m_rsa = null;
		}
		public override void OnRecvPacket(GSPacketIn pkg)
		{
			short code = pkg.Code;
			if (code <= 25)
			{
				switch (code)
				{
				case 1:
					this.HandleLogin(pkg);
					break;
				case 2:
					this.HanleSendToGame(pkg);
					break;
				case 3:
					this.HandleSysNotice(pkg);
					break;
				case 4:
					this.HandleKitOffPlayer(pkg);
					break;
				case 5:
				case 6:
				case 7:
					break;
				case 8:
					this.HandleServerProperties(pkg);
					break;
				default:
					if (code != 19)
					{
						if (code == 25)
						{
							this.HandlerAreaBigBugle(pkg);
						}
					}
					else
					{
						this.HandlePlayerMessage(pkg);
					}
					break;
				}
			}
			else
			{
				if (code != 36)
				{
					switch (code)
					{
					case 64:
						this.HandleGameRoomCreate(pkg);
						break;
					case 65:
						this.HandleGameRoomCancel(pkg);
						break;
					case 66:
					case 67:
					case 68:
					case 70:
						break;
					case 69:
						this.HandleConsortiaAlly(pkg);
						break;
					case 71:
						this.HandleSendToGameAllPlayer(pkg);
						break;
					case 72:
						this.HandleChangeGameType(pkg);
						break;
					case 73:
						this.HandlerPickUpNPC(pkg);
						break;
					default:
						if (code == 83)
						{
							this.HandlePlayerDisconnet(pkg);
						}
						break;
					}
				}
				else
				{
					this.HandlePlayerUsingProp(pkg);
				}
			}
		}
		private void HandlerPickUpNPC(GSPacketIn pkg)
		{
			int roomid = pkg.ReadInt();
			ProxyRoom room = null;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(roomid))
				{
					room = this.m_rooms[roomid];
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (room != null && room.PickUpCount >= 2)
			{
				Console.WriteLine("可以撮合NPC");
				ProxyRoomMgr.FightWithNPC(room);
			}
		}
		private void HandlerAreaBigBugle(GSPacketIn pkg)
		{
			ServerClient[] clientlist = FightServer.Instance.GetAllClients();
			ServerClient[] array = clientlist;
			for (int i = 0; i < array.Length; i++)
			{
				ServerClient client = array[i];
				client.SendTCP(pkg);
			}
		}
		private void HandleChangeGameType(GSPacketIn pkg)
		{
			ProxyRoom room = null;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(pkg.Parameter1))
				{
					room = this.m_rooms[pkg.Parameter1];
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (room != null)
			{
				room.GameType = (eGameType)pkg.ReadInt();
			}
		}
		private void HandleKitOffPlayer(GSPacketIn pkg)
		{
			int playerid = pkg.ReadInt();
			int areaid = pkg.ReadInt();
			List<BaseGame> games = GameMgr.GetGames();
			foreach (BaseGame game in games)
			{
				Player[] players = game.GetAllPlayers();
				Player[] array = players;
				for (int i = 0; i < array.Length; i++)
				{
					Player p = array[i];
					if (p.PlayerDetail.PlayerCharacter.ID == playerid && p.PlayerDetail.AreaID == areaid)
					{
						game.RemovePlayer(p.PlayerDetail, false);
						p.Die();
						break;
					}
				}
			}
		}
		private void HandleSendToGameAllPlayer(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				GSPacketIn content = pkg.ReadPacket();
				Player except = game.FindPlayer(pkg.Parameter1);
				if (except != null)
				{
					game.SendToAll(content, except.PlayerDetail);
				}
				else
				{
					game.SendToAll(content);
				}
			}
		}
		private void HandlePlayerUsingProp(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				game.Resume();
				if (pkg.ReadBoolean())
				{
					Player player = game.FindPlayer(pkg.Parameter1);
					ItemTemplateInfo template = ItemMgr.FindItemTemplate(pkg.Parameter2);
					if (player != null && template != null)
					{
						player.UseItem(template);
					}
				}
			}
		}
		private void HandlePlayerDisconnet(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				Player player = game.FindPlayer(pkg.Parameter1);
				if (player != null)
				{
					GSPacketIn pkg2 = new GSPacketIn(83, player.PlayerDetail.PlayerCharacter.ID);
					pkg2.WriteInt(player.PlayerDetail.AreaID);
					game.SendToAll(pkg2, player.PlayerDetail);
					game.RemovePlayer(player.PlayerDetail, false);
				}
			}
		}
		public void HandleConsortiaAlly(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				game.consortiaAlly = pkg.ReadInt();
				game.richesRate = pkg.ReadInt();
			}
		}
		private void HandleSysNotice(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				Player player = game.FindPlayer(pkg.Parameter1);
				GSPacketIn pkg2 = new GSPacketIn(3);
				pkg2.WriteInt(3);
				pkg2.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[]
				{
					player.PlayerDetail.PlayerCharacter.Grade * 12,
					15
				}));
				player.PlayerDetail.SendTCP(pkg2);
				pkg2.ClearContext();
				pkg2.WriteInt(3);
				pkg2.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[]
				{
					player.PlayerDetail.PlayerCharacter.NickName,
					player.PlayerDetail.PlayerCharacter.Grade * 12,
					15
				}));
				game.SendToAll(pkg2, player.PlayerDetail);
			}
		}
		private void HandlePlayerMessage(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				Player player = game.FindPlayer(pkg.ReadInt());
				bool team = pkg.ReadBoolean();
				byte channl = pkg.ReadByte();
				string msg = pkg.ReadString();
				if (player != null)
				{
					GSPacketIn pkg2 = new GSPacketIn(19);
					pkg2.ClientID = player.PlayerDetail.PlayerCharacter.ID;
					pkg2.WriteInt(player.PlayerDetail.AreaID);
					pkg2.WriteByte(channl);
					pkg2.WriteBoolean(team);
					pkg2.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
					pkg2.WriteString(msg);
					if (team)
					{
						game.SendToTeam(pkg2, player.Team, player.PlayerDetail);
					}
					else
					{
						game.SendToAll(pkg2);
					}
				}
			}
		}
		public void HandleLogin(GSPacketIn pkg)
		{
			byte[] rgb = pkg.ReadBytes();
			string[] temp = Encoding.UTF8.GetString(this.m_rsa.Decrypt(rgb, false)).Split(new char[]
			{
				','
			});
			if (temp.Length == 2)
			{
				this.m_rsa = null;
				int id = int.Parse(temp[0]);
				base.Strict = false;
                if (temp[1] != FightServer.Instance.Configuration.FightKey)
                {
                    log.Debug(temp[1]);
                    ServerClient.log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
                    this.Disconnect();
                }

			}
			else
			{
				ServerClient.log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
				this.Disconnect();
			}
		}
		public void HandleGameRoomCreate(GSPacketIn pkg)
		{
			int totalLevel = 0;
			int totalFightPower = 0;
			int roomId = pkg.ReadInt();
			int gameType = pkg.ReadInt();
			int guildId = pkg.ReadInt();
			int areaId = pkg.ReadInt();
            bool IsArea = pkg.ReadBoolean();
			int count = pkg.ReadInt();
			IGamePlayer[] players = new IGamePlayer[count];
			for (int i = 0; i < count; i++)
			{
				PlayerInfo info = new PlayerInfo();
				ProxyPlayerInfo proxyPlayerInfo = new ProxyPlayerInfo();
				proxyPlayerInfo.m_AreaID = pkg.ReadInt();
				proxyPlayerInfo.m_AreaName = pkg.ReadString();
				info.ID = pkg.ReadInt();
				info.NickName = pkg.ReadString();
				info.Sex = pkg.ReadBoolean();
				info.Hide = pkg.ReadInt();
				info.Style = pkg.ReadString();
				info.Colors = pkg.ReadString();
				info.Skin = pkg.ReadString();
				info.Offer = pkg.ReadInt();
				info.GP = pkg.ReadInt();
				info.Grade = pkg.ReadInt();
				info.Repute = pkg.ReadInt();
				info.Nimbus = pkg.ReadInt();
				info.ConsortiaID = pkg.ReadInt();
				info.ConsortiaName = pkg.ReadString();
				info.ConsortiaLevel = pkg.ReadInt();
				info.ConsortiaRepute = pkg.ReadInt();
				info.Win = pkg.ReadInt();
				info.Total = pkg.ReadInt();
				info.Attack = pkg.ReadInt();
				info.Defence = pkg.ReadInt();
				info.Agility = pkg.ReadInt();
				info.Luck = pkg.ReadInt();
				info.FightPower = pkg.ReadInt();
				info.IsMarried = pkg.ReadBoolean();
				if (info.IsMarried)
				{
					info.SpouseID = pkg.ReadInt();
					info.SpouseName = pkg.ReadString();
				}
				totalFightPower += info.FightPower;
				proxyPlayerInfo.BaseAttack = pkg.ReadDouble();
				proxyPlayerInfo.BaseDefence = pkg.ReadDouble();
				proxyPlayerInfo.BaseAgility = pkg.ReadDouble();
				proxyPlayerInfo.BaseBlood = pkg.ReadDouble();
				proxyPlayerInfo.TemplateId = pkg.ReadInt();
				proxyPlayerInfo.CanUserProp = pkg.ReadBoolean();
				proxyPlayerInfo.SecondWeapon = pkg.ReadInt();
				proxyPlayerInfo.StrengthLevel = pkg.ReadInt();
				proxyPlayerInfo.GPAddPlus = pkg.ReadDouble();
				proxyPlayerInfo.GMExperienceRate = pkg.ReadFloat();
				proxyPlayerInfo.AuncherExperienceRate = pkg.ReadFloat();
				proxyPlayerInfo.OfferAddPlus = pkg.ReadDouble();
				proxyPlayerInfo.GMOfferRate = pkg.ReadFloat();
				proxyPlayerInfo.AuncherOfferRate = pkg.ReadFloat();
				proxyPlayerInfo.GMRichesRate = pkg.ReadFloat();
				proxyPlayerInfo.AuncherRichesRate = pkg.ReadFloat();
				proxyPlayerInfo.AntiAddictionRate = pkg.ReadDouble();
				List<BufferInfo> infos = new List<BufferInfo>();
				int buffercout = pkg.ReadInt();
				for (int j = 0; j < buffercout; j++)
				{
					BufferInfo buffinfo = new BufferInfo();
					buffinfo.Type = pkg.ReadInt();
					buffinfo.IsExist = pkg.ReadBoolean();
					buffinfo.BeginDate = pkg.ReadDateTime();
					buffinfo.ValidDate = pkg.ReadInt();
					buffinfo.Value = pkg.ReadInt();
					if (info != null)
					{
						infos.Add(buffinfo);
					}
				}
				players[i] = new ProxyPlayer(this, info, proxyPlayerInfo, infos);
				players[i].CanUseProp = proxyPlayerInfo.CanUserProp;
				int ec = pkg.ReadInt();
				for (int j = 0; j < ec; j++)
				{
					players[i].EquipEffect.Add(pkg.ReadInt());
				}
				totalLevel += info.Grade;
			}
			if (players.Length != 0)
			{
				ProxyRoom room = new ProxyRoom(ProxyRoomMgr.NextRoomId(), roomId, players, this, totalLevel, totalFightPower,IsArea);
				room.GuildId = guildId;
				room.AreaID = areaId;
				room.GameType = (eGameType)gameType;
				ProxyRoom oldroom = null;
				Dictionary<int, ProxyRoom> rooms;
				Monitor.Enter(rooms = this.m_rooms);
				try
				{
					if (this.m_rooms.ContainsKey(roomId))
					{
						oldroom = this.m_rooms[roomId];
						this.m_rooms.Remove(roomId);
					}
				}
				finally
				{
					Monitor.Exit(rooms);
				}
				if (oldroom != null)
				{
					ProxyRoomMgr.RemoveRoom(oldroom);
				}
				Monitor.Enter(rooms = this.m_rooms);
				try
				{
					if (!this.m_rooms.ContainsKey(roomId))
					{
						this.m_rooms.Add(roomId, room);
						this.SendFightRoomID(roomId, room.RoomId);
					}
					else
					{
						room = null;
					}
				}
				finally
				{
					Monitor.Exit(rooms);
				}
				if (room != null)
				{
					ProxyRoomMgr.AddRoom(room);
				}
				else
				{
					ServerClient.log.ErrorFormat("Room already exists:{0}", roomId);
				}
			}
		}
		public void HandleGameRoomCancel(GSPacketIn pkg)
		{
			ProxyRoom room = null;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(pkg.Parameter1))
				{
					room = this.m_rooms[pkg.Parameter1];
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (room != null)
			{
				ProxyRoomMgr.RemoveRoom(room);
			}
		}
		public void HanleSendToGame(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				GSPacketIn inner = pkg.ReadPacket();
				game.ProcessData(inner);
			}
		}
		private void HandleServerProperties(GSPacketIn pkg)
		{
			//GameProperties.Refresh();
		}
		public void SendRSAKey(byte[] m, byte[] e)
		{
			GSPacketIn pkg = new GSPacketIn(0);
			pkg.Write(m);
			pkg.Write(e);
			this.SendTCP(pkg);
		}
		public void SendPacketToPlayer(int playerId, GSPacketIn pkg, int gameId)
		{
			GSPacketIn p = new GSPacketIn(32, playerId);
			p.Parameter1 = gameId;
			p.WritePacket(pkg);
			this.SendTCP(p);
		}
		public void SendRemoveRoom(int roomId, int fightRoomId)
		{
			GSPacketIn pkg = new GSPacketIn(65, roomId);
			pkg.WriteInt(fightRoomId);
			this.SendTCP(pkg);
		}
		public void SendFightRoomID(int roomId, int fightRoomId)
		{
			GSPacketIn pkg = new GSPacketIn(69, roomId);
			pkg.WriteInt(fightRoomId);
			this.SendTCP(pkg);
		}
		public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
		{
			GSPacketIn p = new GSPacketIn(67, roomId);
			if (except != null)
			{
				p.Parameter1 = except.PlayerCharacter.ID;
				p.Parameter2 = except.GamePlayerId;
			}
			else
			{
				p.Parameter1 = 0;
				p.Parameter2 = 0;
			}
			p.WritePacket(pkg);
			this.SendTCP(p);
		}
		public void SendStartGame(int oldRoomId, AbstractGame game)
		{
			GSPacketIn pkg = new GSPacketIn(66);
			pkg.Parameter1 = oldRoomId;
			pkg.Parameter2 = game.Id;
			pkg.WriteInt((int)game.RoomType);
			pkg.WriteInt((int)game.GameType);
			pkg.WriteInt(game.TimeType);
			this.SendTCP(pkg);
		}
		public void SendStopGame(int oldRoomId, int gameId, int roomId)
		{
			GSPacketIn pkg = new GSPacketIn(68);
			pkg.Parameter1 = oldRoomId;
			pkg.Parameter2 = gameId;
			pkg.WriteInt(roomId);
			this.SendTCP(pkg);
		}
		public void SendGamePlayerId(IGamePlayer player)
		{
			this.SendTCP(new GSPacketIn(33)
			{
				Parameter1 = player.PlayerCharacter.ID,
				Parameter2 = player.GamePlayerId
			});
		}
		public void SendDisconnectPlayer(int playerId)
		{
			GSPacketIn pkg = new GSPacketIn(34, playerId);
			this.SendTCP(pkg);
		}
		public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			GSPacketIn pkg = new GSPacketIn(35, playerId);
			pkg.Parameter1 = gameId;
			pkg.WriteBoolean(isWin);
			pkg.WriteInt(gainXp);
			pkg.WriteBoolean(isSpanArea);
			pkg.WriteBoolean(isCouple);
			this.SendTCP(pkg);
		}
		public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
		{
			GSPacketIn pkg = new GSPacketIn(36, playerId);
			pkg.Parameter1 = bag;
			pkg.Parameter2 = place;
			pkg.WriteInt(templateId);
			pkg.WriteBoolean(isLiving);
			this.SendTCP(pkg);
		}
		public void SendPlayerAddGold(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(38, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendPlayerRemoveGold(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(44, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendPlayerAddGP(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(39, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendPlayerRemoveGP(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(49, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea)
		{
			GSPacketIn pkg = new GSPacketIn(40, playerId);
			pkg.WriteInt(type);
			pkg.WriteBoolean(isLiving);
			pkg.WriteInt(demage);
			pkg.WriteBoolean(isSpanArea);
			this.SendTCP(pkg);
		}
		public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
		{
			GSPacketIn pkg = new GSPacketIn(41, playerId);
			pkg.WriteBoolean(isWin);
			pkg.WriteInt(MissionID);
			pkg.WriteInt(turnNum);
			this.SendTCP(pkg);
		}
		public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
		{
			GSPacketIn pkg = new GSPacketIn(42, playerId);
			pkg.WriteInt(consortiaWin);
			pkg.WriteInt(consortiaLose);
			pkg.WriteInt(count);
			pkg.WriteByte((byte)roomType);
			pkg.WriteByte((byte)gameClass);
			pkg.WriteInt(totalKillHealth);
			pkg.WriteInt(totalKillHealth);
			this.SendTCP(pkg);
		}
		public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(43, playerId);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(riches);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendPlayerRemoveMoney(int playerId, int value, int master, int son)
		{
			GSPacketIn pkg = new GSPacketIn(45, playerId);
			pkg.WriteInt(value);
			pkg.WriteInt(master);
			pkg.WriteInt(son);
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendPlayerRemoveOffer(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(50, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendAddRobRiches(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(52, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendPlayerAddOffer(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(51, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			this.SendTCP(pkg);
		}
		public void SendPlayerAddTemplate(int playerId, ItemInfo cloneItem, eBageType bagType, int count)
		{
			if (cloneItem != null)
			{
				GSPacketIn pkg = new GSPacketIn(48, playerId);
				pkg.WriteInt(cloneItem.TemplateID);
				pkg.WriteByte((byte)bagType);
				pkg.WriteInt(count);
				pkg.WriteInt(cloneItem.ValidDate);
				pkg.WriteBoolean(cloneItem.IsBinds);
				pkg.WriteBoolean(cloneItem.IsUsed);
				pkg.WriteBoolean(cloneItem.IsTips);
				pkg.WriteBoolean(cloneItem.IsLogs);
				pkg.WriteInt(cloneItem.TemplateID + count);
				this.SendTCP(pkg);
			}
		}
		public void SendRemovePlayer(int playerid, int roomid)
		{
			GSPacketIn pkg = new GSPacketIn(70, playerid);
			pkg.WriteInt(roomid);
			this.SendTCP(pkg);
		}
		public void SendClearBag(int playerid, int type)
		{
			GSPacketIn pkg = new GSPacketIn(53, playerid);
			pkg.WriteInt(type);
			this.SendTCP(pkg);
		}
		public void SendLogFight(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
		{
			GSPacketIn pkg = new GSPacketIn(5);
			pkg.WriteInt(_roomId);
			pkg.WriteInt((int)_roomType);
			pkg.WriteInt((int)_fightType);
			pkg.WriteInt(_changeTeam);
			pkg.WriteDateTime(_playBegin);
			pkg.WriteDateTime(_playEnd);
			pkg.WriteInt(_userCount);
			pkg.WriteInt(_mapId);
			pkg.WriteString(_teamA);
			pkg.WriteString(_teamB);
			pkg.WriteString(_playResult);
			pkg.WriteInt(_winTeam);
			pkg.WriteString(BossWar);
			this.SendTCP(pkg);
		}
		public GSPacketIn SendInfoToGameServer(int roomCount, int waitingRoomCount, int type)
		{
			GSPacketIn pkg = new GSPacketIn(7);
			pkg.WriteInt(roomCount);
			pkg.WriteInt(waitingRoomCount);
			pkg.WriteInt(type);
			this.SendTCP(pkg);
			return pkg;
		}
		private void SendPickUpNPC(int orientId, int p)
		{
			GSPacketIn pkg = new GSPacketIn(73, orientId);
			pkg.WriteInt(p);
			this.SendTCP(pkg);
		}
		public ServerClient(FightServer svr) : base(new byte[20480], new byte[20480])
		{
			this.m_svr = svr;
			base.Strict = false;
		}
		public override string ToString()
		{
			return string.Format("Server Client: {0} IsConnected:{1}  RoomCount:{2}", 0, base.IsConnected, this.m_rooms.Count);
		}
		public void RemoveRoom(int orientId, ProxyRoom room)
		{
			bool result = false;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(orientId) && this.m_rooms[orientId] == room)
				{
					result = this.m_rooms.Remove(orientId);
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (result)
			{
				this.SendRemoveRoom(orientId, room.RoomId);
			}
		}
		public void PickUpNPC(int orientId, ProxyRoom room)
		{
			bool result = false;
			Dictionary<int, ProxyRoom> rooms;
			Monitor.Enter(rooms = this.m_rooms);
			try
			{
				if (this.m_rooms.ContainsKey(orientId) && this.m_rooms[orientId] == room)
				{
					result = this.m_rooms.Remove(orientId);
				}
			}
			finally
			{
				Monitor.Exit(rooms);
			}
			if (result)
			{
				this.SendPickUpNPC(orientId, room.RoomId);
			}
		}
	}
}
