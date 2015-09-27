using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	public class AbstractPlayerPacketHandler : AbstractClientHandler
	{
		public sealed override int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int result;
			if (client.IsConnected && client.Player != null && client.Player.PlayerState == ePlayerState.Playing)
			{
				result = this.HandlePacket(client.Player, packet);
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public virtual int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			return 0;
		}
	}
}
