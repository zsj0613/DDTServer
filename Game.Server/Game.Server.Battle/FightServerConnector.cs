using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.LogEnum;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;

using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
namespace Game.Server.Battle
{
	public class FightServerConnector : BaseConnector
	{
		private static LogProvider log => LogProvider.Default;
		private BattleServer m_server;
		private string m_key;
		public override void OnRecvPacket(GSPacketIn pkg)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsynProcessPacket), pkg);
		}
		protected void AsynProcessPacket(object state)
		{
			try
			{
				GSPacketIn pkg = state as GSPacketIn;
				short code = pkg.Code;
				if (code != 0)
				{
					switch (code)
					{
					case 5:
						this.HandleLogFight(pkg);
						break;
					case 6:
						break;
					case 7:
						this.HandleInfoUpate(pkg);
						break;
					default:
						switch (code)
						{
						case 25:
							this.HandleAreaBigBugle(pkg);
							break;
						case 32:
							this.HandleSendToPlayer(pkg);
							break;
						case 33:
							this.HandleUpdatePlayerGameId(pkg);
							break;
						case 34:
							this.HandleDisconnectPlayer(pkg);
							break;
						case 35:
							this.HandlePlayerOnGameOver(pkg);
							break;
						case 36:
							this.HandlePlayerOnUsingItem(pkg);
							break;
						case 38:
							this.HandlePlayerAddGold(pkg);
							break;
						case 39:
							this.HandlePlayerAddGP(pkg);
							break;
						case 40:
							this.HandlePlayerOnKillingLiving(pkg);
							break;
						case 41:
							this.HandlePlayerOnMissionOver(pkg);
							break;
						case 42:
							this.HandlePlayerConsortiaFight(pkg);
							break;
						case 43:
							this.HandlePlayerSendConsortiaFight(pkg);
							break;
						case 44:
							this.HandlePlayerRemoveGold(pkg);
							break;
						case 45:
							this.HandlePlayerRemoveMoney(pkg);
							break;
						case 48:
							this.HandlePlayerAddTemplate(pkg);
							break;
						case 49:
							this.HandlePlayerRemoveGP(pkg);
							break;
						case 50:
							this.HandlePlayerRemoveOffer(pkg);
							break;
						case 51:
							this.HandlePlayerAddOffer(pkg);
							break;
						case 52:
							this.HandPlayerAddRobRiches(pkg);
							break;
						case 53:
							this.HandleClearBag(pkg);
							break;
						case 65:
							this.HandleRoomRemove(pkg);
							break;
						case 66:
							this.HandleStartGame(pkg);
							break;
						case 67:
							this.HandleSendToRoom(pkg);
							break;
						case 68:
							this.HandleStopGame(pkg);
							break;
						case 69:
							this.HandleUpdateRoomId(pkg);
							break;
						case 70:
							this.HandlePlayerRemove(pkg);
							break;
						case 73:
							this.HandlerPickUpNPC(pkg);
							break;
						}
						break;
					}
				}
				else
				{
					this.HandleRSAKey(pkg);
				}
			}
			catch (Exception ex)
			{
				GameServer.log.Error("AsynProcessPacket", ex);
			}
		}
		private void HandlerPickUpNPC(GSPacketIn pkg)
		{
			this.m_server.RandomNPC(pkg.ClientID, pkg.ReadInt());
		}
		private void HandleAreaBigBugle(GSPacketIn pkg)
		{
			MessageMgr.SendMessages(pkg);
		}
		private void HandleInfoUpate(GSPacketIn pkg)
		{
			this.m_server.RoomCount = pkg.ReadInt();
			this.m_server.WaitingRoomCount = pkg.ReadInt();
			this.m_server.ServerType = pkg.ReadInt();
		}
		private void HandleAreaMessages(GSPacketIn pkg)
		{
			GSPacketIn packet = pkg.ReadPacket();
			GameServer.Instance.LoginServer.SendPacket(packet);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer player = array[i];
				player.Out.SendTCP(packet);
			}
		}
		private void HandleLogFight(GSPacketIn pkg)
		{
			//LogMgr.LogFightAdd(pkg.ReadInt(), (eRoomType)pkg.ReadInt(), (eGameType)pkg.ReadInt(), pkg.ReadInt(), pkg.ReadDateTime(), pkg.ReadDateTime(), pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString(), pkg.ReadString(), pkg.ReadString(), pkg.ReadInt(), pkg.ReadString());
		}
		private void HandleClearBag(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int type = pkg.ReadInt();
			if (player != null)
			{
				if (type == 1)
				{
					player.ClearTempBag();
				}
				else
				{
					if (type == 2)
					{
						player.ClearFightBag();
					}
				}
			}
		}
		private void HandleUpdateRoomId(GSPacketIn pkg)
		{
			this.m_server.UpdateRoomId(pkg.ClientID, pkg.ReadInt());
		}
		private void HandlePlayerRemove(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int roomId = pkg.ReadInt();
			if (player != null && player.CurrentRoom != null && player.CurrentRoom.RoomId == roomId)
			{
				RoomMgr.AddAction(new ExitRoomAction(player.CurrentRoom, player));
				RoomMgr.AddAction(new EnterWaitingRoomAction(player, 1));
			}
		}
		private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null && player.CurrentRoom != null)
			{
				AbstractGame game = player.CurrentRoom.Game;
				if (player != null)
				{
					player.OnKillingLiving(game, pkg.ReadInt(), pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadBoolean());
				}
			}
		}
		private void HandlePlayerOnMissionOver(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			AbstractGame game = player.CurrentRoom.Game;
			if (player != null)
			{
				player.OnMissionOver(game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
			}
		}
		private void HandlePlayerConsortiaFight(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int consortiaWin = pkg.ReadInt();
			int consortiaLose = pkg.ReadInt();
			int count = pkg.ReadInt();
			eRoomType roomtype = (eRoomType)pkg.ReadByte();
			eGameType gametype = (eGameType)pkg.ReadByte();
			int totalKillHealth = pkg.ReadInt();
			int check = pkg.ReadInt();
			if (player != null && check == totalKillHealth)
			{
				player.ConsortiaFight(consortiaWin, consortiaLose, roomtype, gametype, totalKillHealth, count);
			}
		}
		private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.SendConsortiaFight(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString());
			}
		}
		private void HandPlayerAddRobRiches(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.AddRobRiches(pkg.Parameter1);
			}
		}
		private void HandlePlayerRemoveMoney(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int value = pkg.ReadInt();
			int type = pkg.ReadInt();
			int type2 = pkg.ReadInt();
			int check = pkg.ReadInt();
			if (player != null && check == value)
			{
				player.RemoveMoney(value, (LogMoneyType)type, (LogMoneyType)type2);
			}
		}
		private void HandlePlayerAddOffer(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.AddOffer(pkg.Parameter1, false);
			}
		}
		private void HandlePlayerRemoveOffer(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.RemoveOffer(pkg.Parameter1);
			}
		}
		private void HandlePlayerAddTemplate(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				ItemTemplateInfo template = ItemMgr.FindItemTemplate(pkg.ReadInt());
				eBageType type = (eBageType)pkg.ReadByte();
				if (template != null)
				{
					int Count = pkg.ReadInt();
					ItemInfo item = ItemInfo.CreateFromTemplate(template, Count, 118);
					item.Count = Count;
					item.ValidDate = pkg.ReadInt();
					item.IsBinds = pkg.ReadBoolean();
					item.IsUsed = pkg.ReadBoolean();
					item.IsTips = pkg.ReadBoolean();
					item.IsLogs = pkg.ReadBoolean();
					int check = pkg.ReadInt();
					if (check == template.TemplateID + Count)
					{
						if (!player.AddTemplate(item, type, item.Count, item.IsTips))
						{
						}
					}
					else
					{
						FightServerConnector.log.ErrorFormat("add template item error: userid {0} template id {1} count {2}", pkg.ClientID, template.TemplateID, Count);
					}
				}
			}
		}
		private void HandlePlayerAddGP(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.AddGP(pkg.Parameter1);
			}
		}
		private void HandlePlayerRemoveGP(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.RemoveGP(pkg.Parameter1);
			}
		}
		private void HandlePlayerAddGold(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.AddGold(pkg.Parameter1);
			}
		}
		private void HandlePlayerRemoveGold(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.RemoveGold(pkg.Parameter1);
			}
		}
		private void HandlePlayerOnUsingItem(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				int templateId = pkg.ReadInt();
				bool result = player.UsePropItem(null, pkg.Parameter1, pkg.Parameter2, templateId, pkg.ReadBoolean());
				if (player.CurrentRoom != null && player.CurrentRoom.Game != null)
				{
					this.SendUsingPropInGame(player.CurrentRoom.Game.Id, player.GamePlayerId, templateId, result);
				}
			}
		}
		public void SendAddRoom(BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(64);
			pkg.WriteInt(room.RoomId);
			pkg.WriteInt((int)room.GameType);
			pkg.WriteInt(room.GuildId);
			pkg.WriteInt(GameServer.Instance.Configuration.AreaID);
			List<GamePlayer> players = room.GetPlayers();
			pkg.WriteInt(players.Count);
			foreach (GamePlayer p in players)
			{
				pkg.WriteInt(p.AreaID);
				pkg.WriteString(p.AreaName);
				pkg.WriteInt(p.PlayerCharacter.ID);
				pkg.WriteString(p.PlayerCharacter.NickName);
				pkg.WriteBoolean(p.PlayerCharacter.Sex);
				pkg.WriteInt(p.PlayerCharacter.Hide);
				pkg.WriteString(p.PlayerCharacter.Style);
				pkg.WriteString(p.PlayerCharacter.Colors);
				pkg.WriteString(p.PlayerCharacter.Skin);
				pkg.WriteInt(p.PlayerCharacter.Offer);
				pkg.WriteInt(p.PlayerCharacter.GP);
				pkg.WriteInt(p.PlayerCharacter.Grade);
				pkg.WriteInt(p.PlayerCharacter.Repute);
				pkg.WriteInt(p.PlayerCharacter.Nimbus);
				pkg.WriteInt(p.PlayerCharacter.ConsortiaID);
				pkg.WriteString(p.PlayerCharacter.ConsortiaName);
				pkg.WriteInt(p.PlayerCharacter.ConsortiaLevel);
				pkg.WriteInt(p.PlayerCharacter.ConsortiaRepute);
				pkg.WriteInt(p.PlayerCharacter.Win);
				pkg.WriteInt(p.PlayerCharacter.Total);
				pkg.WriteInt(p.PlayerCharacter.Attack);
				pkg.WriteInt(p.PlayerCharacter.Defence);
				pkg.WriteInt(p.PlayerCharacter.Agility);
				pkg.WriteInt(p.PlayerCharacter.Luck);
				pkg.WriteInt(p.PlayerCharacter.FightPower);
				pkg.WriteBoolean(p.PlayerCharacter.IsMarried);
				if (p.PlayerCharacter.IsMarried)
				{
					pkg.WriteInt(p.PlayerCharacter.SpouseID);
					pkg.WriteString(p.PlayerCharacter.SpouseName);
				}
				pkg.WriteDouble(p.GetBaseAttack());
				pkg.WriteDouble(p.GetBaseDefence());
				pkg.WriteDouble(p.GetBaseAgility());
				pkg.WriteDouble(p.GetBaseBlood());
				if (p.MainWeapon != null)
				{
					pkg.WriteInt(p.MainWeapon.TemplateID);
				}
				else
				{
					pkg.WriteInt(0);
				}
				pkg.WriteBoolean(p.CanUseProp);
				if (p.SecondWeapon != null)
				{
					pkg.WriteInt(p.SecondWeapon.TemplateID);
					pkg.WriteInt(p.SecondWeapon.StrengthenLevel);
				}
				else
				{
					pkg.WriteInt(0);
					pkg.WriteInt(0);
				}
				pkg.WriteDouble(p.GPAddPlus);
				pkg.WriteFloat(p.GMExperienceRate);
				pkg.WriteFloat(p.AuncherExperienceRate);
				pkg.WriteDouble(p.OfferAddPlus);
				pkg.WriteFloat(p.GMOfferRate);
				pkg.WriteFloat(p.AuncherOfferRate);
				pkg.WriteFloat(p.GMRichesRate);
				pkg.WriteFloat(p.AuncherRichesRate);
				pkg.WriteDouble(p.AntiAddictionRate);
				List<AbstractBuffer> infos = p.BufferList.GetAllBuffer();
				pkg.WriteInt(infos.Count);
				foreach (AbstractBuffer bufferInfo in infos)
				{
					BufferInfo info = bufferInfo.Info;
					pkg.WriteInt(info.Type);
					pkg.WriteBoolean(info.IsExist);
					pkg.WriteDateTime(info.BeginDate);
					pkg.WriteInt(info.ValidDate);
					pkg.WriteInt(info.Value);
				}
				pkg.WriteInt(p.EquipEffect.Count);
				foreach (int i in p.EquipEffect)
				{
					pkg.WriteInt(i);
				}
			}
			this.SendTCP(pkg);
		}
		public void SendChangeGameType(BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(72);
			pkg.Parameter1 = room.RoomId;
			pkg.WriteInt((int)room.GameType);
			this.SendTCP(pkg);
		}
		public void SendRemoveRoom(BaseRoom room)
		{
			this.SendTCP(new GSPacketIn(65)
			{
				Parameter1 = room.RoomId
			});
		}
		public void SendToGame(int gameId, GSPacketIn pkg)
		{
			GSPacketIn wrapper = new GSPacketIn(2, gameId);
			wrapper.WritePacket(pkg);
			this.SendTCP(wrapper);
		}
		protected void HandleRoomRemove(GSPacketIn packet)
		{
			this.m_server.RemoveRoomImp(packet.ClientID, packet.ReadInt());
		}
		protected void HandleStartGame(GSPacketIn pkg)
		{
			ProxyGame game = new ProxyGame(pkg.Parameter2, this, (eRoomType)pkg.ReadInt(), (eGameType)pkg.ReadInt(), pkg.ReadInt());
			this.m_server.StartGame(pkg.Parameter1, game);
		}
		protected void HandleStopGame(GSPacketIn pkg)
		{
			int roomId = pkg.Parameter1;
			int gameId = pkg.Parameter2;
			this.m_server.StopGame(roomId, gameId, pkg.ReadInt());
		}
		protected void HandleSendToRoom(GSPacketIn pkg)
		{
			int roomId = pkg.ClientID;
			GSPacketIn inner = pkg.ReadPacket();
			this.m_server.SendToRoom(roomId, inner, pkg.Parameter1, pkg.Parameter2);
		}
		protected void HandleSendToPlayer(GSPacketIn pkg)
		{
			int playerId = pkg.ClientID;
			try
			{
				int gameId = pkg.Parameter1;
				GSPacketIn inner = pkg.ReadPacket();
				this.m_server.SendToUser(playerId, inner, gameId);
			}
			catch (Exception ex)
			{
				FightServerConnector.log.Error(string.Format("pkg len:{0}", pkg.Length), ex);
				FightServerConnector.log.Error(Marshal.ToHexDump("pkg content:", pkg.Buffer, 0, pkg.Length));
			}
		}
		private void HandleUpdatePlayerGameId(GSPacketIn pkg)
		{
			this.m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
		}
		public void SendChatMessage(string msg, GamePlayer player, bool team, byte channl)
		{
			GSPacketIn pkg = new GSPacketIn(19, player.CurrentRoom.Game.Id);
			pkg.WriteInt(player.GamePlayerId);
			pkg.WriteBoolean(team);
			pkg.WriteByte(channl);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendFightNotice(GamePlayer player, int GameId)
		{
			this.SendTCP(new GSPacketIn(3, GameId)
			{
				Parameter1 = player.GamePlayerId
			});
		}
		public void SendToGameAllPlayer(int gameId, IGamePlayer except, GSPacketIn content)
		{
			GSPacketIn pkg = new GSPacketIn(71, gameId);
			pkg.Parameter1 = ((except != null) ? except.GamePlayerId : -1);
			pkg.WritePacket(content);
			this.SendTCP(pkg);
		}
		public void SendServerProperties()
		{
			GSPacketIn pkg = new GSPacketIn(8);
			this.SendTCP(pkg);
		}
		private void SendUsingPropInGame(int gameId, int playerId, int templateId, bool result)
		{
			GSPacketIn pkg = new GSPacketIn(36, gameId);
			pkg.Parameter1 = playerId;
			pkg.Parameter2 = templateId;
			pkg.WriteBoolean(result);
			this.SendTCP(pkg);
		}
		public void SendPlayerDisconnet(int gameId, int playerId, int roomId)
		{
			this.SendTCP(new GSPacketIn(83, gameId)
			{
				Parameter1 = playerId,
				Parameter2 = roomId
			});
		}
		public void SendKitOffPlayer(int playerid, int areaid)
		{
			GSPacketIn pkg = new GSPacketIn(4);
			pkg.WriteInt(playerid);
			pkg.WriteInt(areaid);
			this.SendTCP(pkg);
		}
		private void HandlePlayerOnGameOver(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null && player.CurrentRoom != null && player.CurrentRoom.Game != null)
			{
				player.OnGameOver(player.CurrentRoom.Game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadBoolean(), pkg.ReadBoolean());
			}
		}
		private void HandleDisconnectPlayer(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.Disconnect();
			}
		}
		public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
		{
			GSPacketIn pkg = new GSPacketIn(1);
			pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
			this.SendTCP(pkg);
		}
		protected void HandleRSAKey(GSPacketIn packet)
		{
			RSAParameters para = default(RSAParameters);
			para.Modulus = packet.ReadBytes(128);
			para.Exponent = packet.ReadBytes();
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(para);
			this.SendRSALogin(rsa, this.m_key);
		}
		public void SendPickUpNPC(int roomid)
		{
			GSPacketIn pkg = new GSPacketIn(73);
			pkg.WriteInt(roomid);
			this.SendTCP(pkg);
		}
		public FightServerConnector(BattleServer server, string ip, int port, string key) : base(ip, port, false, new byte[20480], new byte[20480])
		{
			this.m_server = server;
			this.m_key = key;
			base.Strict = false;
		}
	}
}
