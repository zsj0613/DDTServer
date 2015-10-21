using Bussiness;
using Game.Server;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using Game.Server.SpaRooms;
using Game.Base.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Base.Packets
{

	[PacketLib(1)]
	public class AbstractPacketLib : IPacketLib
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected readonly GameClient m_gameClient;
		public AbstractPacketLib(GameClient client)
		{
			this.m_gameClient = client;
		}
		public static IPacketLib CreatePacketLibForVersion(int rawVersion, GameClient client)
		{
			Type[] derivedClasses = ScriptMgr.GetDerivedClasses(typeof(IPacketLib));
			IPacketLib result;
			for (int i = 0; i < derivedClasses.Length; i++)
			{
				Type t = derivedClasses[i];
				object[] customAttributes = t.GetCustomAttributes(typeof(PacketLibAttribute), false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					PacketLibAttribute attr = (PacketLibAttribute)customAttributes[j];
					if (attr.RawVersion == rawVersion)
					{
						try
						{
							IPacketLib lib = (IPacketLib)Activator.CreateInstance(t, new object[]
							{
								client
							});
							result = lib;
							return result;
						}
						catch (Exception e)
						{
							if (AbstractPacketLib.log.IsErrorEnabled)
							{
								AbstractPacketLib.log.Error(string.Concat(new object[]
								{
									"error creating packetlib (",
									t.FullName,
									") for raw version ",
									rawVersion
								}), e);
							}
						}
					}
				}
			}
			result = null;
			return result;
		}
		public void SendTCP(GSPacketIn packet)
		{
			this.m_gameClient.SendTCP(packet);
		}
		public void SendLoginFailed(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(1);
			pkg.WriteByte(1);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendLoginSuccess()
		{
			if (this.m_gameClient.Player != null)
			{
				GSPacketIn pkg = new GSPacketIn(1, this.m_gameClient.Player.PlayerCharacter.ID);
				pkg.WriteByte(0);
				pkg.WriteInt(GameServer.Instance.Configuration.AreaID);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Attack);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Defence);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Agility);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Luck);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.GP);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Repute);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Gold);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Money);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Hide);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.FightPower);

                pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.AchievementPoint);
                pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.Honor);
                pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.VipLevel);
                pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.ChargedMoney);

                pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.AntiAddiction);
				pkg.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.Sex);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.Style + "&" + this.m_gameClient.Player.PlayerCharacter.Colors);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.Skin);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaID);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.ConsortiaName);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.DutyLevel);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.DutyName);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Right);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.ChairmanName);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaHonor);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.ConsortiaRiches);
				pkg.WriteBoolean(this.m_gameClient.Player.PlayerCharacter.HasBagPassword);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest1);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.PasswordQuest2);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.FailedPasswordAttemptCount);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.UserName);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.Nimbus);
				pkg.WriteString(Convert.ToBase64String(this.m_gameClient.Player.PlayerCharacter.QuestSite));
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.PvePermission);
				pkg.WriteString(this.m_gameClient.Player.PlayerCharacter.FightLabPermission);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.AnswerSite);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.BoxProgression);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.GetBoxLevel);
				pkg.WriteInt(this.m_gameClient.Player.PlayerCharacter.AlreadyGetBox);
				pkg.WriteDateTime(this.m_gameClient.Player.PlayerCharacter.LastSpaDate);
				this.SendTCP(pkg);
			}
		}
		public void SendRSAKey(byte[] m, byte[] e)
		{
			GSPacketIn pkg = new GSPacketIn(7);
			pkg.Write(m);
			pkg.Write(e);
			this.SendTCP(pkg);
		}
		public void SendCheckCode()
		{
			if (this.m_gameClient.Player != null && (this.m_gameClient.Player.PlayerCharacter.CheckCount >= GameProperties.CHECKCODE_PER_GAME_COUNT || this.m_gameClient.Player.PlayerCharacter.ChatCount >= 5))
			{
				if (this.m_gameClient.Player.PlayerCharacter.CheckError == 0 && this.m_gameClient.Player.PlayerCharacter.ChatCount == 0)
				{
					this.m_gameClient.Player.PlayerCharacter.CheckCount += 10000;
				}
				GSPacketIn pkg = new GSPacketIn(200, this.m_gameClient.Player.PlayerCharacter.ID, 10240);
				if (this.m_gameClient.Player.PlayerCharacter.CheckError < 1)
				{
					pkg.WriteByte(0);
				}
				else
				{
					pkg.WriteByte(2);
				}
				pkg.WriteBoolean(true);
				if (this.m_gameClient.Player.PlayerCharacter.ChatCount == 5)
				{
					pkg.WriteString(LanguageMgr.GetTranslation("CheckCode.Chat", new object[0]));
				}
				else
				{
					pkg.WriteString(LanguageMgr.GetTranslation("CheckCode.Fight", new object[0]));
				}
				this.m_gameClient.Player.PlayerCharacter.CheckCode = CheckCode.GenerateCheckCode();
				pkg.Write(CheckCode.CreateImage(this.m_gameClient.Player.PlayerCharacter.CheckCode));
				this.SendTCP(pkg);
			}
		}
		public void SendKitoff(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(4);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendEditionError(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(12);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public void SendWaitingRoom(bool result)
		{
			GSPacketIn pkg = new GSPacketIn(16);
			pkg.WriteByte(result ? (byte)1 : (byte)0);
			this.SendTCP(pkg);
		}
		public GSPacketIn SendPlayerState(int id, byte state)
		{
			GSPacketIn pkg = new GSPacketIn(32, id);
			pkg.WriteByte(state);
			this.SendTCP(pkg);
			return pkg;
		}
		public virtual GSPacketIn SendMessage(eMessageType type, string message)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt((int)type);
			pkg.WriteString(message);
			this.SendTCP(pkg);
			return pkg;
		}
		public void SendReady()
		{
			GSPacketIn pkg = new GSPacketIn(0);
			this.SendTCP(pkg);
		}
		public void SendUpdatePrivateInfo(PlayerInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(38, info.ID);
			pkg.WriteInt(info.Money);
			pkg.WriteInt(info.Gold);
			pkg.WriteInt(info.GiftToken);
			this.SendTCP(pkg);
		}
		public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(67, info.ID);
			pkg.WriteInt(info.GP);
			pkg.WriteInt(info.Offer);
			pkg.WriteInt(info.RichesOffer);
			pkg.WriteInt(info.RichesRob);
			pkg.WriteInt(info.Win);
			pkg.WriteInt(info.Total);
			pkg.WriteInt(info.Escape);
			pkg.WriteInt(info.Attack);
			pkg.WriteInt(info.Defence);
			pkg.WriteInt(info.Agility);
			pkg.WriteInt(info.Luck);
			pkg.WriteInt(info.Hide);
			pkg.WriteString(info.Style);
			pkg.WriteString(info.Colors);
			pkg.WriteString(info.Skin);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.ConsortiaLevel);
			pkg.WriteInt(info.ConsortiaRepute);
			pkg.WriteInt(info.Nimbus);
			pkg.WriteString(info.PvePermission);
			pkg.WriteString(info.FightLabPermission);
			pkg.WriteInt(info.FightPower);
            pkg.WriteInt(info.AchievementPoint);
            pkg.WriteString(info.Honor);
            pkg.WriteInt(info.VipLevel);
			pkg.WriteDateTime(info.LastSpaDate);
			this.SendTCP(pkg);
			return pkg;
		}
		public void SendPingTime(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(4);
			player.PingStart = DateTime.Now.Ticks;
			pkg.WriteInt(player.PlayerCharacter.AntiAddiction);
			this.SendTCP(pkg);
		}
		public GSPacketIn SendNetWork(int id, long delay)
		{
			GSPacketIn pkg = new GSPacketIn(6, id);
			pkg.WriteInt((int)delay / 1000 / 10);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendUserEquip(PlayerInfo player, List<ItemInfo> items)
		{
			GSPacketIn pkg = new GSPacketIn(74, player.ID, 10240);
			pkg.WriteInt(player.ID);
			pkg.WriteInt(player.Agility);
			pkg.WriteInt(player.Attack);
			pkg.WriteString(player.Colors);
			pkg.WriteString(player.Skin);
			pkg.WriteInt(player.Defence);
			pkg.WriteInt(player.GP);
			pkg.WriteInt(player.Grade);
			pkg.WriteInt(player.Luck);
			pkg.WriteInt(player.Hide);
			pkg.WriteInt(player.Repute);
			pkg.WriteBoolean(player.Sex);
			pkg.WriteString(player.Style);
			pkg.WriteInt(player.Offer);
			pkg.WriteString(player.NickName);
			pkg.WriteInt(player.Win);
			pkg.WriteInt(player.Total);
			pkg.WriteInt(player.Escape);
			pkg.WriteInt(player.ConsortiaID);
			pkg.WriteString(player.ConsortiaName);
			pkg.WriteInt(player.RichesOffer);
			pkg.WriteInt(player.RichesRob);
			pkg.WriteBoolean(player.IsMarried);
			pkg.WriteInt(player.SpouseID);
			pkg.WriteString(player.SpouseName);
			pkg.WriteString(player.DutyName);
			pkg.WriteInt(player.Nimbus);
			pkg.WriteInt(player.FightPower);
            pkg.WriteInt(player.AchievementPoint);
            pkg.WriteString(player.Honor);
            pkg.WriteInt(player.VipLevel);


			pkg.WriteInt(items.Count);
			foreach (ItemInfo info in items)
			{
				pkg.WriteByte((byte)info.BagType);
				pkg.WriteInt(info.UserID);
				pkg.WriteInt(info.ItemID);
				pkg.WriteInt(info.Count);
				pkg.WriteInt(info.Place);
				pkg.WriteInt(info.TemplateID);
				pkg.WriteInt(info.AttackCompose);
				pkg.WriteInt(info.DefendCompose);
				pkg.WriteInt(info.AgilityCompose);
				pkg.WriteInt(info.LuckCompose);
				pkg.WriteInt(info.StrengthenLevel);
				pkg.WriteBoolean(info.IsBinds);
				pkg.WriteBoolean(info.IsJudge);
				pkg.WriteDateTime(info.BeginDate);
				pkg.WriteInt(info.ValidDate);
				pkg.WriteString(info.Color);
				pkg.WriteString(info.Skin);
				pkg.WriteBoolean(info.IsUsed);
				pkg.WriteInt(info.Hole1);
				pkg.WriteInt(info.Hole2);
				pkg.WriteInt(info.Hole3);
				pkg.WriteInt(info.Hole4);
				pkg.WriteInt(info.Hole5);
				pkg.WriteInt(info.Hole6);
			}
			pkg.Compress();
			this.SendTCP(pkg);
			return pkg;
		}
		public void SendDateTime()
		{
			GSPacketIn pkg = new GSPacketIn(5);
			pkg.WriteDateTime(DateTime.Now);
			this.SendTCP(pkg);
		}
		public GSPacketIn SendDailyAward(GamePlayer player, int getWay)
		{
			bool result = false;
			if (getWay == 1)
			{
				if (DateTime.Now.Date != player.PlayerCharacter.LastAuncherAward.Date)
				{
					result = true;
				}
			}
			else
			{
				if (DateTime.Now.Date != player.PlayerCharacter.LastAward.Date)
				{
					result = true;
				}
			}
			GSPacketIn pkg = new GSPacketIn(13);
			pkg.WriteBoolean(result);
			pkg.WriteInt(getWay);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendUpdateNickname(string name)
		{
			GSPacketIn result;
			if (this.m_gameClient.Player == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(33, this.m_gameClient.Player.PlayerCharacter.ID);
				pkg.WriteString(name);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendUpdateRoomList(List<BaseRoom> rooms)
		{
			GSPacketIn pkg = new GSPacketIn(95);
			pkg.WriteInt(RoomMgr.UsingRoomCount);
			pkg.WriteInt(rooms.Count);
			foreach (BaseRoom room in rooms)
			{
				pkg.WriteInt(room.RoomId);
				pkg.WriteByte((byte)room.RoomType);
				pkg.WriteByte(room.TimeMode);
				pkg.WriteByte((byte)room.PlayerCount);
				pkg.WriteByte((byte)room.PlacesCount);
				pkg.WriteBoolean(!string.IsNullOrEmpty(room.Password));
				pkg.WriteInt(room.MapId);
				pkg.WriteBoolean(room.IsPlaying);
				pkg.WriteString(room.Name);
				pkg.WriteByte((byte)room.GameType);
				pkg.WriteByte((byte)room.HardLevel);
				pkg.WriteInt(room.LevelLimits);
			}
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendSceneAddPlayer(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(18, player.PlayerCharacter.ID);
			pkg.WriteInt(player.PlayerCharacter.Grade);
			pkg.WriteBoolean(player.PlayerCharacter.Sex);
			pkg.WriteString(player.PlayerCharacter.NickName);
			pkg.WriteString(player.PlayerCharacter.ConsortiaName);
			pkg.WriteInt(player.PlayerCharacter.Offer);
			pkg.WriteInt(player.PlayerCharacter.Win);
			pkg.WriteInt(player.PlayerCharacter.Total);
			pkg.WriteInt(player.PlayerCharacter.Escape);
			pkg.WriteInt(player.PlayerCharacter.ConsortiaID);
			pkg.WriteInt(player.PlayerCharacter.Repute);
			pkg.WriteBoolean(player.PlayerCharacter.IsMarried);
			if (player.PlayerCharacter.IsMarried)
			{
				pkg.WriteInt(player.PlayerCharacter.SpouseID);
				pkg.WriteString(player.PlayerCharacter.SpouseName);
			}
			pkg.WriteString(player.PlayerCharacter.UserName);
			pkg.WriteInt(player.PlayerCharacter.FightPower);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(21, player.PlayerCharacter.ID);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(82, player.PlayerId);
			bool isInGame = false;
			if (player.CurrentRoom.Game != null)
			{
				isInGame = true;
			}
			pkg.WriteBoolean(isInGame);
			pkg.WriteByte((byte)player.CurrentRoomIndex);
			pkg.WriteByte((byte)player.CurrentRoomTeam);
			pkg.WriteInt(player.PlayerCharacter.Grade);
			pkg.WriteInt(player.PlayerCharacter.Offer);
			pkg.WriteInt(player.PlayerCharacter.Hide);
			pkg.WriteInt(player.PlayerCharacter.Repute);
			pkg.WriteInt((int)player.PingTime / 1000 / 10);
			pkg.WriteInt(player.AreaID);

			pkg.WriteInt(player.PlayerCharacter.ID);
			pkg.WriteString(player.PlayerCharacter.NickName);
			pkg.WriteBoolean(player.PlayerCharacter.Sex);
			pkg.WriteString(player.PlayerCharacter.Style);
			pkg.WriteString(player.PlayerCharacter.Colors);
			pkg.WriteString(player.PlayerCharacter.Skin);
			ItemInfo item = player.MainBag.GetItemAt(6);
			pkg.WriteInt((item == null) ? -1 : item.TemplateID);
			if (player.SecondWeapon == null)
			{
				pkg.WriteInt(0);
			}
			else
			{
				pkg.WriteInt(player.SecondWeapon.TemplateID);
			}
			pkg.WriteInt(player.PlayerCharacter.ConsortiaID);
			pkg.WriteString(player.PlayerCharacter.ConsortiaName);
			pkg.WriteInt(player.PlayerCharacter.Win);
			pkg.WriteInt(player.PlayerCharacter.Total);
			pkg.WriteInt(player.PlayerCharacter.Escape);
			pkg.WriteInt(player.PlayerCharacter.ConsortiaLevel);
			pkg.WriteInt(player.PlayerCharacter.ConsortiaRepute);
			pkg.WriteBoolean(player.PlayerCharacter.IsMarried);
			if (player.PlayerCharacter.IsMarried)
			{
				pkg.WriteInt(player.PlayerCharacter.SpouseID);
				pkg.WriteString(player.PlayerCharacter.SpouseName);
			}
			pkg.WriteString(player.PlayerCharacter.UserName);
			pkg.WriteInt(player.PlayerCharacter.Nimbus);
			pkg.WriteInt(player.PlayerCharacter.FightPower);
            pkg.WriteInt(player.PlayerCharacter.VipLevel);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(83, player.PlayerId);
			pkg.WriteInt(player.AreaID);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
		{
			GSPacketIn pkg = new GSPacketIn(87);
			for (int i = 0; i < states.Length; i++)
			{
				pkg.WriteByte(states[i]);
			}
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
		{
			GSPacketIn pkg = new GSPacketIn(100);
			for (int i = 0; i < states.Length; i++)
			{
				pkg.WriteInt(states[i]);
			}
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(102, player.PlayerId);
			pkg.WriteByte((byte)player.CurrentRoomTeam);
			pkg.WriteByte((byte)player.CurrentRoomIndex);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomCreate(BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(94);
			pkg.WriteInt(room.RoomId);
			pkg.WriteByte((byte)room.RoomType);
			pkg.WriteByte((byte)room.HardLevel);
			pkg.WriteByte(room.TimeMode);
			pkg.WriteByte((byte)room.PlayerCount);
			pkg.WriteByte((byte)room.PlacesCount);
			pkg.WriteBoolean(!string.IsNullOrEmpty(room.Password));
			pkg.WriteInt(room.MapId);
			pkg.WriteBoolean(room.IsPlaying);
			pkg.WriteString(room.Name);
			pkg.WriteByte((byte)room.GameType);
			pkg.WriteInt(room.LevelLimits);
			pkg.WriteBoolean(room.IsArea);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomLoginResult(bool result)
		{
			GSPacketIn pkg = new GSPacketIn(81);
			pkg.WriteBoolean(result);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomPairUpStart(BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(208);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendGameRoomInfo(GamePlayer player, BaseRoom game)
		{
			return new GSPacketIn(94, player.PlayerCharacter.ID);
		}
		public GSPacketIn SendRoomType(GamePlayer player, BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(211);
			pkg.WriteByte((byte)room.GameStyle);
			pkg.WriteInt((int)room.GameType);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(210);
			pkg.WriteBoolean(false);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomClear(GamePlayer player, BaseRoom game)
		{
			GSPacketIn pkg = new GSPacketIn(96, player.PlayerCharacter.ID);
			pkg.WriteInt(game.RoomId);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
		{
			GSPacketIn pkg = new GSPacketIn(66, player.PlayerCharacter.ID);
			pkg.WriteByte((byte)place);
			pkg.WriteInt(goodsID);
			pkg.WriteString(style);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRoomChange(BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(107);
			pkg.WriteInt(room.MapId);
			pkg.WriteByte((byte)room.RoomType);
			pkg.WriteByte(room.TimeMode);
			pkg.WriteByte((byte)room.HardLevel);
			pkg.WriteInt(room.LevelLimits);
			pkg.WriteBoolean(room.IsArea);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isbind, int MinValid)
		{
			GSPacketIn pkg = new GSPacketIn(76, player.PlayerCharacter.ID);
            AbstractPacketLib.log.Warn("SendFusionPreview"+ previewItemList.Count.ToString());
			pkg.WriteInt(previewItemList.Count);
            foreach (KeyValuePair<int, double> p in previewItemList)
            {
                pkg.WriteInt(p.Key);
                pkg.WriteInt(MinValid);
                int value = Convert.ToInt32(p.Value);
                if (p.Key>= 2000005&&p.Key<=2000008)
                {
                    pkg.WriteInt(90);
                }
                else if (p.Key>=16&&p.Key<=19)
                {
                    pkg.WriteInt(5);
                }
                else
                {
                    pkg.WriteInt((value > 100) ? 100 : ((value < 0) ? 0 : value));
                }
			}
			pkg.WriteBoolean(isbind);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendFusionResult(GamePlayer player, bool result)
		{
			GSPacketIn pkg = new GSPacketIn(78, player.PlayerCharacter.ID);
			pkg.WriteBoolean(result);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item)
		{
			GSPacketIn pkg = new GSPacketIn(111, player.PlayerCharacter.ID);
			pkg.WriteInt(templateid);
			pkg.WriteInt(item.ValidDate);
			pkg.WriteBoolean(isbind);
			pkg.WriteInt(item.AgilityCompose);
			pkg.WriteInt(item.AttackCompose);
			pkg.WriteInt(item.DefendCompose);
			pkg.WriteInt(item.LuckCompose);
			this.SendTCP(pkg);
			return pkg;
		}
		public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
		{
			if (this.m_gameClient.Player != null)
			{
				GSPacketIn pkg = new GSPacketIn(64, this.m_gameClient.Player.PlayerCharacter.ID, 10240);
				pkg.WriteInt(bag.BagType);
				pkg.WriteInt(updatedSlots.Length);
				for (int j = 0; j < updatedSlots.Length; j++)
				{
					int i = updatedSlots[j];
					pkg.WriteInt(i);
					ItemInfo item = bag.GetItemAt(i);
					if (item == null)
					{
						pkg.WriteBoolean(false);
					}
					else
					{
						pkg.WriteBoolean(true);
						pkg.WriteInt(item.UserID);
						pkg.WriteInt(item.ItemID);
						pkg.WriteInt(item.Count);
						pkg.WriteInt(item.Place);
						pkg.WriteInt(item.TemplateID);
						pkg.WriteInt(item.AttackCompose);
						pkg.WriteInt(item.DefendCompose);
						pkg.WriteInt(item.AgilityCompose);
						pkg.WriteInt(item.LuckCompose);
						pkg.WriteInt(item.StrengthenLevel);
						pkg.WriteBoolean(item.IsBinds);
						pkg.WriteBoolean(item.IsJudge);
						pkg.WriteDateTime(item.BeginDate);
						pkg.WriteInt(item.ValidDate);
						pkg.WriteString((item.Color == null) ? "" : item.Color);
						pkg.WriteString((item.Skin == null) ? "" : item.Skin);
						pkg.WriteBoolean(item.IsUsed);
						pkg.WriteInt(item.Hole1);
						pkg.WriteInt(item.Hole2);
						pkg.WriteInt(item.Hole3);
						pkg.WriteInt(item.Hole4);
						pkg.WriteInt(item.Hole5);
						pkg.WriteInt(item.Hole6);
					}
				}
				this.SendTCP(pkg);
			}
		}
		public GSPacketIn SendFriendRemove(int FriendID)
		{
			GSPacketIn pkg = new GSPacketIn(161, FriendID);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendFriendState(int playerID, bool state)
		{
			GSPacketIn pkg = new GSPacketIn(165, playerID);
			pkg.WriteBoolean(state);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] infos)
		{
			GSPacketIn result;
			if (player == null || states == null || infos == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(178, player.PlayerCharacter.ID);
				pkg.WriteInt(infos.Length);
				for (int i = 0; i < infos.Length; i++)
				{
					BaseQuest info = infos[i];
					pkg.WriteInt(info.Data.QuestID);
					pkg.WriteBoolean(info.Data.IsComplete);
					pkg.WriteInt(info.Data.Condition1);
					pkg.WriteInt(info.Data.Condition2);
					pkg.WriteInt(info.Data.Condition3);
					pkg.WriteInt(info.Data.Condition4);
					pkg.WriteDateTime(info.Data.CompletedDate);
					pkg.WriteInt(info.Data.RepeatFinish);
					pkg.WriteInt(info.Data.RandDobule);
					pkg.WriteBoolean(info.Data.ExistInCurrent);
				}
				pkg.Write(states);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendInitAchievements(List<UsersRecordInfo> infos)
		{
			bool arg_2E_0;
			if (infos != null && this.m_gameClient.Player != null)
			{
				int arg_26_0 = this.m_gameClient.Player.PlayerCharacter.ID;
				arg_2E_0 = (0 == 0);
			}
			else
			{
				arg_2E_0 = false;
			}
			GSPacketIn result;
			if (!arg_2E_0)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(228, this.m_gameClient.Player.PlayerCharacter.ID);
				pkg.WriteInt(infos.Count);
				foreach (UsersRecordInfo info in infos)
				{
					pkg.WriteInt(info.RecordID);
					pkg.WriteInt(info.Total);
				}
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendUpdateAchievements(UsersRecordInfo info)
		{
			bool arg_36_0;
			if (info != null && this.m_gameClient != null && this.m_gameClient.Player != null)
			{
				int arg_2E_0 = this.m_gameClient.Player.PlayerCharacter.ID;
				arg_36_0 = (0 == 0);
			}
			else
			{
				arg_36_0 = false;
			}
			GSPacketIn result;
			if (!arg_36_0)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(229, this.m_gameClient.Player.PlayerCharacter.ID);
				pkg.WriteInt(info.RecordID);
				pkg.WriteInt(info.Total);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendUpdateAchievementData(List<AchievementDataInfo> infos)
		{
			bool arg_21_0;
			if (infos != null)
			{
				int arg_19_0 = this.m_gameClient.Player.PlayerCharacter.ID;
				arg_21_0 = (0 == 0);
			}
			else
			{
				arg_21_0 = false;
			}
			GSPacketIn result;
			if (!arg_21_0)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(231, this.m_gameClient.Player.PlayerCharacter.ID);
				pkg.WriteInt(infos.Count);
				foreach (AchievementDataInfo info in infos)
				{
					pkg.WriteInt(info.AchievementID);
					pkg.WriteDateTime(info.CompletedDate);
				}
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)
		{
			GSPacketIn pkg = new GSPacketIn(185, player.PlayerId);
			pkg.WriteInt(infos.Length);
			for (int i = 0; i < infos.Length; i++)
			{
				BufferInfo info = infos[i];
				pkg.WriteInt(info.Type);
				pkg.WriteBoolean(info.IsExist);
				pkg.WriteDateTime(info.BeginDate);
				pkg.WriteInt(info.ValidDate);
				pkg.WriteInt(info.Value);
			}
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
		{
			GSPacketIn pkg = new GSPacketIn(186, player.PlayerId);
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
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
		{
			GSPacketIn pkg = new GSPacketIn(117);
			pkg.WriteInt(playerID);
			pkg.WriteInt((int)type);
			GameServer.Instance.LoginServer.SendPacket(pkg);
			return pkg;
		}
		public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item)
		{
			GSPacketIn pkg = new GSPacketIn(195);
			pkg.WriteInt(auctionID);
			pkg.WriteBoolean(isExist);
			if (isExist)
			{
				pkg.WriteInt(info.AuctioneerID);
				pkg.WriteString(info.AuctioneerName);
				pkg.WriteDateTime(info.BeginDate);
				pkg.WriteInt(info.BuyerID);
				pkg.WriteString(info.BuyerName);
				pkg.WriteInt(info.ItemID);
				pkg.WriteInt(info.Mouthful);
				pkg.WriteInt(info.PayType);
				pkg.WriteInt(info.Price);
				pkg.WriteInt(info.Rise);
				pkg.WriteInt(info.ValidDate);
				pkg.WriteBoolean(item != null);
				if (item != null)
				{
					pkg.WriteInt(item.Count);
					pkg.WriteInt(item.TemplateID);
					pkg.WriteInt(item.AttackCompose);
					pkg.WriteInt(item.DefendCompose);
					pkg.WriteInt(item.AgilityCompose);
					pkg.WriteInt(item.LuckCompose);
					pkg.WriteInt(item.StrengthenLevel);
					pkg.WriteBoolean(item.IsBinds);
					pkg.WriteBoolean(item.IsJudge);
					pkg.WriteDateTime(item.BeginDate);
					pkg.WriteInt(item.ValidDate);
					pkg.WriteString(item.Color);
					pkg.WriteString(item.Skin);
					pkg.WriteBoolean(item.IsUsed);
				}
			}
			pkg.Compress();
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendAASState(bool result)
		{
			GSPacketIn pkg = new GSPacketIn(224);
			pkg.WriteBoolean(result);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendIDNumberCheck(bool result)
		{
			GSPacketIn pkg = new GSPacketIn(226);
			pkg.WriteBoolean(result);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendAASInfoSet(bool result)
		{
			GSPacketIn pkg = new GSPacketIn(224);
			pkg.WriteBoolean(result);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
		{
			GSPacketIn pkg = new GSPacketIn(227);
			pkg.WriteBoolean(result);
			pkg.WriteBoolean(IsAASInfo);
			pkg.WriteBoolean(IsMinor);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(241, player.PlayerCharacter.ID);
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
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result)
		{
			GSPacketIn pkg = new GSPacketIn(242, player.PlayerCharacter.ID);
			pkg.WriteBoolean(result);
			if (result)
			{
				pkg.WriteInt(player.CurrentMarryRoom.Info.ID);
				pkg.WriteString(player.CurrentMarryRoom.Info.Name);
				pkg.WriteInt(player.CurrentMarryRoom.Info.MapIndex);
				pkg.WriteInt(player.CurrentMarryRoom.Info.AvailTime);
				pkg.WriteInt(player.CurrentMarryRoom.Count);
				pkg.WriteInt(player.CurrentMarryRoom.Info.PlayerID);
				pkg.WriteString(player.CurrentMarryRoom.Info.PlayerName);
				pkg.WriteInt(player.CurrentMarryRoom.Info.GroomID);
				pkg.WriteString(player.CurrentMarryRoom.Info.GroomName);
				pkg.WriteInt(player.CurrentMarryRoom.Info.BrideID);
				pkg.WriteString(player.CurrentMarryRoom.Info.BrideName);
				pkg.WriteDateTime(player.CurrentMarryRoom.Info.BeginTime);
				pkg.WriteBoolean(player.CurrentMarryRoom.Info.IsHymeneal);
				pkg.WriteByte((byte)player.CurrentMarryRoom.RoomState);
				pkg.WriteString(player.CurrentMarryRoom.Info.RoomIntroduction);
				pkg.WriteBoolean(player.CurrentMarryRoom.Info.GuestInvite);
				pkg.WriteInt(player.MarryMap);
				pkg.WriteBoolean(player.CurrentMarryRoom.Info.IsGunsaluteUsed);
			}
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(243, player.PlayerCharacter.ID);
			pkg.WriteInt(player.PlayerCharacter.Grade);
			pkg.WriteInt(player.PlayerCharacter.Hide);
			pkg.WriteInt(player.PlayerCharacter.Repute);
			pkg.WriteInt(player.PlayerCharacter.ID);
			pkg.WriteString(player.PlayerCharacter.NickName);
			pkg.WriteBoolean(player.PlayerCharacter.Sex);
			pkg.WriteString(player.PlayerCharacter.Style);
			pkg.WriteString(player.PlayerCharacter.Colors);
			pkg.WriteString(player.PlayerCharacter.Skin);
			pkg.WriteInt(player.X);
			pkg.WriteInt(player.Y);
			pkg.WriteInt(player.FightPower);
			pkg.WriteInt(player.PlayerCharacter.Win);
			pkg.WriteInt(player.PlayerCharacter.Total);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
		{
			GSPacketIn pkg = new GSPacketIn(239);
			pkg.WriteInt(ID);
			pkg.WriteBoolean(isExist);
			if (isExist)
			{
				pkg.WriteInt(info.UserID);
				pkg.WriteBoolean(info.IsPublishEquip);
				pkg.WriteString(info.Introduction);
			}
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
		{
			GSPacketIn pkg = new GSPacketIn(246, player.PlayerCharacter.ID);
			pkg.WriteInt(userID);
			pkg.WriteBoolean(isMarried);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int id)
		{
			GSPacketIn pkg = new GSPacketIn(247, player.PlayerCharacter.ID);
			pkg.WriteInt(userID);
			pkg.WriteString(userName);
			pkg.WriteString(loveProclamation);
			pkg.WriteInt(id);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
		{
			GSPacketIn pkg = new GSPacketIn(248, player.PlayerCharacter.ID);
			pkg.WriteBoolean(result);
			pkg.WriteBoolean(isProposer);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int id)
		{
			GSPacketIn pkg = new GSPacketIn(250, player.PlayerCharacter.ID);
			pkg.WriteInt(UserID);
			pkg.WriteBoolean(result);
			pkg.WriteString(UserName);
			pkg.WriteBoolean(isApplicant);
			pkg.WriteInt(id);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendBigSpeakerMsg(GamePlayer player, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(72, player.PlayerCharacter.ID);
			pkg.WriteInt(player.PlayerCharacter.ID);
			pkg.WriteString(player.PlayerCharacter.NickName);
			pkg.WriteString(msg);
			GameServer.Instance.LoginServer.SendPacket(pkg);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(pkg);
			}
			return pkg;
		}
		public GSPacketIn SendAreaBigSpeakerMsg(GamePlayer player, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(73, player.PlayerCharacter.ID);
            pkg.WriteInt(player.AreaID);
			pkg.WriteInt(player.PlayerCharacter.ID);
			pkg.WriteString(player.PlayerCharacter.NickName);
			pkg.WriteString(msg);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(pkg);
			}
			return pkg;
		}
		public GSPacketIn SendDispatchesMsg(GSPacketIn pkg)
		{
			string msg = pkg.ReadString();
			GameServer.Instance.LoginServer.SendDispatches(msg);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(pkg);
			}
			return pkg;
		}
		public GSPacketIn SendPickUpNPC()
		{
			string msg = LanguageMgr.GetTranslation("PickUpWithNpc.success", new object[]
			{
				this.m_gameClient.Player.PlayerCharacter.NickName
			});
			GameServer.Instance.LoginServer.SendDispatches(msg);
			GSPacketIn pkg = new GSPacketIn(123);
			pkg.WriteString(msg);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(pkg);
			}
			return pkg;
		}
		public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(244, player.PlayerCharacter.ID);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(252, player.PlayerCharacter.ID);
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
			return pkg;
		}
		public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(235, player.PlayerCharacter.ID);
			pkg.WriteString(info.Introduction);
			pkg.WriteBoolean(info.IsPublishEquip);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(249, player.PlayerCharacter.ID);
			pkg.WriteByte(3);
			pkg.WriteInt(info.AvailTime);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
		{
			GSPacketIn pkg = new GSPacketIn(234, player.PlayerCharacter.ID);
			pkg.WriteBoolean(info.IsMarried);
			pkg.WriteInt(info.SpouseID);
			pkg.WriteString(info.SpouseName);
			pkg.WriteBoolean(info.IsCreatedMarryRoom);
			pkg.WriteInt(info.SelfMarryRoomID);
			pkg.WriteBoolean(info.IsGotRing);
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendConsortiaMailMessage(GamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(43, player.PlayerCharacter.ID);
			pkg.WriteString(LanguageMgr.GetTranslation("ClearConsortiaItemToMail", new object[]
			{
				DateTime.Now.Month,
				DateTime.Now.Day,
				DateTime.Now.AddDays(10.0).Month,
				DateTime.Now.AddDays(10.0).Day
			}));
			this.SendTCP(pkg);
			return pkg;
		}
		public GSPacketIn SendPlayerLeaveSpaRoom(GamePlayer player, string msg)
		{
			GSPacketIn result;
			if (player == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(169, player.PlayerCharacter.ID);
				pkg.WriteString(msg);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendPlayerLeaveSpaRoomForTimeOut(GamePlayer player)
		{
			GSPacketIn result;
			if (player == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(196, player.PlayerCharacter.ID);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendSpaRoomInfo(GamePlayer player, SpaRoom room)
		{
			GSPacketIn result2;
			if (player == null || room == null)
			{
				result2 = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(175, player.PlayerCharacter.ID);
				bool result = room != null;
				pkg.WriteBoolean(result);
				if (result)
				{
					pkg.WriteInt(room.Spa_Room_Info.RoomID);
					pkg.WriteString(room.Spa_Room_Info.RoomName);
					pkg.WriteString(room.Spa_Room_Info.Pwd);
					pkg.WriteInt(room.Spa_Room_Info.AvailTime);
					pkg.WriteInt(room.Count);
					pkg.WriteInt(room.Spa_Room_Info.PlayerID);
					pkg.WriteString(room.Spa_Room_Info.PlayerName);
					pkg.WriteDateTime(room.Spa_Room_Info.BeginTime);
					pkg.WriteString(room.Spa_Room_Info.RoomIntroduction);
					pkg.WriteInt(room.Spa_Room_Info.RoomType);
					pkg.WriteInt(room.Spa_Room_Info.MaxCount);
				}
				this.SendTCP(pkg);
				result2 = pkg;
			}
			return result2;
		}
		public GSPacketIn SendSpaRoomLogin(GamePlayer player)
		{
			GSPacketIn result;
			if (player == null || player.CurrentSpaRoom == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(202, player.PlayerCharacter.ID);
				pkg.WriteInt(player.CurrentSpaRoom.Spa_Room_Info.RoomID);
				pkg.WriteInt(player.CurrentSpaRoom.Spa_Room_Info.RoomNumber);
				pkg.WriteString(player.CurrentSpaRoom.Spa_Room_Info.RoomName);
				pkg.WriteString(player.CurrentSpaRoom.Spa_Room_Info.Pwd);
				pkg.WriteInt(player.CurrentSpaRoom.RoomLeftMin);
				pkg.WriteInt(player.CurrentSpaRoom.Count);
				pkg.WriteInt(player.CurrentSpaRoom.Spa_Room_Info.PlayerID);
				pkg.WriteString(player.CurrentSpaRoom.Spa_Room_Info.PlayerName);
				pkg.WriteDateTime(player.CurrentSpaRoom.Spa_Room_Info.BeginTime);
				pkg.WriteString(player.CurrentSpaRoom.Spa_Room_Info.RoomIntroduction);
				pkg.WriteInt(player.CurrentSpaRoom.Spa_Room_Info.RoomType);
				pkg.WriteInt(player.CurrentSpaRoom.Spa_Room_Info.MaxCount);
				pkg.WriteDateTime(player.DayOrNightInSpa);
				if (player.CurrentSpaRoom.Spa_Room_Info.RoomType == 1)
				{
					pkg.WriteInt(player.PlayerCharacter.SpaPubGoldRoomLimit);
				}
				else
				{
					if (player.CurrentSpaRoom.Spa_Room_Info.RoomType == 2)
					{
						pkg.WriteInt(player.PlayerCharacter.SpaPubMoneyRoomLimit);
					}
					else
					{
						pkg.WriteInt(0);
					}
				}
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendSpaRoomList(GamePlayer player, SpaRoom[] rooms)
		{
			GSPacketIn result;
			if (player == null || rooms == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(197, player.PlayerCharacter.ID);
				pkg.WriteInt(rooms.Length);
				for (int i = 0; i < rooms.Length; i++)
				{
					SpaRoom room = rooms[i];
					pkg.WriteInt(room.Spa_Room_Info.RoomNumber);
					pkg.WriteInt(room.Spa_Room_Info.RoomID);
					pkg.WriteString(room.Spa_Room_Info.RoomName);
					pkg.WriteString(room.Spa_Room_Info.Pwd);
					pkg.WriteInt(room.Spa_Room_Info.AvailTime);
					pkg.WriteInt(room.Count);
					pkg.WriteInt(room.Spa_Room_Info.PlayerID);
					pkg.WriteString(room.Spa_Room_Info.PlayerName);
					pkg.WriteDateTime(room.Spa_Room_Info.BeginTime);
					pkg.WriteString(room.Spa_Room_Info.RoomIntroduction);
					pkg.WriteInt(room.Spa_Room_Info.RoomType);
					pkg.WriteInt(room.Spa_Room_Info.MaxCount);
				}
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendSpaRoomAddGuest(GamePlayer player)
		{
			GSPacketIn result;
			if (this.m_gameClient == null || this.m_gameClient.Player == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(198, this.m_gameClient.Player.PlayerCharacter.ID);
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
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendSpaRoomRemoveGuest(GamePlayer removePlayer)
		{
			GSPacketIn result;
			if (this.m_gameClient == null || this.m_gameClient.Player == null || removePlayer == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(199, this.m_gameClient.Player.PlayerCharacter.ID);
				pkg.WriteInt(removePlayer.PlayerCharacter.ID);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendSpaRoomInfoPerMin(GamePlayer player, int leftTime)
		{
			GSPacketIn result;
			if (player == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(191, player.PlayerCharacter.ID);
				pkg.WriteByte(7);
				pkg.WriteInt(leftTime);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendSpaRoomLoginRemind(SpaRoom room)
		{
			GSPacketIn result;
			if (this.m_gameClient == null || this.m_gameClient.Player == null || room == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(212, this.m_gameClient.Player.PlayerCharacter.ID);
				pkg.WriteInt(room.Spa_Room_Info.RoomID);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
		public GSPacketIn SendIsContinueNextDay(GamePlayer player)
		{
			GSPacketIn result;
			if (player == null)
			{
				result = null;
			}
			else
			{
				GSPacketIn pkg = new GSPacketIn(191, player.PlayerCharacter.ID);
				pkg.WriteByte(10);
				this.SendTCP(pkg);
				result = pkg;
			}
			return result;
		}
        public GSPacketIn SendUpdateVIP(GamePlayer player)
        {
            GSPacketIn result;
            if (player == null)
            {
                result = null;
            }
            else
            {
                GSPacketIn pkg = new GSPacketIn(26,player.PlayerCharacter.ID);
                pkg.WriteInt(player.ChargedMoney);
                pkg.WriteInt(player.VIPLevel);
                result = pkg;
                this.SendTCP(pkg);
                
            }
            return result;
        }
    }
}
