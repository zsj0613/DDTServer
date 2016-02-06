using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(118, "取消付款邮件")]
	public class MailPaymentCancelHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			int senderID = 0;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				if (db.CancelPaymentMail(player.PlayerCharacter.ID, id, ref senderID))
				{
					player.Out.SendMailResponse(senderID, eMailRespose.Receiver);
					packet.WriteBoolean(true);
				}
				else
				{
					packet.WriteBoolean(false);
				}
			}
			player.Out.SendTCP(packet);
			return 1;
		}
	}
}
