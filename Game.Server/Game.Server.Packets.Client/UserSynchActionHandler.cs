using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(36, "用户同步动作")]
	public class UserSynchActionHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int toUser = packet.ClientID;
			GamePlayer otherp = WorldMgr.GetPlayerById(toUser);
			if (otherp != null)
			{
				packet.Code = 35;
				packet.ClientID = player.PlayerCharacter.ID;
				otherp.Out.SendTCP(packet);
			}
			return 1;
		}
	}
}
