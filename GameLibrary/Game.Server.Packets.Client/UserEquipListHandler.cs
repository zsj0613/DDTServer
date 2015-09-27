using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(74, "获取用户装备")]
	public class UserEquipListHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			GamePlayer player = WorldMgr.GetPlayerById(id);
			PlayerInfo info;
			List<ItemInfo> items;
			if (player != null)
			{
				info = player.PlayerCharacter;
				items = player.MainBag.GetItems(0, 31);
			}
			else
			{
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					info = pb.GetUserSingleByUserID(id);
					items = pb.GetUserEuqip(id);
				}
			}
			if (info != null && items != null)
			{
				client.Out.SendUserEquip(info, items);
			}
			return 0;
		}
	}
}
