using System;
namespace SqlDataProvider.Data
{
	public class BufferInfo : DataObject
	{
		private int _userID;
		private int _type;
		private int _value;
		private DateTime _beginDate;
		private int _validDate;
		private string _data;
		private bool _isExist;
		public int UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this._userID = value;
				this._isDirty = true;
			}
		}
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
				this._isDirty = true;
			}
		}
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
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
				this._beginDate = value;
				this._isDirty = true;
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
				this._validDate = value;
				this._isDirty = true;
			}
		}
		public string Data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
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
		public DateTime GetEndDate()
		{
			return this._beginDate.AddMinutes((double)this._validDate);
		}
		public bool IsValid()
		{
			return this._beginDate.AddMinutes((double)this._validDate) > DateTime.Now;
		}
	}
}
