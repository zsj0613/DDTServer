using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.LogEnum;
using Game.Server.Achievement;
using Game.Server.Buffer;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using Game.Server.SpaRooms;

using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Game.Server.GameObjects
{
    public class GamePlayer : IGamePlayer
    {
        public delegate void PlayerItemPropertyEventHandle(int templateID);
        public delegate void PlayerGameOverEventHandle(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple);
        public delegate void PlayerMissionOverEventHandle(AbstractGame game, int missionId, bool isWin);
        public delegate void PlayerMissionTurnOverEventHandle(AbstractGame game, int missionId, int turnNum);
        public delegate void PlayerItemStrengthenEventHandle(int categoryID, int level);
        public delegate void PlayerShopEventHandle(int money, int gold, int offer, int gifttoken, string payGoods);
        public delegate void PlayerItemFusionEventHandle(int fusionType);
        public delegate void PlayerItemMeltEventHandle(int categoryID);
        public delegate void PlayerGameKillEventHandel(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea);
        public delegate void PlayerOwnConsortiaEventHandle();
        public delegate void PlayerItemComposeEventHandle(int composeType);
        public delegate void GameKillDropEventHandel(AbstractGame game, int npcId, bool playResult);
        public delegate void PlayerItemInsertEventHandle();
        public delegate void PlayerOwnSpaEventHandle();
        public delegate void PlayerPropertyChangedEventHandel(PlayerInfo character);
        public delegate void PlayerAddItemEventHandel(string type, int value);
        public delegate void PlayerGoodsPresentEventHandel(int count);
        public delegate void PlayerDispatchesEventHandel();
        public delegate void PlayerQuestFinishEventHandel(BaseQuest baseQuest);
        public delegate void PlayerMarryEventHandel();
        public delegate void PlayerEventHandle(GamePlayer player);
        private static LogProvider log => LogProvider.Default;
        private int m_playerId;
        protected GameClient m_client;
        private PlayerInfo m_character;
        private eClientType m_clientType;
        private string m_account;
        private int m_immunity = 255;
        public int FightPower;
        private bool m_isMinor;
        private bool m_isAASInfo;
        private long m_pingTime;
        public DateTime BoxBeginTime;
        public bool ASSonSend;
        private char[] m_pvepermissions;
        private char[] m_fightlabpermissions;
        public long PingStart;
        private Dictionary<string, object> m_tempProperties = new Dictionary<string, object>();
        private ePlayerState m_playerState = ePlayerState.NotLogin;
        private PlayerEquipInventory m_mainBag;
        private PlayerInventory m_propBag;
        private PlayerInventory m_fightBag;
        private PlayerInventory m_storeBag;
        private PlayerInventory m_tempBag;
        private PlayerInventory m_hideBag;
        private QuestInventory m_questInventory;
        private AchievementInventory m_achievementInventory;
        private BufferList m_bufferList;
        private List<int> m_equipEffect;
        private int m_changed;
        public double GPAddPlus = 1.0;
        public double OfferAddPlus = 1.0;
        public double GuildRichAddPlus = 1.0;
        public DateTime LastChatTime;
        public bool KickProtect;
        private bool m_styleChanged;
        private ItemInfo m_MainWeapon;
        private ItemInfo m_currentSecondWeapon;
        private object charge_locker = new object();
        private Dictionary<int, int> _friends;
        private BaseRoom m_currentRoom;
        public int CurrentRoomIndex;
        public int CurrentRoomTeam;
        public int X;
        public int Y;
        public int MarryMap;
        private MarryRoom _currentMarryRoom;
        public int Spa_X = 480;
        public int Spa_Y = 560;
        public int LastPosX = 480;
        public int LastPosY = 560;
        public int Spa_Player_Direction = 3;
        public bool Spa_Last_Area = false;
        public bool Spa_Day_Alter_Continue = true;
        private DateTime _spaRoomEnterDate;
        private SpaRoom _currentSpaRoom;
        private int _spaRoomAddGPTotal = 0;
        private DateTime _dayOrNightInSpa = DateTime.Now;


        public string Honor
        {
            get
            {
                return "TEST";
            }
        }


        private static char[] permissionChars = new char[]
        {
            '1',
            '3',
            '7',
            'F'
        };
        private static char[] fightlabpermissionChars = new char[]
        {
            '0',
            '1',
            '2',
            '3'
        };
        public event PlayerEventHandle LevelUp;
        public event PlayerItemPropertyEventHandle AfterUsingItem;
        public event PlayerGameOverEventHandle GameOver;
        public event PlayerMissionOverEventHandle MissionOver;
        public event PlayerMissionTurnOverEventHandle MissionTurnOver;
        public event PlayerItemStrengthenEventHandle ItemStrengthen;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.ItemStrengthen = (GamePlayer.PlayerItemStrengthenEventHandle)Delegate.Combine(this.ItemStrengthen, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.ItemStrengthen = (GamePlayer.PlayerItemStrengthenEventHandle)Delegate.Remove(this.ItemStrengthen, value);
        //    }
        //}
        public event GamePlayer.PlayerShopEventHandle Paid;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.Paid = (GamePlayer.PlayerShopEventHandle)Delegate.Combine(this.Paid, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.Paid = (GamePlayer.PlayerShopEventHandle)Delegate.Remove(this.Paid, value);
        //    }
        //}
        public event GamePlayer.PlayerItemFusionEventHandle ItemFusion;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.ItemFusion = (GamePlayer.PlayerItemFusionEventHandle)Delegate.Combine(this.ItemFusion, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.ItemFusion = (GamePlayer.PlayerItemFusionEventHandle)Delegate.Remove(this.ItemFusion, value);
        //    }
        //}
        public event GamePlayer.PlayerItemMeltEventHandle ItemMelt;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.ItemMelt = (GamePlayer.PlayerItemMeltEventHandle)Delegate.Combine(this.ItemMelt, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.ItemMelt = (GamePlayer.PlayerItemMeltEventHandle)Delegate.Remove(this.ItemMelt, value);
        //    }
        //}
        public event GamePlayer.PlayerGameKillEventHandel AfterKillingLiving;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.AfterKillingLiving = (GamePlayer.PlayerGameKillEventHandel)Delegate.Combine(this.AfterKillingLiving, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.AfterKillingLiving = (GamePlayer.PlayerGameKillEventHandel)Delegate.Remove(this.AfterKillingLiving, value);
        //    }
        //}
        public event GamePlayer.PlayerOwnConsortiaEventHandle GuildChanged;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.GuildChanged = (GamePlayer.PlayerOwnConsortiaEventHandle)Delegate.Combine(this.GuildChanged, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.GuildChanged = (GamePlayer.PlayerOwnConsortiaEventHandle)Delegate.Remove(this.GuildChanged, value);
        //    }
        //}
        public event GamePlayer.PlayerItemComposeEventHandle ItemCompose;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.ItemCompose = (GamePlayer.PlayerItemComposeEventHandle)Delegate.Combine(this.ItemCompose, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.ItemCompose = (GamePlayer.PlayerItemComposeEventHandle)Delegate.Remove(this.ItemCompose, value);
        //    }
        //}
        public event GamePlayer.GameKillDropEventHandel GameKillDrop;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.GameKillDrop = (GamePlayer.GameKillDropEventHandel)Delegate.Combine(this.GameKillDrop, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.GameKillDrop = (GamePlayer.GameKillDropEventHandel)Delegate.Remove(this.GameKillDrop, value);
        //    }
        //}
        public event GamePlayer.PlayerItemInsertEventHandle ItemInsert;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.ItemInsert = (GamePlayer.PlayerItemInsertEventHandle)Delegate.Combine(this.ItemInsert, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.ItemInsert = (GamePlayer.PlayerItemInsertEventHandle)Delegate.Remove(this.ItemInsert, value);
        //    }
        //}
        public event GamePlayer.PlayerOwnSpaEventHandle PlayerSpa;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerSpa = (GamePlayer.PlayerOwnSpaEventHandle)Delegate.Combine(this.PlayerSpa, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerSpa = (GamePlayer.PlayerOwnSpaEventHandle)Delegate.Remove(this.PlayerSpa, value);
        //    }
        //}
        public event GamePlayer.PlayerPropertyChangedEventHandel PlayerPropertyChanged;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerPropertyChanged = (GamePlayer.PlayerPropertyChangedEventHandel)Delegate.Combine(this.PlayerPropertyChanged, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerPropertyChanged = (GamePlayer.PlayerPropertyChangedEventHandel)Delegate.Remove(this.PlayerPropertyChanged, value);
        //    }
        //}
        public event GamePlayer.PlayerAddItemEventHandel PlayerAddItem;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerAddItem = (GamePlayer.PlayerAddItemEventHandel)Delegate.Combine(this.PlayerAddItem, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerAddItem = (GamePlayer.PlayerAddItemEventHandel)Delegate.Remove(this.PlayerAddItem, value);
        //    }
        //}
        public event GamePlayer.PlayerGoodsPresentEventHandel PlayerGoodsPresent;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerGoodsPresent = (GamePlayer.PlayerGoodsPresentEventHandel)Delegate.Combine(this.PlayerGoodsPresent, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerGoodsPresent = (GamePlayer.PlayerGoodsPresentEventHandel)Delegate.Remove(this.PlayerGoodsPresent, value);
        //    }
        //}
        public event GamePlayer.PlayerDispatchesEventHandel PlayerDispatches;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerDispatches = (GamePlayer.PlayerDispatchesEventHandel)Delegate.Combine(this.PlayerDispatches, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerDispatches = (GamePlayer.PlayerDispatchesEventHandel)Delegate.Remove(this.PlayerDispatches, value);
        //    }
        //}
        public event GamePlayer.PlayerQuestFinishEventHandel PlayerQuestFinish;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerQuestFinish = (GamePlayer.PlayerQuestFinishEventHandel)Delegate.Combine(this.PlayerQuestFinish, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerQuestFinish = (GamePlayer.PlayerQuestFinishEventHandel)Delegate.Remove(this.PlayerQuestFinish, value);
        //    }
        //}
        public event GamePlayer.PlayerMarryEventHandel PlayerMarry;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerMarry = (GamePlayer.PlayerMarryEventHandel)Delegate.Combine(this.PlayerMarry, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerMarry = (GamePlayer.PlayerMarryEventHandel)Delegate.Remove(this.PlayerMarry, value);
        //    }
        //}
        public event GamePlayer.PlayerEventHandle UseBuffer;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.UseBuffer = (GamePlayer.PlayerEventHandle)Delegate.Combine(this.UseBuffer, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.UseBuffer = (GamePlayer.PlayerEventHandle)Delegate.Remove(this.UseBuffer, value);
        //    }
        //}
        public int Immunity
        {
            get
            {
                return this.m_immunity;
            }
            set
            {
                this.m_immunity = value;
            }
        }
        public int PlayerId
        {
            get
            {
                return this.m_playerId;
            }
        }
        public string Account
        {
            get
            {
                return this.m_account;
            }
        }
        public PlayerInfo PlayerCharacter
        {
            get
            {
                return this.m_character;
            }
        }
        public eClientType ClientType
        {
            get
            {
                return this.m_clientType;
            }
        }
        public GameClient Client
        {
            get
            {
                return this.m_client;
            }
        }
        public bool IsActive
        {
            get
            {
                return this.m_client.IsConnected;
            }
        }
        public IPacketLib Out
        {
            get
            {
                return this.m_client.Out;
            }
        }
        public bool IsMinor
        {
            get
            {
                return this.m_isMinor;
            }
            set
            {
                this.m_isMinor = value;
            }
        }
        public bool IsAASInfo
        {
            get
            {
                return this.m_isAASInfo;
            }
            set
            {
                this.m_isAASInfo = value;
            }
        }
        public long PingTime
        {
            get
            {
                return this.m_pingTime;
            }
            set
            {
                this.m_pingTime = value;
                GSPacketIn pkg = this.Out.SendNetWork(this.m_character.ID, this.m_pingTime);
                if (this.m_currentRoom != null)
                {
                    this.m_currentRoom.SendToAll(pkg, this);
                }
            }
        }
        public double GPPlusRate
        {
            get
            {
                return this.OfferAddPlus;
            }
        }
        public double OfferPlusRate
        {
            get
            {
                return this.OfferAddPlus;
            }
        }
        public float GMExperienceRate
        {
            get
            {
                return RateMgr.GetRate(eRateType.Experience_Rate);
            }
        }
        public float GMOfferRate
        {
            get
            {
                return RateMgr.GetRate(eRateType.Offer_Rate);
            }
        }
        public float GMRichesRate
        {
            get
            {
                return RateMgr.GetRate(eRateType.Riches_Rate);
            }
        }
        public float AuncherExperienceRate
        {
            get
            {
                return RateMgr.GetRate(this, eRateType.Auncher_Experience_Rate);
            }
        }
        public float AuncherOfferRate
        {
            get
            {
                return RateMgr.GetRate(this, eRateType.Auncher_Offer_Rate);
            }
        }
        public float AuncherRichesRate
        {
            get
            {
                return RateMgr.GetRate(this, eRateType.Auncher_Riches_Rate);
            }
        }
        public double AntiAddictionRate
        {
            get
            {
                return AntiAddictionMgr.GetAntiAddictionCoefficient(this.m_character.AntiAddiction);
            }
        }
        public bool IsArea
        {
            get
            {
                return false;
            }
        }
        public Dictionary<string, object> TempProperties
        {
            get
            {
                return this.m_tempProperties;
            }
        }
        public ePlayerState PlayerState
        {
            get
            {
                return this.m_playerState;
            }
        }
        public PlayerEquipInventory MainBag
        {
            get
            {
                return this.m_mainBag;
            }
        }
        public PlayerInventory PropBag
        {
            get
            {
                return this.m_propBag;
            }
        }
        public PlayerInventory FightBag
        {
            get
            {
                return this.m_fightBag;
            }
        }
        public PlayerInventory TempBag
        {
            get
            {
                return this.m_tempBag;
            }
        }
        public PlayerInventory StoreBag
        {
            get
            {
                return this.m_storeBag;
            }
        }
        public PlayerInventory HideBag
        {
            get
            {
                return this.m_hideBag;
            }
        }
        public QuestInventory QuestInventory
        {
            get
            {
                return this.m_questInventory;
            }
        }
        public AchievementInventory AchievementInventory
        {
            get
            {
                return this.m_achievementInventory;
            }
        }
        public BufferList BufferList
        {
            get
            {
                return this.m_bufferList;
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
        public bool CanUseProp
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        public int Level
        {
            get
            {
                return this.m_character.Grade;
            }
            set
            {
                if (value != this.m_character.Grade)
                {
                    if (value > this.m_character.Grade)
                    {
                        this.OnLevelUp(value);
                    }
                    this.m_character.Grade = value;
                    this.OnPropertiesChanged();
                }
            }
        }
        public bool StyleChanged
        {
            get
            {
                return this.m_styleChanged;
            }
        }
        public int LevelPlusBlood
        {
            get
            {
                int plusblood = 0;
                for (int i = 10; i <= 80; i += 10)
                {
                    if (this.PlayerCharacter.Grade - i > 0)
                    {
                        plusblood += (this.PlayerCharacter.Grade - i) * (i + 20);
                    }
                }
                return plusblood;
            }
        }
        public ItemTemplateInfo MainWeapon
        {
            get
            {
                ItemTemplateInfo result;
                if (this.m_MainWeapon == null)
                {
                    result = null;
                }
                else
                {
                    result = this.m_MainWeapon.Template;
                }
                return result;
            }
        }
        public ItemInfo SecondWeapon
        {
            get
            {
                ItemInfo result;
                if (this.m_currentSecondWeapon == null)
                {
                    result = null;
                }
                else
                {
                    result = this.m_currentSecondWeapon;
                }
                return result;
            }
        }
        public Dictionary<int, int> Friends
        {
            get
            {
                return this._friends;
            }
        }
        public BaseRoom CurrentRoom
        {
            get
            {
                return this.m_currentRoom;
            }
            set
            {
                BaseRoom old = Interlocked.Exchange<BaseRoom>(ref this.m_currentRoom, value);
                if (old != null)
                {
                    RoomMgr.ExitRoom(old, this);
                }
            }
        }
        public int GamePlayerId
        {
            get;
            set;
        }
        public MarryRoom CurrentMarryRoom
        {
            get
            {
                return this._currentMarryRoom;
            }
            set
            {
                this._currentMarryRoom = value;
            }
        }
        public bool IsInMarryRoom
        {
            get
            {
                return this._currentMarryRoom != null;
            }
        }
        public DateTime SpaRoomEnterDate
        {
            get
            {
                return this._spaRoomEnterDate;
            }
            set
            {
                this._spaRoomEnterDate = value;
            }
        }
        public SpaRoom CurrentSpaRoom
        {
            get
            {
                return this._currentSpaRoom;
            }
            set
            {
                this._currentSpaRoom = value;
            }
        }
        public int SpaRoomAddGPTotal
        {
            get
            {
                return this._spaRoomAddGPTotal;
            }
            set
            {
                this._spaRoomAddGPTotal = value;
            }
        }
        public DateTime DayOrNightInSpa
        {
            get
            {
                return this._dayOrNightInSpa;
            }
            set
            {
                this._dayOrNightInSpa = value;
            }
        }
        public int AreaID
        {
            get
            {
                return GameServer.Instance.Config.AreaID;
            }
        }
        public string AreaName
        {
            get
            {
                return GameServer.Instance.Config.AreaName;
            }
        }
        public int m_ChargedMoney;
        public int ChargedMoney
        {
            get
            {
                return this.m_ChargedMoney;
            }
        }
        public int VIPLevel
        {
            get
            {
                return VIPMgr.GetVIPlevel(ChargedMoney);
            }
        }

        public bool IsInWorldBossRoom
        {
            get;
            set;
        }

        public GamePlayer(int playerId, string account, GameClient client, PlayerInfo info, eClientType clientType)
		{
			this.m_playerId = playerId;
			this.m_account = account;
			this.m_client = client;
			this.m_character = info;
			this.m_clientType = clientType;
			this.LastChatTime = DateTime.Today;
			this.m_mainBag = new PlayerEquipInventory(this);
			this.m_propBag = new PlayerInventory(this, true, 49, 1, 0, true);
			this.m_fightBag = new PlayerInventory(this, false, 3, 3, 0, false);
			this.m_tempBag = new PlayerInventory(this, false, 60, 4, 0, true);
			this.m_storeBag = new PlayerInventory(this, true, 100, 11, 0, true);
			this.m_hideBag = new PlayerInventory(this, true, 11, 12, 0, true);
			this.m_questInventory = new QuestInventory(this);
			this.m_achievementInventory = new AchievementInventory(this);
			this.m_bufferList = new BufferList(this);
			this.m_equipEffect = new List<int>();
			this.X = 646;
			this.Y = 1241;
			this.MarryMap = 0;
			this.ASSonSend = true;
		}
		public PlayerInventory GetInventory(eBageType bageType)
		{
			switch (bageType)
			{
			case eBageType.MainBag:
			{
				PlayerInventory result = this.m_mainBag;
				return result;
			}
			case eBageType.PropBag:
			{
				PlayerInventory result = this.m_propBag;
				return result;
			}
			case eBageType.TaskBag:
				break;
			case eBageType.FightBag:
			{
				PlayerInventory result = this.m_fightBag;
				return result;
			}
			case eBageType.TempBag:
			{
				PlayerInventory result = this.m_tempBag;
				return result;
			}
			default:
				switch (bageType)
				{
				case eBageType.Bank:
				{
					PlayerInventory result = this.m_storeBag;
					return result;
				}
				case eBageType.HideBag:
				{
					PlayerInventory result = this.m_hideBag;
					return result;
				}
				}
				break;
			}
			throw new NotSupportedException(string.Format("Did not support this type bag: {0}", bageType));
		}
		public string GetInventoryName(eBageType bageType)
		{
			string result;
			switch (bageType)
			{
			case eBageType.MainBag:
				result = LanguageMgr.GetTranslation("Game.Server.GameObjects.Equip", new object[0]);
				break;
			case eBageType.PropBag:
				result = LanguageMgr.GetTranslation("Game.Server.GameObjects.Prop", new object[0]);
				break;
			default:
				result = bageType.ToString();
				break;
			}
			return result;
		}
		public PlayerInventory GetItemInventory(ItemTemplateInfo template)
		{
			return this.GetInventory(template.BagType);
		}
		public ItemInfo GetItemAt(eBageType bagType, int place)
		{
			PlayerInventory bag = this.GetInventory(bagType);
			ItemInfo result;
			if (bag != null)
			{
				result = bag.GetItemAt(place);
			}
			else
			{
				result = null;
			}
			return result;
		}
		public int GetItemCount(int templateId)
		{
			return this.m_propBag.GetItemCount(templateId) + this.m_mainBag.GetItemCount(templateId) + this.m_storeBag.GetItemCount(templateId);
		}
		public bool AddItem(ItemInfo item)
		{
			AbstractInventory bg = this.GetItemInventory(item.Template);
			return bg.AddItem(item);
		}
		public void UpdateItem(ItemInfo item)
		{
			this.m_mainBag.UpdateItem(item);
			this.m_propBag.UpdateItem(item);
			this.m_hideBag.UpdateItem(item);
		}
		public bool TakeOutItem(ItemInfo item)
		{
			bool result;
			if (item.BagType == this.m_propBag.BagType)
			{
				result = this.m_propBag.TakeOutItem(item);
			}
			else
			{
				if (item.BagType == this.m_fightBag.BagType)
				{
					result = this.m_fightBag.TakeOutItem(item);
				}
				else
				{
					if (item.BagType == this.m_hideBag.BagType)
					{
						result = this.m_hideBag.TakeOutItem(item);
					}
					else
					{
						if (item.BagType == this.m_storeBag.BagType)
						{
							result = this.m_storeBag.TakeOutItem(item);
						}
						else
						{
							result = this.m_mainBag.TakeOutItem(item);
						}
					}
				}
			}
			return result;
		}
		public bool RemoveItem(ItemInfo item, eItemRemoveType type)
		{
			bool result;
			if (item.BagType == this.m_propBag.BagType)
			{
				result = this.m_propBag.RemoveItem(item, type);
			}
			else
			{
				if (item.BagType == this.m_fightBag.BagType)
				{
					result = this.m_fightBag.RemoveItem(item, type);
				}
				else
				{
					if (item.BagType == this.m_hideBag.BagType)
					{
						result = this.m_hideBag.RemoveItem(item, type);
					}
					else
					{
						if (item.BagType == this.m_tempBag.BagType)
						{
							result = this.m_tempBag.RemoveItem(item, type);
						}
						else
						{
							result = this.m_mainBag.RemoveItem(item, type);
						}
					}
				}
			}
			return result;
		}
		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count)
		{
			return this.AddTemplate(cloneItem, bagType, count, true);
		}
		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, bool isdrop)
		{
			PlayerInventory bag = this.GetInventory(bagType);
			bool result;
			if (bag != null)
			{
				if (bag.AddTemplate(cloneItem, count))
				{
					if (isdrop)
					{
						this.SendItemNotice(cloneItem, null, 1);
					}
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public bool RemoveTemplate(int templateId, int count, eItemRemoveType type)
		{
			int mainItem = this.m_mainBag.GetItemCount(templateId);
			int propItem = this.m_propBag.GetItemCount(templateId);
			int storeItem = this.m_storeBag.GetItemCount(templateId);
			int tempCount = mainItem + propItem + storeItem;
			ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateId);
			bool result;
			if (template != null && tempCount >= count)
			{
				if (mainItem > 0 && count > 0)
				{
					if (this.m_mainBag.RemoveTemplate(templateId, (mainItem > count) ? count : mainItem, type))
					{
						count = ((count < mainItem) ? 0 : (count - mainItem));
					}
				}
				if (propItem > 0 && count > 0)
				{
					if (this.m_propBag.RemoveTemplate(templateId, (propItem > count) ? count : propItem, type))
					{
						count = ((count < propItem) ? 0 : (count - propItem));
					}
				}
				if (storeItem > 0 && count > 0)
				{
					if (this.m_storeBag.RemoveTemplate(templateId, (storeItem > count) ? count : storeItem, type))
					{
						count = ((count < storeItem) ? 0 : (count - storeItem));
					}
				}
				if (count != 0)
				{
					GamePlayer.log.Error(string.Format("Item Remover Error：PlayerId {0},TemplateId:{1},Count:{2} Main:{3} Prop:{4} Store:{5}", new object[]
					{
						this.m_playerId,
						templateId,
						count,
						mainItem,
						propItem,
						storeItem
					}));
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}
		public bool RemoveTemplate(eBageType bagType, ItemTemplateInfo template, int count, eItemRemoveType type)
		{
			PlayerInventory bag = this.GetInventory(bagType);
			return bag != null && bag.RemoveTemplate(template.TemplateID, count, type);
		}
		public bool RemoveTemplate(ItemTemplateInfo template, int count, eItemRemoveType type)
		{
			PlayerInventory bag = this.GetItemInventory(template);
			return bag != null && bag.RemoveTemplate(template.TemplateID, count, type);
		}
		public void ClearTempBag()
		{
			this.TempBag.ClearBag(eItemRemoveType.ItemInTemp);
		}
		public void ClearFightBag()
		{
			this.FightBag.ClearBag(eItemRemoveType.ItemInFight);
		}
		public void ClearHideBag()
		{
			List<ItemInfo> list = this.m_hideBag.GetItems();
			foreach (ItemInfo item in list)
			{
				PlayerInventory bag = this.GetItemInventory(item.Template);
				string key = string.Format("temp_place_{0}", item.ItemID);
				if (this.TempProperties.ContainsKey(key))
				{
					int toSlot = (int)this.TempProperties[key];
					this.TempProperties.Remove(key);
					if (bag.AddItemTo(item, toSlot))
					{
						this.m_hideBag.TakeOutItem(item);
					}
					else
					{
						if (bag.AddItem(item))
						{
							this.m_hideBag.TakeOutItem(item);
						}
					}
				}
				else
				{
					if (bag.StackItemToAnother(item))
					{
						this.m_hideBag.RemoveItem(item, eItemRemoveType.Stack);
					}
					else
					{
						if (bag.AddItem(item))
						{
							this.m_hideBag.TakeOutItem(item);
						}
					}
				}
			}
			list = this.m_hideBag.GetItems();
			if (list.Count > 0)
			{
				string str = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content2", new object[0]);
				string str2 = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title2", new object[0]);
				this.SendItemsToMail(list, str, str2, eMailType.ItemOverdue);
				this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
		public void OnLevelUp(int grade)
		{
			if (this.LevelUp != null)
			{
				this.LevelUp(this);
			}
            if(Math.Round(((decimal)(grade)/(decimal)10),MidpointRounding.AwayFromZero)> Math.Round(((decimal)(this.Level) / (decimal)10), MidpointRounding.AwayFromZero))
            {
                int a = Convert.ToInt32(Math.Round(((decimal)(grade) / (decimal)10), MidpointRounding.AwayFromZero))-1;
                if (this.PlayerCharacter.InviteID != 0)
                {
                    using (PlayerBussiness db = new PlayerBussiness())
                    {
                        int x = this.PlayerCharacter.InviteID;
                        PlayerInfo info = db.GetUserSingleByUserID(x);
                        if (info == null)
                        {
                            return;
                        }
                        MailInfo c = new MailInfo();
                        c.Content = "你邀请的玩家"+this.PlayerCharacter.NickName+"已升到"+grade.ToString()+"级，你获得奖励。";
                        c.Title = "邀请系统奖励";                       
                        c.IsExist = true;
                        int[] moneys = new int[] {0,0,0,0,100,500,1000,1000};
                        int[] GiftTokens = new int[] { 100, 200, 300, 400, 500, 1000, 1000, 1000 };
                        int[] Golds = new int[] { 1000, 2000, 3000, 4000, 5000, 10000, 2000, 50000 };
                        c.Money = moneys[a];
                        c.GiftToken = GiftTokens[a];
                        c.Gold = Golds[a];
                        c.Receiver = info.NickName;
                        c.ReceiverID = x;
                        c.Sender = "邀请系统";
                        c.SenderID = 0;
                        c.Type = 1;
                        if (db.SendMail(c))
                        {
                            GamePlayer client = WorldMgr.GetPlayerById(x);
                            if (client != null)
                            {
                                GSPacketIn pkgMsg = new GSPacketIn(117);
                                pkgMsg.WriteInt(x);
                                pkgMsg.WriteInt(1);
                                client.SendTCP(pkgMsg);
                            }
                        }
                    }
                }
            }

		}
		public void OnUsingItem(int templateID)
		{
			if (this.AfterUsingItem != null && templateID > 0)
			{
				this.AfterUsingItem(templateID);
			}
		}
		public void OnGameOver(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			if (game.RoomType == eRoomType.Match)
			{
				this.PlayerCharacter.CheckCount++;
				this.Out.SendCheckCode();
				if (isWin)
				{
					this.m_character.Win++;
				}
				this.m_character.Total++;
				this.UpdateProperties();
			}
			if (this.GameOver != null)
			{
				this.GameOver(game, isWin, gainXp, isSpanArea, isCouple);
			}
		}
		public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum)
		{
			if (this.MissionOver != null)
			{
				this.MissionOver(game, missionId, isWin);
			}
			if (this.MissionTurnOver != null && isWin)
			{
				this.MissionTurnOver(game, missionId, turnNum);
			}
		}
		public void OnItemStrengthen(int categoryID, int level)
		{
			if (this.ItemStrengthen != null)
			{
				this.ItemStrengthen(categoryID, level);
			}
		}
		public void OnPaid(int money, int gold, int offer, int gifttoken, string payGoods)
		{
			if (this.Paid != null)
			{
				this.Paid(money, gold, offer, gifttoken, payGoods);
			}
		}
		public void OnItemFusion(int fusionType)
		{
			if (this.ItemFusion != null)
			{
				this.ItemFusion(fusionType);
			}
		}
		public void OnItemMelt(int categoryID)
		{
			if (this.ItemMelt != null)
			{
				this.ItemMelt(categoryID);
			}
		}
		public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage, bool isSpanArea)
		{
			if (this.AfterKillingLiving != null)
			{
				this.AfterKillingLiving(game, type, id, isLiving, damage, isSpanArea);
			}
			if (this.GameKillDrop != null && !isLiving)
			{
				this.GameKillDrop(game, id, isLiving);
			}
		}
		public void OnGuildChanged()
		{
			if (this.GuildChanged != null)
			{
				this.GuildChanged();
			}
		}
		public void OnItemCompose(int composeType)
		{
			if (this.ItemCompose != null)
			{
				this.ItemCompose(composeType);
			}
		}
		public void OnItemInsert()
		{
			if (this.ItemInsert != null)
			{
				this.ItemInsert();
			}
		}
		public void OnPlayerSpa()
		{
			if (this.PlayerSpa != null)
			{
				this.PlayerSpa();
			}
		}
		public void OnPlayerPropertyChanged(PlayerInfo character)
		{
			if (this.PlayerPropertyChanged != null)
			{
				this.PlayerPropertyChanged(character);
			}
		}
		public void OnPlayerAddItem(string type, int value)
		{
			if (this.PlayerAddItem != null)
			{
				this.PlayerAddItem(type, value);
			}
		}
		public void OnPlayerGoodsPresent(int count)
		{
			if (this.PlayerGoodsPresent != null)
			{
				this.PlayerGoodsPresent(count);
			}
		}
		public void OnPlayerDispatches()
		{
			if (this.PlayerDispatches != null)
			{
				this.PlayerDispatches();
			}
		}
		public void OnPlayerQuestFinish(BaseQuest baseQuest)
		{
			if (this.PlayerQuestFinish != null)
			{
				this.PlayerQuestFinish(baseQuest);
			}
		}
		public void OnPlayerMarry()
		{
			if (this.PlayerMarry != null)
			{
				this.PlayerMarry();
			}
		}
		public void OnUseBuffer()
		{
			if (this.UseBuffer != null)
			{
				this.UseBuffer(this);
			}
		}
		public void BeginAllChanges()
		{
			this.BeginChanges();
			this.m_bufferList.BeginChanges();
			this.m_mainBag.BeginChanges();
			this.m_propBag.BeginChanges();
			this.m_hideBag.BeginChanges();
		}
		public void CommitAllChanges()
		{
			this.CommitChanges();
			this.m_bufferList.CommitChanges();
			this.m_mainBag.CommitChanges();
			this.m_propBag.CommitChanges();
			this.m_hideBag.CommitChanges();
		}
		public void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changed);
		}
		public void CommitChanges()
		{
			Interlocked.Decrement(ref this.m_changed);
			this.OnPropertiesChanged();
		}
		protected void OnPropertiesChanged()
		{
			if (this.m_changed <= 0)
			{
				if (this.m_changed < 0)
				{
					GamePlayer.log.Error("Player changed count < 0");
					Thread.VolatileWrite(ref this.m_changed, 0);
				}
				this.UpdateProperties();
				this.OnPlayerPropertyChanged(this.m_character);
			}
		}
		public void UpdateProperties()
		{
			this.Out.SendUpdatePrivateInfo(this.m_character);
			GSPacketIn pkg = this.Out.SendUpdatePublicPlayer(this.m_character);
			if (this.m_currentRoom != null)
			{
				this.m_currentRoom.SendToAll(pkg, this);
			}
		}
		public int AddAchievementPoint(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 990000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ===== player.nickname : {0}, player. : {1}, add AchievementPoint : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.AchievementPoint, value));
					result = 0;
				}
				else
				{
					this.m_character.AchievementPoint += value;
					this.OnPropertiesChanged();
					result = value;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddGold(int value)
		{
			int result;
			if (value > 0)
			{
				//if (value > 990000)
				//{
				//	GamePlayer.log.Error(string.Format("GamePlayer ===== player.nickname : {0}, player. : {1}, add gold : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Gold, value));
				//	result = 0;
				//}
				//else
				{
					this.m_character.Gold += value;
					this.OnPlayerAddItem("Gold", value);
					this.OnPropertiesChanged();
					result = value;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int RemoveGold(int value)
		{
			if (value > 100000)
			{
				GamePlayer.log.Error(string.Format("GamePlayer ===== player.nickname : {0}, player. : {1}, remove gold : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Gold, value));
			}
			int result;
			if (value > 0 && value <= this.m_character.Gold)
			{
				this.m_character.Gold -= value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddMoney(int value, LogMoneyType master, LogMoneyType son)
		{
			int result;
			if (value > 0)
			{
				this.m_character.Money += value;
				this.OnPropertiesChanged();
				//LogMgr.LogWealthAdd(master, son, this.PlayerCharacter.ID, value, this.m_character.Money);
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int RemoveMoney(int value, LogMoneyType master, LogMoneyType son)
		{
			int result;
			if (value > 0 && value <= this.m_character.Money)
			{
				if (value > 100000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ===== player.nickname : {0}, player. : {1}, remove money : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Money, value));
				}
				this.m_character.Money -= value;
				this.OnPropertiesChanged();
				//LogMgr.LogWealthAdd(master, son, this.PlayerCharacter.ID, -value, this.m_character.Money);
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
			return this.AddOffer(value, true);
		}
		public int AddOffer(int value, bool IsRate)
		{
			int result;
			if (value > 0)
			{
				if (value > 1000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ===== player.nickname : {0}, player.offer : {1}, add offer : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Offer, value));
					result = 0;
				}
				else
				{
					this.m_character.Offer += value;
					this.OnPropertiesChanged();
					result = value;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int RemoveOffer(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 10000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ===== player.nickname : {0}, player.offer : {1}, remove offer : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.Offer, value));
					result = 0;
				}
				else
				{
					if (value >= this.m_character.Offer)
					{
						value = this.m_character.Offer;
					}
					this.m_character.Offer -= value;
					this.OnPropertiesChanged();
					result = value;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int RemoveGiftToken(int value)
		{
			if (value > 10000)
			{
				GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.GiftToken {1}, add giftToken : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.GiftToken, value));
			}
			int result;
			if (value > 0 && value <= this.m_character.GiftToken)
			{
				this.m_character.GiftToken -= value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddGP(int gp)
		{
			int result;
			if (gp >= 0)
			{
				gp = (int)((double)gp * this.GPAddPlus * (double)this.GMExperienceRate * (double)this.AuncherExperienceRate * this.AntiAddictionRate);

				this.m_character.GP += gp;
				if (this.m_character.GP < 1)
				{
					this.m_character.GP = 1;
				}
				if (this.m_character.GP >= LevelMgr.GetGP(100))
				{
					int temp = this.m_character.GP - gp;
					this.m_character.GP = LevelMgr.GetGP(100);
					gp = ((this.m_character.GP - temp > 0) ? (this.m_character.GP - temp) : 0);
				}
				this.Level = LevelMgr.GetLevel(this.m_character.GP);
				this.UpdateFightPower();
				this.OnPropertiesChanged();
				if (gp > 0)
				{
					using (ProduceBussiness db = new ProduceBussiness())
					{
						db.UpdateBoxProgression(this.PlayerCharacter.ID, this.PlayerCharacter.BoxProgression, this.PlayerCharacter.GetBoxLevel, DateTime.Now, this.PlayerCharacter.BoxGetDate, this.PlayerCharacter.AlreadyGetBox);
					}
				}
				result = gp;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddGpDirect(int gp)
		{
			int result;
			if (gp >= 0)
			{
				{
					this.m_character.GP += gp;
					if (this.m_character.GP < 1)
					{
						this.m_character.GP = 1;
					}
					if (this.m_character.GP >= LevelMgr.GetGP(100))
					{
						int temp = this.m_character.GP - gp;
						this.m_character.GP = LevelMgr.GetGP(100);
						gp = ((this.m_character.GP - temp > 0) ? (this.m_character.GP - temp) : 0);
					}
					this.Level = LevelMgr.GetLevel(this.m_character.GP);
					this.UpdateFightPower();
					this.OnPropertiesChanged();
					if (gp > 0)
					{
						using (ProduceBussiness db = new ProduceBussiness())
						{
							db.UpdateBoxProgression(this.PlayerCharacter.ID, this.PlayerCharacter.BoxProgression, this.PlayerCharacter.GetBoxLevel, DateTime.Now, this.PlayerCharacter.BoxGetDate, this.PlayerCharacter.AlreadyGetBox);
						}
					}
					result = gp;
				}
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
			if (gp > 0)
			{
				if (gp > 100000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.gp : {1}, remove gp : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.GP, gp));
					result = 0;
				}
				else
				{
					this.m_character.GP -= gp;
					if (this.m_character.GP < 1)
					{
						this.m_character.GP = 1;
					}
					this.Level = LevelMgr.GetLevel(this.m_character.GP);
					this.OnPropertiesChanged();
					result = gp;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddRobRiches(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 100000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.RichesRob : {1}, add rob riches: {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.RichesRob, value));
				}
				this.m_character.RichesRob += value;
				this.OnPlayerAddItem("RichesRob", value);
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddRichesOffer(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 1000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.gp : {1}, add riches offer : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.RichesOffer, value));
					result = 0;
				}
				else
				{
					this.m_character.RichesOffer += value;
					this.OnPropertiesChanged();
					result = value;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddGiftToken(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 10000)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.GiftToken {1}, add giftToken : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.GiftToken, value));
				}
				this.m_character.GiftToken += value;
				this.OnPlayerAddItem("GiftToken", value);
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public bool CanEquip(ItemTemplateInfo item)
		{
			bool result = true;
			string msg = "";
			if (!item.CanEquip)
			{
				result = false;
				msg = LanguageMgr.GetTranslation("Game.Server.GameObjects.NoEquip", new object[0]);
			}
			else
			{
				if (item.NeedSex != 0 && item.NeedSex != (this.m_character.Sex ? 1 : 2))
				{
					result = false;
					msg = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanEquip", new object[0]);
				}
				else
				{
					if (this.m_character.Grade < item.NeedLevel)
					{
						result = false;
						msg = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanLevel", new object[0]);
					}
				}
			}
			if (!result)
			{
				this.Out.SendMessage(eMessageType.ERROR, msg);
			}
			return result;
		}
		public void UpdateBaseProperties(int attack, int defence, int agility, int lucky)
		{
			if (attack != this.m_character.Attack || defence != this.m_character.Defence || agility != this.m_character.Agility || lucky != this.m_character.Luck)
			{
				this.m_character.Attack = attack;
				this.m_character.Defence = defence;
				this.m_character.Agility = agility;
				this.m_character.Luck = lucky;
				this.OnPropertiesChanged();
			}
		}
		public int AddSpaPubGoldRoomLimit(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 1440)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.SpaPubGoldRoomLimit {1} min, add SpaPubGoldRoomLimit : {2} min", this.PlayerCharacter.NickName, this.PlayerCharacter.SpaPubGoldRoomLimit, value));
				}
				this.m_character.SpaPubGoldRoomLimit += value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int RemoveSpaPubGoldRoomLimit(int value)
		{
			int result;
			if (value > 0 && value <= this.PlayerCharacter.SpaPubGoldRoomLimit)
			{
				if (value > 1440)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.SpaPubGoldRoomLimit {1} min, remove SpaPubGoldRoomLimit : {2} min", this.PlayerCharacter.NickName, this.PlayerCharacter.SpaPubGoldRoomLimit, value));
				}
				this.m_character.SpaPubGoldRoomLimit -= value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int UpdateSpaPubGoldRoomLimit(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 1440)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.SpaPubGoldRoomLimit {1} min, update SpaPubGoldRoomLimit : {2} min", this.PlayerCharacter.NickName, this.PlayerCharacter.SpaPubGoldRoomLimit, value));
				}
				this.m_character.SpaPubGoldRoomLimit = value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int AddSpaPubMoneyRoomLimit(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 1440)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.SpaPubMoneyRoomLimit {1} min, add SpaPubMoneyRoomLimit : {2} min", this.PlayerCharacter.NickName, this.PlayerCharacter.SpaPubMoneyRoomLimit, value));
				}
				this.m_character.SpaPubMoneyRoomLimit += value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int RemoveSpaPubMoneyRoomLimit(int value)
		{
			int result;
			if (value > 0 && value <= this.PlayerCharacter.SpaPubMoneyRoomLimit)
			{
				if (value > 1440)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.SpaPubGoldRoomLimit {1} min, remove SpaPubGoldRoomLimit : {2} min", this.PlayerCharacter.NickName, this.PlayerCharacter.SpaPubMoneyRoomLimit, value));
				}
				this.m_character.SpaPubMoneyRoomLimit -= value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public int UpdateSpaPubMoneyRoomLimit(int value)
		{
			int result;
			if (value > 0)
			{
				if (value > 1440)
				{
					GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.SpaPubMoneyRoomLimit {1} min, update SpaPubMoneyRoomLimit : {2} min", this.PlayerCharacter.NickName, this.PlayerCharacter.SpaPubMoneyRoomLimit, value));
				}
				this.m_character.SpaPubMoneyRoomLimit = value;
				this.OnPropertiesChanged();
				result = value;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public void UpdateLastSpaDate(DateTime value)
		{
			DateTime arg_0C_0 = this.m_character.LastSpaDate;
			//bool flag = 1 == 0;
			if (value < this.m_character.LastSpaDate)
			{
				GamePlayer.log.Error(string.Format("GamePlayer ====== player.nickname : {0}, player.LastSpaDate {1}, update LastSpaDate : {2}", this.PlayerCharacter.NickName, this.PlayerCharacter.LastSpaDate, value));
			}
			this.m_character.LastSpaDate = value;
			this.OnPropertiesChanged();
		}
		public void UpdateIsInSpaPubGoldToday(bool value)
		{
			this.m_character.IsInSpaPubGoldToday = value;
			this.OnPropertiesChanged();
		}
		public void UpdateIsInSpaPubMoneyToday(bool value)
		{
			this.m_character.IsInSpaPubMoneyToday = value;
			this.OnPropertiesChanged();
		}
		public void UpdateNickName(string nickname)
		{
			this.m_character.NickName = nickname;
			GSPacketIn pkg = this.Out.SendUpdateNickname(nickname);
		}
		public void UpdateStyle(string style, string colors, string skin)
		{
			if (style != this.m_character.Style || colors != this.m_character.Colors || skin != this.m_character.Skin)
			{
				this.m_styleChanged = true;
				this.m_character.Style = style;
				this.m_character.Colors = colors;
				this.m_character.Skin = skin;
				this.OnPropertiesChanged();
			}
		}
		public void UpdateFightPower()
		{
			int baseproperty = 0;
			this.FightPower = 0;
			int blood = (int)((double)(950 + this.PlayerCharacter.Grade * 50 + this.LevelPlusBlood + this.PlayerCharacter.Defence / 10) * this.GetBaseBlood());
			baseproperty += this.PlayerCharacter.Attack;
			baseproperty += this.PlayerCharacter.Defence;
			baseproperty += this.PlayerCharacter.Agility;
			baseproperty += this.PlayerCharacter.Luck;
			double baseAttack = this.GetBaseAttack();
			double baseDefence = this.GetBaseDefence();
			this.FightPower += (int)((double)(baseproperty + 1000) * (baseAttack * baseAttack * baseAttack + 3.5 * baseDefence * baseDefence * baseDefence) / 100000000.0 + (double)blood * 0.95 - 950.0);
			if (this.m_currentSecondWeapon != null)
			{
				this.FightPower += (int)(500.0 * Math.Pow(1.2, (double)this.m_currentSecondWeapon.StrengthenLevel));
			}
			this.PlayerCharacter.FightPower = this.FightPower;
			this.OnPlayerPropertyChanged(this.m_character);
		}
		public void UpdateHide(int hide)
		{
			if (hide != this.m_character.Hide)
			{
				this.m_character.Hide = hide;
				this.OnPropertiesChanged();
			}
		}
		public void UpdateWeapon(ItemInfo item)
		{
			if (item != this.m_MainWeapon)
			{
				this.m_MainWeapon = item;
				this.OnPropertiesChanged();
			}
		}
		public void UpdateSecondWeapon(ItemInfo item)
		{
			if (item != this.m_currentSecondWeapon)
			{
				this.m_currentSecondWeapon = item;
				this.OnPropertiesChanged();
			}
		}
		public bool EquipItem(ItemInfo item, int place)
		{
			bool result;
			if (item.CanEquip() && item.BagType == this.m_mainBag.BagType)
			{
				int slot = this.m_mainBag.FindItemEpuipSlot(item.Template);
                if ((slot == 9 || slot == 10) && (place == 9 || place == 10))
                {
                    slot = place;
                }
                else if ((slot == 7 || slot == 8) && (place == 7 || place == 8))
                {
                    slot = place;
                }
                else if ((slot >= 18 && slot <= 27) && (place >= 18 && place <= 27))
                {
                    slot = place;
                }
				result = this.m_mainBag.MoveItem(item.Place, slot, item.Count);
			}
			else
			{
				result = false;
			}
			return result;
		}
		public void HideEquip(int categoryID, bool hide)
		{
			if (categoryID >= 0 && categoryID < 10)
			{
				this.EquipShowImp(categoryID, hide ? 2 : 1);
			}
		}
		public void ApertureEquip(int level)
		{
			this.EquipShowImp(0, (level < 5) ? 1 : ((level < 7) ? 2 : 3));
		}
		private void EquipShowImp(int categoryID, int para)
		{
			this.UpdateHide((int)((double)this.m_character.Hide + Math.Pow(10.0, (double)categoryID) * (double)(para - this.m_character.Hide / (int)Math.Pow(10.0, (double)categoryID) % 10)));
		}
		public bool Login()
		{
			bool result;
			if (WorldMgr.AddPlayer(this.m_character.ID, this))
			{
				try
				{
					if (this.LoadFromDatabase())
					{
						if (this.PlayerCharacter.BoxGetDate.ToShortDateString() != DateTime.Now.ToShortDateString())
						{
							this.PlayerCharacter.AlreadyGetBox = 0;
							this.PlayerCharacter.BoxProgression = 0;
						}
						this.Out.SendLoginSuccess();
						this.Out.SendDateTime();
						this.Out.SendCheckCode();
                       // GamePlayer.log.Debug("ChargedMoney:" + ChargedMoney.ToString() + "VIPLevel:" + VIPLevel.ToString());
                        this.Out.SendUpdateVIP(this);
                        AntiAddictionMgr.AASStateGet(this);
						this.Out.SendDailyAward(this, 0);
						this.Out.SendDailyAward(this, 1);
						this.LoadMarryMessage();
						this.m_playerState = ePlayerState.Playing;
						this.BoxBeginTime = DateTime.Now;
						//if (GameProperties.GoHomeState)
						//{
						//	this.OldPlayerGetBack();
						//}
                        if (this.VIPLevel >= 6)
                        {
                            GamePlayer[] players = WorldMgr.GetAllPlayers();
                            foreach (var player in players)
                            {
                                player.Out.SendMessage(eMessageType.ChatERROR,"尊贵的VIP"+this.VIPLevel.ToString()+"玩家"+this.m_character.NickName+"上线了！");
                            }
                        }
						result = true;
						return result;
					}
					WorldMgr.RemovePlayer(this.m_character.ID);
				}
				catch (Exception ex)
				{
					GamePlayer.log.Error("Error Login!", ex);
				}
			}
			result = false;
			return result;
		}
		public void LoadMarryMessage()
		{
			using (PlayerBussiness db = new PlayerBussiness())
			{
				MarryApplyInfo[] infos = db.GetPlayerMarryApply(this.PlayerCharacter.ID);
				if (infos != null)
				{
					MarryApplyInfo[] array = infos;
					for (int i = 0; i < array.Length; i++)
					{
						MarryApplyInfo info = array[i];
						switch (info.ApplyType)
						{
						case 1:
							this.Out.SendPlayerMarryApply(this, info.ApplyUserID, info.ApplyUserName, info.LoveProclamation, info.ID);
							break;
						case 2:
							this.Out.SendMarryApplyReply(this, info.ApplyUserID, info.ApplyUserName, info.ApplyResult, true, info.ID);
							if (!info.ApplyResult)
							{
								this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
							}
							break;
						case 3:
							this.Out.SendPlayerDivorceApply(this, true, false);
							break;
						}
					}
				}
			}
		}
		public void ChargeToUser()
		{
			using (PlayerBussiness db = new PlayerBussiness())
			{
				int money = 0;
				object obj;
				Monitor.Enter(obj = this.charge_locker);
				try
				{
					db.ChargeToUser(this.m_character.UserName, ref money, this.m_character.NickName);
				}
				finally
				{
					Monitor.Exit(obj);
				}
				if (money > 0)
				{
					this.AddMoney(money, LogMoneyType.Charge, LogMoneyType.Charge_RMB);
					if (db.SendMail(new MailInfo
					{
						Content = LanguageMgr.GetTranslation("ChargeToUser.Content", new object[]
						{
							money
						}),
						Title = LanguageMgr.GetTranslation("ChargeToUser.Title", new object[0]),
						Gold = 0,
						IsExist = true,
						Money = 0,
						GiftToken = 0,
						Receiver = this.PlayerCharacter.NickName,
						ReceiverID = this.PlayerCharacter.ID,
						Sender = this.PlayerCharacter.NickName,
						SenderID = this.PlayerCharacter.ID,
						Type = 1
					}))
					{
						this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
			}
		}
		public void ChargeGiftTokenToUser(int giftToken)
		{
			using (PlayerBussiness db = new PlayerBussiness())
			{
				if (giftToken > 0)
				{
					this.AddGiftToken(giftToken);
					if (db.SendMail(new MailInfo
					{
						Content = LanguageMgr.GetTranslation("ChargeGiftTokenToUser.Content", new object[]
						{
							giftToken
						}),
						Title = LanguageMgr.GetTranslation("ChargeGiftTokenToUser.Title", new object[0]),
						Gold = 0,
						IsExist = true,
						Money = 0,
						GiftToken = 0,
						Receiver = this.PlayerCharacter.NickName,
						ReceiverID = this.PlayerCharacter.ID,
						Sender = this.PlayerCharacter.NickName,
						SenderID = this.PlayerCharacter.ID,
						Type = 1
					}))
					{
						this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
			}
		}
		public bool LoadFromDatabase()
		{
			bool result;
            if (m_character != null && m_character.IsDirty)
            {
                this.SaveIntoDatabase();
            }
			using (PlayerBussiness db = new PlayerBussiness())
			{
				PlayerInfo detail = db.GetUserSingleByUserID(this.m_character.ID);
				if (detail == null)
				{
					this.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid", new object[0]));
					this.Client.Disconnect();
					result = false;
				}
				else
				{
                    this.m_character = null;
					this.m_character = detail;
					int[] sole = new int[]
					{
						0,
						1,
						2
					};
					this.Out.SendUpdateInventorySlot(this.FightBag, sole);
					ItemInfo[] items = db.GetUserItem(this.PlayerCharacter.ID);
					this.m_mainBag.LoadItems(items);
					this.m_propBag.LoadItems(items);
					this.m_storeBag.LoadItems(items);
					this.m_hideBag.LoadItems(items);
					this.ClearHideBag();
					this.ClearConsortia(false);
					this.m_questInventory.LoadFromDatabase(this.m_character.ID);
					this.m_achievementInventory.LoadFromDatabase(this.m_character.ID);
					this.m_bufferList.LoadFromDatabase(this.m_character.ID);
					this.AddGP(0);
					this.UpdateWeapon(this.m_mainBag.GetItemAt(6));
					this.UpdateSecondWeapon(this.m_mainBag.GetItemAt(15));
                    this.m_ChargedMoney = detail.ChargedMoney;
                    this.m_character.VipLevel = this.VIPLevel;

                    
                    if (string.IsNullOrEmpty(detail.PvePermission))
					{
						this.m_pvepermissions = this.InitPvePermission();
						detail.PvePermission = new string(this.m_pvepermissions).ToString();
					}
					else
					{
						this.m_pvepermissions = detail.PvePermission.ToCharArray();
					}
					this.m_fightlabpermissions = (string.IsNullOrEmpty(detail.FightLabPermission) ? this.InitFightLabPermission() : detail.FightLabPermission.ToCharArray());
					this._friends = new Dictionary<int, int>();
					this._friends = db.GetFriendsIDAll(this.m_character.ID);
					this.m_character.State = 1;
                    VIPMgr.CheckReward(this);
					db.UpdatePlayer(this.m_character);
					this.m_styleChanged = false;
                    
					result = true;
				}
			}
			return result;
		}
		public bool SaveIntoDatabase()
		{
			bool result;
			try
			{
				if (this.m_character.IsDirty)
				{
					this.m_styleChanged = false;
					using (PlayerBussiness db = new PlayerBussiness())
					{
						db.UpdatePlayer(this.m_character);
					}
				}
				this.MainBag.SaveToDatabase();
				this.PropBag.SaveToDatabase();
				this.StoreBag.SaveToDatabase();
				this.HideBag.SaveToDatabase();
				this.QuestInventory.SaveToDatabase();
				this.AchievementInventory.SaveToDatabase();
				this.BufferList.SaveToDatabase();
				result = true;
			}
			catch (Exception e)
			{
				GamePlayer.log.Error("Error saving player " + this.m_character.NickName + "!", e);
				result = false;
			}
			return result;
		}
		public virtual bool Quit()
		{
			try
			{
				try
				{
					if (this.CurrentRoom != null)
					{
						this.CurrentRoom.RemovePlayerUnsafe(this);
						this.CurrentRoom = null;
					}
					else
					{
						RoomMgr.WaitingRoom.RemovePlayer(this);
					}
					if (this._currentMarryRoom != null)
					{
						this._currentMarryRoom.RemovePlayer(this);
						this._currentMarryRoom = null;
					}
					if (this._currentSpaRoom != null)
					{
						this._currentSpaRoom.RemovePlayer(this);
						this._currentSpaRoom = null;
					}
				}
				catch (Exception ex)
				{
					GamePlayer.log.Error("Player exit Game Error!", ex);
				}
				this.m_character.State = 0;
				this.m_playerState = ePlayerState.Exited;
				this.SaveIntoDatabase();
			}
			catch (Exception ex)
			{
				GamePlayer.log.Error("Player exit Error!!!", ex);
			}
			finally
			{
				WorldMgr.RemovePlayer(this.m_character.ID);
			}
			return true;
		}
		public void FriendsAdd(int playerID, int relation)
		{
			if (!this._friends.ContainsKey(playerID))
			{
				this._friends.Add(playerID, relation);
			}
			else
			{
				this._friends[playerID] = relation;
			}
		}
		public void FriendsRemove(int playerID)
		{
			if (this._friends.ContainsKey(playerID))
			{
				this._friends.Remove(playerID);
			}
		}
		public bool IsBlackFriend(int playerID)
		{
			return this._friends == null || (this._friends.ContainsKey(playerID) && this._friends[playerID] == 1);
		}
		public void ClearConsortia(bool isclear)
		{
			if (isclear)
			{
				this.PlayerCharacter.ClearConsortia();
			}
			if (this.PlayerCharacter.ConsortiaID == 0 && this.StoreBag.GetItems().Count > 0)
			{
				this.OnPropertiesChanged();
				this.QuestInventory.ClearConsortiaQuest();
				string sender = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender", new object[0]);
				string title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]);
				if (this.StoreBag.SendAllItemsToMail(sender, title, eMailType.StoreCanel))
				{
					this.Out.SendConsortiaMailMessage(this);
				}
			}
		}
		public double GetBaseAgility()
		{
			return 1.0 - (double)this.m_character.Agility * 0.001;
		}
		public void AddProperty(ItemInfo item, ref double defence, ref double attack)
		{
			if (item.Hole1 > 0)
			{
				this.Base(item.Hole1, ref defence, ref attack);
			}
			if (item.Hole2 > 0)
			{
				this.Base(item.Hole2, ref defence, ref attack);
			}
			if (item.Hole3 > 0)
			{
				this.Base(item.Hole3, ref defence, ref attack);
			}
			if (item.Hole4 > 0)
			{
				this.Base(item.Hole4, ref defence, ref attack);
			}
			if (item.Hole5 > 0)
			{
				this.Base(item.Hole5, ref defence, ref attack);
			}
			if (item.Hole6 > 0)
			{
				this.Base(item.Hole6, ref defence, ref attack);
			}
		}
		public void Base(int template, ref double defence, ref double attack)
		{
			ItemTemplateInfo temp = ItemMgr.FindItemTemplate(template);
			if (temp != null)
			{
				if (temp.CategoryID == 11 && temp.Property1 == 31 && temp.Property2 == 3)
				{
					defence += (double)temp.Property8;
					attack += (double)temp.Property7;
				}
			}
		}
		public double GetBaseAttack()
		{
			double baseattack = 0.0;
			double basedefence = 0.0;
			ItemInfo weapon = this.m_mainBag.GetItemAt(6);
			if (weapon != null)
			{
				baseattack += (double)weapon.Template.Property7 * Math.Pow(1.05, (double)weapon.StrengthenLevel);
			}
            for (int i = 0; i < 31; i++)
            {
                var a = m_mainBag.GetItemAt(i);
                if (a != null)
                {
                    this.AddProperty(a, ref basedefence, ref baseattack);
                    if (i >= 17 && i <= 26)
                    {
                        if (a.Template.CategoryID == 19||a.Template.CategoryID == 18)
                        {
                            baseattack += (double)a.Template.Property7 * Math.Pow(1.1, a.StrengthenLevel);
                        }
                    }
                }
            }
            return baseattack;
		}
		public double GetBaseDefence()
		{
			double basedefence = 0.0;
			double baseattack = 0.0;
			ItemInfo head = this.m_mainBag.GetItemAt(0);
			if (head != null)
			{
				basedefence += (double)((int)((double)head.Template.Property7 * Math.Pow(1.05, (double)head.StrengthenLevel)));
			}
			ItemInfo cloth = this.m_mainBag.GetItemAt(4);
			if (cloth != null)
			{
				this.AddProperty(cloth, ref basedefence, ref baseattack);
				basedefence += (double)((int)((double)cloth.Template.Property7 * Math.Pow(1.05, (double)cloth.StrengthenLevel)));
			}
            for (int i = 0; i < 31; i++)
            {
                var a = m_mainBag.GetItemAt(i);
                if (a != null)
                {
                    this.AddProperty(a, ref basedefence, ref baseattack);
                    if (i >= 17 && i <= 26)
                    {
                        if (a.Template.CategoryID == 20)
                        {
                            basedefence += (double)a.Template.Property7 * Math.Pow(1.1, a.StrengthenLevel);
                        }
                    }
                }
            }
            return basedefence;
		}
		public double GetBaseBlood()
		{
			ItemInfo info = this.MainBag.GetItemAt(12);
			return (info == null) ? 1.0 : ((100.0 + (double)info.Template.Property1) / 100.0);
		}
		public void SendItemNotice(ItemInfo info, string name, int type)
		{
			GSPacketIn pkg = new GSPacketIn(14);
			pkg.WriteString(this.PlayerCharacter.NickName);
			if (this.CurrentRoom != null && this.CurrentRoom.Game != null)
			{
				if (this.CurrentRoom.Game.RoomType == eRoomType.Match)
				{
					pkg.WriteInt(0);
				}
				else
				{
					pkg.WriteInt(1);
				}
			}
			else
			{
				pkg.WriteInt(2);
			}
			pkg.WriteInt(info.TemplateID);
			pkg.WriteBoolean(info.IsBinds);
			if (info.IsTips)
			{
				pkg.WriteInt(type);
				if (name != null)
				{
					pkg.WriteString(name);
				}
				GameServer.Instance.LoginServer.SendPacket(pkg);
				GamePlayer[] players = WorldMgr.GetAllPlayers();
				GamePlayer[] array = players;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p != this)
					{
						p.Out.SendTCP(pkg);
					}
				}
			}
			else
			{
				if (this.CurrentRoom != null && this.CurrentRoom.Game != null)
				{
					if (this.CurrentRoom.Game is BaseGame && (this.CurrentRoom.Game as BaseGame).GameState != eGameState.SessionPrepared)
					{
						if (info.Template.Quality >= 3)
						{
							pkg.WriteInt(2);
							if (name != null)
							{
								pkg.WriteString(name);
							}
							if (this.CurrentRoom != null)
							{
								this.CurrentRoom.SendToTeam(pkg, this.CurrentRoomTeam, this);
							}
						}
					}
				}
			}
		}
		public PlayerInventory FindItemByID(int itemid)
		{
			List<ItemInfo> items = this.MainBag.GetItems();
			PlayerInventory result;
			foreach (ItemInfo info in items)
			{
				if (info.ItemID == itemid && info.UserID == this.PlayerCharacter.ID)
				{
					result = this.MainBag;
					return result;
				}
			}
			List<ItemInfo> items2 = this.PropBag.GetItems();
			foreach (ItemInfo info in items2)
			{
				if (info.ItemID == itemid && info.UserID == this.PlayerCharacter.ID)
				{
					result = this.PropBag;
					return result;
				}
			}
			result = null;
			return result;
		}
		public bool StackItem(ref List<ItemInfo> items)
		{
			List<ItemInfo> removeitem = new List<ItemInfo>();
			for (int i = 0; i < items.Count; i++)
			{
				for (int j = i + 1; j < items.Count; j++)
				{
					if (items[i] != null && items[j] != null && items[i].CanStackedTo(items[j]) && items[j].Count + items[i].Count <= items[i].Template.MaxCount)
					{
						items[j].Count += items[i].Count;
						removeitem.Add(items[i]);
						break;
					}
				}
			}
			foreach (ItemInfo info in removeitem)
			{
				items.Remove(info);
			}
			return true;
		}
		public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
		{
			bool result;
			if (bag == 1)
			{
				ItemTemplateInfo template = PropItemMgr.FindFightingProp(templateId);
				if (isLiving && template != null)
				{
					this.OnUsingItem(template.TemplateID);
					if (place == -1 && this.CanUseProp)
					{
						result = true;
						return result;
					}
					ItemInfo item = this.m_propBag.GetItemAt(place);
					if (item != null && item.IsValidItem() && item.Count >= 0)
					{
						this.m_propBag.RemoveCountFromStack(item, 1, eItemRemoveType.Use);
						result = true;
						return result;
					}
				}
			}
			else
			{
				ItemInfo item = this.m_fightBag.GetItemAt(place);
				if (item != null)
				{
					this.OnUsingItem(item.TemplateID);
					if (item.TemplateID == templateId)
					{
						result = this.m_fightBag.RemoveItem(item, eItemRemoveType.Use);
						return result;
					}
				}
			}
			result = false;
			return result;
		}
		public void Disconnect()
		{
			this.m_client.Disconnect();
		}
		public void SendTCP(GSPacketIn pkg)
		{
			if (this.m_client.IsConnected)
			{
				this.m_client.SendTCP(pkg);
			}
		}
		public void LoadMarryProp()
		{
			using (PlayerBussiness db = new PlayerBussiness())
			{
				MarryProp info = db.GetMarryProp(this.PlayerCharacter.ID);
				this.PlayerCharacter.IsMarried = info.IsMarried;
				this.PlayerCharacter.SpouseID = info.SpouseID;
				this.PlayerCharacter.SpouseName = info.SpouseName;
				this.PlayerCharacter.IsCreatedMarryRoom = info.IsCreatedMarryRoom;
				this.PlayerCharacter.SelfMarryRoomID = info.SelfMarryRoomID;
				this.PlayerCharacter.IsGotRing = info.IsGotRing;
				this.Out.SendMarryProp(this, info);
			}
		}
		public override string ToString()
		{
			return string.Format("Id:{0} nickname:{1} room:{2} ", this.PlayerId, this.PlayerCharacter.NickName, this.CurrentRoom);
		}
		public int ConsortiaFight(int consortiaWin, int consortiaLose, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
		{
			return ConsortiaMgr.ConsortiaFight(this, consortiaWin, consortiaLose, roomType, gameClass, totalKillHealth, count);
		}
		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(158);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(riches);
			pkg.WriteString(msg);
			GameServer.Instance.LoginServer.SendPacket(pkg);
		}
		public void SendMessage(eMessageType type, string message)
		{
			this.Out.SendMessage(type, message);
		}
		public int[] PvePermissionInt()
		{
			int[] pp = new int[50];
			for (int i = 0; i < 50; i++)
			{
				pp[i] = (int)this.m_pvepermissions[i];
			}
			return pp;
		}
		public char[] InitPvePermission()
		{
			char[] tempByte = new char[50];
			for (int i = 0; i < 50; i++)
			{
				if (i < 2)
				{
					tempByte[i] = '1';
				}
				else
				{
					tempByte[i] = '3';
				}
			}
			return tempByte;
		}
		public bool SetPvePermission(int copyId, eHardLevel hardLevel)
		{
			bool result;
			if (copyId > this.m_pvepermissions.Length || copyId <= 0)
			{
				result = true;
			}
			else
			{
				if (hardLevel == eHardLevel.Terror || this.m_pvepermissions[copyId - 1] != GamePlayer.permissionChars[(int)hardLevel])
				{
					result = true;
				}
				else
				{
					this.m_pvepermissions[copyId - 1] = GamePlayer.permissionChars[(int)(hardLevel + 1)];
					this.m_character.PvePermission = new string(this.m_pvepermissions).ToString();
					this.OnPropertiesChanged();
					result = true;
				}
			}
			return result;
		}
		public bool IsPvePermission(int copyId, eHardLevel hardLevel)
		{
			bool result;
			if (copyId > this.m_pvepermissions.Length || copyId <= 0)
			{
				result = true;
			}
			else
			{
				char temp = this.m_pvepermissions[copyId - 1];
				result = (temp >= GamePlayer.permissionChars[(int)hardLevel]);
			}
			return result;
		}
		public eHardLevel GetMaxPvePermission(int copyId)
		{
			eHardLevel result;
			if (copyId > this.m_pvepermissions.Length)
			{
				result = eHardLevel.Simple;
			}
			else
			{
				char temp = this.m_pvepermissions[copyId - 1];
				if (temp == 'F')
				{
					result = eHardLevel.Terror;
				}
				else
				{
					if (temp == '7')
					{
						result = eHardLevel.Hard;
					}
					else
					{
						if (temp == '3')
						{
							result = eHardLevel.Normal;
						}
						else
						{
							result = eHardLevel.Simple;
						}
					}
				}
			}
			return result;
		}
		public int[] FightLabPermissionInt()
		{
			int[] pp = new int[50];
			for (int i = 0; i < 50; i++)
			{
				pp[i] = (int)this.m_fightlabpermissions[i];
			}
			return pp;
		}
		public char[] InitFightLabPermission()
		{
			char[] tempByte = new char[50];
			for (int i = 0; i < 50; i++)
			{
				if (i == 0)
				{
					tempByte[i] = '1';
				}
				else
				{
					tempByte[i] = '0';
				}
			}
			return tempByte;
		}
		public bool SetFightLabPermission(int copyId, eHardLevel hardLevel, int missionId)
		{
			bool result;
			if (copyId > this.m_fightlabpermissions.Length || copyId <= 0)
			{
				result = true;
			}
			else
			{
				int ID = (copyId - 5) * 2;
				if (this.m_fightlabpermissions[ID] != GamePlayer.fightlabpermissionChars[(int)(hardLevel + 1)])
				{
					result = true;
				}
				else
				{
					if (this.m_fightlabpermissions[ID + 1] <= '2' && this.m_fightlabpermissions[ID] - this.m_fightlabpermissions[ID + 1] == '\u0001')
					{
						this.m_fightlabpermissions[ID + 1] = this.m_fightlabpermissions[ID];
						string msg = "";
						int gold = 0;
						int money = 0;
						int giftToken = 0;
						int gp = 0;
						List<ItemInfo> infos = new List<ItemInfo>();
						if (DropInventory.FightLabUserDrop(missionId, ref infos))
						{
							if (infos != null)
							{
								bool confine = false;
								msg = LanguageMgr.GetTranslation("OpenUpArkHandler.FightLabStart", new object[0]) + ": ";
								foreach (ItemInfo info in infos)
								{
									msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", new object[]
									{
										info.Template.Name,
										info.Count
									}) + " ";
									if (infos.Count > 0 && this.PropBag.GetEmptyCount() < 1)
									{
										if (info.TemplateID != 11107 && info.TemplateID != -100 && info.TemplateID != -200 && info.TemplateID != -300)
										{
											string str = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content2", new object[0]);
											string str2 = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title2", new object[0]);
											if (this.SendItemsToMail(new List<ItemInfo>
											{
												info
											}, str, str2, eMailType.ItemOverdue))
											{
												this.Out.SendMailResponse(this.PlayerCharacter.ID, eMailRespose.Receiver);
											}
											confine = true;
										}
									}
									else
									{
										if (!this.PropBag.StackItemToAnother(info))
										{
											if (info.TemplateID != 11107 && info.TemplateID != -100 && info.TemplateID != -200 && info.TemplateID != -300)
											{
												this.PropBag.AddItem(info);
											}
										}
									}
									ItemInfo tempInfo = ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref gp);
								}
								this.AddGold(gold);
								this.AddMoney(money, LogMoneyType.Award, LogMoneyType.Award_Drop);
								this.AddGiftToken(giftToken);
								this.AddGpDirect(gp);
								if (confine)
								{
									msg += LanguageMgr.GetTranslation("Game.Server.GameUtils.Title2", new object[0]);
								}
								this.Out.SendMessage(eMessageType.Normal, msg);
							}
						}
					}
					if (copyId == 5 && hardLevel == eHardLevel.Normal)
					{
						if (this.m_fightlabpermissions[2] == '0')
						{
							this.m_fightlabpermissions[2] = '1';
						}
						if (this.m_fightlabpermissions[4] == '0')
						{
							this.m_fightlabpermissions[4] = '1';
						}
						if (this.m_fightlabpermissions[6] == '0')
						{
							this.m_fightlabpermissions[6] = '1';
						}
					}
					if ((copyId == 7 || copyId == 8) && hardLevel == eHardLevel.Hard)
					{
						if (this.m_fightlabpermissions[8] == '0')
						{
							this.m_fightlabpermissions[8] = '1';
						}
					}
					if (hardLevel < eHardLevel.Hard && this.m_fightlabpermissions[ID] < GamePlayer.fightlabpermissionChars[(int)(hardLevel + 2)])
					{
						this.m_fightlabpermissions[ID] = GamePlayer.fightlabpermissionChars[(int)(hardLevel + 2)];
					}
					this.m_character.FightLabPermission = new string(this.m_fightlabpermissions).ToString();
					this.OnPropertiesChanged();
					result = true;
				}
			}
			return result;
		}
		public bool IsFightLabPermission(int copyId, eHardLevel hardLevel)
		{
			bool result;
			if (copyId > this.m_fightlabpermissions.Length || copyId <= 0)
			{
				result = true;
			}
			else
			{
				int ID = (copyId - 5) * 2;
				char temp = this.m_fightlabpermissions[ID];
				result = (temp >= GamePlayer.fightlabpermissionChars[(int)(hardLevel + 1)]);
			}
			return result;
		}
		public eHardLevel GetMaxFightLabPermission(int copyId)
		{
			eHardLevel result;
			if (copyId > this.m_fightlabpermissions.Length)
			{
				result = eHardLevel.Simple;
			}
			else
			{
				char temp = this.m_fightlabpermissions[copyId - 5];
				if (temp == '3')
				{
					result = eHardLevel.Hard;
				}
				else
				{
					if (temp == '2')
					{
						result = eHardLevel.Normal;
					}
					else
					{
						result = eHardLevel.Simple;
					}
				}
			}
			return result;
		}
		public void UpdateAnswerSite(int id)
		{
			if (this.PlayerCharacter.AnswerSite < id)
			{
				List<ItemInfo> infos = null;
				this.PlayerCharacter.AnswerSite = id;
				if (DropInventory.AnswerDrop(id, ref infos))
				{
					if (infos != null)
					{
						int gold = 0;
						int money = 0;
						int giftToken = 0;
						string msg = "";
						foreach (ItemInfo info in infos)
						{
							ItemInfo tempInfo = ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
							if (tempInfo != null)
							{
								this.AddItem(tempInfo);
								msg = msg + LanguageMgr.GetTranslation("Game.Server.GameObjects.AnswerHandler.RewardPro", new object[]
								{
									tempInfo.Template.Name,
									info.Count
								}) + " ";
							}
							if (gold > 0)
							{
								this.AddGold(gold);
								msg = msg + LanguageMgr.GetTranslation("Game.Server.GameObjects.AnswerHandler.RewardGold", new object[]
								{
									gold
								}) + " ";
							}
							if (money > 0)
							{
								this.AddMoney(money, LogMoneyType.Award, LogMoneyType.Award_Answer);
								msg = msg + LanguageMgr.GetTranslation("Game.Server.GameObjects.AnswerHandler.RewardPoint", new object[]
								{
									money
								}) + " ";
							}
							if (giftToken > 0)
							{
								this.AddGiftToken(giftToken);
								msg = msg + LanguageMgr.GetTranslation("Game.Server.GameObjects.AnswerHandler.RewardGiftToken", new object[]
								{
									giftToken
								}) + " ";
							}
						}
						if (msg != "")
						{
							msg = LanguageMgr.GetTranslation("Game.Server.GameObjects.AnswerHandler.RewardGet", new object[]
							{
								""
							}) + msg;
							this.Out.SendMessage(eMessageType.Normal, msg);
						}
					}
				}
			}
			GSPacketIn pkg = new GSPacketIn(15);
			pkg.WriteInt(this.PlayerCharacter.AnswerSite);
			this.SendTCP(pkg);
		}
		public void SendInsufficientMoney(int type)
		{
			GSPacketIn pkg = new GSPacketIn(88, this.PlayerId);
			pkg.WriteByte((byte)type);
			pkg.WriteBoolean(false);
			this.SendTCP(pkg);
		}
		public void SendMessage(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(0);
			pkg.WriteString(msg);
			this.SendTCP(pkg);
		}
		public bool SendMail(string content, string title, string Sender, eMailType type)
		{
			MailInfo mail = new MailInfo();
			mail.Gold = 0;
			mail.Money = 0;
			mail.GiftToken = 0;
			mail.Receiver = this.PlayerCharacter.NickName;
			mail.ReceiverID = this.PlayerCharacter.ID;
			mail.Sender = ((Sender == null) ? mail.Receiver : Sender);
			mail.SenderID = mail.ReceiverID;
			mail.Title = title;
			mail.Type = (int)type;
			mail.Content = content;
			bool result;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				result = db.SendMail(mail);
			}
			return result;
		}
		public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type)
		{
			bool result;
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				result = this.SendItemsToMail(items, content, title, type, pb);
			}
			return result;
		}
		public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type, PlayerBussiness pb)
		{
			bool result = true;
			for (int i = 0; i < items.Count; i += 5)
			{
				MailInfo mail = new MailInfo();
				mail.Title = ((title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]));
				mail.Gold = 0;
				mail.IsExist = true;
				mail.Money = 0;
				mail.Receiver = this.PlayerCharacter.NickName;
				mail.ReceiverID = this.PlayerId;
				mail.Sender = this.PlayerCharacter.NickName;
				mail.SenderID = this.PlayerId;
				mail.Type = (int)type;
				mail.GiftToken = 0;
				List<ItemInfo> sent = new List<ItemInfo>();
				StringBuilder annexRemark = new StringBuilder();
				StringBuilder mailContent = new StringBuilder();
				annexRemark.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark", new object[0]));
				content = ((content != null) ? LanguageMgr.GetTranslation(content, new object[0]) : "");
				int index = i;
				if (items.Count > index)
				{
					ItemInfo it = items[index];
					if (it.ItemID == 0)
					{
						pb.AddGoods(it);
					}
					mail.Annex1 = it.ItemID.ToString();
					mail.Annex1Name = it.Template.Name;
					mail.Title = it.Template.Name;
					annexRemark.Append(string.Concat(new object[]
					{
						"1、",
						mail.Annex1Name,
						"x",
						it.Count,
						";"
					}));
					mailContent.Append(string.Concat(new object[]
					{
						"1、",
						mail.Annex1Name,
						"x",
						it.Count,
						";"
					}));
					sent.Add(it);
				}
				index = i + 1;
				if (items.Count > index)
				{
					ItemInfo it = items[index];
					if (it.ItemID == 0)
					{
						pb.AddGoods(it);
					}
					mail.Annex2 = it.ItemID.ToString();
					mail.Annex2Name = it.Template.Name;
					annexRemark.Append(string.Concat(new object[]
					{
						"2、",
						mail.Annex2Name,
						"x",
						it.Count,
						";"
					}));
					mailContent.Append(string.Concat(new object[]
					{
						"2、",
						mail.Annex2Name,
						"x",
						it.Count,
						";"
					}));
					sent.Add(it);
				}
				index = i + 2;
				if (items.Count > index)
				{
					ItemInfo it = items[index];
					if (it.ItemID == 0)
					{
						pb.AddGoods(it);
					}
					mail.Annex3 = it.ItemID.ToString();
					mail.Annex3Name = it.Template.Name;
					annexRemark.Append(string.Concat(new object[]
					{
						"3、",
						mail.Annex3Name,
						"x",
						it.Count,
						";"
					}));
					mailContent.Append(string.Concat(new object[]
					{
						"3、",
						mail.Annex3Name,
						"x",
						it.Count,
						";"
					}));
					sent.Add(it);
				}
				index = i + 3;
				if (items.Count > index)
				{
					ItemInfo it = items[index];
					if (it.ItemID == 0)
					{
						pb.AddGoods(it);
					}
					mail.Annex4 = it.ItemID.ToString();
					mail.Annex4Name = it.Template.Name;
					annexRemark.Append(string.Concat(new object[]
					{
						"4、",
						mail.Annex4Name,
						"x",
						it.Count,
						";"
					}));
					mailContent.Append(string.Concat(new object[]
					{
						"4、",
						mail.Annex4Name,
						"x",
						it.Count,
						";"
					}));
					sent.Add(it);
				}
				index = i + 4;
				if (items.Count > index)
				{
					ItemInfo it = items[index];
					if (it.ItemID == 0)
					{
						pb.AddGoods(it);
					}
					mail.Annex5 = it.ItemID.ToString();
					mail.Annex5Name = it.Template.Name;
					annexRemark.Append(string.Concat(new object[]
					{
						"5、",
						mail.Annex5Name,
						"x",
						it.Count,
						";"
					}));
					mailContent.Append(string.Concat(new object[]
					{
						"5、",
						mail.Annex5Name,
						"x",
						it.Count,
						";"
					}));
					sent.Add(it);
				}
				mail.AnnexRemark = annexRemark.ToString();
				if (content == null && mailContent.ToString() == null)
				{
					mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content", new object[0]);
				}
				else
				{
					if (content != "")
					{
						mail.Content = content;
					}
					else
					{
						mail.Content = mailContent.ToString();
					}
				}
				if (pb.SendMail(mail))
				{
					foreach (ItemInfo it in sent)
					{
						this.TakeOutItem(it);
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
		public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, string content, string title, eMailType type)
		{
			MailInfo mail = new MailInfo();
			mail.Content = ((content != null) ? content : LanguageMgr.GetTranslation("Game.Server.GameUtils.Content", new object[0]));
			mail.Title = ((title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title", new object[0]));
			mail.Gold = 0;
			mail.IsExist = true;
			mail.Money = 0;
			mail.GiftToken = 0;
			mail.Receiver = this.PlayerCharacter.NickName;
			mail.ReceiverID = this.PlayerCharacter.ID;
			mail.Sender = this.PlayerCharacter.NickName;
			mail.SenderID = this.PlayerCharacter.ID;
			mail.Type = (int)type;
			if (item.ItemID == 0)
			{
				pb.AddGoods(item);
			}
			mail.Annex1 = item.ItemID.ToString();
			mail.Annex1Name = item.Template.Name;
			bool result;
			if (pb.SendMail(mail))
			{
				this.TakeOutItem(item);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public bool SendItemToMail(ItemInfo item, string content, string title, eMailType type)
		{
			bool result;
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				result = this.SendItemToMail(item, pb, content, title, type);
			}
			return result;
		}
		public void OldPlayerGetBack()
		{
			StringBuilder msg = new StringBuilder();
			StringBuilder content = new StringBuilder();
			List<ActiveAwardInfo> activeAwardInfo = ActiveMgr.GetAwardInfo(this.PlayerCharacter.LastDate, this.PlayerCharacter.Grade);
			List<ItemInfo> mailItems = new List<ItemInfo>();
			foreach (ActiveAwardInfo info in activeAwardInfo)
			{
				int gold = 0;
				int money = 0;
				int gp = 0;
				int giftToken = 0;
				ItemTemplateInfo temp = ItemMgr.FindItemTemplate(info.ItemID);
				ItemInfo item = ItemInfo.CreateFromTemplate(temp, 1, 101);
				ItemInfo.FindSpecialItemInfo(item, ref gold, ref money, ref giftToken, ref gp);
				if (item != null)
				{
					if (gold != 0 || money != 0 || gp != 0 || giftToken != 0)
					{
						if (gold > 0)
						{
							this.AddGold(gold);
						}
						if (money > 0)
						{
							this.AddMoney(money, LogMoneyType.Award, LogMoneyType.Award);
						}
						if (gp > 0)
						{
							this.AddGpDirect(gp);
						}
						if (giftToken > 0)
						{
							this.AddGiftToken(giftToken);
						}
						msg.Append(item.Template.Name).Append(info.Count);
					}
					else
					{
						for (int i = 0; i < info.Count; i++)
						{
							ItemInfo award = item.Clone();
							award.ValidDate = info.ValidDate;
							award.StrengthenLevel = info.StrengthenLevel;
							award.LuckCompose = info.LuckCompose;
							award.DefendCompose = info.DefendCompose;
							award.AgilityCompose = info.AgilityCompose;
							award.AttackCompose = info.AttackCompose;
							award.IsBinds = true;
							if (!this.AddItem(award))
							{
								mailItems.Add(award);
							}
							msg.Append(item.Template.Name).Append("X").Append(info.Count);
							content.Append(award.Template.Name).Append("X").Append(award.Count);
						}
					}
				}
			}
			if (mailItems.Count > 0)
			{
				this.SendItemsToMail(mailItems, LanguageMgr.GetTranslation("ActiveGome.Content", new object[]
				{
					content.ToString()
				}), LanguageMgr.GetTranslation("ActiveGome.Title", new object[0]), eMailType.Active);
			}
			if (activeAwardInfo.Count != 0)
			{
				this.SendMessage(eMessageType.ALERT, LanguageMgr.GetTranslation("ActiveAward.GoHome", new object[]
				{
					msg
				}));
			}
		}
        public void UpdateVIP()
        {
            this.m_character.VipLevel = this.VIPLevel;
            this.LoadFromDatabase();          
            this.Out.SendUpdateVIP(this);
        }
	}
}
