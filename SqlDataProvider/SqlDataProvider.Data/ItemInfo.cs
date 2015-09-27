using System;
namespace SqlDataProvider.Data
{
	public class ItemInfo : DataObject
	{
		private ItemTemplateInfo _template;
		private int _itemID;
		private int _userID;
		private int _bagType;
		private int _templateId;
		private int _place;
		private int _count;
		private bool _isJudage;
		private string _color;
		private bool _isExist;
		private int _strengthenLevel;
		private int _attackCompose;
		private int _defendCompose;
		private int _luckCompose;
		private int _agilityCompose;
		private bool _isBinds;
		private bool _isUsed;
		private string _skin;
		private DateTime _beginDate;
		private int _validDate;
		private DateTime _removeDate;
		private int _removeType;
		private int _hole1;
		private int _hole2;
		private int _hole3;
		private int _hole4;
		private int _hole5;
		private int _hole6;
		private bool _isTips = false;
		private bool _isLogs = false;
		public bool IsOpenHole = false;
		public ItemTemplateInfo Template
		{
			get
			{
				return this._template;
			}
		}
		public int ItemID
		{
			get
			{
				return this._itemID;
			}
			set
			{
				this._itemID = value;
				this._isDirty = true;
			}
		}
		public int UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				if (this._userID != value)
				{
					this._userID = value;
					this._isDirty = true;
				}
			}
		}
		public int BagType
		{
			get
			{
				return this._bagType;
			}
			set
			{
				if (this._bagType != value)
				{
					this._bagType = value;
					this._isDirty = true;
				}
			}
		}
		public int TemplateID
		{
			get
			{
				return this._templateId;
			}
			set
			{
				if (this._templateId != value)
				{
					this._templateId = value;
					this._isDirty = true;
				}
			}
		}
		public int Place
		{
			get
			{
				return this._place;
			}
			set
			{
				if (this._place != value)
				{
					this._place = value;
					this._isDirty = true;
				}
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
				if (this._count != value)
				{
					this._count = value;
					this._isDirty = true;
				}
			}
		}
		public bool IsJudge
		{
			get
			{
				return this._isJudage;
			}
			set
			{
				if (this._isJudage != value)
				{
					this._isJudage = value;
					this._isDirty = true;
				}
			}
		}
		public string Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (!(this._color == value))
				{
					this._color = value;
					this._isDirty = true;
				}
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
				if (this._isExist != value)
				{
					this._isExist = value;
					this._isDirty = true;
				}
			}
		}
		public int StrengthenLevel
		{
			get
			{
				return this._strengthenLevel;
			}
			set
			{
				if (this._strengthenLevel != value)
				{
					this._strengthenLevel = value;
					this._isDirty = true;
					ItemInfo.OpenHole(this);
				}
			}
		}
		public int AttackCompose
		{
			get
			{
				return this._attackCompose;
			}
			set
			{
				if (this._attackCompose != value)
				{
					this._attackCompose = value;
					this._isDirty = true;
				}
			}
		}
		public int DefendCompose
		{
			get
			{
				return this._defendCompose;
			}
			set
			{
				if (this._defendCompose != value)
				{
					this._defendCompose = value;
					this._isDirty = true;
				}
			}
		}
		public int LuckCompose
		{
			get
			{
				return this._luckCompose;
			}
			set
			{
				if (this._luckCompose != value)
				{
					this._luckCompose = value;
					this._isDirty = true;
				}
			}
		}
		public int AgilityCompose
		{
			get
			{
				return this._agilityCompose;
			}
			set
			{
				if (this._agilityCompose != value)
				{
					this._agilityCompose = value;
					this._isDirty = true;
				}
			}
		}
		public bool IsBinds
		{
			get
			{
				return this._isBinds;
			}
			set
			{
				if (this._isBinds != value)
				{
					this._isBinds = value;
					this._isDirty = true;
				}
			}
		}
		public bool IsUsed
		{
			get
			{
				return this._isUsed;
			}
			set
			{
				if (this._isUsed != value)
				{
					this._isUsed = value;
					this._isDirty = true;
				}
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
		public DateTime BeginDate
		{
			get
			{
				return this._beginDate;
			}
			set
			{
				if (!(this._beginDate == value))
				{
					this._beginDate = value;
					this._isDirty = true;
				}
			}
		}
		public int ValidDate
		{
			get
			{
				return this._validDate;
			}
			set
			{
				if (this._validDate != value)
				{
					this._validDate = value;
					this._isDirty = true;
				}
			}
		}
		public DateTime RemoveDate
		{
			get
			{
				return this._removeDate;
			}
			set
			{
				if (!(this._removeDate == value))
				{
					this._removeDate = value;
					this._isDirty = true;
				}
			}
		}
		public int RemoveType
		{
			get
			{
				return this._removeType;
			}
			set
			{
				if (this._removeType != value)
				{
					this._removeType = value;
					this._removeDate = DateTime.Now;
					this._isDirty = true;
				}
			}
		}
		public int Hole1
		{
			get
			{
				return this._hole1;
			}
			set
			{
				this._hole1 = value;
				this._isDirty = true;
			}
		}
		public int Hole2
		{
			get
			{
				return this._hole2;
			}
			set
			{
				this._hole2 = value;
				this._isDirty = true;
			}
		}
		public int Hole3
		{
			get
			{
				return this._hole3;
			}
			set
			{
				this._hole3 = value;
				this._isDirty = true;
			}
		}
		public int Hole4
		{
			get
			{
				return this._hole4;
			}
			set
			{
				this._hole4 = value;
				this._isDirty = true;
			}
		}
		public int Hole5
		{
			get
			{
				return this._hole5;
			}
			set
			{
				this._hole5 = value;
				this._isDirty = true;
			}
		}
		public int Hole6
		{
			get
			{
				return this._hole6;
			}
			set
			{
				this._hole6 = value;
				this._isDirty = true;
			}
		}
		public int Attack
		{
			get
			{
				return this._attackCompose + this._template.Attack;
			}
		}
		public int Defence
		{
			get
			{
				return this._defendCompose + this._template.Defence;
			}
		}
		public int Agility
		{
			get
			{
				return this._agilityCompose + this._template.Agility;
			}
		}
		public int Luck
		{
			get
			{
				return this._luckCompose + this._template.Luck;
			}
		}
		public bool IsTips
		{
			get
			{
				return this._isTips;
			}
			set
			{
				this._isTips = value;
			}
		}
		public bool IsLogs
		{
			get
			{
				return this._isLogs;
			}
			set
			{
				this._isLogs = value;
			}
		}
		internal ItemInfo(ItemTemplateInfo template)
		{
			this._template = template;
			if (this._template != null)
			{
				this._templateId = this._template.TemplateID;
			}
		}
		public ItemInfo Clone()
		{
			return new ItemInfo(this._template)
			{
				_userID = this._userID,
				_validDate = this._validDate,
				_templateId = this._templateId,
				_strengthenLevel = this._strengthenLevel,
				_luckCompose = this._luckCompose,
				_itemID = 0,
				_isJudage = this._isJudage,
				_isExist = this._isExist,
				_isBinds = this._isBinds,
				_isUsed = this._isUsed,
				_defendCompose = this._defendCompose,
				_count = this._count,
				_color = this._color,
				_skin = this._skin,
				_beginDate = this._beginDate,
				_attackCompose = this._attackCompose,
				_agilityCompose = this._agilityCompose,
				_hole1 = this._hole1,
				_hole2 = this._hole2,
				_hole3 = this._hole3,
				_hole4 = this._hole4,
				_hole5 = this._hole5,
				_hole6 = this._hole6,
				_removeDate = this._removeDate,
				_removeType = this._removeType,
				//_itemID = 0,
				_bagType = -1,
				_place = -1,
				_isDirty = true
			};
		}
		public bool IsValidItem()
		{
			return this._validDate == 0 || !this._isUsed || DateTime.Compare(this._beginDate.AddDays((double)this._validDate), DateTime.Now) > 0;
		}
		public bool CanStackedTo(ItemInfo to)
		{
			bool result;
			if (this._templateId == to.TemplateID && this.Template.MaxCount > 1 && this._isBinds == to.IsBinds && this._isUsed == to._isUsed)
			{
				if (this.ValidDate == 0 || (this.BeginDate == to.BeginDate && this.ValidDate == this.ValidDate))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public int GetBagType()
		{
			int result;
			switch (this._template.CategoryID)
			{
			case 10:
			case 11:
				result = 1;
				break;
			case 12:
				result = 2;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}
		public bool CanEquip()
		{
			return this._template.CategoryID < 10 || (this._template.CategoryID >= 13 && this._template.CategoryID <= 16);
		}
		public string GetBagName()
		{
			string result;
			switch (this._template.CategoryID)
			{
			case 10:
			case 11:
				result = "Game.Server.GameObjects.Prop";
				break;
			case 12:
				result = "Game.Server.GameObjects.Task";
				break;
			default:
				result = "Game.Server.GameObjects.Equip";
				break;
			}
			return result;
		}
		public string GetPropertyString()
		{
			return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", new object[]
			{
				this.StrengthenLevel,
				this.Attack,
				this.Defence,
				this.Agility,
				this.Luck,
				this.AttackCompose,
				this.DefendCompose,
				this.AgilityCompose,
				this.LuckCompose
			});
		}
		public string ToShortString()
		{
			return string.Format("{0}:{1}", this.ItemID, this.Template.Name);
		}
		public static ItemInfo CreateWithoutInit(ItemTemplateInfo template)
		{
			return new ItemInfo(template);
		}
		public static ItemInfo CreateFromTemplate(ItemTemplateInfo template, int count, int type)
		{
			if (template == null)
			{
				throw new ArgumentNullException("template");
			}
			return new ItemInfo(template)
			{
				TemplateID = template.TemplateID,
				AgilityCompose = 0,
				AttackCompose = 0,
				BeginDate = DateTime.Now,
				Color = "",
				Skin = "",
				DefendCompose = 0,
			//	IsBinds = false,
				IsUsed = false,
				IsDirty = false,
				IsExist = true,
				IsJudge = true,
				LuckCompose = 0,
				StrengthenLevel = 0,
				ValidDate = 0,
				Count = count,
				IsBinds = template.BindType == 1,
				_removeDate = DateTime.Now,
				_removeType = type,
				Hole1 = -1,
				Hole2 = -1,
				Hole3 = -1,
				Hole4 = -1,
				Hole5 = -1,
				Hole6 = -1,
				IsTips = false,
				IsLogs = false
			};
		}
		public static ItemInfo FindSpecialItemInfo(ItemInfo info, ref int gold, ref int money, ref int giftToken)
		{
			int gp = 0;
			return ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref gp);
		}
		public static ItemInfo FindSpecialItemInfo(ItemInfo info, ref int gold, ref int money, ref int giftToken, ref int gp)
		{
			int templateID = info.TemplateID;
			if (templateID <= -200)
			{
				if (templateID != -300)
				{
					if (templateID == -200)
					{
						money += info.Count;
						info = null;
					}
				}
				else
				{
					giftToken += info.Count;
					info = null;
				}
			}
			else
			{
				if (templateID != -100)
				{
					if (templateID == 11107)
					{
						gp += info.Count;
						info = null;
					}
				}
				else
				{
					gold += info.Count;
					info = null;
				}
			}
			return info;
		}
		public static void OpenHole(ItemInfo item)
		{
			if (item.Template != null)
			{
				item.IsOpenHole = false;
				string[] Hole = item.Template.Hole.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < Hole.Length; i++)
				{
					string[] NeedLevel = Hole[i].Split(new char[]
					{
						','
					});
					if (item.StrengthenLevel >= Convert.ToInt32(NeedLevel[0]) && Convert.ToInt32(NeedLevel[1]) != -1)
					{
						switch (i)
						{
						case 0:
							if (item.Hole1 < 0)
							{
								item.Hole1 = 0;
								item.IsOpenHole = true;
							}
							break;
						case 1:
							if (item.Hole2 < 0)
							{
								item.Hole2 = 0;
								item.IsOpenHole = true;
							}
							break;
						case 2:
							if (item.Hole3 < 0)
							{
								item.Hole3 = 0;
								item.IsOpenHole = true;
							}
							break;
						case 3:
							if (item.Hole4 < 0)
							{
								item.Hole4 = 0;
								item.IsOpenHole = true;
							}
							break;
						case 4:
							if (item.Hole5 < 0)
							{
								item.Hole5 = 0;
								item.IsOpenHole = true;
							}
							break;
						case 5:
							if (item.Hole6 < 0)
							{
								item.Hole6 = 0;
								item.IsOpenHole = true;
							}
							break;
						}
					}
				}
			}
		}
	}
}
