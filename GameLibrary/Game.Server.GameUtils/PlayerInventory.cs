using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class PlayerInventory : AbstractInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected GamePlayer m_player;
		private bool m_saveToDb;
		private List<ItemInfo> m_removedList = new List<ItemInfo>();
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
		}
		public PlayerInventory(GamePlayer player, bool saveTodb, int capibility, int type, int beginSlot, bool autoStack) : base(capibility, type, beginSlot, autoStack)
		{
			this.m_player = player;
			this.m_saveToDb = saveTodb;
		}
		public virtual void LoadFromDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					ItemInfo[] list = pb.GetUserBagByType(this.m_player.PlayerCharacter.ID, base.BagType);
					base.BeginChanges();
					try
					{
						ItemInfo[] array = list;
						for (int i = 0; i < array.Length; i++)
						{
							ItemInfo item = array[i];
							this.AddItemTo(item, item.Place);
						}
					}
					finally
					{
						base.CommitChanges();
					}
				}
			}
		}
		public virtual void SaveToDatabase()
		{
			if (this.m_saveToDb)
			{
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						for (int i = 0; i < this.m_items.Length; i++)
						{
							ItemInfo item = this.m_items[i];
							if (item != null && item.IsDirty)
							{
								if (item.ItemID > 0)
								{
									pb.UpdateGoods(item);
								}
								else
								{
									pb.AddGoods(item);
								}
							}
						}
					}
					finally
					{
						Monitor.Exit(@lock);
					}
					List<ItemInfo> removedList;
					Monitor.Enter(removedList = this.m_removedList);
					try
					{
						foreach (ItemInfo item in this.m_removedList)
						{
							if (item.ItemID > 0 && item.IsDirty)
							{
								pb.UpdateGoods(item);
							}
						}
						this.m_removedList.Clear();
					}
					finally
					{
						Monitor.Exit(removedList);
					}
				}
			}
		}
		public virtual void LoadItems(ItemInfo[] list)
		{
			if (list != null && list.Length != 0)
			{
				base.BeginChanges();
				try
				{
					for (int i = 0; i < list.Length; i++)
					{
						ItemInfo item = list[i];
						if (item.BagType == base.BagType && item.Count <= item.Template.MaxCount && item.Count > 0)
						{
							this.AddItemTo(item, item.Place);
						}
					}
				}
				finally
				{
					base.CommitChanges();
				}
			}
		}
		public override bool AddItemTo(ItemInfo item, int place)
		{
			bool result;
			if (base.AddItemTo(item, place))
			{
				item.UserID = this.m_player.PlayerCharacter.ID;
				item.IsExist = true;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public override bool TakeOutItem(ItemInfo item)
		{
			bool result;
			if (base.TakeOutItem(item))
			{
				if (this.m_saveToDb)
				{
					using (PlayerBussiness pb = new PlayerBussiness())
					{
						pb.UpdateGoods(item);
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public override bool RemoveItem(ItemInfo item, eItemRemoveType type)
		{
			bool result;
			if (base.RemoveItem(item, type))
			{
				item.IsExist = false;
				item.RemoveType = (int)type;
				if (this.m_saveToDb)
				{
					List<ItemInfo> removedList;
					Monitor.Enter(removedList = this.m_removedList);
					try
					{
						this.m_removedList.Add(item);
					}
					finally
					{
						Monitor.Exit(removedList);
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public override void UpdateChangedPlaces()
		{
			int[] changedPlaces = null;
			List<int> changedPlaces2;
			Monitor.Enter(changedPlaces2 = this.m_changedPlaces);
			try
			{
				changedPlaces = this.m_changedPlaces.ToArray();
			}
			finally
			{
				Monitor.Exit(changedPlaces2);
			}
			if (changedPlaces != null)
			{
				this.m_player.Out.SendUpdateInventorySlot(this, changedPlaces);
			}
			base.UpdateChangedPlaces();
		}
		public bool SendAllItemsToMail(string sender, string title, eMailType type)
		{
			bool result = true;
			bool sentMail = false;
			if (this.m_saveToDb)
			{
				base.BeginChanges();
				try
				{
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						List<ItemInfo> items = this.GetItems();
						int count = items.Count;
						if (count > 0)
						{
							sentMail = true;
						}
						result = this.m_player.SendItemsToMail(items, "Game.Server.GameUtils.CommonBag.Sender", title, type);
					}
					finally
					{
						Monitor.Exit(@lock);
					}
				}
				catch (Exception ex)
				{
					PlayerInventory.log.Error("Send Items Mail Error:", ex);
				}
				finally
				{
					this.SaveToDatabase();
					base.CommitChanges();
				}
				if (sentMail)
				{
					this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
			}
			return result;
		}
	}
}
