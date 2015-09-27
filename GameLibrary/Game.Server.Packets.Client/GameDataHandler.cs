using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(91, "游戏数据")]
	public class GameDataHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null)
			{
				packet.ClientID = player.PlayerId;
				packet.Parameter1 = player.GamePlayerId;
				player.CurrentRoom.ProcessData(packet);
			}
			return 0;
		}
	}
}
