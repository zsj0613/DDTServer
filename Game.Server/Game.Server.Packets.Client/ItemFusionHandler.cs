using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;

using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(78, "熔化")]
	public class ItemFusionHandler : AbstractPlayerPacketHandler
	{
        private static ThreadSafeRandom random = new ThreadSafeRandom();
        public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			StringBuilder str = new StringBuilder();
			int opertionType = (int)packet.ReadByte();
			int count = 4;
			int MinValid = 2147483647;
			List<ItemInfo> items = new List<ItemInfo>();
			List<ItemInfo> appendItems = new List<ItemInfo>();
			List<eBageType> bagTypes = new List<eBageType>();
			int result2;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result2 = 1;
			}
			else
			{
                GameServer.log.Debug("Fuck1");
                for (int i = 1; i < 5; i++)
				{
					ItemInfo info = player.HideBag.GetItemAt(i);
					if (info != null)
					{
						if (items.Contains(info))
						{
							player.Out.SendMessage(eMessageType.Normal, "Bad Input");
							result2 = 1;
							return result2;
						}
						if (!info.IsValidItem())
						{
							player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.Itemvalid", new object[0]));
							result2 = 1;
							return result2;
						}
						str.Append(string.Concat(new object[]
						{
							info.ItemID,
							":",
							info.TemplateID,
							","
						}));
						items.Add(info);
						if (info.ValidDate != 0)
						{
							if (!info.IsUsed)
							{
								if (info.ValidDate < MinValid)
								{
									MinValid = info.ValidDate;
								}
							}
							else
							{
								int spareDate = info.BeginDate.AddDays((double)info.ValidDate).Subtract(DateTime.Now).Days + 1;
								if (spareDate > info.ValidDate || spareDate <= 0)
								{
									player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.ItemValidDate", new object[0]));
									result2 = 1;
									return result2;
								}
								if (spareDate < MinValid)
								{
									MinValid = spareDate;
								}
							}
						}
					}
				}
				if (MinValid == 2147483647)
				{
					MinValid = 0;
				}
				ItemInfo formul = player.HideBag.GetItemAt(0);
				ItemInfo tempitem = null;
				//string beginProperty = null;
				string AddItem = "";
				//foreach (ItemInfo item in items)
				//{
				//	ItemInfo item;
					//ItemRecordBussiness.FusionItem(item, ref beginProperty);
				//}
				if (items.Count != 4 || formul == null)
				{
                    GameServer.log.Debug("Fuck2");
                    result2 = 0;
				}
				else
				{
                    GameServer.log.Debug("Fuck3");
                    int appendCount = 0;
					List<eBageType> bagTypesAppend = new List<eBageType>();
					for (int i = 5; i < 11; i++)
					{
						ItemInfo info = player.HideBag.GetItemAt(i);
						if (info != null)
						{
							if (items.Contains(info) || appendItems.Contains(info))
							{
								player.Out.SendMessage(eMessageType.Normal, "Bad Input");
								result2 = 1;
								return result2;
							}
							str.Append(string.Concat(new object[]
							{
								info.ItemID,
								":",
								info.TemplateID,
								","
							}));
							appendItems.Add(info);
							appendCount++;
							object obj = AddItem;
							AddItem = string.Concat(new object[]
							{
								obj,
								info.ItemID,
								":",
								info.Template.Name,
								",",
								info.IsBinds,
								"|"
							});
						}
					}
					if (opertionType==0)
					{
                        GameServer.log.Debug("Fuck4");
                        bool isBind = false;
						Dictionary<int, double> previewItemList = FusionMgr.FusionPreview(items, appendItems, formul, ref isBind);
                        if (previewItemList != null)
                        {
                            GameServer.log.Debug("OnSendFusionPreview");
                            if (previewItemList.Count != 0)
                            {
                                GameServer.log.Debug("Fuck5");
                                player.Out.SendFusionPreview(player, previewItemList, isBind, MinValid);
                            }
                        }
                        else
                        {
                            GameServer.log.Debug("FusionPreview is Null");
                        }
					}
					else
					{
                        GameServer.log.Debug("Fuck5");
                        int mustGold = (count + appendCount) * 400;
						if (player.PlayerCharacter.Gold < mustGold)
						{
							player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney", new object[0]));
							result2 = 0;
							return result2;
						}
						bool isBind = false;
						bool result = false;
						ItemTemplateInfo rewardItem = FusionMgr.Fusion(items, appendItems, formul, ref isBind, ref result);
						if (rewardItem != null)
						{
                            if (rewardItem.IsOnly)
                            {
                                if (player.GetItemCount(rewardItem.TemplateID) >= 1)
                                {
                                    player.Out.SendMessage(eMessageType.ERROR, "该物品为唯一物品，只能拥有一个");
                                    return 0;
                                }

                            }
							player.BeginAllChanges();
							player.RemoveGold(mustGold);
							for (int i = 0; i < items.Count; i++)
							{
								items[i].Count--;
								player.UpdateItem(items[i]);
							}
							formul.Count--;
							player.UpdateItem(formul);
							for (int i = 0; i < appendItems.Count; i++)
							{
								appendItems[i].Count--;
								player.UpdateItem(appendItems[i]);
							}
							if (result)
							{
								str.Append(rewardItem.TemplateID + ",");
								ItemInfo item = ItemInfo.CreateFromTemplate(rewardItem, 1, 105);
								if (item == null)
								{
									result2 = 0;
									return result2;
								}
								tempitem = item;
								item.IsBinds = isBind;
								item.ValidDate = MinValid;
								player.OnItemFusion(item.Template.FusionType);
								player.Out.SendFusionResult(player, result);
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1", new object[0]) + item.Template.Name);
								if (item.TemplateID >= 201 && item.TemplateID <= 221 || item.TemplateID>=301&&item.TemplateID<=310)
								{
									string msg = LanguageMgr.GetTranslation("ItemFusionHandler.Notice", new object[]
									{
										player.PlayerCharacter.NickName,
                                        "|"
									});
									GSPacketIn sys_notice = WorldMgr.SendSysNotice(msg, item, player);
									GameServer.Instance.LoginServer.SendPacket(sys_notice);
								}
								PlayerInventory bag = player.GetItemInventory(item.Template);
								if (!bag.StackItemToAnother(item) && !player.AddItem(item))
								{
									str.Append("NoPlace");
									List<ItemInfo> its = new List<ItemInfo>();
									its.Add(item);
									player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.full", new object[0]));
									player.SendItemsToMail(its, LanguageMgr.GetTranslation("ItemFusionHandler.mail", new object[0]), LanguageMgr.GetTranslation("ItemFusionHandler.mail", new object[0]), eMailType.ItemOverdue);
									player.Out.SendMailResponse(player.PlayerId, eMailRespose.Receiver);
								}
							}
							else
							{
								str.Append("false");
								player.Out.SendFusionResult(player, result);
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed", new object[0]));
							}
							//LogMgr.LogItemAdd(player.PlayerCharacter.ID, LogItemType.Fusion, beginProperty, tempitem, AddItem, Convert.ToInt32(result));
							player.CommitAllChanges();
							player.SaveIntoDatabase();
						}
						else
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition", new object[0]));
						}
					}
					result2 = 0;
				}
			}
			return result2;
		}
	}
}
