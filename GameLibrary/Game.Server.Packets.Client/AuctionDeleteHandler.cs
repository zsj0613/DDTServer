using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(194, "撤消拍卖")]
	public class AuctionDeleteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			string msg = LanguageMgr.GetTranslation("AuctionDeleteHandler.Fail", new object[0]);
			using (PlayerBussiness db = new PlayerBussiness())
			{
				if (db.DeleteAuction(id, player.PlayerCharacter.ID, ref msg))
				{
					player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
					player.Out.SendAuctionRefresh(null, id, false, null);
				}
				else
				{
					AuctionInfo info = db.GetAuctionSingle(id);
					player.Out.SendAuctionRefresh(info, id, info != null, null);
				}
				player.Out.SendMessage(eMessageType.Normal, msg);
			}
			return 0;
		}
	}
}
