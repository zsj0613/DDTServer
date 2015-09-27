using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(168, "更新限量商品数量")]
	public class UpdateShopLimitCountHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			Dictionary<int, int> infos = ShopMgr.GetLimitShopItemInfo();
			pkg.WriteInt(infos.Count);
			foreach (int key in infos.Keys)
			{
				pkg.WriteInt(key);
				pkg.WriteInt(infos[key]);
			}
			player.SendTCP(pkg);
			return 0;
		}
	}
}
