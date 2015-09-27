using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(114, "修改邮件的已读未读标志")]
	public class UserUpdateMailHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			int id = packet.ReadInt();
			using (PlayerBussiness db = new PlayerBussiness())
			{
				MailInfo mes = db.GetMailSingle(player.PlayerCharacter.ID, id);
				if (mes != null && !mes.IsRead)
				{
					mes.IsRead = true;
					if (mes.Type < 100)
					{
						mes.ValidDate = 72;
						mes.SendTime = DateTime.Now;
					}
					db.UpdateMail(mes, mes.Money);
					pkg.WriteBoolean(true);
				}
				else
				{
					pkg.WriteBoolean(false);
				}
			}
			player.Out.SendTCP(pkg);
			return 0;
		}
	}
}
