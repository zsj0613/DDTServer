using System;
namespace SqlDataProvider.Data
{
	public class MarryProp
	{
		private bool _isMarried;
		private int _spouseID;
		private string _spouseName;
		private bool _isCreatedMarryRoom;
		private int _selfMarryRoomID;
		private bool _isGotRing;
		public bool IsMarried
		{
			get
			{
				return this._isMarried;
			}
			set
			{
				this._isMarried = value;
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
				this._spouseID = value;
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
				this._spouseName = value;
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
				this._isCreatedMarryRoom = value;
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
				this._selfMarryRoomID = value;
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
				this._isGotRing = value;
			}
		}
	}
}
