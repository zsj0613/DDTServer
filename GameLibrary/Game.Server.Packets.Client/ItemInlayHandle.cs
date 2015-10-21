using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(121, "物品镶嵌")]
	public class ItemInlayHandle : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			int ItemBagType = packet.ReadInt();
			int ItemPlace = packet.ReadInt();
			int HoleNum = packet.ReadInt();
			int GemBagType = packet.ReadInt();
			int GemPlace = packet.ReadInt();
			ItemInfo Item = player.GetItemAt((eBageType)ItemBagType, ItemPlace);
			ItemInfo Gem = player.GetItemAt((eBageType)GemBagType, GemPlace);
			string BeginProperty = Item.GetPropertyString();
			string AddItem = "";
			int Glod = 2000;
			int result2;
			if (Item == null || Gem == null || Gem.Template.Property1 != 31)
			{
				result2 = 0;
			}
			else
			{
				if (player.PlayerCharacter.Gold > Glod)
				{
					string[] Hole = Item.Template.Hole.Split(new char[]
					{
						'|'
					});
					if (HoleNum > 0 && HoleNum < 7)
					{
						player.RemoveGold(Glod);
						bool result = false;
						switch (HoleNum)
						{
						case 1:
							if (Item.Hole1 >= 0)
							{
								string[] str = Hole[0].Split(new char[]
								{
									','
								});
								if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
								{
									Item.Hole1 = Gem.TemplateID;
									object obj = AddItem;
									AddItem = string.Concat(new object[]
									{
										obj,
										",",
										Gem.ItemID,
										",",
										Gem.Template.Name
									});
									result = true;
								}
							}
							break;
						case 2:
							if (Item.Hole2 >= 0)
							{
								string[] str = Hole[1].Split(new char[]
								{
									','
								});
								if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
								{
									Item.Hole2 = Gem.TemplateID;
									object obj = AddItem;
									AddItem = string.Concat(new object[]
									{
										obj,
										",",
										Gem.ItemID,
										",",
										Gem.Template.Name
									});
									result = true;
								}
							}
							break;
						case 3:
							if (Item.Hole3 >= 0)
							{
								string[] str = Hole[2].Split(new char[]
								{
									','
								});
								if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
								{
									Item.Hole3 = Gem.TemplateID;
									object obj = AddItem;
									AddItem = string.Concat(new object[]
									{
										obj,
										",",
										Gem.ItemID,
										",",
										Gem.Template.Name
									});
									result = true;
								}
							}
							break;
						case 4:
							if (Item.Hole4 >= 0)
							{
								string[] str = Hole[3].Split(new char[]
								{
									','
								});
								if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
								{
									Item.Hole4 = Gem.TemplateID;
									object obj = AddItem;
									AddItem = string.Concat(new object[]
									{
										obj,
										",",
										Gem.ItemID,
										",",
										Gem.Template.Name
									});
									result = true;
								}
							}
							break;
						case 5:
							if (Item.Hole5 >= 0)
							{
								string[] str = Hole[4].Split(new char[]
								{
									','
								});
								if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
								{
									Item.Hole5 = Gem.TemplateID;
									object obj = AddItem;
									AddItem = string.Concat(new object[]
									{
										obj,
										",",
										Gem.ItemID,
										",",
										Gem.Template.Name
									});
									result = true;
								}
							}
							break;
						case 6:
							if (Item.Hole6 >= 0)
							{
								string[] str = Hole[5].Split(new char[]
								{
									','
								});
								if (Convert.ToInt32(str[1]) == Gem.Template.Property2)
								{
									Item.Hole6 = Gem.TemplateID;
									object obj = AddItem;
									AddItem = string.Concat(new object[]
									{
										obj,
										",",
										Gem.ItemID,
										",",
										Gem.Template.Name
									});
									result = true;
								}
							}
							break;
						}
						if (result)
						{
							pkg.WriteInt(0);
							Gem.Count--;
							if (Item.IsBinds != Gem.IsBinds)
							{
								Item.IsBinds = true;
							}
							player.UpdateItem(Gem);
							player.UpdateItem(Item);
							player.OnItemInsert();
						}
						//LogMgr.LogItemAdd(player.PlayerCharacter.ID, LogItemType.Insert, BeginProperty, Item, AddItem, Convert.ToInt32(result));
					}
					else
					{
						pkg.WriteByte(1);
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemInlayHandle.NoPlace", new object[0]));
					}
					player.SendTCP(pkg);
				}
				else
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney", new object[0]));
				}
				result2 = 0;
			}
			return result2;
		}
	}
}
