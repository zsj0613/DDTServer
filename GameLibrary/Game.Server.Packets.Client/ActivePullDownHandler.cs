using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(11, "领取奖品")]
	public class ActivePullDownHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int activeID = packet.ReadInt();
			string awardID = packet.ReadString();
			string msg = "ActivePullDownHandler.Fail";
			using (ActiveBussiness db = new ActiveBussiness())
			{
				int result = db.PullDown(activeID, awardID, player.PlayerCharacter.ID, ref msg);
				if (result == 0)
				{
					player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				if (msg != "ActiveBussiness.Msg0")
				{
					player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation(msg, new object[0]));
				}
				else
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg, new object[0]));
				}
			}
			return 0;
		}
	}
}
