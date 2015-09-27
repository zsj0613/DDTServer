using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(133, "申请进入通过")]
	public class ConsortiaApplyLoginPassHandler : AbstractPlayerPacketHandler
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
				string msg = "ConsortiaApplyLoginPassHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					int consortiaRepute = 0;
					ConsortiaUserInfo info = new ConsortiaUserInfo();
					if (db.PassConsortiaApplyUsers(id, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, player.PlayerCharacter.ConsortiaID, ref msg, info, ref consortiaRepute))
					{
						msg = "ConsortiaApplyLoginPassHandler.Success";
						result = true;
						if (info.UserID != 0)
						{
							info.ConsortiaID = player.PlayerCharacter.ConsortiaID;
							info.ConsortiaName = player.PlayerCharacter.ConsortiaName;
							GameServer.Instance.LoginServer.SendConsortiaUserPass(player.PlayerCharacter.ID, player.PlayerCharacter.NickName, info, false, consortiaRepute, info.LoginName, info.FightPower,player.PlayerCharacter.AchievementPoint,player.PlayerCharacter.Honor);
						}
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
