using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(134, "删除进入申请")]
	public class ConsortiaApplyLoginDeleteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			bool result = false;
			string msg = "ConsortiaApplyAllyDeleteHandler.Failed";
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				if (db.DeleteConsortiaApplyUsers(id, player.PlayerCharacter.ID, player.PlayerCharacter.ConsortiaID, ref msg))
				{
					msg = ((player.PlayerCharacter.ID == 0) ? "ConsortiaApplyAllyDeleteHandler.Success" : "ConsortiaApplyAllyDeleteHandler.Success2");
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
