using SqlDataProvider.Data;
using System;
using System.Drawing;
namespace Game.Logic.Phy.Object
{
	public class Box : PhysicalObj
	{
		private int _userID;
		private int _liveCount;
		private ItemInfo m_item;
		public int UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this._userID = value;
			}
		}
		public int LiveCount
		{
			get
			{
				return this._liveCount;
			}
			set
			{
				this._liveCount = value;
			}
		}
		public ItemInfo Item
		{
			get
			{
				return this.m_item;
			}
		}
		public override int Type
		{
			get
			{
				return 1;
			}
		}
		public Box(int id, string model, ItemInfo item) : base(id, "", model, "", 1, 1, 1)
		{
			this._userID = 0;
			this.m_rect = new Rectangle(-15, -15, 30, 30);
			this.m_item = item;
		}
		public override void CollidedByObject(Physics phy, int delay)
		{
			if (phy is SimpleBomb)
			{
				SimpleBomb bomb = phy as SimpleBomb;
				bomb.Owner.PickBox(this);
			}
		}
	}
}
