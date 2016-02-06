using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Client
{
	[PacketHandler(17, "Client scene ready1")]
	public class UserSceneReadyHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null)
			{
				GSPacketIn pkgMsg = null;
				List<GamePlayer> players = player.CurrentRoom.GetPlayers();
				foreach (GamePlayer p in players)
				{
					if (p != player)
					{
						if (pkgMsg == null)
						{
							pkgMsg = p.Out.SendSceneAddPlayer(player);
						}
						else
						{
							p.Out.SendTCP(pkgMsg);
						}
						player.Out.SendSceneRemovePlayer(p);
					}
				}
			}
			return 1;
		}
	}
}
