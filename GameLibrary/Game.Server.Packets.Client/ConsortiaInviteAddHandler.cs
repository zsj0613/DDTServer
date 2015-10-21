using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(140, "公会邀请")]
	public class ConsortiaInviteAddHandler : AbstractPlayerPacketHandler
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
				string name = packet.ReadString();
				bool result = false;
				string msg = "ConsortiaInviteAddHandler.Failed";
				if (string.IsNullOrEmpty(name))
				{
					result2 = 0;
				}
				else
				{
					using (ConsortiaBussiness db = new ConsortiaBussiness())
					{
						ConsortiaInviteUserInfo info = new ConsortiaInviteUserInfo();
						info.ConsortiaID = player.PlayerCharacter.ConsortiaID;
						info.ConsortiaName = player.PlayerCharacter.ConsortiaName;
						info.InviteDate = DateTime.Now;
						info.InviteID = player.PlayerCharacter.ID;
						info.InviteName = player.PlayerCharacter.NickName;
						info.IsExist = true;
						info.Remark = "";
						info.UserID = 0;
						info.UserName = name;
						if (db.AddConsortiaInviteUsers(info, ref msg))
						{
							msg = "ConsortiaInviteAddHandler.Success";
							result = true;
							GameServer.Instance.LoginServer.SendConsortiaInvite(info.ID, info.UserID, info.UserName, info.InviteID, info.InviteName, info.ConsortiaName, info.ConsortiaID);
						}
					}
					packet.WriteBoolean(result);
					packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
					player.Out.SendTCP(packet);
					result2 = 0;
				}
			}
			return result2;
		}
	}
}
