using Game.Base.Packets;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(60, "隐藏装备")]
	public class UserHideItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			bool hide = packet.ReadBoolean();
			int categroyID = packet.ReadInt();
			int num = categroyID;
			if (num == 13)
			{
				categroyID = 3;
			}
			client.Player.HideEquip(categroyID, hide);
			return 0;
		}
	}
}
