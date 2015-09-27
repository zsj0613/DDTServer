using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(165, "改变状态")]
	internal class FriendStateChangeHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			packet.ClientID = player.PlayerCharacter.ID;
			bool state = packet.ReadBoolean();
			GameServer.Instance.LoginServer.SendPacket(packet);
			WorldMgr.ChangePlayerState(packet.ClientID, state, player.PlayerCharacter.ConsortiaID);
			return 0;
		}
	}
}
