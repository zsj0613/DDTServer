using Game.Base.Packets;
using Game.Logic;
using Game.Logic.LogEnum;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Fighting.Server.GameObjects
{
	public class ProxyPlayer : IGamePlayer
	{
		public const int MaxGPLevel40 = 14324419;
		private static LogProvider log => LogProvider.Default;
		private ServerClient m_client;
		private PlayerInfo m_character;
		private ItemTemplateInfo m_currentWeapon;
		private ItemInfo m_secondWeapon;
		private bool m_canUseProp;
		private int m_gamePlayerId;
		private double GPRate;
		private double OfferRate;
		public List<BufferInfo> Buffers;
		public int m_AreaID;
		public string m_AreaName;
		public int GameId;
		private double m_baseAglilty;
		private double m_baseAttack;
		private double m_baseDefence;
		private double m_baseBlood;
		private List<int> m_equipEffect;
		private float m_gmExperienceRate = 1f;
		private float m_gmOfferRate = 1f;
		private float m_gmRichesRate = 1f;
		private float m_auncherExperienceRate = 1f;
		private float m_auncherOfferRate = 1f;
		private float m_auncherRichesRate = 1f;
		private double m_antiAddictionRate = 1.0;
        private string m_Honer;

        public int AreaID
		{
			get
			{
				return this.m_AreaID;
			}
		}
		public string AreaName
		{
			get
			{
				return this.m_AreaName;
			}
		}
		public bool IsArea
		{
			get
			{
				return FightServer.Instance.Configuration.ServerType != 0;
			}
		}
		public int GamePlayerId
		{
			get
			{
				return this.m_gamePlayerId;
			}
			set
			{
				this.m_gamePlayerId = value;
				this.m_client.SendGamePlayerId(this);
			}
		}
		public PlayerInfo PlayerCharacter
		{
			get
			{
				return this.m_character;
			}
		}
		public ItemTemplateInfo MainWeapon
		{
			get
			{
				return this.m_currentWeapon;
			}
		}
		public ItemInfo SecondWeapon
		{
			get
			{
				return this.m_secondWeapon;
			}
		}
		public bool CanUseProp
		{
			get
			{
				return this.m_canUseProp;
			}
			set
			{
				this.m_canUseProp = value;
			}
		}
		public List<int> EquipEffect
		{
			get
			{
				return this.m_equipEffect;
			}
			set
			{
				this.m_equipEffect = value;
			}
		}
		public double OfferPlusRate
		{
			get
			{
				return this.OfferRate;
			}
		}
		public double GPPlusRate
		{
			get
			{
				return this.GPRate;
			}
		}
		public float GMExperienceRate
		{
			get
			{
				return this.m_gmExperienceRate;
			}
		}
		public float GMOfferRate
		{
			get
			{
				return this.m_gmOfferRate;
			}
		}
		public float GMRichesRate
		{
			get
			{
				return this.m_gmRichesRate;
			}
		}
		public float AuncherExperienceRate
		{
			get
			{
				return this.m_auncherExperienceRate;
			}
		}
		public float AuncherOfferRate
		{
			get
			{
				return this.m_auncherOfferRate;
			}
		}
		public float AuncherRichesRate
		{
			get
			{
				return this.m_auncherRichesRate;
			}
		}
		public double AntiAddictionRate
		{
			get
			{
				return this.m_antiAddictionRate;
			}
		}
        public string Honor
        {
            get
            {
                return this.m_Honer;
            }
        }
        public ProxyPlayer(ServerClient client, PlayerInfo character, ProxyPlayerInfo proxyPlayer, List<BufferInfo> infos)
		{
			this.m_client = client;
			this.m_character = character;
			this.m_AreaID = proxyPlayer.m_AreaID;
			this.m_AreaName = proxyPlayer.m_AreaName;
			this.m_baseAttack = proxyPlayer.BaseAttack;
			this.m_baseDefence = proxyPlayer.BaseDefence;
			this.m_baseAglilty = proxyPlayer.BaseAgility;
			this.m_baseBlood = proxyPlayer.BaseBlood;
			this.m_currentWeapon = proxyPlayer.GetItemTemplateInfo();
			this.m_secondWeapon = proxyPlayer.GetItemInfo();
			this.GPRate = proxyPlayer.GPAddPlus;
			this.m_gmExperienceRate = proxyPlayer.GMExperienceRate;
			this.m_auncherExperienceRate = proxyPlayer.AuncherExperienceRate;
			this.OfferRate = proxyPlayer.OfferAddPlus;
			this.m_gmOfferRate = proxyPlayer.GMOfferRate;
			this.m_auncherOfferRate = proxyPlayer.AuncherOfferRate;
			this.m_gmRichesRate = proxyPlayer.GMRichesRate;
			this.m_auncherRichesRate = proxyPlayer.AuncherRichesRate;
			this.m_antiAddictionRate = proxyPlayer.AntiAddictionRate;
			this.m_equipEffect = new List<int>();
			this.Buffers = infos;
            this.m_Honer = "TEST";
		}
		public double GetBaseAgility()
		{
			return this.m_baseAglilty;
		}
		public double GetBaseAttack()
		{
			return this.m_baseAttack;
		}
		public double GetBaseDefence()
		{
			return this.m_baseDefence;
		}
		public double GetBaseBlood()
		{
			return this.m_baseBlood;
		}
		public int AddGP(int gp)
		{
			int result;
			if (gp >= 0)
			{
				this.m_client.SendPlayerAddGP(this.PlayerCharacter.ID, gp);
				gp = (int)((double)gp * this.GPRate * (double)this.m_gmExperienceRate * (double)this.m_auncherExperienceRate * this.m_antiAddictionRate);
				if (gp > 100000)
				{
					ProxyPlayer.log.Error(string.Format("ProxyPlayer  =====  player.nickname : {0}, player.gp : {1}, add gp : {2}, GPRate : {3}, m_gmExperienceRate : {4}, m_auncherExperienceRate : {5}, m_antiAddictionRate : {6}  ======== gp > 10000", new object[]
					{
						this.PlayerCharacter.NickName,
						this.PlayerCharacter.GP,
						gp,
						this.GPRate,
						this.m_gmExperienceRate,
						this.m_auncherExperienceRate,
						this.m_antiAddictionRate
					}));
					gp = 1;
				}
				this.m_character.GP += gp;
				if (this.m_character.GP < 1)
				{
					this.m_character.GP = 1;
				}
				if (this.m_character.GP >= 14324419)
				{
					int temp = this.m_character.GP - gp;
					this.m_character.GP = 14324419;
					gp = ((this.m_character.GP - temp > 0) ? (this.m_character.GP - temp) : 0);
				}
				result = gp;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int RemoveGP(int gp)
		{
			int result;
			if (gp > 100000)
			{
				ProxyPlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.gp : {1}, remove gp : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.GP, gp));
				result = 0;
			}
			else
			{
				this.m_client.SendPlayerRemoveGP(this.PlayerCharacter.ID, gp);
				result = gp;
			}
			return result;
		}
		public int AddGold(int value)
		{
			int result;
			if (value > 100000)
			{
				ProxyPlayer.log.Error(string.Format("ProxyPlayer ===== player.nickname : {0}, player. : {1}, add gold : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Gold, value));
				result = 0;
			}
			else
			{
				this.m_client.SendPlayerAddGold(this.PlayerCharacter.ID, value);
				result = 0;
			}
			return result;
		}
		public int RemoveGold(int value)
		{
			int result;
			if (value > 100000)
			{
				ProxyPlayer.log.Error(string.Format("ProxyPlayer ===== player.nickname : {0}, player. : {1}, remove gold : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Gold, value));
				result = 0;
			}
			else
			{
				this.m_client.SendPlayerRemoveGold(this.m_character.ID, value);
				result = 0;
			}
			return result;
		}
		public int AddMoney(int value, LogMoneyType master, LogMoneyType son)
		{
			return 0;
		}
		public int RemoveMoney(int value, LogMoneyType master, LogMoneyType son)
		{
			if (value > 100000)
			{
				ProxyPlayer.log.Error(string.Format("ProxyPlayer ===== player.nickname : {0}, player. : {1}, remove money : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Money, value));
			}
			this.m_client.SendPlayerRemoveMoney(this.m_character.ID, value, (int)master, (int)son);
			return 0;
		}
		public int AddGiftToken(int value)
		{
			return 0;
		}
		public int RemoveGiftToken(int value)
		{
			return 0;
		}
		public int AddRobRiches(int value)
		{
			int result;
			if (value > 0)
			{
				value = (int)((double)value * this.AntiAddictionRate);
				if (value > 100000)
				{
					ProxyPlayer.log.Error(string.Format("ProxyPlayer ====== player.nickname : {0}, player.RichesRob : {1}, AntiAddictionRate : {2} add rob riches: {3}", new object[]
					{
						this.PlayerCharacter.NickName,
						this.AntiAddictionRate,
						this.PlayerCharacter.RichesRob,
						value
					}));
				}
				this.m_client.SendAddRobRiches(this.m_character.ID, value);
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddOffer(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 1000)
				{
					ProxyPlayer.log.Error(string.Format("ProxyPlayer ===== player.nickname : {0}, player.offer : {1}, add offer : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Offer, value));
					result = 0;
				}
				else
				{
					this.m_client.SendPlayerAddOffer(this.PlayerCharacter.ID, value);
					result = value;
				}
			}
			else
			{
				this.RemoveOffer(Math.Abs(value));
				result = value;
			}
			return result;
		}
		public int RemoveOffer(int value)
		{
			if (value > 1000)
			{
				ProxyPlayer.log.Error(string.Format("GamePlayer ===== player.nickname : {0}, player.offer : {1}, remove offer : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Offer, value));
			}
			if (value >= this.m_character.Offer)
			{
				value = this.m_character.Offer;
			}
			this.m_client.SendPlayerRemoveOffer(this.m_character.ID, value);
			return value;
		}
		public void LogAddMoney(LogMoneyType masterType, LogMoneyType sonType, int userId, int moneys, int SpareMoney)
		{
		}
		public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
		{
			this.m_client.SendPlayerUsePropInGame(this.PlayerCharacter.ID, bag, place, templateId, isLiving);
			game.Pause(500);
			return false;
		}
		public void OnGameOver(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			this.m_client.SendPlayerOnGameOver(this.PlayerCharacter.ID, game.Id, isWin, gainXp, isSpanArea, isCouple);
		}
		public void Disconnect()
		{
			this.m_client.SendDisconnectPlayer(this.m_character.ID);
		}
		public void SendTCP(GSPacketIn pkg)
		{
			this.m_client.SendPacketToPlayer(this.m_character.ID, pkg, this.GameId);
		}
		public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea)
		{
			this.m_client.SendPlayerOnKillingLiving(this.m_character.ID, game, type, id, isLiving, demage, isSpanArea);
		}
		public void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int turnNum)
		{
			this.m_client.SendPlayerOnMissionOver(this.m_character.ID, game, isWin, MissionID, turnNum);
		}
		public int ConsortiaFight(int consortiaWin, int consortiaLose, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
		{
			this.m_client.SendPlayerConsortiaFight(this.m_character.ID, consortiaWin, consortiaLose, roomType, gameClass, totalKillHealth, count);
			return 0;
		}
		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			this.m_client.SendPlayerSendConsortiaFight(this.m_character.ID, consortiaID, riches, msg);
		}
		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count)
		{
			this.m_client.SendPlayerAddTemplate(this.m_character.ID, cloneItem, bagType, count);
			return true;
		}
		public void SendMessage(eMessageType type, string msg)
		{
		}
		public bool IsPvePermission(int missionId, eHardLevel hardLevel)
		{
			return true;
		}
		public bool SetPvePermission(int missionId, eHardLevel hardLevel)
		{
			return true;
		}
		public void SendInsufficientMoney(int type)
		{
		}
		public void ClearTempBag()
		{
			this.m_client.SendClearBag(this.m_character.ID, 1);
		}
		public void ClearFightBag()
		{
			this.m_client.SendClearBag(this.m_character.ID, 2);
		}
		public int AddGpDirect(int gp)
		{
			return 0;
		}
		public void UpdateAnswerSite(int id)
		{
		}
		public bool SetFightLabPermission(int missionId, eHardLevel hardLevel)
		{
			return true;
		}
		public bool IsFightLabPermission(int missionId, eHardLevel hardLevel)
		{
			return true;
		}
		public bool SetFightLabPermission(int copyId, eHardLevel hardLevel, int missionId)
		{
			return true;
		}
	}
}
