using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(153, "用户等级更新")]
	public class ConsortiaUserGradeUpdateHandler : AbstractPlayerPacketHandler
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
				bool upGrade = packet.ReadBoolean();
				bool result = false;
				string msg = "ConsortiaUserGradeUpdateHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					string tempUserName = "";
					ConsortiaDutyInfo info = new ConsortiaDutyInfo();
					if (db.UpdateConsortiaUserGrade(id, player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, upGrade, ref msg, ref info, ref tempUserName))
					{
						msg = "ConsortiaUserGradeUpdateHandler.Success";
						result = true;
						GameServer.Instance.LoginServer.SendConsortiaDuty(info, upGrade ? 6 : 7, player.PlayerCharacter.ConsortiaID, id, tempUserName, player.PlayerCharacter.ID, player.PlayerCharacter.NickName);
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
