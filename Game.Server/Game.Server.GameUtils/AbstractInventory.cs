using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public abstract class AbstractInventory
	{
		private static LogProvider log => LogProvider.Default;
		protected object m_lock = new object();
		private int m_type;
		private int m_capalility;
		private int m_beginSlot;
		private bool m_autoStack;
		protected ItemInfo[] m_items;
		protected List<int> m_changedPlaces = new List<int>();
		private int m_changeCount;
		public int BeginSlot
		{
			get
			{
				return this.m_beginSlot;
			}
		}
		public int Capalility
		{
			get
			{
				return this.m_capalility;
			}
			set
			{
				this.m_capalility = ((value < 0) ? 0 : ((value > this.m_items.Length) ? this.m_items.Length : value));
			}
		}
		public int BagType
		{
			get
			{
				return this.m_type;
			}
		}
		public AbstractInventory(int capability, int type, int beginSlot, bool autoStack)
		{
			this.m_items = new ItemInfo[capability];
			this.m_capalility = capability;
			this.m_type = type;
			this.m_beginSlot = beginSlot;
			this.m_autoStack = autoStack;
		}
		public bool AddItem(ItemInfo item)
		{
			return this.AddItem(item, this.m_beginSlot);
		}
		public bool AddItem(ItemInfo item, int minSlot)
		{
			bool result;
			if (item == null)
			{
				result = false;
			}
			else
			{
				int place = this.FindFirstEmptySlot(minSlot);
				result = this.AddItemTo(item, place);
			}
			return result;
		}
		public virtual bool AddItemTo(ItemInfo item, int place)
		{
			bool result;
            if (item == null || place >= this.m_capalility || place < 0)
            {
                result = false;
            }
            else
            {
                if (item.Template.IsOnly)
                {
                    if (GetItemCount(item.TemplateID) >= 1)
                        return false;
                }

                object @lock;
                Monitor.Enter(@lock = this.m_lock);
                try
                {
                    if (this.m_items[place] != null)
                    {
                        place = -1;
                    }
                    else
                    {                      
                        this.m_items[place] = item;
                        item.Place = place;
                        item.BagType = this.m_type;
                    }
                }
                finally
                {
                    Monitor.Exit(@lock);
                }
                if (place != -1)
                {
                    this.OnPlaceChanged(place);
                }
                result = (place != -1);
            }
			return result;
		}
		public virtual bool TakeOutItem(ItemInfo item)
		{
			bool result;
			if (item == null)
			{
				result = false;
			}
			else
			{
				int place = -1;
				object @lock;
				Monitor.Enter(@lock = this.m_lock);
				try
				{
					for (int i = 0; i < this.m_capalility; i++)
					{
						if (this.m_items[i] == item)
						{
							place = i;
							this.m_items[i] = null;
							break;
						}
					}
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				if (place != -1)
				{
					this.OnPlaceChanged(place);
					if (item.BagType == this.BagType)
					{
						item.Place = -1;
						item.BagType = -1;
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}
		public bool TakeOutItemAt(int place)
		{
			return this.TakeOutItem(this.GetItemAt(place));
		}
		public virtual bool RemoveItem(ItemInfo item, eItemRemoveType type)
		{
			bool result;
			if (item == null)
			{
				result = false;
			}
			else
			{
				int place = -1;
				object @lock;
				Monitor.Enter(@lock = this.m_lock);
				try
				{
					for (int i = 0; i < this.m_capalility; i++)
					{
						if (this.m_items[i] == item)
						{
							place = i;
							this.m_items[i] = null;
							break;
						}
					}
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				if (place != -1)
				{
					this.OnPlaceChanged(place);
					if (item.BagType == this.BagType && item.Place == place)
					{
						item.Place = -1;
						item.BagType = -1;
					}
				}
				result = (place != -1);
			}
			return result;
		}
		public bool RemoveItemAt(int place, eItemRemoveType type)
		{
			return this.RemoveItem(this.GetItemAt(place), type);
		}
		public virtual bool AddCountToStack(ItemInfo item, int count)
		{
			bool result;
			if (item == null)
			{
				result = false;
			}
			else
			{
				if (count <= 0 || item.BagType != this.m_type)
				{
					result = false;
				}
				else
				{
					if (item.Count + count > item.Template.MaxCount)
					{
						result = false;
					}
					else
					{
						item.Count += count;
						this.OnPlaceChanged(item.Place);
						result = true;
					}
				}
			}
			return result;
		}
		public virtual bool RemoveCountFromStack(ItemInfo item, int count, eItemRemoveType type)
		{
			bool result;
			if (item == null)
			{
				result = false;
			}
			else
			{
				if (count <= 0 || item.BagType != this.m_type)
				{
					result = false;
				}
				else
				{
					if (item.Count < count)
					{
						result = false;
					}
					else
					{
						if (item.Count == count)
						{
							result = this.RemoveItem(item, type);
						}
						else
						{
							item.Count -= count;
							this.OnPlaceChanged(item.Place);
							result = true;
						}
					}
				}
			}
			return result;
		}
		public virtual bool AddTemplate(ItemInfo cloneItem, int count)
		{
			return this.AddTemplate(cloneItem, count, this.m_beginSlot, this.m_capalility - 1);
		}
		public virtual bool AddTemplate(ItemInfo cloneItem, int count, int minSlot, int maxSlot)
		{
			bool result;
			if (cloneItem == null)
			{
				result = false;
			}
			else
			{
				ItemTemplateInfo template = cloneItem.Template;
				if (template == null)
				{
					result = false;
				}
				else
				{
					if (count <= 0)
					{
						result = false;
					}
					else
					{
						if (minSlot < this.m_beginSlot || minSlot > this.m_capalility - 1)
						{
							result = false;
						}
						else
						{
							if (maxSlot < this.m_beginSlot || maxSlot > this.m_capalility - 1)
							{
								result = false;
							}
							else
							{
								if (minSlot > maxSlot)
								{
									result = false;
								}
								else
								{
									object @lock;
									Monitor.Enter(@lock = this.m_lock);
									try
									{
										List<int> changedSlot = new List<int>();
										int itemcount = count;
										for (int i = minSlot; i <= maxSlot; i++)
										{
											ItemInfo item = this.m_items[i];
											if (item == null)
											{
												itemcount -= template.MaxCount;
												changedSlot.Add(i);
											}
											else
											{
												if (this.m_autoStack && cloneItem.CanStackedTo(item))
												{
													itemcount -= template.MaxCount - item.Count;
													changedSlot.Add(i);
												}
											}
											if (itemcount <= 0)
											{
												break;
											}
										}
										if (itemcount <= 0)
										{
											this.BeginChanges();
											try
											{
												itemcount = count;
												foreach (int i in changedSlot)
												{
													ItemInfo item = this.m_items[i];
													if (item == null)
													{
														item = cloneItem.Clone();
														item.Count = ((itemcount < template.MaxCount) ? itemcount : template.MaxCount);
														itemcount -= item.Count;
														this.AddItemTo(item, i);
													}
													else
													{
														if (item.TemplateID == template.TemplateID)
														{
															int add = (item.Count + itemcount < template.MaxCount) ? itemcount : (template.MaxCount - item.Count);
															item.Count += add;
															itemcount -= add;
															this.OnPlaceChanged(i);
														}
														else
														{
															AbstractInventory.log.Error("Add template erro: select slot's TemplateId not equest templateId");
														}
													}
												}
												if (itemcount != 0)
												{
													AbstractInventory.log.Error("Add template error: last count not equal Zero.");
												}
											}
											finally
											{
												this.CommitChanges();
											}
											result = true;
										}
										else
										{
											result = false;
										}
									}
									finally
									{
										Monitor.Exit(@lock);
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public virtual bool RemoveTemplate(int templateId, int count, eItemRemoveType type)
		{
			return this.RemoveTemplate(templateId, count, 0, this.m_capalility - 1, type);
		}
		public virtual bool RemoveTemplate(int templateId, int count, int minSlot, int maxSlot, eItemRemoveType type)
		{
			bool result;
			if (count <= 0)
			{
				result = false;
			}
			else
			{
				if (minSlot < 0 || minSlot > this.m_capalility - 1)
				{
					result = false;
				}
				else
				{
					if (maxSlot <= 0 || maxSlot > this.m_capalility - 1)
					{
						result = false;
					}
					else
					{
						if (minSlot > maxSlot)
						{
							result = false;
						}
						else
						{
							object @lock;
							Monitor.Enter(@lock = this.m_lock);
							try
							{
								List<int> changedSlot = new List<int>();
								int itemcount = count;
								for (int i = minSlot; i <= maxSlot; i++)
								{
									ItemInfo item = this.m_items[i];
									if (item != null && item.TemplateID == templateId)
									{
										changedSlot.Add(i);
										itemcount -= item.Count;
										if (itemcount <= 0)
										{
											break;
										}
									}
								}
								if (itemcount <= 0)
								{
									this.BeginChanges();
									itemcount = count;
									try
									{
										foreach (int i in changedSlot)
										{
											ItemInfo item = this.m_items[i];
											if (item != null && item.TemplateID == templateId)
											{
												if (item.Count <= itemcount)
												{
													this.RemoveItem(item, type);
													itemcount -= item.Count;
												}
												else
												{
													int dec = (item.Count - itemcount < item.Count) ? itemcount : 0;
													item.Count -= dec;
													itemcount -= dec;
													this.OnPlaceChanged(i);
												}
											}
										}
										if (itemcount != 0)
										{
											AbstractInventory.log.Error("Remove template error:last item cout not equal Zero.");
										}
									}
									finally
									{
										this.CommitChanges();
									}
									result = true;
								}
								else
								{
									result = false;
								}
							}
							finally
							{
								Monitor.Exit(@lock);
							}
						}
					}
				}
			}
			return result;
		}
		public virtual bool MoveItem(int fromSlot, int toSlot, int count)
		{
			bool result2;
			if (fromSlot < 0 || toSlot < 0 || fromSlot >= this.m_capalility || toSlot >= this.m_capalility || count < 0)
			{
				result2 = false;
			}
			else
			{
				if (fromSlot == toSlot)
				{
					result2 = false;
				}
				else
				{
					bool result = false;
					object @lock;
					Monitor.Enter(@lock = this.m_lock);
					try
					{
						result = (this.CombineItems(fromSlot, toSlot) || this.StackItems(fromSlot, toSlot, count) || this.ExchangeItems(fromSlot, toSlot));
					}
					finally
					{
						Monitor.Exit(@lock);
					}
					if (result)
					{
						this.BeginChanges();
						try
						{
							this.OnPlaceChanged(fromSlot);
							this.OnPlaceChanged(toSlot);
						}
						finally
						{
							this.CommitChanges();
						}
					}
					result2 = result;
				}
			}
			return result2;
		}
		public bool IsSolt(int slot)
		{
			return slot >= 0 && slot < this.m_capalility;
		}
		public bool IsEmpty(int slot)
		{
			return slot < 0 || slot >= this.m_capalility || this.m_items[slot] == null;
		}
		public void ClearBag(eItemRemoveType type)
		{
			this.BeginChanges();
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = this.m_beginSlot; i < this.m_capalility; i++)
				{
					if (this.m_items[i] != null)
					{
						this.RemoveItem(this.m_items[i], type);
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			this.CommitChanges();
		}
		protected virtual bool CombineItems(int fromSlot, int toSlot)
		{
			return fromSlot == toSlot && false;
		}
		protected virtual bool StackItems(int fromSlot, int toSlot, int itemCount)
		{
			bool result;
			if (fromSlot == toSlot)
			{
				result = false;
			}
			else
			{
				ItemInfo fromItem = this.m_items[fromSlot];
				ItemInfo toItem = this.m_items[toSlot];
				if (fromItem == null)
				{
					result = false;
				}
				else
				{
					if (itemCount < 0)
					{
						result = false;
					}
					else
					{
						if (itemCount == 0)
						{
							if (fromItem.Count > 0)
							{
								itemCount = fromItem.Count;
							}
							else
							{
								itemCount = 1;
							}
						}
						if (fromItem.Count < itemCount)
						{
							result = false;
						}
						else
						{
							if (toItem != null && toItem.TemplateID == fromItem.TemplateID && toItem.CanStackedTo(fromItem))
							{
								if (itemCount + toItem.Count > fromItem.Template.MaxCount)
								{
									fromItem.Count -= toItem.Template.MaxCount - toItem.Count;
									toItem.Count = toItem.Template.MaxCount;
								}
								else
								{
									toItem.Count += itemCount;
									if (itemCount == fromItem.Count)
									{
										this.RemoveItem(fromItem, eItemRemoveType.Stack);
									}
									else
									{
										fromItem.Count -= itemCount;
										this.UpdateItem(fromItem);
									}
								}
								result = true;
							}
							else
							{
								if (toItem == null && fromItem.Count > itemCount)
								{
									ItemInfo newItem = fromItem.Clone();
									newItem.Count = itemCount;
									if (this.AddItemTo(newItem, toSlot))
									{
										fromItem.Count -= itemCount;
										result = true;
									}
									else
									{
										result = false;
									}
								}
								else
								{
									result = false;
								}
							}
						}
					}
				}
			}
			return result;
		}
		protected virtual bool ExchangeItems(int fromSlot, int toSlot)
		{
			bool result;
			if (fromSlot == toSlot)
			{
				result = false;
			}
			else
			{
				ItemInfo fromItem = this.m_items[toSlot];
				ItemInfo toItem = this.m_items[fromSlot];
				this.m_items[fromSlot] = fromItem;
				this.m_items[toSlot] = toItem;
				if (fromItem != null)
				{
					fromItem.Place = fromSlot;
				}
				if (toItem != null)
				{
					toItem.Place = toSlot;
				}
				result = true;
			}
			return result;
		}
		public bool StackItemToAnother(ItemInfo item)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			bool result;
			try
			{
                if (item.Template.IsOnly&&GetItemCount(item.TemplateID)>=1)
                {
                    return false;
                }
				for (int i = this.m_capalility - 1; i >= 0; i--)
				{
					if (item != null && this.m_items[i] != null && this.m_items[i] != item && item.CanStackedTo(this.m_items[i]) && this.m_items[i].Count + item.Count <= item.Template.MaxCount)
					{
						this.m_items[i].Count += item.Count;
						item.IsExist = false;
						item.RemoveType = 26;
						this.UpdateItem(this.m_items[i]);
						result = true;
						return result;
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			result = false;
			return result;
		}
		public virtual ItemInfo GetItemAt(int slot)
		{
			ItemInfo result;
			if (slot < 0 || slot >= this.m_capalility)
			{
				result = null;
			}
			else
			{
				result = this.m_items[slot];
			}
			return result;
		}
		public int FindFirstEmptySlot()
		{
			return this.FindFirstEmptySlot(this.m_beginSlot);
		}
		public int FindFirstEmptySlot(int minSlot)
		{
			int result;
			if (minSlot >= this.m_capalility)
			{
				result = -1;
			}
			else
			{
				object @lock;
				Monitor.Enter(@lock = this.m_lock);
				try
				{
					for (int i = minSlot; i < this.m_capalility; i++)
					{
						if (this.m_items[i] == null)
						{
							result = i;
							return result;
						}
					}
					result = -1;
				}
				finally
				{
					Monitor.Exit(@lock);
				}
			}
			return result;
		}
		public int FindLastEmptySlot()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			int result;
			try
			{
				for (int i = this.m_capalility - 1; i >= 0; i--)
				{
					if (this.m_items[i] == null)
					{
						result = i;
						return result;
					}
				}
				result = -1;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public void Clear(int minSlot, int maxSlot)
		{
			this.BeginChanges();
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = minSlot; i <= maxSlot; i++)
				{
					this.m_items[i] = null;
					this.OnPlaceChanged(i);
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			this.CommitChanges();
		}
		public virtual ItemInfo GetItemByCategoryID(int minSlot, int categoryID, int property)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			ItemInfo result;
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_items[i] != null && this.m_items[i].Template.CategoryID == categoryID)
					{
						if (property == -1 || this.m_items[i].Template.Property1 == property)
						{
							result = this.m_items[i];
							return result;
						}
					}
				}
				result = null;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public virtual ItemInfo GetItemByTemplateID(int minSlot, int templateId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			ItemInfo result;
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_items[i] != null && this.m_items[i].TemplateID == templateId)
					{
						result = this.m_items[i];
						return result;
					}
				}
				result = null;
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return result;
		}
		public virtual int GetItemCount(int templateId)
		{
			return this.GetItemCount(0, templateId);
		}
		public int GetItemCount(int minSlot, int templateId)
		{
			int count = 0;
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = minSlot; i < this.m_capalility; i++)
				{
					if (this.m_items[i] != null && this.m_items[i].TemplateID == templateId)
					{
						count += this.m_items[i].Count;
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return count;
		}
		public virtual List<ItemInfo> GetItems()
		{
			return this.GetItems(0, this.m_capalility - 1);
		}
		public virtual List<ItemInfo> GetItems(int minSlot, int maxSlot)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = minSlot; i <= maxSlot; i++)
				{
					if (this.m_items[i] != null)
					{
						list.Add(this.m_items[i]);
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return list;
		}
		public Dictionary<int, ItemInfo> GetRawSpaces()
		{
			Dictionary<int, ItemInfo> dics = new Dictionary<int, ItemInfo>();
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				for (int i = 0; i < this.m_items.Length; i++)
				{
					if (this.m_items[i] != null)
					{
						dics.Add(i, this.m_items[i]);
					}
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return dics;
		}
		public int GetEmptyCount()
		{
			return this.GetEmptyCount(this.m_beginSlot);
		}
		public virtual int GetEmptyCount(int minSlot)
		{
			int result;
			if (minSlot < 0 || minSlot > this.m_capalility - 1)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				object @lock;
				Monitor.Enter(@lock = this.m_lock);
				try
				{
					for (int i = minSlot; i < this.m_capalility; i++)
					{
						if (this.m_items[i] == null)
						{
							count++;
						}
					}
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				result = count;
			}
			return result;
		}
		public virtual void UseItem(ItemInfo item)
		{
			bool changed = false;
			if (!item.IsBinds && (item.Template.BindType == 2 || item.Template.BindType == 3))
			{
				item.IsBinds = true;
				changed = true;
			}
			if (!item.IsUsed)
			{
				item.IsUsed = true;
				item.BeginDate = DateTime.Now;
				changed = true;
			}
			if (changed)
			{
				this.OnPlaceChanged(item.Place);
			}
		}
		public virtual void UpdateItem(ItemInfo item)
		{
			if (item.BagType == this.m_type)
			{
				if (item.Count <= 0)
				{
					this.RemoveItem(item, eItemRemoveType.Other);
				}
				else
				{
					this.OnPlaceChanged(item.Place);
				}
			}
		}
		protected void OnPlaceChanged(int place)
		{
			List<int> changedPlaces;
			Monitor.Enter(changedPlaces = this.m_changedPlaces);
			try
			{
				if (!this.m_changedPlaces.Contains(place))
				{
					this.m_changedPlaces.Add(place);
				}
			}
			finally
			{
				Monitor.Exit(changedPlaces);
			}
			if (this.m_changeCount <= 0 && this.m_changedPlaces.Count > 0)
			{
				this.UpdateChangedPlaces();
			}
		}
		public void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changeCount);
		}
		public void CommitChanges()
		{
			int changes = Interlocked.Decrement(ref this.m_changeCount);
			if (changes < 0)
			{
				//if (AbstractInventory.log.IsErrorEnabled)
				{
					AbstractInventory.log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref this.m_changeCount, 0);
			}
			if (changes <= 0 && this.m_changedPlaces.Count > 0)
			{
				this.UpdateChangedPlaces();
			}
		}
		public virtual void UpdateChangedPlaces()
		{
			List<int> changedPlaces;
			Monitor.Enter(changedPlaces = this.m_changedPlaces);
			try
			{
				this.m_changedPlaces.Clear();
			}
			finally
			{
				Monitor.Exit(changedPlaces);
			}
		}
	}
}
