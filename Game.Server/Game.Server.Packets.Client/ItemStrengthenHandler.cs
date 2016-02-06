using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Lsj.Util;
using Lsj.Util.Config;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Packets.Client
{
    [PacketHandler(59, "物品强化")]
    public class ItemStrengthenHandler : AbstractPlayerPacketHandler
    {
        private static ThreadSafeRandom random = new ThreadSafeRandom();
        public override int HandlePacket(GamePlayer player, GSPacketIn packet)
        {
            int pRICE_STRENGHTN_GOLD = AppConfig.AppSettings["StrengthenGold"].ConvertToInt(1000);
            bool flag = player.PlayerCharacter.Gold < pRICE_STRENGHTN_GOLD;
            int num;
            int result;
            if (flag)
            {
                player.Out.SendMessage(0, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoMoney", new object[0]));
                num = 0;
            }
            else
            {
                GSPacketIn gSPacketIn = packet.Clone();
                gSPacketIn.ClearContext();
                ItemInfo itemInfo = player.HideBag.GetItemAt(5);
                bool flag2 = itemInfo != null && itemInfo.Template.CanStrengthen && itemInfo.Count == 1;
                if (flag2)
                {
                    StrengthenInfo strengthenInfo = StrengthenMgr.FindStrengthenInfo(itemInfo.StrengthenLevel + 1);
                    bool flag3 = strengthenInfo == null;
                    if (flag3)
                    {
                        bool flag4 = itemInfo.StrengthenLevel == 9 || itemInfo.StrengthenLevel == 15;
                        if (flag4)
                        {
                            player.Out.SendMessage(0, LanguageMgr.GetTranslation("ItemStrengthenHandler.FullStrengthLevel", new object[0]));
                        }
                        else
                        {
                            player.Out.SendMessage(0, LanguageMgr.GetTranslation("ItemStrengthenHandler.NoStrength", new object[0]));
                        }
                        num = 0;
                        result = num;
                        return result;
                    }
                    bool flag5 = itemInfo.IsBinds;
                    bool flag6 = packet.ReadBoolean();
                    StringBuilder stringBuilder = new StringBuilder();
                    string propertyString = itemInfo.GetPropertyString();
                    double num2 = 0.0;
                    bool flag7 = false;
                    ItemInfo itemInfo2 = player.HideBag.GetItemAt(3);
                    bool flag8 = itemInfo2 != null && itemInfo2.Template.CategoryID == 11 && itemInfo2.Template.Property1 == 7;
                    if (flag8)
                    {
                        flag5 |= itemInfo2.IsBinds;
                        stringBuilder.Append(itemInfo2.ToShortString());
                        flag7 = true;
                    }
                    else
                    {
                        itemInfo2 = null;
                    }
                    bool flag9 = false;
                    List<ItemInfo> list = new List<ItemInfo>();
                    ItemInfo itemAt = player.HideBag.GetItemAt(0);
                    bool flag10 = itemAt != null && itemAt.Template.CategoryID == 11 && (itemAt.Template.Property1 == 2 || itemAt.Template.Property1 == 35) && !list.Contains(itemAt);
                    if (flag10)
                    {
                        bool flag11 = itemInfo.StrengthenLevel >= 15 && itemAt.Template.Property8 == 2;
                        if (flag11)
                        {
                            player.Out.SendMessage(0, "不能使用[" + itemAt.Template.Name + "]来强化此装备.");
                            num = 1;
                            result = num;
                            return result;
                        }
                        flag5 |= itemAt.IsBinds;
                        flag9 |= (itemAt.Template.Property1 == 35 && itemAt.Template.CategoryID == 11);
                        num2 += (double)itemAt.Template.Property2;
                        stringBuilder.Append(itemAt.ToShortString());
                        list.Add(itemAt);
                    }
                    ItemInfo itemAt2 = player.HideBag.GetItemAt(1);
                    bool flag12 = itemAt2 != null && itemAt2.Template.CategoryID == 11 && (itemAt2.Template.Property1 == 2 || itemAt2.Template.Property1 == 35) && !list.Contains(itemAt2);
                    if (flag12)
                    {
                        bool flag13 = itemInfo.StrengthenLevel >= 15 && itemAt2.Template.Property8 == 2;
                        if (flag13)
                        {
                            player.Out.SendMessage(0, "不能使用[" + itemAt2.Template.Name + "]来强化此装备.");
                            num = 1;
                            result = num;
                            return result;
                        }
                        flag5 |= itemAt2.IsBinds;
                        flag9 |= (itemAt2.Template.Property1 == 35 && itemAt2.Template.CategoryID == 11);
                        num2 += (double)itemAt2.Template.Property2;
                        stringBuilder.Append(itemAt2.ToShortString());
                        list.Add(itemAt2);
                    }
                    ItemInfo itemAt3 = player.HideBag.GetItemAt(2);
                    bool flag14 = itemAt3 != null && itemAt3.Template.CategoryID == 11 && (itemAt3.Template.Property1 == 2 || itemAt3.Template.Property1 == 35) && !list.Contains(itemAt3);
                    if (flag14)
                    {
                        bool flag15 = itemInfo.StrengthenLevel >= 15 && itemAt3.Template.Property8 == 2;
                        if (flag15)
                        {
                            player.Out.SendMessage(0, "不能使用[" + itemAt3.Template.Name + "]来强化此装备.");
                            num = 1;
                            result = num;
                            return result;
                        }
                        flag5 |= itemAt3.IsBinds;
                        flag9 |= (itemAt3.Template.Property1 == 35 && itemAt3.Template.CategoryID == 11);
                        num2 += (double)itemAt3.Template.Property2;
                        stringBuilder.Append(itemAt3.ToShortString());
                        list.Add(itemAt3);
                    }
                    ItemInfo itemInfo3 = player.HideBag.GetItemAt(4);
                    bool flag16 = itemInfo3 != null && itemInfo3.Template.CategoryID == 11 && itemInfo3.Template.Property1 == 3;
                    if (flag16)
                    {
                        flag5 |= itemInfo3.IsBinds;
                        num2 *= (double)(itemInfo3.Template.Property2 + 100);
                        stringBuilder.Append(itemInfo3.ToShortString());
                    }
                    else
                    {
                        num2 *= 100.0;
                        itemInfo3 = null;
                    }
                    bool flag17 = false;
                    ConsortiaInfo consortiaInfo = null;
                    bool flag18 = flag6;
                    if (flag18)
                    {
                        consortiaInfo = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
                        bool flag19 = consortiaInfo == null;
                        if (flag19)
                        {
                            player.Out.SendMessage(0, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail", new object[0]));
                        }
                        else
                        {
                            using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
                            {
                                ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(player.PlayerCharacter.ConsortiaID, 0, 2);
                                bool flag20 = player.PlayerCharacter.Riches < consortiaEuqipRiches.Riches;
                                if (flag20)
                                {
                                    player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission", new object[0]));
                                    num = 1;
                                    result = num;
                                    return result;
                                }
                                flag17 = true;
                            }
                        }
                    }
                    bool flag21 = list.Count >= 1;
                    if (flag21)
                    {
                        StrengthenGoodsInfo strengthenGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(itemInfo.StrengthenLevel + 1, itemInfo.TemplateID);
                        ItemInfo itemInfo4 = null;
                        bool flag22 = strengthenGoodsInfo != null;
                        if (flag22)
                        {
                            itemInfo4 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(strengthenGoodsInfo.GainEquip), 1, 116);
                            bool flag23 = itemInfo4 == null;
                            if (flag23)
                            {
                                num = 0;
                                result = num;
                                return result;
                            }
                        }
                        string text = null;
                        player.HideBag.BeginChanges();
                        try
                        {
                            num2 /= (double)strengthenInfo.Rock;
                            int num3;
                            for (int i = 0; i < list.Count; i = num3 + 1)
                            {
                                player.HideBag.RemoveCountFromStack(list[i], 1, (eItemRemoveType)5);
                                num3 = i;
                            }
                            bool flag24 = itemInfo3 != null;
                            if (flag24)
                            {
                                player.HideBag.RemoveCountFromStack(itemInfo3, 1, (eItemRemoveType)5);
                            }
                            bool flag25 = itemInfo2 != null;
                            if (flag25)
                            {
                                player.HideBag.RemoveCountFromStack(itemInfo2, 1, (eItemRemoveType)5);
                            }
                            bool flag26 = flag17;
                            if (flag26)
                            {
                                num2 *= 1.0 + 0.1 * (double)consortiaInfo.SmithLevel;
                            }
                            itemInfo.IsBinds=flag5;
                            int num4 = 1;
                            bool flag27 = num2 > (double)ItemStrengthenHandler.random.Next(10000);
                            if (flag27)
                            {
                                num4 = 0;
                                bool flag28 = strengthenGoodsInfo != null && itemInfo4 != null;
                                if (flag28)
                                {
                                    StrengthenMgr.InheritProperty(itemInfo, itemInfo4);
                                    itemInfo4.StrengthenLevel=(itemInfo.StrengthenLevel + 1);
                                    player.HideBag.RemoveItem(itemInfo, (eItemRemoveType)5);
                                    player.HideBag.AddItemTo(itemInfo4, 5);
                                    text = string.Format("temp_place_{0}", itemInfo.ItemID);
                                    itemInfo = itemInfo4;
                                    bool flag29 = itemInfo.ItemID == 0;
                                    if (flag29)
                                    {
                                        using (PlayerBussiness playerBussiness = new PlayerBussiness())
                                        {
                                            playerBussiness.AddGoods(itemInfo);
                                        }
                                    }
                                }
                                else
                                {
                                    ItemInfo itemInfo5 = itemInfo;
                                    num3 = itemInfo5.StrengthenLevel;
                                    itemInfo5.StrengthenLevel=(num3 + 1);
                                }
                                player.OnItemStrengthen(itemInfo.Template.CategoryID, itemInfo.StrengthenLevel);
                                bool flag30 = itemInfo.StrengthenLevel >= 7;
                                if (flag30)
                                {
                                    string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation", new object[]
                                    {
                                        player.PlayerCharacter.NickName,
                                        itemInfo.StrengthenLevel
                                    });
                                    GSPacketIn packet2 = WorldMgr.SendSysNotice(translation, itemInfo, player);
                                    GameServer.Instance.LoginServer.SendPacket(packet2);
                                }
                            }
                            else
                            {
                                num4 = 1;
                                bool flag31 = !flag7;
                                if (flag31)
                                {
                                    StrengthenGoodsInfo strengthenGoodsInfo2 = StrengthenMgr.FindStrengthenFailGoodsInfo(itemInfo.StrengthenLevel, itemInfo.TemplateID);
                                    itemInfo.StrengthenLevel=((itemInfo.StrengthenLevel == 0) ? 0 : (itemInfo.StrengthenLevel - 1));
                                    bool flag32 = strengthenGoodsInfo2 != null;
                                    if (flag32)
                                    {
                                        ItemInfo itemInfo6 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(strengthenGoodsInfo2.CurrentEquip), 1, 116);
                                        bool flag33 = itemInfo6 == null;
                                        if (flag33)
                                        {
                                            num = 0;
                                            result = num;
                                            return result;
                                        }
                                        StrengthenMgr.InheritProperty(itemInfo, itemInfo6);
                                        itemInfo6.StrengthenLevel=(itemInfo.StrengthenLevel);
                                        player.HideBag.RemoveItem(itemInfo, (eItemRemoveType)5);
                                        player.HideBag.AddItemTo(itemInfo6, 5);
                                    }
                                }
                            }
                            player.HideBag.UpdateItem(itemInfo);
                            gSPacketIn.WriteByte((byte)num4);
                            gSPacketIn.WriteBoolean(itemInfo.IsOpenHole);
                            player.RemoveGold(pRICE_STRENGHTN_GOLD);
                            //LogMgr.LogItemAdd(player.PlayerCharacter.ID, 1, propertyString, itemInfo, stringBuilder.ToString(), (num4 == 1) ? 0 : 1);
                        }
                        finally
                        {
                            player.HideBag.CommitChanges();
                            player.Out.SendTCP(gSPacketIn);
                        }
                        player.SaveIntoDatabase();
                        bool flag34 = text != null && player.TempProperties.ContainsKey(text);
                        if (flag34)
                        {
                            int num5 = (int)player.TempProperties[text];
                            player.TempProperties.Remove(text);
                            text = string.Format("temp_place_{0}", itemInfo.ItemID);
                            player.TempProperties.Add(text, num5);
                        }
                    }
                    else
                    {
                        player.Out.SendMessage(0, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1", new object[0]) + 1 + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2", new object[0]));
                    }
                }
                else
                {
                    player.Out.SendMessage(0, LanguageMgr.GetTranslation("ItemStrengthenHandler.Success", new object[0]));
                }
                num = 0;
            }
            result = num;
            return result;
        }
    }
}
