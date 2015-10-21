using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;

using SqlDataProvider.Data;
using System;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(58, "物品合成")]
	public class ItemComposeHandler : AbstractPlayerPacketHandler
	{
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			StringBuilder str = new StringBuilder();
			int mustGold = GameProperties.PRICE_COMPOSE_GOLD;
			int result2;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result2 = 0;
			}
			else
			{
				if (player.PlayerCharacter.Gold < mustGold)
				{
					player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemComposeHandler.NoMoney", new object[0]));
					result2 = 0;
				}
				else
				{
					bool isBinds = false;
					bool consortia = packet.ReadBoolean();
					ItemInfo item = player.HideBag.GetItemAt(1);
					ItemInfo stone = player.HideBag.GetItemAt(2);
					ItemInfo luck = null;
					string BeginProperty = "";
					if (item != null)
					{
						BeginProperty = item.GetPropertyString();
					}
					string AddItem = null;
					if (item != null && stone != null && item.Template.CanCompose && (item.Template.CategoryID < 10 || item.Template.CategoryID>=18) && stone.Template.CategoryID == 11 && stone.Template.Property1 == 1)
					{
						isBinds = (isBinds || item.IsBinds);
						isBinds = (isBinds || stone.IsBinds);
						str.Append(string.Concat(new object[]
						{
							item.ItemID,
							":",
							item.TemplateID,
							",",
							stone.ItemID,
							":",
							stone.TemplateID,
							","
						}));
						bool result = false;
						byte isSuccess = 1;
						bool isGod = false;
						double probability = (double)stone.Template.Property2;
						luck = player.HideBag.GetItemAt(0);
						if (luck != null)
						{
							if (luck.Template.CategoryID == 11 && luck.Template.Property1 == 3)
							{
								isBinds = (isBinds || luck.IsBinds);
								object obj = AddItem;
								AddItem = string.Concat(new object[]
								{
									obj,
									"|",
									luck.ItemID,
									":",
									luck.Template.Name,
									"|",
									stone.ItemID,
									":",
									stone.Template.Name
								});
								probability *= (double)(luck.Template.Property2 + 100);
								str.Append(string.Concat(new object[]
								{
									luck.ItemID,
									":",
									luck.TemplateID,
									","
								}));
							}
							else
							{
								probability *= 100.0;
								luck = null;
							}
						}
						else
						{
							probability *= 100.0;
						}
						if (consortia)
						{
							ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
							using (ConsortiaBussiness csbs = new ConsortiaBussiness())
							{
								ConsortiaEquipControlInfo cecInfo = csbs.GetConsortiaEuqipRiches(player.PlayerCharacter.ConsortiaID, 0, 2);
								if (info == null)
								{
									player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail", new object[0]));
								}
								else
								{
									if (player.PlayerCharacter.Riches < cecInfo.Riches)
									{
										player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission", new object[0]));
										result2 = 1;
										return result2;
									}
									probability *= 1.0 + 0.1 * (double)info.SmithLevel;
								}
							}
						}
						int rand = ItemComposeHandler.random.Next(10000);
						switch (stone.Template.Property3)
						{
						case 1:
							if (stone.Template.Property4 > item.AttackCompose)
							{
								result = true;
								if (probability > (double)rand)
								{
									isSuccess = 0;
									item.AttackCompose = stone.Template.Property4;
								}
								else
								{
									if (!isGod)
									{
									}
								}
							}
							break;
						case 2:
							if (stone.Template.Property4 > item.DefendCompose)
							{
								result = true;
								if (probability > (double)rand)
								{
									isSuccess = 0;
									item.DefendCompose = stone.Template.Property4;
								}
								else
								{
									if (!isGod)
									{
									}
								}
							}
							break;
						case 3:
							if (stone.Template.Property4 > item.AgilityCompose)
							{
								result = true;
								if (probability > (double)rand)
								{
									isSuccess = 0;
									item.AgilityCompose = stone.Template.Property4;
								}
								else
								{
									if (!isGod)
									{
									}
								}
							}
							break;
						case 4:
							if (stone.Template.Property4 > item.LuckCompose)
							{
								result = true;
								if (probability > (double)rand)
								{
									isSuccess = 0;
									item.LuckCompose = stone.Template.Property4;
								}
								else
								{
									if (!isGod)
									{
									}
								}
							}
							break;
						}
						if (result)
						{
							item.IsBinds = isBinds;
							if (isSuccess != 0)
							{
								str.Append("false!");
								result = false;
								if (!isGod)
								{
									if (item.Template.Level < 3)
									{
									}
								}
							}
							else
							{
								str.Append("true!");
								result = true;
								player.OnItemCompose(stone.TemplateID);
							}
							//LogMgr.LogItemAdd(player.PlayerCharacter.ID, LogItemType.Compose, BeginProperty, item, AddItem, Convert.ToInt32(result));
							stone.Count--;
							player.UpdateItem(stone);
							if (luck != null)
							{
								luck.Count--;
								player.UpdateItem(luck);
							}
							player.RemoveGold(mustGold);
							player.HideBag.UpdateItem(item);
							pkg.WriteByte(isSuccess);
							player.Out.SendTCP(pkg);
							player.SaveIntoDatabase();
						}
						else
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.NoLevel", new object[0]));
						}
					}
					else
					{
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemComposeHandler.Fail", new object[0]));
					}
					result2 = 0;
				}
			}
			return result2;
		}
	}
}
