using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class PlayerEquipInventory : PlayerInventory
	{
		private const int BAG_START = 31;
		private static readonly int[] StyleIndex = new int[]
		{
			1,
			2,
			3,
			4,
			5,
			6,
			11,
			13,
			14
		};
		public PlayerEquipInventory(GamePlayer player) : base(player, true, 80, 0, 31, true)
		{
		}
		public override void LoadFromDatabase()
		{
			base.BeginChanges();
			try
			{
				base.LoadFromDatabase();
				List<ItemInfo> overdueItems = new List<ItemInfo>();
                //<31? 身上
				for (int i = 0; i < 31; i++)
				{
					ItemInfo item = this.m_items[i];
					if (this.m_items[i] != null && !this.m_items[i].IsValidItem())
					{
						int slot = base.FindFirstEmptySlot(31);
						if (slot >= 0)
						{
							this.MoveItem(item.Place, slot, item.Count);
						}
						else
						{
							overdueItems.Add(item);
						}
					}
				}
				if (overdueItems.Count > 0)
				{
					this.m_player.SendItemsToMail(overdueItems, null, null, eMailType.ItemOverdue);
					this.m_player.Out.SendMailResponse(this.m_player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
			}
			finally
			{
				base.CommitChanges();
			}
		}
		public override void LoadItems(ItemInfo[] list)
		{
			base.BeginChanges();
			try
			{
				base.LoadItems(list);
			}
			finally
			{
				base.CommitChanges();
			}
		}
		public override bool MoveItem(int fromSlot, int toSlot, int count)
		{
			bool result;
			if (this.m_items[fromSlot] == null)
			{
				result = false;
			}
			else
			{
				if (this.IsEquipSlot(fromSlot) && !this.IsEquipSlot(toSlot) && this.m_items[toSlot] != null && this.m_items[toSlot].Template.CategoryID != this.m_items[fromSlot].Template.CategoryID)
				{
					if (!this.CanEquipSlotContains(fromSlot, this.m_items[toSlot].Template))
					{
						toSlot = base.FindFirstEmptySlot(31);
					}
				}
				else
				{
					if (this.IsEquipSlot(toSlot))
					{
						if (!this.CanEquipSlotContains(toSlot, this.m_items[fromSlot].Template))
						{
							this.UpdateItem(this.m_items[fromSlot]);
							result = false;
							return result;
						}
						if (!this.m_player.CanEquip(this.m_items[fromSlot].Template) || !this.m_items[fromSlot].IsValidItem())
						{
							this.UpdateItem(this.m_items[fromSlot]);
							result = false;
							return result;
						}
					}
					if (this.IsEquipSlot(fromSlot))
					{
						if (this.m_items[toSlot] != null && (!this.CanEquipSlotContains(fromSlot, this.m_items[toSlot].Template) || !this.m_items[toSlot].IsValidItem()))
						{
							this.UpdateItem(this.m_items[toSlot]);
							result = false;
							return result;
						}
					}
				}
				result = base.MoveItem(fromSlot, toSlot, count);
			}
			return result;
		}
		public override void UpdateChangedPlaces()
		{
			int[] changedSlot = this.m_changedPlaces.ToArray();
			bool updateStyle = false;
			int[] array = changedSlot;
			for (int j = 0; j < array.Length; j++)
			{
				int i = array[j];
				if (this.IsEquipSlot(i))
				{
					ItemInfo item = this.GetItemAt(i);
					if (item != null)
					{
						this.m_player.OnUsingItem(item.TemplateID);
						item.IsBinds = true;
						if (!item.IsUsed)
						{
							item.IsUsed = true;
							item.BeginDate = DateTime.Now;
						}
					}
					updateStyle = true;
					break;
				}
			}
			base.UpdateChangedPlaces();
			if (updateStyle)
			{
				this.UpdatePlayerProperties();
			}
		}
		public void UpdatePlayerProperties()
		{
			this.m_player.BeginChanges();
			try
			{
				int attack = 0;
				int defence = 0;
				int agility = 0;
				int lucky = 0;
				int strengthenLevel = 0;
				string style = "";
				string color = "";
				string skin = "";
				object @lock;
				Monitor.Enter(@lock = this.m_lock);
				try
				{
					style = ((this.m_items[0] == null) ? "" : this.m_items[0].TemplateID.ToString());
					color = ((this.m_items[0] == null) ? "" : this.m_items[0].Color);
					skin = ((this.m_items[5] == null) ? "" : this.m_items[5].Skin);
					ItemInfo weapon = this.m_items[6];
					for (int i = 0; i < 31; i++)
					{
						ItemInfo item = this.m_items[i];
						if (item != null && item.IsValidItem())
						{
							attack += item.Attack;
							defence += item.Defence;
							agility += item.Agility;
							lucky += item.Luck;
							strengthenLevel = ((strengthenLevel > item.StrengthenLevel) ? strengthenLevel : item.StrengthenLevel);
							this.AddProperty(item, ref attack, ref defence, ref agility, ref lucky);
						}
					}
					this.EquipBuffer();
					for (int i = 0; i < PlayerEquipInventory.StyleIndex.Length; i++)
					{
						style += ",";
						color += ",";
						if (this.m_items[PlayerEquipInventory.StyleIndex[i]] != null)
						{
							style += this.m_items[PlayerEquipInventory.StyleIndex[i]].TemplateID;
							color += this.m_items[PlayerEquipInventory.StyleIndex[i]].Color;
						}
					}
				}
				finally
				{
					Monitor.Exit(@lock);
				}
				this.m_player.UpdateBaseProperties(attack, defence, agility, lucky);
				this.m_player.UpdateStyle(style, color, skin);
				this.GetUserNimbus();
				this.m_player.ApertureEquip(strengthenLevel);
				this.m_player.UpdateWeapon(this.m_items[6]);
				this.m_player.UpdateSecondWeapon(this.m_items[15]);
				this.m_player.UpdateFightPower();
			}
			finally
			{
				this.m_player.CommitChanges();
			}
		}
        public int FindItemEpuipSlot(ItemTemplateInfo item)
        {
            int result;
            switch (item.CategoryID)
            {
                case 8:
                    if (this.m_items[7] == null)
                    {
                        result = 7;
                        return result;
                    }
                    result = 8;
                    return result;
                case 9:
                    if (this.m_items[9] == null)
                    {
                        result = 9;
                        return result;
                    }
                    result = 10;
                    return result;
                case 13:
                    result = 11;
                    return result;
                case 14:
                    result = 12;
                    return result;
                case 15:
                    result = 13;
                    return result;
                case 16:
                    result = 14;
                    return result;
                case 17:
                    result = 15;
                    return result;
                case 18:
                    result = 16;
                    return result;
                case 19:
                case 20:
                    result = 17;
                    while (m_items[result] != null&&result<=26)
                    {
                        result++;
                    }
                    return result;
            }
            result = item.CategoryID - 1;
            return result;
        }
        public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
		{
			bool result;
			if (temp.CategoryID == 8)  //手镯 78
			{
				result = (slot == 7 || slot == 8);
			}
			else
			{
				if (temp.CategoryID == 9)
				{
					if (temp.TemplateID == 9022 || temp.TemplateID == 9122 || temp.TemplateID == 9222 || temp.TemplateID == 9322 || temp.TemplateID == 9422 || temp.TemplateID == 9522)
					{
						result = (slot == 16);//婚戒16
					}
					else
					{
						result = (slot == 9 || slot == 10);//戒指910
					}
				}
				else
				{
                    var a = temp.CategoryID;
                    switch (a)
                    {
                        case 13:
                            result = (slot == 11); //套装11
                            break;
                        case 14:
                            result = (slot == 12); //项链12
                            break;
                        case 15:
                            result = (slot == 13); //翅膀13
                            break;
                        case 16:
                            result = (slot == 14); //泡泡14
                            break;
                        case 17:
                            result = (slot == 15); //副手15
                            break;
                        case 18:
                            result = (slot == 17); //灵虫17
                            break;
                        case 19:
                        case 20:
                            result = (slot >= 18 && slot <= 27); //神器 18 19 20 21 22 23 24 25 26 27
                            break;
                        default:
                            result = (a - 1 == slot); //其他0 - 6
                            break;
                    }									
				}
			}
			return result;
		}
		public bool IsEquipSlot(int slot)
		{
			return slot >= 0 && slot < 31;
		}
		public void GetUserNimbus()
		{
			int i = 0;
			int j = 0;
			for (int k = 0; k < 31; k++)
			{
				ItemInfo item = this.GetItemAt(k);
				if (item != null)
				{
					if (item.StrengthenLevel >= 1 && item.StrengthenLevel <= 3)
					{
						if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
						{
							i = 1;
						}
						if (item.Template.CategoryID == 7)
						{
							j = 1;
						}
					}
					else if (item.StrengthenLevel >= 4 && item.StrengthenLevel <= 6)
					{
						if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
						{
							i = 2;
						}
						if (item.Template.CategoryID == 7)
						{
							j = 2;
						}
					}
					else if (item.StrengthenLevel >= 7 && item.StrengthenLevel <= 9)
					{
						if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        {                            
                            {
                                i = 3;
                            }
						}
						if (item.Template.CategoryID == 7)
						{
                            {
                                j = 3;
                            }
						}
					}
                    else if (item.StrengthenLevel >= 10 && item.StrengthenLevel <= 12)
                    {
                        if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        {
                            {
                                i = 4;
                            }
                        }
                        if (item.Template.CategoryID == 7)
                        {
                            {
                                j = 4;
                            }
                        }
                    }
                    else if (item.StrengthenLevel >=13  && item.StrengthenLevel <= 15)
                    {
                        if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        {
                            {
                                i = 5;
                            }
                        }
                        if (item.Template.CategoryID == 7)
                        {
                            {
                                j = 5;
                            }
                        }
                    }
                    else if (item.StrengthenLevel >= 16 && item.StrengthenLevel <= 18)
                    {
                        if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        {
                            {
                                i = 6;
                            }
                        }
                        if (item.Template.CategoryID == 7)
                        {
                            {
                                j = 6;
                            }
                        }
                    }
                    else if (item.StrengthenLevel >= 19)
                    {
                        if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        {
                            {
                                i = 7;
                            }
                        }
                        if (item.Template.CategoryID == 7)
                        {
                            {
                                j = 7;
                            }
                        }
                    }
                }
			}
			this.m_player.PlayerCharacter.Nimbus = i * 100 + j;
			this.m_player.Out.SendUpdatePublicPlayer(this.m_player.PlayerCharacter);
		}
		public void EquipBuffer()
		{
			this.m_player.EquipEffect.Clear();
			for (int i = 0; i < 31; i++)
			{
				ItemInfo item = this.GetItemAt(i);
				if (item != null)
				{
					string[] hole = item.Template.Hole.Split(new char[]
					{
						'|'
					});
					if (item.Hole1 > 0 && item.StrengthenLevel >= Convert.ToInt32(hole[0].Split(new char[]
					{
						','
					})[0]))
					{
						if (ItemMgr.FindItemTemplate(item.Hole1).Property2 != 3)
						{
							this.m_player.EquipEffect.Add(item.Hole1);
						}
					}
					if (item.Hole2 > 0 && item.StrengthenLevel >= Convert.ToInt32(hole[1].Split(new char[]
					{
						','
					})[0]))
					{
						if (ItemMgr.FindItemTemplate(item.Hole2).Property2 != 3)
						{
							this.m_player.EquipEffect.Add(item.Hole2);
						}
					}
					if (item.Hole3 > 0 && item.StrengthenLevel >= Convert.ToInt32(hole[2].Split(new char[]
					{
						','
					})[0]))
					{
						if (ItemMgr.FindItemTemplate(item.Hole3).Property2 != 3)
						{
							this.m_player.EquipEffect.Add(item.Hole3);
						}
					}
					if (item.Hole4 > 0 && item.StrengthenLevel >= Convert.ToInt32(hole[3].Split(new char[]
					{
						','
					})[0]))
					{
						if (ItemMgr.FindItemTemplate(item.Hole4).Property2 != 3)
						{
							this.m_player.EquipEffect.Add(item.Hole4);
						}
					}
					if (item.Hole5 > 0 && item.StrengthenLevel >= Convert.ToInt32(hole[4].Split(new char[]
					{
						','
					})[0]))
					{
						if (ItemMgr.FindItemTemplate(item.Hole5).Property2 != 3)
						{
							this.m_player.EquipEffect.Add(item.Hole5);
						}
					}
					if (item.Hole6 > 0 && item.StrengthenLevel >= Convert.ToInt32(hole[5].Split(new char[]
					{
						','
					})[0]))
					{
						if (ItemMgr.FindItemTemplate(item.Hole6).Property2 != 3)
						{
							this.m_player.EquipEffect.Add(item.Hole6);
						}
					}
				}
			}
		}
		public void AddProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky)
		{
			if (item != null)
			{
				if (item.Hole1 > 0)
				{
					this.AddBaseProperty(item.Hole1, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole2 > 0)
				{
					this.AddBaseProperty(item.Hole2, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole3 > 0)
				{
					this.AddBaseProperty(item.Hole3, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole4 > 0)
				{
					this.AddBaseProperty(item.Hole4, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole5 > 0)
				{
					this.AddBaseProperty(item.Hole5, ref attack, ref defence, ref agility, ref lucky);
				}
				if (item.Hole6 > 0)
				{
					this.AddBaseProperty(item.Hole6, ref attack, ref defence, ref agility, ref lucky);
				}
			}
		}
		public void AddBaseProperty(int templateid, ref int attack, ref int defence, ref int agility, ref int lucky)
		{
			ItemTemplateInfo temp = ItemMgr.FindItemTemplate(templateid);
			if (temp != null)
			{
				if (temp.CategoryID == 11 && temp.Property1 == 31 && temp.Property2 == 3)
				{
					attack += temp.Property3;
					defence += temp.Property4;
					agility += temp.Property5;
					lucky += temp.Property6;
				}
			}
		}
	}
}
