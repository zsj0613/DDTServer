using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(142, "通过邀请")]
	public class ConsortiaInvitePassHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerCharacter.ConsortiaID != 0)
			{
				result2 = 0;
			}
			else
			{
				int id = packet.ReadInt();
				bool result = false;
				int consortiaID = 0;
				string consortiaName = "";
				string msg = "ConsortiaInvitePassHandler.Failed";
				int tempID = 0;
				string tempName = "";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					int consortiaRepute = 0;
					ConsortiaUserInfo info = new ConsortiaUserInfo();
					if (db.PassConsortiaInviteUsers(id, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, ref consortiaID, ref consortiaName, ref msg, info, ref tempID, ref tempName, ref consortiaRepute))
					{
						player.PlayerCharacter.ConsortiaID = consortiaID;
						player.PlayerCharacter.ConsortiaName = consortiaName;
						player.PlayerCharacter.DutyLevel = info.Level;
						player.PlayerCharacter.DutyName = info.DutyName;
						player.PlayerCharacter.Right = info.Right;
						ConsortiaInfo consotia = ConsortiaMgr.FindConsortiaInfo(consortiaID);
						if (consotia != null)
						{
							player.PlayerCharacter.ConsortiaLevel = consotia.Level;
						}
						msg = "ConsortiaInvitePassHandler.Success";
						result = true;
						info.UserID = player.PlayerCharacter.ID;
						info.UserName = player.PlayerCharacter.NickName;
						info.Grade = player.PlayerCharacter.Grade;
						info.Offer = player.PlayerCharacter.Offer;
						info.RichesOffer = player.PlayerCharacter.RichesOffer;
						info.RichesRob = player.PlayerCharacter.RichesRob;
						info.Win = player.PlayerCharacter.Win;
						info.Total = player.PlayerCharacter.Total;
						info.Escape = player.PlayerCharacter.Escape;
						info.ConsortiaID = consortiaID;
						info.ConsortiaName = consortiaName;
						GameServer.Instance.LoginServer.SendConsortiaUserPass(tempID, tempName, info, true, consortiaRepute, player.PlayerCharacter.UserName, player.PlayerCharacter.FightPower,player.PlayerCharacter.AchievementPoint,player.PlayerCharacter.Honor);
					}
				}
				packet.WriteBoolean(result);
				packet.WriteInt(consortiaID);
				packet.WriteString(consortiaName);
				packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
				player.Out.SendTCP(packet);
				result2 = 0;
			}
			return result2;
		}
	}
}
