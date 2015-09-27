using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(72, "大喇叭")]
	public class BigBugleHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			ItemInfo item = player.PropBag.GetItemByCategoryID(0, 11, 5);
			if (item != null)
			{
				player.PropBag.RemoveCountFromStack(item, 1, eItemRemoveType.Use);
				player.OnUsingItem(item.TemplateID);
				int senderID = packet.ReadInt();
				string senderName = packet.ReadString();
				string msg = packet.ReadString();
				player.Out.SendBigSpeakerMsg(player, msg);
			}
			return 0;
		}
	}
}
