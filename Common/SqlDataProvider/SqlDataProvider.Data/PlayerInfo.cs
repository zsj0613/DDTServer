using System;
namespace SqlDataProvider.Data
{
	public class PlayerInfo : DataObject
	{
		private int _id;
		private string _userName;
		private string _nickName;
		private bool _sex;
		private int _attack;
		private int _defence;
		private int _luck;
		private int _agility;
		private int _gold;
		private int _money;
		private string _style;
		private string _colors;
		private int _hide;
		private int _grade;
		private int _gp;
		private int _state;
		private int _consortiaID;
		private int _repute;
		private DateTime? _expendDate;
		private int _offer;
		private string _consortiaName;
		private int _win;
		private int _total;
		private int _escape;
		private string _skin;
		private bool _isConsortia;
		private int _antiAddiction;
		private DateTime _antiDate;
		private int _richesOffer;
		private int _richesRob;
		private int _checkCount;
		private string _checkCode;
		private int _checkError;
		private DateTime _checkDate;
		private bool _isMarried;
		private int _spouseID;
		private string _spouseName;
		private int _marryInfoID;
		private bool _isLocked;
		private string _PasswordTwo;
		private int _dayLoginCount;
		private bool _isCreatedMarryRoom;
		private int _selfMarryRoomID;
		private bool _isGotRing;
		private bool _rename;
		private bool _consortiaRename;
		private int _nimbus;
		private int _fightPower;
		private int _IsFirst;
		private int _GiftToken;
		private DateTime _LastAward;
		private DateTime _LastAuncherAward;
		private byte[] _QuestSite;
		private DateTime m_lastDate;
		private string m_pvePermission;
		private string m_fightlabPermission;
		private string m_PasswordQuest1;
		private string m_PasswordQuest2;
		private int m_FailedPasswordAttemptCount;
		private int m_AnswerSite;
		private int m_ChatCount;
		private int m_AchievementPoint;
		private int m_Rank;
		private int m_OnlineTime;
		private int m_BoxProgression;
		private DateTime m_BoxGetDate;
		private int m_getBoxLevel;
		private int m_SpaPubGoldRoomLimit;
		private DateTime m_LastSpaDate;
		private DateTime m_AddGPLastDate;
		private int m_SpaPubMoneyRoomLimit;
		private bool m_IsInSpaPubGoldToday;
		private bool m_IsInSpaPubMoneyToday;
		private int m_AlreadyGetBox;
        private int m_VipLevel;
        private int m_ChargedMoney;
		public int ID
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
				this._isDirty = true;
			}
		}
		public string UserName
		{
			get
			{
				return this._userName;
			}
			set
			{
				this._userName = value;
				this._isDirty = true;
			}
		}
		public string NickName
		{
			get
			{
				return this._nickName;
			}
			set
			{
				this._nickName = value;
				this._isDirty = true;
			}
		}
		public bool Sex
		{
			get
			{
				return this._sex;
			}
			set
			{
				this._sex = value;
				this._isDirty = true;
			}
		}
		public int Attack
		{
			get
			{
				return this._attack;
			}
			set
			{
				this._attack = value;
				this._isDirty = true;
			}
		}
		public int Defence
		{
			get
			{
				return this._defence;
			}
			set
			{
				this._defence = value;
				this._isDirty = true;
			}
		}
		public int Luck
		{
			get
			{
				return this._luck;
			}
			set
			{
				this._luck = value;
				this._isDirty = true;
			}
		}
		public int Agility
		{
			get
			{
				return this._agility;
			}
			set
			{
				this._agility = value;
				this._isDirty = true;
			}
		}
		public int Gold
		{
			get
			{
				return this._gold;
			}
			set
			{
				this._gold = value;
				this._isDirty = true;
			}
		}
		public int Money
		{
			get
			{
				return this._money;
			}
			set
			{
				this._money = value;
				this._isDirty = true;
			}
		}
		public string Style
		{
			get
			{
				return this._style;
			}
			set
			{
				this._style = value;
				this._isDirty = true;
			}
		}
		public string Colors
		{
			get
			{
				return this._colors;
			}
			set
			{
				this._colors = value;
				this._isDirty = true;
			}
		}
		public int Hide
		{
			get
			{
				return this._hide;
			}
			set
			{
				this._hide = value;
				this._isDirty = true;
			}
		}
		public int Grade
		{
			get
			{
				return this._grade;
			}
			set
			{
				this._grade = value;
				this._isDirty = true;
			}
		}
		public int GP
		{
			get
			{
				return this._gp;
			}
			set
			{
				this._gp = value;
				this._isDirty = true;
			}
		}
		public int State
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
				this._isDirty = true;
			}
		}
		public int ConsortiaID
		{
			get
			{
				return this._consortiaID;
			}
			set
			{
				if (this._consortiaID == 0 || value == 0)
				{
					this._richesRob = 0;
					this._richesOffer = 0;
				}
				this._consortiaID = value;
			}
		}
		public int Repute
		{
			get
			{
				return this._repute;
			}
			set
			{
				this._repute = value;
				this._isDirty = true;
			}
		}
		public DateTime? ExpendDate
		{
			get
			{
				return this._expendDate;
			}
			set
			{
				this._expendDate = value;
				this._isDirty = true;
			}
		}
		public int Offer
		{
			get
			{
				return this._offer;
			}
			set
			{
				this._offer = value;
				this._isDirty = true;
			}
		}
		public string ConsortiaName
		{
			get
			{
				return this._consortiaName;
			}
			set
			{
				this._consortiaName = value;
			}
		}
		public int Win
		{
			get
			{
				return this._win;
			}
			set
			{
				this._win = value;
				this._isDirty = true;
			}
		}
		public int Total
		{
			get
			{
				return this._total;
			}
			set
			{
				this._total = value;
				this._isDirty = true;
			}
		}
		public int Escape
		{
			get
			{
				return this._escape;
			}
			set
			{
				this._escape = value;
				this._isDirty = true;
			}
		}
		public string Skin
		{
			get
			{
				return this._skin;
			}
			set
			{
				this._skin = value;
				this._isDirty = true;
			}
		}
		public bool IsConsortia
		{
			get
			{
				return this._isConsortia;
			}
			set
			{
				this._isConsortia = value;
			}
		}
		public bool IsBanChat
		{
			get;
			set;
		}
		public int ReputeOffer
		{
			get;
			set;
		}
		public int ConsortiaRepute
		{
			get;
			set;
		}
		public int ConsortiaLevel
		{
			get;
			set;
		}
		public int StoreLevel
		{
			get;
			set;
		}
		public int ShopLevel
		{
			get;
			set;
		}
		public int SmithLevel
		{
			get;
			set;
		}
		public int ConsortiaHonor
		{
			get;
			set;
		}
		public string ChairmanName
		{
			get;
			set;
		}
		public int AntiAddiction
		{
			get
			{
				return this._antiAddiction + (int)(DateTime.Now - this._antiDate).TotalMinutes;
			}
			set
			{
				this._antiAddiction = value;
				this._antiDate = DateTime.Now;
			}
		}
		public DateTime AntiDate
		{
			get
			{
				return this._antiDate;
			}
			set
			{
				this._antiDate = value;
			}
		}
		public int RichesOffer
		{
			get
			{
				return this._richesOffer;
			}
			set
			{
				this._richesOffer = value;
				this._isDirty = true;
			}
		}
		public int RichesRob
		{
			get
			{
				return this._richesRob;
			}
			set
			{
				this._richesRob = value;
				this._isDirty = true;
			}
		}
		public int DutyLevel
		{
			get;
			set;
		}
		public string DutyName
		{
			get;
			set;
		}
		public int Right
		{
			get;
			set;
		}
		public int AddDayGP
		{
			get;
			set;
		}
		public int AddWeekGP
		{
			get;
			set;
		}
		public int AddDayOffer
		{
			get;
			set;
		}
		public int AddWeekOffer
		{
			get;
			set;
		}
		public int ConsortiaRiches
		{
			get;
			set;
		}
		public int CheckCount
		{
			get
			{
				return this._checkCount;
			}
			set
			{
				if (value == 0)
				{
					this._checkCode = string.Empty;
					this._checkError = 0;
				}
				this._checkCount = value;
				this._isDirty = true;
			}
		}
		public string CheckCode
		{
			get
			{
				return this._checkCode;
			}
			set
			{
				this._checkDate = DateTime.Now;
				this._checkCode = value;
			}
		}
		public int CheckError
		{
			get
			{
				return this._checkError;
			}
			set
			{
				this._checkError = value;
			}
		}
		public DateTime CheckDate
		{
			get
			{
				return this._checkDate;
			}
		}
		public bool IsMarried
		{
			get
			{
				return this._isMarried;
			}
			set
			{
				this._isMarried = value;
				this._isDirty = true;
			}
		}
		public int SpouseID
		{
			get
			{
				return this._spouseID;
			}
			set
			{
				if (this._spouseID != value)
				{
					this._spouseID = value;
					this._isDirty = true;
				}
			}
		}
		public string SpouseName
		{
			get
			{
				return this._spouseName;
			}
			set
			{
				if (this._spouseName != value)
				{
					this._spouseName = value;
					this._isDirty = true;
				}
			}
		}
		public int MarryInfoID
		{
			get
			{
				return this._marryInfoID;
			}
			set
			{
				if (this._marryInfoID != value)
				{
					this._marryInfoID = value;
					this._isDirty = true;
				}
			}
		}
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				this._isLocked = value;
			}
		}
		public bool HasBagPassword
		{
			get
			{
				return !string.IsNullOrEmpty(this._PasswordTwo);
			}
		}
		public string PasswordTwo
		{
			get
			{
				return this._PasswordTwo;
			}
			set
			{
				this._PasswordTwo = value;
				this._isDirty = true;
			}
		}
		public int DayLoginCount
		{
			get
			{
				return this._dayLoginCount;
			}
			set
			{
				this._dayLoginCount = value;
				this._isDirty = true;
			}
		}
		public bool IsCreatedMarryRoom
		{
			get
			{
				return this._isCreatedMarryRoom;
			}
			set
			{
				if (this._isCreatedMarryRoom != value)
				{
					this._isCreatedMarryRoom = value;
					this._isDirty = true;
				}
			}
		}
		public int Riches
		{
			get
			{
				return this.RichesRob + this.RichesOffer;
			}
		}
		public int SelfMarryRoomID
		{
			get
			{
				return this._selfMarryRoomID;
			}
			set
			{
				if (this._selfMarryRoomID != value)
				{
					this._selfMarryRoomID = value;
					this._isDirty = true;
				}
			}
		}
		public bool IsGotRing
		{
			get
			{
				return this._isGotRing;
			}
			set
			{
				if (this._isGotRing != value)
				{
					this._isGotRing = value;
					this._isDirty = true;
				}
			}
		}
		public bool Rename
		{
			get
			{
				return this._rename;
			}
			set
			{
				if (this._rename != value)
				{
					this._rename = value;
					this._isDirty = true;
				}
			}
		}
		public bool ConsortiaRename
		{
			get
			{
				return this._consortiaRename;
			}
			set
			{
				if (this._consortiaRename != value)
				{
					this._consortiaRename = value;
					this._isDirty = true;
				}
			}
		}
		public int Nimbus
		{
			get
			{
				return this._nimbus;
			}
			set
			{
				if (this._nimbus != value)
				{
					this._nimbus = value;
					this._isDirty = true;
				}
			}
		}
		public int FightPower
		{
			get
			{
				return this._fightPower;
			}
			set
			{
				if (this._fightPower != value)
				{
					this._fightPower = value;
					this._isDirty = true;
				}
			}
		}
		public int IsFirst
		{
			get
			{
				return this._IsFirst;
			}
			set
			{
				this._IsFirst = value;
			}
		}
		public int GiftToken
		{
			get
			{
				return this._GiftToken;
			}
			set
			{
				this._GiftToken = value;
			}
		}
		public DateTime LastAward
		{
			get
			{
				return this._LastAward;
			}
			set
			{
				this._LastAward = value;
			}
		}
		public DateTime LastAuncherAward
		{
			get
			{
				return this._LastAuncherAward;
			}
			set
			{
				this._LastAuncherAward = value;
			}
		}
		public byte[] QuestSite
		{
			get
			{
				return this._QuestSite;
			}
			set
			{
				this._QuestSite = value;
			}
		}
		public DateTime LastDate
		{
			get
			{
				return this.m_lastDate;
			}
			set
			{
				this.m_lastDate = value;
			}
		}
		public string PvePermission
		{
			get
			{
				return this.m_pvePermission;
			}
			set
			{
				this.m_pvePermission = value;
			}
		}
		public string FightLabPermission
		{
			get
			{
				return this.m_fightlabPermission;
			}
			set
			{
				this.m_fightlabPermission = value;
			}
		}
		public string PasswordQuest1
		{
			get
			{
				return this.m_PasswordQuest1;
			}
			set
			{
				this.m_PasswordQuest1 = value;
			}
		}
		public string PasswordQuest2
		{
			get
			{
				return this.m_PasswordQuest2;
			}
			set
			{
				this.m_PasswordQuest2 = value;
			}
		}
		public int FailedPasswordAttemptCount
		{
			get
			{
				return this.m_FailedPasswordAttemptCount;
			}
			set
			{
				this.m_FailedPasswordAttemptCount = value;
			}
		}
		public int AnswerSite
		{
			get
			{
				return this.m_AnswerSite;
			}
			set
			{
				this.m_AnswerSite = value;
			}
		}
		public int ChatCount
		{
			get
			{
				return this.m_ChatCount;
			}
			set
			{
				this.m_ChatCount = value;
			}
		}
		public int AchievementPoint
		{
			get
			{
				return this.m_AchievementPoint;
			}
			set
			{
				this.m_AchievementPoint = value;
			}
		}
		public int Rank
		{
			get
			{
				return this.m_Rank;
			}
			set
			{
				this.m_Rank = value;
			}
		}
		public int OnlineTime
		{
			get
			{
				return this.m_OnlineTime;
			}
			set
			{
				this.m_OnlineTime = value;
			}
		}
		public int BoxProgression
		{
			get
			{
				return this.m_BoxProgression;
			}
			set
			{
				this.m_BoxProgression = value;
			}
		}
		public DateTime BoxGetDate
		{
			get
			{
				return this.m_BoxGetDate;
			}
			set
			{
				this.m_BoxGetDate = value;
			}
		}
		public int GetBoxLevel
		{
			get
			{
				return this.m_getBoxLevel;
			}
			set
			{
				this.m_getBoxLevel = value;
			}
		}
		public int SpaPubGoldRoomLimit
		{
			get
			{
				return this.m_SpaPubGoldRoomLimit;
			}
			set
			{
				this.m_SpaPubGoldRoomLimit = value;
			}
		}
		public DateTime LastSpaDate
		{
			get
			{
				return this.m_LastSpaDate;
			}
			set
			{
				this.m_LastSpaDate = value;
			}
		}
		public DateTime AddGPLastDate
		{
			get
			{
				return this.m_AddGPLastDate;
			}
			set
			{
				this.m_AddGPLastDate = value;
			}
		}
		public int SpaPubMoneyRoomLimit
		{
			get
			{
				return this.m_SpaPubMoneyRoomLimit;
			}
			set
			{
				this.m_SpaPubMoneyRoomLimit = value;
			}
		}
		public bool IsInSpaPubGoldToday
		{
			get
			{
				return this.m_IsInSpaPubGoldToday;
			}
			set
			{
				this.m_IsInSpaPubGoldToday = value;
			}
		}
		public bool IsInSpaPubMoneyToday
		{
			get
			{
				return this.m_IsInSpaPubMoneyToday;
			}
			set
			{
				this.m_IsInSpaPubMoneyToday = value;
			}
		}
		public int AlreadyGetBox
		{
			get
			{
				return this.m_AlreadyGetBox;
			}
			set
			{
				this.m_AlreadyGetBox = value;
			}
		}
        public int VipLevel
        {
            get
            {
                return m_VipLevel;
            }
            set
            {
                if (m_VipLevel != value)
                {
                    this.m_VipLevel = value;
                    this.IsDirty = true;
                }
            }
        }
        public int ChargedMoney
        {
            get
            {
                return m_ChargedMoney;
            }
            set
            {
                this.m_ChargedMoney = value;
            }
        }
        public int InviteID
        {
            get
            {
                return m_InviteID;
            }
            set
            {
                this.m_InviteID = value;
            }
        }
        public int VIPGiftLevel
        {
            get
            {
                return m_VIPGiftLevel;
            }
            set
            {
                if (m_VIPGiftLevel != value)
                {
                    this.IsDirty = true;
                    this.m_VIPGiftLevel = value;
                }
            }
        }
        public PlayerInfo()
		{
			this._isLocked = true;
		}
		public void ClearConsortia()
		{
			this.ConsortiaID = 0;
			this.ConsortiaName = "";
			this.RichesOffer = 0;
			this.ConsortiaRepute = 0;
			this.ConsortiaLevel = 0;
			this.StoreLevel = 0;
			this.ShopLevel = 0;
			this.SmithLevel = 0;
			this.ConsortiaHonor = 0;
			this.RichesOffer = 0;
			this.RichesRob = 0;
			this.DutyLevel = 0;
			this.DutyName = "";
			this.Right = 0;
			this.AddDayGP = 0;
			this.AddWeekGP = 0;
			this.AddDayOffer = 0;
			this.AddWeekOffer = 0;
			this.ConsortiaRiches = 0;
		}
        public string Honor = "TEST";
        private int m_InviteID;
        private int m_VIPGiftLevel;
    }
}
