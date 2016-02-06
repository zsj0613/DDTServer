using System;
namespace SqlDataProvider.Data
{
	public class UsersRecordInfo : DataObject
	{
		private int _userID;
		private int _recordID;
		private int _total;
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
		public int RecordID
		{
			get
			{
				return this._recordID;
			}
			set
			{
				this._recordID = value;
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
	}
}
