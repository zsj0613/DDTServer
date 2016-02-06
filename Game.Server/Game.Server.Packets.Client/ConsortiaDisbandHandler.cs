using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(131, "公会解散")]
	public class ConsortiaDisbandHandler : AbstractPlayerPacketHandler
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
				int id = player.PlayerCharacter.ConsortiaID;
				string consortiaName = player.PlayerCharacter.ConsortiaName;
				bool result = false;
				string msg = "ConsortiaDisbandHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.DeleteConsortia(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, ref msg))
					{
						result = true;
						msg = "ConsortiaDisbandHandler.Success1";
						player.ClearConsortia(true);
						GameServer.Instance.LoginServer.SendConsortiaDelete(id);
					}
				}
				string temp;
				if (msg == "ConsortiaDisbandHandler.Success1")
				{
					temp = string.Format(LanguageMgr.GetTranslation(msg, new object[0]) + consortiaName + LanguageMgr.GetTranslation("ConsortiaDisbandHandler.Success2", new object[0]), new object[0]);
				}
				else
				{
					temp = LanguageMgr.GetTranslation(msg, new object[0]);
				}
				packet.WriteBoolean(result);
				packet.WriteInt(player.PlayerCharacter.ID);
				packet.WriteString(temp);
				player.Out.SendTCP(packet);
				result2 = 0;
			}
			return result2;
		}
	}
}
