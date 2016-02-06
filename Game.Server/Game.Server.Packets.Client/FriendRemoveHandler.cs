using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(161, "删除好友")]
	public class FriendRemoveHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			using (PlayerBussiness db = new PlayerBussiness())
			{
				if (db.DeleteFriends(player.PlayerCharacter.ID, id))
				{
					player.FriendsRemove(id);
					player.Out.SendFriendRemove(id);
				}
			}
			return 0;
		}
	}
}
