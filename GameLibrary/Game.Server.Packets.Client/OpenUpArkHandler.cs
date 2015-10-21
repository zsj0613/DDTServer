using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(63, "打开物品")]
	public class OpenUpArkHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int bagType = (int)packet.ReadByte();
			int place = packet.ReadInt();
			PlayerInventory arkBag = player.GetInventory((eBageType)bagType);
			ItemInfo goods = arkBag.GetItemAt(place);
			string full = "";
			List<ItemInfo> infos = new List<ItemInfo>();
			int result;
			if (goods != null && goods.IsValidItem() && goods.Template.CategoryID == 11 && goods.Template.Property1 == 6 && player.PlayerCharacter.Grade >= goods.Template.NeedLevel)
			{
				int money = 0;
				int gold = 0;
				int giftToken = 0;
				int[] bags = new int[3];
				int gp = 0;
				this.OpenUpItem(goods.Template.Data, bags, infos, ref gold, ref money, ref giftToken, ref gp);
				if (infos.Count == 0 && gold == 0 && money == 0 && giftToken == 0 && gp == 0)
				{
					result = 0;
					return result;
				}
				bags[goods.GetBagType()]--;
				if (player.RemoveItem(goods, eItemRemoveType.Use))
				{
					player.OnUsingItem(goods.Template.TemplateID);
					StringBuilder notice = new StringBuilder();
					StringBuilder msg = new StringBuilder();
					msg.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start", new object[0]));
					if (money != 0)
					{
						msg.Append(money + LanguageMgr.GetTranslation("OpenUpArkHandler.Money", new object[0]));
						player.AddMoney(money, LogMoneyType.Box, LogMoneyType.Box_Open);
					}
					if (gold != 0)
					{
						msg.Append(gold + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold", new object[0]));
						player.AddGold(gold);
					}
					if (giftToken != 0)
					{
						msg.Append(giftToken + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken", new object[0]));
						player.AddGiftToken(giftToken);
					}
					if (gp != 0)
					{
						msg.Append(gp + LanguageMgr.GetTranslation("OpenUpArkHandler.Gp", new object[0]));
						player.AddGpDirect(gp);
					}
					StringBuilder msga = new StringBuilder();
					foreach (ItemInfo info in infos)
					{
						msg.Append(info.Template.Name + "x" + info.Count.ToString() + ",");
						List<ItemInfo> list = ItemMgr.SpiltGoodsMaxCount(info);
						List<ItemInfo> last = new List<ItemInfo>();
						foreach (ItemInfo it in list)
						{
							if (!player.AddItem(it))
							{
								last.Add(it);
							}
						}
						if (last.Count > 0)
						{
							player.SendItemsToMail(last, LanguageMgr.GetTranslation("OpenUpArkHandler.Content1", new object[0]) + info.Template.Name + LanguageMgr.GetTranslation("OpenUpArkHandler.Content2", new object[0]), LanguageMgr.GetTranslation("OpenUpArkHandler.Title", new object[0]) + info.Template.Name + "]", eMailType.Common);
							full = LanguageMgr.GetTranslation("OpenUpArkHandler.Mail", new object[0]);
						}
					}
					foreach (ItemInfo info in infos)
					{
						player.SendItemNotice(info, goods.Template.Name, 3);
					}
					msg.Remove(msg.Length - 1, 1);
					msg.Append(".");
					player.Out.SendMessage(eMessageType.Normal, full + msg.ToString());
					if (!string.IsNullOrEmpty(full))
					{
						player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
				}
			}
			result = 1;
			return result;
		}
		public void OpenUpItem(string data, int[] bag, List<ItemInfo> infos, ref int gold, ref int money, ref int giftToken, ref int gp)
		{
			if (!string.IsNullOrEmpty(data))
			{
				ItemBoxMgr.CreateItemBox(Convert.ToInt32(data), infos, ref gold, ref money, ref giftToken, ref gp);
			}
		}
	}
}
