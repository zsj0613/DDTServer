using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(146, "删除申请")]
	public class ConsortiaApplyAllyDeleteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			bool result = false;
			string msg = "ConsortiaApplyAllyDeleteHandler.Failed";
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				if (db.DeleteConsortiaApplyAlly(id, player.PlayerCharacter.ID, player.PlayerCharacter.ConsortiaID, ref msg))
				{
					msg = "ConsortiaApplyAllyDeleteHandler.Success";
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
