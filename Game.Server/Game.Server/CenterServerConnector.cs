using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.SceneMarryRooms;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Game.Server
{
	public class CenterServerConnector : BaseConnector
	{
        private new static LogProvider log => GameServer.log;
		private int m_serverId;
		private string m_loginKey;
		protected override void OnConnect()
		{
			base.OnConnect();
			this.m_privLevel = ePrivLevel.Admin;
		}
		protected override void OnDisconnect()
		{
			base.OnDisconnect();
		}
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
				if (code <= 123)
				{
					if (code <= 38)
					{
						switch (code)
						{
						case 0:
							this.HandleRSAKey(pkg);
							break;
						case 1:
						case 6:
						case 7:
						case 8:
						case 12:
						case 17:
						case 18:
						case 20:
						case 21:
						case 22:
						case 23:
						case 24:
							break;
						case 2:
							this.HandleKitoffPlayer(pkg);
							break;
						case 3:
							this.HandleAllowUserLogin(pkg);
							break;
						case 4:
							this.HandleUserOffline(pkg);
							break;
						case 5:
							this.HandleUserOnline(pkg);
							break;
						case 9:
							this.HandleChargeMoney(pkg);
							break;
						case 10:
							this.HandleSystemNotice(pkg);
							break;
						case 11:
							this.HandleCmd(pkg);
							break;
						case 13:
							this.HandleUpdatePlayerMarriedState(pkg);
							break;
						case 14:
							this.HandleGetItemMessage(pkg);
							break;
						case 15:
							this.HandleShutdown(pkg);
							break;
						case 16:
							this.HandleGiftToken(pkg);
							break;
						case 19:
							this.HandleChatConsortia(pkg);
							break;
						case 25:
							this.HandleAreaBigBugle(pkg);
							break;
						case 26:
							this.HandleMarryRoomInfoToPlayer(pkg);
							break;
						default:
							switch (code)
							{
							case 37:
								this.HandleChatPersonal(pkg);
								break;
							case 38:
								this.HandleSysMess(pkg);
								break;
							}
							break;
						}
					}
					else
					{
						if (code != 72)
						{
							if (code != 117)
							{

                                if (code == 118)
                                {
                                    this.HandleCharge(pkg);
                                }

                                if (code == 123)
								{
									this.HandleDispatches(pkg);
								}
							}
							else
							{
								this.HandleMailResponse(pkg);
							}
						}
						else
						{
							this.HandleBigBugle(pkg);
						}
					}
				}
				else
				{
					if (code <= 168)
					{
						switch (code)
						{
						case 128:
							this.HandleConsortiaResponse(pkg);
							break;
						case 129:
							break;
						case 130:
							this.HandleConsortiaCreate(pkg);
							break;
						default:
							if (code != 158)
							{
								switch (code)
								{
								case 165:
									this.HandleFriendState(pkg);
									break;
								case 166:
									this.HandleFirendResponse(pkg);
									break;
								case 168:
									this.HandleUpdateLimitCount(pkg);
									break;
								}
							}
							else
							{
								this.HandleConsortiaFight(pkg);
							}
							break;
						}
					}
					else
					{
						switch (code)
						{
						case 177:
							this.HandleRate(pkg);
							break;
						case 178:
							this.HandleMacroDrop(pkg);
							break;
						default:
							switch (code)
							{
							case 204:
								this.HandleUpdateShopNotice(pkg);
								break;
							case 205:
								this.HandleServerProperties(pkg);
								break;
							default:
								if (code == 241)
								{
									this.HandleMarryRoomDispose(pkg);
								}
								break;
							}
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				GameServer.log.Error("AsynProcessPacket", ex);
			}
		}
		private void HandleGetItemMessage(GSPacketIn pkg)
		{
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(pkg);
			}
		}
		private void HandleAreaBigBugle(GSPacketIn pkg)
		{
			GSPacketIn packet = new GSPacketIn(73);
			packet.WriteInt(pkg.ReadInt());
			packet.WriteInt(pkg.ReadInt());
			packet.WriteString(pkg.ReadString());
            packet.WriteString(pkg.ReadString());
            packet.WriteString(pkg.ReadString());
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(packet);
			}
		}
		protected void HandleRSAKey(GSPacketIn packet)
		{
			RSAParameters para = default(RSAParameters);
			para.Modulus = packet.ReadBytes(128);
			para.Exponent = packet.ReadBytes();
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(para);
			this.SendRSALogin(rsa, this.m_loginKey);
			this.SendListenIPPort(IPAddress.Parse(GameServer.Instance.Configuration.IP), GameServer.Instance.Configuration.Port);
		}
		protected void HandleKitoffPlayer(object stateInfo)
		{
			try
			{
				GSPacketIn packet = (GSPacketIn)stateInfo;
				int playerid = packet.ReadInt();
				GamePlayer client = WorldMgr.GetPlayerById(playerid);
				if (client != null)
				{
					string msg = packet.ReadString();
					client.Out.SendKitoff(msg);
					client.Client.Disconnect();
				}
				else
				{
					this.SendUserOffline(playerid, 0);
				}
			}
			catch (Exception e)
			{
				GameServer.log.Error("HandleKitoffPlayer", e);
			}
		}
		protected void HandleAllowUserLogin(object stateInfo)
		{
			try
			{
				GSPacketIn packet = (GSPacketIn)stateInfo;
				int playerid = packet.ReadInt();
				if (packet.ReadBoolean())
				{
					GamePlayer player = LoginMgr.LoginClient(playerid);
					if (player != null)
					{
						if (player.Login())
						{
							this.SendUserOnline(playerid, player.PlayerCharacter.ConsortiaID);
							WorldMgr.OnPlayerOnline(playerid, player.PlayerCharacter.ConsortiaID, player.AreaID, true);
						}
						else
						{
							player.Client.Disconnect();
							this.SendUserOffline(playerid, 0);
						}
					}
					else
					{
						this.SendUserOffline(playerid, 0);
					}
				}
			}
			catch (Exception e)
			{
				GameServer.log.Error("HandleAllowUserLogin", e);
			}
		}
		protected void HandleUserOffline(GSPacketIn packet)
		{
			int count = packet.ReadInt();
			for (int i = 0; i < count; i++)
			{
				int playerid = packet.ReadInt();
				int consortiaID = packet.ReadInt();
				if (LoginMgr.ContainsUser(playerid))
				{
					this.SendAllowUserLogin(playerid);
				}
				WorldMgr.OnPlayerOffline(playerid, consortiaID);
			}
		}
		protected void HandleUserOnline(GSPacketIn packet)
		{
			int count = packet.ReadInt();
			for (int i = 0; i < count; i++)
			{
				int playerid = packet.ReadInt();
				int consortiaID = packet.ReadInt();
				LoginMgr.ClearLoginPlayer(playerid);
				GamePlayer player = WorldMgr.GetPlayerById(playerid);
				if (player != null)
				{
					GameServer.log.Error("Player hang in server!!!");
					player.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext", new object[0]));
					player.Client.Disconnect();
				}
				WorldMgr.OnPlayerOnline(playerid, consortiaID, 0, false);
			}
		}
		protected void HandleChatPersonal(GSPacketIn packet)
		{
			int playerID = packet.ReadInt();
			GamePlayer client = WorldMgr.GetPlayerById(playerID);
			if (client != null && !client.IsBlackFriend(packet.ClientID))
			{
				client.Out.SendTCP(packet);
			}
		}
		protected void HandleBigBugle(GSPacketIn packet)
		{
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(packet);
			}
		}
		protected void HandleDispatches(GSPacketIn packet)
		{
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(packet);
			}
		}
		public void HandleFriendState(GSPacketIn pkg)
		{
			WorldMgr.ChangePlayerState(pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt());
		}
		public void HandleFirendResponse(GSPacketIn packet)
		{
			int playerID = packet.ReadInt();
			GamePlayer client = WorldMgr.GetPlayerById(playerID);
			if (client != null)
			{
				client.Out.SendTCP(packet);
			}
		}
		public void HandleMailResponse(GSPacketIn packet)
		{
			int playerID = packet.ReadInt();
			GamePlayer client = WorldMgr.GetPlayerById(playerID);
			if (client != null)
			{
				client.Out.SendTCP(packet);
			}
		}
        public void HandleCharge(GSPacketIn packet)
        {
            int playerID = packet.ReadInt();
            GamePlayer client = WorldMgr.GetPlayerById(playerID);
            if (client != null)
            {
                client.UpdateVIP();
                //ÓÊ¼þÌáÐÑ
                GSPacketIn pkgMsg = new GSPacketIn(117);
                pkgMsg.WriteInt(playerID);
                pkgMsg.WriteInt(1);
                client.Out.SendTCP(pkgMsg);
            }
        }
        public void HandleCmd(GSPacketIn packet)
		{
			try
			{
				string cmdLine = packet.ReadString();
				bool result = CommandMgr.HandleCommandNoPlvl(this, cmdLine);
				this.SendCmdResult(cmdLine, result);
			}
			catch (Exception ex)
			{
				CenterServerConnector.log.Error(ex.Message, ex);
			}
		}
		public void HandleChargeMoney(GSPacketIn packet)
		{
			int playerID = packet.ClientID;
			GamePlayer client = WorldMgr.GetPlayerById(playerID);
			if (client != null)
			{
				client.ChargeToUser();
			}
		}
		public void HandleGiftToken(GSPacketIn packet)
		{
			int playerID = packet.ClientID;
			int giftToken = packet.ReadInt();
			GamePlayer client = WorldMgr.GetPlayerById(playerID);
			if (client != null)
			{
				client.ChargeGiftTokenToUser(giftToken);
			}
		}
		public void HandleSystemNotice(GSPacketIn packet)
		{
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(packet);
			}
		}
		public void HandleSysMess(GSPacketIn packet)
		{
			int type = packet.ReadInt();
			int num = type;
			if (num == 1)
			{
				int playerID = packet.ReadInt();
				string nickname = packet.ReadString().Replace("\0", "");
				GamePlayer client = WorldMgr.GetPlayerById(playerID);
				if (client != null)
				{
					client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg1", new object[]
					{
						nickname
					}));
				}
			}
		}
		protected void HandleChatConsortia(GSPacketIn packet)
		{
			int area = packet.ReadInt();
			byte channel = packet.ReadByte();
			bool team = packet.ReadBoolean();
			string nick = packet.ReadString();
			string msg = packet.ReadString();
			int id = packet.ReadInt();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == id)
				{
					p.Out.SendTCP(packet);
				}
			}
		}
		protected void HandleConsortiaResponse(GSPacketIn packet)
		{
			switch (packet.ReadByte())
			{
			case 1:
				this.HandleConsortiaUserPass(packet);
				break;
			case 2:
				this.HandleConsortiaDelete(packet);
				break;
			case 3:
				this.HandleConsortiaUserDelete(packet);
				break;
			case 4:
				this.HandleConsortiaUserInvite(packet);
				break;
			case 5:
				this.HandleConsortiaBanChat(packet);
				break;
			case 6:
				this.HandleConsortiaUpGrade(packet);
				break;
			case 7:
				this.HandleConsortiaAlly(packet);
				break;
			case 8:
				this.HandleConsortiaDuty(packet);
				break;
			case 9:
				this.HandleConsortiaRichesOffer(packet);
				break;
			case 10:
				this.HandleConsortiaShopUpGrade(packet);
				break;
			case 11:
				this.HandleConsortiaSmithUpGrade(packet);
				break;
			case 12:
				this.HandleConsortiaStoreUpGrade(packet);
				break;
			}
		}
		public void HandleConsortiaFight(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			int riches = packet.ReadInt();
			string msg = packet.ReadString();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.Out.SendMessage(eMessageType.ChatNormal, msg);
				}
			}
		}
		public void HandleConsortiaCreate(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			int offer = packet.ReadInt();
			string chairmanName = packet.ReadString();
			ConsortiaMgr.AddConsortia(consortiaID, chairmanName);
		}
		public void HandleConsortiaUserPass(GSPacketIn packet)
		{
			int cid = packet.ReadInt();
			bool isInvite = packet.ReadBoolean();
			int consortiaID = packet.ReadInt();
			string consortiaName = packet.ReadString();
			int id = packet.ReadInt();
			string userName = packet.ReadString();
			int inviteUserID = packet.ReadInt();
			string inviteUserName = packet.ReadString();
			int dutyID = packet.ReadInt();
			string dutyName = packet.ReadString();
			int offer = packet.ReadInt();
			int richesOffer = packet.ReadInt();
			int richesRob = packet.ReadInt();
			DateTime lastDate = packet.ReadDateTime();
			int grade = packet.ReadInt();
			int level = packet.ReadInt();
			int state = packet.ReadInt();
			bool sex = packet.ReadBoolean();
			int right = packet.ReadInt();
			int win = packet.ReadInt();
			int total = packet.ReadInt();
			int escape = packet.ReadInt();
			int consortiaRepute = packet.ReadInt();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ID == id)
				{
					p.BeginChanges();
					p.PlayerCharacter.ConsortiaID = consortiaID;
					p.PlayerCharacter.ConsortiaName = consortiaName;
					p.PlayerCharacter.DutyName = dutyName;
					p.PlayerCharacter.DutyLevel = level;
					p.PlayerCharacter.Right = right;
					p.PlayerCharacter.ConsortiaRepute = consortiaRepute;
					ConsortiaInfo consotia = ConsortiaMgr.FindConsortiaInfo(consortiaID);
					if (consotia != null)
					{
						p.PlayerCharacter.ConsortiaLevel = consotia.Level;
					}
					p.CommitChanges();
				}
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaDelete(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.SaveIntoDatabase();
					p.ClearConsortia(true);
					p.AddRobRiches(-p.PlayerCharacter.RichesRob);
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaUserDelete(GSPacketIn packet)
		{
			int id = packet.ReadInt();
			int consortiaID = packet.ReadInt();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID || p.PlayerCharacter.ID == id)
				{
					if (p.PlayerCharacter.ID == id)
					{
						p.ClearConsortia(true);
					}
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaUserInvite(GSPacketIn packet)
		{
			int id = packet.ReadInt();
			int playerid = packet.ReadInt();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ID == playerid)
				{
					p.Out.SendTCP(packet);
					break;
				}
			}
		}
		public void HandleConsortiaBanChat(GSPacketIn packet)
		{
			bool isBan = packet.ReadBoolean();
			int playerid = packet.ReadInt();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ID == playerid)
				{
					p.PlayerCharacter.IsBanChat = isBan;
					p.Out.SendTCP(packet);
					break;
				}
			}
		}
		public void HandleConsortiaUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			string consortiaName = packet.ReadString();
			int consortiaLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaUpGrade(consortiaID, consortiaLevel);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.PlayerCharacter.ConsortiaLevel = consortiaLevel;
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaStoreUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			string consortiaName = packet.ReadString();
			int storeLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaStoreUpGrade(consortiaID, storeLevel);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.PlayerCharacter.StoreLevel = storeLevel;
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaShopUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			string consortiaName = packet.ReadString();
			int shopLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaShopUpGrade(consortiaID, shopLevel);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.PlayerCharacter.ShopLevel = shopLevel;
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaSmithUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			string consortiaName = packet.ReadString();
			int smithLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaSmithUpGrade(consortiaID, smithLevel);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.PlayerCharacter.SmithLevel = smithLevel;
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaAlly(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			int consortiaID2 = packet.ReadInt();
			int state = packet.ReadInt();
			ConsortiaMgr.UpdateConsortiaAlly(consortiaID, consortiaID2, state);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID || p.PlayerCharacter.ConsortiaID == consortiaID2)
				{
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleConsortiaDuty(GSPacketIn packet)
		{
			int updateType = (int)packet.ReadByte();
			int consortiaID = packet.ReadInt();
			int playerID = packet.ReadInt();
			string playerName = packet.ReadString();
			int dutyLevel = packet.ReadInt();
			string dutyName = packet.ReadString();
			int right = packet.ReadInt();
			if (updateType == 9)
			{
				ConsortiaMgr.ConsortiaChangChairman(consortiaID, playerName);
			}
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					if (updateType == 2 && p.PlayerCharacter.DutyLevel == dutyLevel)
					{
						p.PlayerCharacter.DutyName = dutyName;
					}
					else
					{
						if (p.PlayerCharacter.ID == playerID && (updateType == 5 || updateType == 6 || updateType == 7 || updateType == 8 || updateType == 9))
						{
							p.PlayerCharacter.DutyLevel = dutyLevel;
							p.PlayerCharacter.DutyName = dutyName;
							p.PlayerCharacter.Right = right;
						}
					}
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleRate(GSPacketIn packet)
		{
			RateMgr.ReLoad();
		}
		public void HandleConsortiaRichesOffer(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.Out.SendTCP(packet);
				}
			}
		}
		public void HandleUpdatePlayerMarriedState(GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer player = WorldMgr.GetPlayerById(playerId);
			if (player != null)
			{
				player.LoadMarryProp();
				player.LoadMarryMessage();
				player.QuestInventory.ClearMarryQuest();
			}
		}
		public void HandleMarryRoomInfoToPlayer(GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer player = WorldMgr.GetPlayerById(playerId);
			if (player != null)
			{
				packet.Code = 252;
				packet.ClientID = playerId;
				player.Out.SendTCP(packet);
			}
		}
		public void HandleMarryRoomDispose(GSPacketIn packet)
		{
			int roomId = packet.ReadInt();
			MarryRoom[] rooms = MarryRoomMgr.GetAllMarryRoom();
			MarryRoom[] array = rooms;
			for (int i = 0; i < array.Length; i++)
			{
				MarryRoom room = array[i];
				if (room.Info.ID == roomId)
				{
					room.KillAllPlayer();
					MarryRoomMgr.RemoveMarryRoom(room);
					GSPacketIn pkg = new GSPacketIn(254);
					pkg.WriteInt(roomId);
					WorldMgr.MarryScene.SendToALL(pkg);
					room.StopTimer();
					GSPacketIn pkg2 = new GSPacketIn(249);
					pkg2.WriteByte(9);
					room.SendToAll(pkg2);
					room.StopTimerForHymeneal();
					room.SendUserRemoveLate();
					room.SendMarryRoomInfoUpdateToScenePlayers(room);
				}
			}
		}
		public void HandleShutdown(GSPacketIn pkg)
		{
			GameServer.Instance.Shutdown();
		}
		public void HandleMacroDrop(GSPacketIn pkg)
		{
			Dictionary<int, MacroDropInfo> temp = new Dictionary<int, MacroDropInfo>();
			int count = pkg.ReadInt();
			for (int i = 0; i < count; i++)
			{
				int templateId = pkg.ReadInt();
				int dropcount = pkg.ReadInt();
				int maxCount = pkg.ReadInt();
				MacroDropInfo mdi = new MacroDropInfo(dropcount, maxCount);
				temp.Add(templateId, mdi);
			}
			MacroDropMgr.UpdateDropInfo(temp);
		}
		public void HandleUpdateLimitCount(GSPacketIn pkg)
		{
			Dictionary<int, int> temp = new Dictionary<int, int>();
			int len = pkg.ReadInt();
			for (int i = 0; i < len; i++)
			{
				temp.Add(pkg.ReadInt(), pkg.ReadInt());
			}
			ShopMgr.UpdateLimitCount(temp);
		}
		public void HandleUpdateShopNotice(GSPacketIn pkg)
		{
			ShopMgr.CloseNotice(pkg.ReadInt(), pkg.ReadInt());
		}
		public void HandleServerProperties(GSPacketIn pkg)
		{
			//GameProperties.Refresh();
			BattleMgr.UpdateServerProperties();
		}
		public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
		{
			GSPacketIn pkg = new GSPacketIn(1);
			pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
			this.SendTCP(pkg);
		}
		public void SendListenIPPort(IPAddress ip, int port)
		{
			GSPacketIn pkg = new GSPacketIn(240);
			pkg.Write(ip.GetAddressBytes());
			pkg.WriteInt(port);
			this.SendTCP(pkg);
		}
		public void SendPingCenter()
		{
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			int playerCount = (players == null) ? 0 : players.Length;
			GSPacketIn pkg = new GSPacketIn(12);
			pkg.WriteInt(playerCount);
			this.SendTCP(pkg);
		}
		public GSPacketIn SendUserOnline(Dictionary<int, int> users)
		{
			GSPacketIn pkg = new GSPacketIn(5);
			pkg.WriteInt(users.Count);
			foreach (KeyValuePair<int, int> i in users)
			{
				pkg.WriteInt(i.Key);
				pkg.WriteInt(i.Value);
			}
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendUserOnline(int playerid, int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(5);
			pkg.WriteInt(1);
			pkg.WriteInt(playerid);
			pkg.WriteInt(consortiaID);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendUserOffline(int playerid, int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(4);
			pkg.WriteInt(1);
			pkg.WriteInt(playerid);
			pkg.WriteInt(consortiaID);
			this.SendTCP(pkg);
			return pkg;
		}
		public void SendAllowUserLogin(int playerid)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(playerid);
			this.SendTCP(pkg);
		}
		public void SendMailResponse(int playerid)
		{
			GSPacketIn pkg = new GSPacketIn(117);
			pkg.WriteInt(playerid);
			this.SendTCP(pkg);
		}
		public void SendConsortiaUserPass(int playerid, string playerName, ConsortiaUserInfo info, bool isInvite, int consortiaRepute, string loginName, int fightpower,int achievementpoint,string honor)
		{
			GSPacketIn pkg = new GSPacketIn(128, playerid);
			pkg.WriteByte(1);
			pkg.WriteInt(info.ID);
			pkg.WriteBoolean(isInvite);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.UserID);
			pkg.WriteString(info.UserName);
			pkg.WriteInt(playerid);
			pkg.WriteString(playerName);
			pkg.WriteInt(info.DutyID);
			pkg.WriteString(info.DutyName);
			pkg.WriteInt(info.Offer);
			pkg.WriteInt(info.RichesOffer);
			pkg.WriteInt(info.RichesRob);
			pkg.WriteDateTime(info.LastDate);
			pkg.WriteInt(info.Grade);
			pkg.WriteInt(info.Level);
			pkg.WriteInt(info.State);
			pkg.WriteBoolean(info.Sex);
			pkg.WriteInt(info.Right);
			pkg.WriteInt(info.Win);
			pkg.WriteInt(info.Total);
			pkg.WriteInt(info.Escape);
			pkg.WriteInt(consortiaRepute);
			pkg.WriteString(loginName);
			pkg.WriteInt(fightpower);
            pkg.WriteInt(achievementpoint);
            pkg.WriteString(honor);
           
			this.SendTCP(pkg);
		}
		public void SendConsortiaDelete(int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(2);
			pkg.WriteInt(consortiaID);
			this.SendTCP(pkg);
		}
		public void SendConsortiaUserDelete(int playerid, int consortiaID, bool isKick, string nickName, string kickName)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(3);
			pkg.WriteInt(playerid);
			pkg.WriteInt(consortiaID);
			pkg.WriteBoolean(isKick);
			pkg.WriteString(nickName);
			pkg.WriteString(kickName);
			this.SendTCP(pkg);
		}
		public void SendConsortiaInvite(int ID, int playerid, string playerName, int inviteID, string intviteName, string consortiaName, int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(4);
			pkg.WriteInt(ID);
			pkg.WriteInt(playerid);
			pkg.WriteString(playerName);
			pkg.WriteInt(inviteID);
			pkg.WriteString(intviteName);
			pkg.WriteInt(consortiaID);
			pkg.WriteString(consortiaName);
			this.SendTCP(pkg);
		}
		public void SendConsortiaBanChat(int playerid, string playerName, int handleID, string handleName, bool isBan)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(5);
			pkg.WriteBoolean(isBan);
			pkg.WriteInt(playerid);
			pkg.WriteString(playerName);
			pkg.WriteInt(handleID);
			pkg.WriteString(handleName);
			this.SendTCP(pkg);
		}
		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(158);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(riches);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendConsortiaOffer(int consortiaID, int offer, int riches)
		{
			GSPacketIn pkg = new GSPacketIn(156);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(offer);
			pkg.WriteInt(riches);
			this.SendTCP(pkg);
		}
		public void SendConsortiaCreate(int consortiaID, int offer, string chairmanName)
		{
			GSPacketIn pkg = new GSPacketIn(130);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(offer);
			pkg.WriteString(chairmanName);
			this.SendTCP(pkg);
		}
		public void SendConsortiaUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(6);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.Level);
			this.SendTCP(pkg);
		}
		public void SendConsortiaShopUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(10);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.ShopLevel);
			this.SendTCP(pkg);
		}
		public void SendConsortiaSmithUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(11);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.SmithLevel);
			this.SendTCP(pkg);
		}
		public void SendConsortiaStoreUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(12);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.StoreLevel);
			this.SendTCP(pkg);
		}
		public void SendConsortiaAlly(int consortiaID1, int consortiaID2, int state)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(7);
			pkg.WriteInt(consortiaID1);
			pkg.WriteInt(consortiaID2);
			pkg.WriteInt(state);
			this.SendTCP(pkg);
			ConsortiaMgr.UpdateConsortiaAlly(consortiaID1, consortiaID2, state);
		}
		public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID)
		{
			this.SendConsortiaDuty(info, updateType, consortiaID, 0, "", 0, "");
		}
		public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID, int playerID, string playerName, int handleID, string handleName)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(8);
			pkg.WriteByte((byte)updateType);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(playerID);
			pkg.WriteString(playerName);
			pkg.WriteInt(info.Level);
			pkg.WriteString(info.DutyName);
			pkg.WriteInt(info.Right);
			pkg.WriteInt(handleID);
			pkg.WriteString(handleName);
			this.SendTCP(pkg);
		}
		public void SendConsortiaRichesOffer(int consortiaID, int playerID, string playerName, int riches)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(9);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(playerID);
			pkg.WriteString(playerName);
			pkg.WriteInt(riches);
			this.SendTCP(pkg);
		}
		public void SendUpdatePlayerMarriedStates(int playerId)
		{
			GSPacketIn pkg = new GSPacketIn(13);
			pkg.WriteInt(playerId);
			this.SendTCP(pkg);
		}
		public void SendMarryRoomInfoToPlayer(int playerId, bool state, MarryRoomInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(26);
			pkg.WriteInt(playerId);
			pkg.WriteBoolean(state);
			if (state)
			{
				pkg.WriteInt(info.ID);
				pkg.WriteString(info.Name);
				pkg.WriteInt(info.MapIndex);
				pkg.WriteInt(info.AvailTime);
				pkg.WriteInt(info.PlayerID);
				pkg.WriteInt(info.GroomID);
				pkg.WriteInt(info.BrideID);
				pkg.WriteDateTime(info.BeginTime);
				pkg.WriteBoolean(info.IsGunsaluteUsed);
			}
			this.SendTCP(pkg);
		}
		public void SendMarryRoomDisposeToPlayer(int roomId)
		{
			GSPacketIn pkg = new GSPacketIn(241);
			pkg.WriteInt(roomId);
			this.SendTCP(pkg);
		}
		public void SendShutdown(bool isStoping)
		{
			GSPacketIn pkg = new GSPacketIn(15);
			pkg.WriteInt(this.m_serverId);
			pkg.WriteBoolean(isStoping);
			this.SendTCP(pkg);
		}
		public void SendDispatches(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(123);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendMessag(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(118);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendCmdResult(string cmdLine, bool result)
		{
			GSPacketIn pkg = new GSPacketIn(11);
			pkg.WriteString(cmdLine);
			pkg.WriteBoolean(result);
			this.SendTCP(pkg);
		}
		public void SendPacket(GSPacketIn packet)
		{
			this.SendTCP(packet);
		}
		public override void DisplayMessage(string msg)
		{
			this.SendMessag(msg);
			CenterServerConnector.log.Info("[Center]:" + msg);
		}
		public CenterServerConnector(string ip, int port, int serverid, string name, byte[] readBuffer, byte[] sendBuffer) : base(ip, port, true, readBuffer, sendBuffer)
		{
			this.m_serverId = serverid;
			this.m_loginKey = string.Format("{0},{1}", serverid, name);
			base.Strict = false;
		}
	}
}
