using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(55, "出售道具")]
	public class PropSellHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int gold = 0;
			int money = 0;
			int offer = 0;
			int gifttoken = 0;
			int type = 1;
			List<int> needitemsinfo = new List<int>();
			int index = packet.ReadInt();
			int GoodsID = packet.ReadInt();
			ItemInfo item = player.FightBag.GetItemAt(index);
			if (item != null)
			{
				player.FightBag.RemoveItem(item, eItemRemoveType.Sell);
				ShopItemInfo shopitem = ShopMgr.GetShopItemInfoById(GoodsID);
				needitemsinfo = ShopMgr.GetShopItemBuyConditions(shopitem, type, ref gold, ref money, ref offer, ref gifttoken);
				player.AddGold(gold);
			}
			return 0;
		}
	}
}
