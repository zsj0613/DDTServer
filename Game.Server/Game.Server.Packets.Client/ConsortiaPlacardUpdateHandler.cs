using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(150, "更新公告")]
	public class ConsortiaPlacardUpdateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			string placard = packet.ReadString();
			int result2;
			if (Encoding.Default.GetByteCount(placard) > 300)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaPlacardUpdateHandler.Long", new object[0]));
				result2 = 1;
			}
			else
			{
				bool result = false;
				string msg = "ConsortiaPlacardUpdateHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.UpdateConsortiaPlacard(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, placard, ref msg))
					{
						msg = "ConsortiaPlacardUpdateHandler.Success";
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
