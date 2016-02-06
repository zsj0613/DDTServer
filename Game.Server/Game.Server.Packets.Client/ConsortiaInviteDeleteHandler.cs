using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(143, "删除公会邀请")]
	internal class ConsortiaInviteDeleteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			bool result = false;
			string msg = "ConsortiaInviteDeleteHandler.Failed";
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				if (db.DeleteConsortiaInviteUsers(id, player.PlayerCharacter.ID))
				{
					msg = "ConsortiaInviteDeleteHandler.Success";
					result = true;
				}
			}
			packet.WriteBoolean(result);
			packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
			player.Out.SendTCP(packet);
			return 0;
		}
	}
}
