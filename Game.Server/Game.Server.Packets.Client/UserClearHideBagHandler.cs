using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(122, "清空隐藏背包")]
	public class UserClearHideBagHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			player.ClearHideBag();
			return 0;
		}
	}
}
