using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(138, "删除职务")]
	public class ConsortiaDutyDeleteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerCharacter.ConsortiaID == 0)
			{
				result2 = 0;
			}
			else
			{
				int id = packet.ReadInt();
				bool result = false;
				string msg = "ConsortiaDutyDeleteHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.DeleteConsortiaDuty(id, player.PlayerCharacter.ID, player.PlayerCharacter.ConsortiaID, ref msg))
					{
						msg = "ConsortiaDutyDeleteHandler.Success";
						result = true;
					}
				}
				packet.WriteBoolean(result);
				packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
				player.Out.SendTCP(packet);
				result2 = 0;
			}
			return result2;
		}
	}
}
