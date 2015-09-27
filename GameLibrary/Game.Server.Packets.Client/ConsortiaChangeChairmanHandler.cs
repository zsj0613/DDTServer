using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(154, "转让会长")]
	public class ConsortiaChangeChairmanHandler : AbstractPlayerPacketHandler
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
				string nickName = packet.ReadString();
				bool result = false;
				string msg = "ConsortiaChangeChairmanHandler.Failed";
				if (string.IsNullOrEmpty(nickName))
				{
					msg = "ConsortiaChangeChairmanHandler.NoName";
				}
				else
				{
					if (nickName == player.PlayerCharacter.NickName)
					{
						msg = "ConsortiaChangeChairmanHandler.Self";
					}
					else
					{
						using (ConsortiaBussiness db = new ConsortiaBussiness())
						{
							string tempUserName = "";
							int tempUserID = 0;
							ConsortiaDutyInfo info = new ConsortiaDutyInfo();
							if (db.UpdateConsortiaChairman(nickName, player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, ref msg, ref info, ref tempUserID, ref tempUserName))
							{
								ConsortiaDutyInfo orderInfo = new ConsortiaDutyInfo();
								orderInfo.Level = player.PlayerCharacter.DutyLevel;
								orderInfo.DutyName = player.PlayerCharacter.DutyName;
								orderInfo.Right = player.PlayerCharacter.Right;
								msg = "ConsortiaChangeChairmanHandler.Success1";
								result = true;
								GameServer.Instance.LoginServer.SendConsortiaDuty(orderInfo, 9, player.PlayerCharacter.ConsortiaID, tempUserID, tempUserName, 0, "");
								GameServer.Instance.LoginServer.SendConsortiaDuty(info, 8, player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, 0, "");
								ConsortiaMgr.ConsortiaChangChairman(player.PlayerCharacter.ConsortiaID, nickName);
							}
							if (db.UpdateConsortiaIsBanChat(tempUserID, player.PlayerCharacter.ConsortiaID, tempUserID, false, ref tempUserID, ref tempUserName, ref msg))
							{
								GameServer.Instance.LoginServer.SendConsortiaBanChat(tempUserID, tempUserName, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, false);
							}
						}
					}
				}
				string temp = LanguageMgr.GetTranslation(msg, new object[0]);
				if (msg == "ConsortiaChangeChairmanHandler.Success1")
				{
					temp = temp + nickName + LanguageMgr.GetTranslation("ConsortiaChangeChairmanHandler.Success2", new object[0]);
				}
				packet.WriteBoolean(result);
				packet.WriteString(temp);
				player.Out.SendTCP(packet);
				result2 = 0;
			}
			return result2;
		}
	}
}
