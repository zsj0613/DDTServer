using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(145, "申请通过")]
	public class ConsortiaApplyAllyPassHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			bool result = false;
			int tempID = 0;
			int state = 0;
			string msg = "ConsortiaApplyAllyPassHandler.Failed";
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				if (db.PassConsortiaApplyAlly(id, player.PlayerCharacter.ID, player.PlayerCharacter.ConsortiaID, ref tempID, ref state, ref msg))
				{
					msg = "ConsortiaApplyAllyPassHandler.Success";
					result = true;
					GameServer.Instance.LoginServer.SendConsortiaAlly(player.PlayerCharacter.ConsortiaID, tempID, state);
				}
			}
			packet.WriteBoolean(result);
			packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
			player.Out.SendTCP(packet);
			return 0;
		}
	}
}
