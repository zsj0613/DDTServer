using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(172, "及时保存玩家身上装备")]
	public class UserItemSaveHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.StyleChanged)
			{
				player.SaveIntoDatabase();
			}
			return 0;
		}
	}
}
