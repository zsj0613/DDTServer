using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(112, "删除邮件")]
	public class UserDeleteMailHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 0;
			}
			else
			{
				int mailId = packet.ReadInt();
				GSPacketIn pkg = packet.Clone();
				using (PlayerBussiness db = new PlayerBussiness())
				{
					MailInfo mail = db.GetMailSingle(player.PlayerCharacter.ID, mailId);
					//LogMgr.LogMailDelete(player.PlayerCharacter.ID, mail);
					int senderID;
					if (db.DeleteMail(player.PlayerCharacter.ID, mailId, out senderID))
					{
						player.Out.SendMailResponse(senderID, eMailRespose.Receiver);
						pkg.WriteBoolean(true);
					}
					else
					{
						pkg.WriteBoolean(false);
					}
				}
				player.Out.SendTCP(pkg);
				result = 0;
			}
			return result;
		}
	}
}
