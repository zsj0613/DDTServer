using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(132, "删除公会成员")]
	public class ConsortiaUserDeleteHandler : AbstractPlayerPacketHandler
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
				string nickName = "";
				string msg = (id == player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitFailed" : "ConsortiaUserDeleteHandler.KickFailed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.DeleteConsortiaUser(player.PlayerCharacter.ID, id, player.PlayerCharacter.ConsortiaID, ref msg, ref nickName))
					{
						msg = ((id == player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitSuccess" : "ConsortiaUserDeleteHandler.KickSuccess");
						int consortiaID = player.PlayerCharacter.ConsortiaID;
						if (id == player.PlayerCharacter.ID)
						{
							player.ClearConsortia(true);
						}
						GameServer.Instance.LoginServer.SendConsortiaUserDelete(id, consortiaID, id != player.PlayerCharacter.ID, nickName, player.PlayerCharacter.NickName);
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
