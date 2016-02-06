using System;
namespace SqlDataProvider.Data
{
	public class ConsortiaInfo : DataObject
	{
		private int _consortiaID;
		private string _consortiaName;
		private int _honor;
		private int _creatorID;
		private string _creatorName;
		private int _chairmanID;
		private string _chairmanName;
		private string _description;
		private string _placard;
		private int _level;
		private int _maxCount;
		private int _celebCount;
		private DateTime _buildDate;
		private int _repute;
		private int _count;
		private string _ip;
		private int _port;
		private bool _isExist;
		private int _riches;
		private DateTime _deductDate;
		public int ConsortiaID
		{
			get
			{
				return this._consortiaID;
			}
			set
			{
				this._consortiaID = value;
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
				this._isDirty = true;
			}
		}
		public int Honor
		{
			get
			{
				return this._honor;
			}
			set
			{
				this._honor = value;
				this._isDirty = true;
			}
		}
		public int CreatorID
		{
			get
			{
				return this._creatorID;
			}
			set
			{
				this._creatorID = value;
				this._isDirty = true;
			}
		}
		public string CreatorName
		{
			get
			{
				return this._creatorName;
			}
			set
			{
				this._creatorName = value;
				this._isDirty = true;
			}
		}
		public int ChairmanID
		{
			get
			{
				return this._chairmanID;
			}
			set
			{
				this._chairmanID = value;
				this._isDirty = true;
			}
		}
		public string ChairmanName
		{
			get
			{
				return this._chairmanName;
			}
			set
			{
				this._chairmanName = value;
				this._isDirty = true;
			}
		}
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				this._description = value;
				this._isDirty = true;
			}
		}
		public string Placard
		{
			get
			{
				return this._placard;
			}
			set
			{
				this._placard = value;
				this._isDirty = true;
			}
		}
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
				this._isDirty = true;
			}
		}
		public int MaxCount
		{
			get
			{
				return this._maxCount;
			}
			set
			{
				this._maxCount = value;
				this._isDirty = true;
			}
		}
		public int CelebCount
		{
			get
			{
				return this._celebCount;
			}
			set
			{
				this._celebCount = value;
				this._isDirty = true;
			}
		}
		public DateTime BuildDate
		{
			get
			{
				return this._buildDate;
			}
			set
			{
				this._buildDate = value;
				this._isDirty = true;
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
		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				this._count = value;
				this._isDirty = true;
			}
		}
		public string IP
		{
			get
			{
				return this._ip;
			}
			set
			{
				this._ip = value;
				this._isDirty = true;
			}
		}
		public int Port
		{
			get
			{
				return this._port;
			}
			set
			{
				this._port = value;
				this._isDirty = true;
			}
		}
		public bool IsExist
		{
			get
			{
				return this._isExist;
			}
			set
			{
				this._isExist = value;
				this._isDirty = true;
			}
		}
		public int Riches
		{
			get
			{
				return this._riches;
			}
			set
			{
				this._riches = value;
				this._isDirty = true;
			}
		}
		public DateTime DeductDate
		{
			get
			{
				return this._deductDate;
			}
			set
			{
				this._deductDate = value;
				this._isDirty = true;
			}
		}
		public int AddDayRiches
		{
			get;
			set;
		}
		public int AddWeekRiches
		{
			get;
			set;
		}
		public int AddDayHonor
		{
			get;
			set;
		}
		public int AddWeekHonor
		{
			get;
			set;
		}
		public int LastDayRiches
		{
			get;
			set;
		}
		public bool OpenApply
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
		public int StoreLevel
		{
			get;
			set;
		}
		public int FightPower
		{
			get;
			set;
		}
		public bool IsSystemCreate
		{
			get;
			set;
		}
		public bool IsActive
		{
			get;
			set;
		}
	}
}
