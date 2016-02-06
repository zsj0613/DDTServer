using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(75, "删除道具")]
	public class PropDeleteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int index = packet.ReadInt();
			player.FightBag.RemoveItemAt(index, eItemRemoveType.Other);
			return 0;
		}
	}
}
