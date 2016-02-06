using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(149, "更新介绍")]
	public class ConsortiaDescriptionUpdateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			string description = packet.ReadString();
			int result2;
			if (Encoding.Default.GetByteCount(description) > 300)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDescriptionUpdateHandler.Long", new object[0]));
				result2 = 1;
			}
			else
			{
				bool result = false;
				string msg = "ConsortiaDescriptionUpdateHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.UpdateConsortiaDescription(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, description, ref msg))
					{
						msg = "ConsortiaDescriptionUpdateHandler.Success";
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
