using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(211, "游戏模式")]
	public class GameUserGameTypeHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null)
			{
				switch (packet.ReadInt())
				{
				case 0:
					player.CurrentRoom.GameType = eGameType.Free;
					goto IL_77;
				case 2:
					player.CurrentRoom.GameType = eGameType.ALL;
					player.CurrentRoom.BattleServer.Connector.SendChangeGameType(player.CurrentRoom);
					goto IL_77;
				}
				player.CurrentRoom.GameType = eGameType.Guild;
				IL_77:
				GSPacketIn pkg = player.Out.SendRoomType(player, player.CurrentRoom);
				player.CurrentRoom.SendToAll(pkg, player);
			}
			return 0;
		}
	}
}
