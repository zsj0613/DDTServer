using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(151, "禁言")]
	public class ConsortiaIsBanChatHandler : AbstractPlayerPacketHandler
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
				int banUserID = packet.ReadInt();
				bool isBanChat = packet.ReadBoolean();
				int userID = 0;
				string userName = "";
				bool result = false;
				string msg = "ConsortiaIsBanChatHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.UpdateConsortiaIsBanChat(banUserID, player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, isBanChat, ref userID, ref userName, ref msg))
					{
						msg = "ConsortiaIsBanChatHandler.Success";
						result = true;
						GameServer.Instance.LoginServer.SendConsortiaBanChat(userID, userName, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, isBanChat);
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
