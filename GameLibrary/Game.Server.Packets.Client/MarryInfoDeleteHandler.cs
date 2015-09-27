using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(238, "撤消征婚信息")]
	public class MarryInfoDeleteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			string msg = LanguageMgr.GetTranslation("MarryInfoDeleteHandler.Fail", new object[0]);
			using (PlayerBussiness db = new PlayerBussiness())
			{
				if (db.DeleteMarryInfo(id, player.PlayerCharacter.ID, ref msg))
				{
					player.Out.SendAuctionRefresh(null, id, false, null);
				}
				player.Out.SendMessage(eMessageType.Normal, msg);
			}
			return 0;
		}
	}
}
