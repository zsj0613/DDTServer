using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(236, "添加征婚信息")]
	public class MarryInfoAddHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.MarryInfoID != 0)
			{
				result = 1;
			}
			else
			{
				bool IsPublishEquip = packet.ReadBoolean();
				string Introduction = packet.ReadString();
				int UserID = player.PlayerCharacter.ID;
				eMessageType eMsg = eMessageType.Normal;
				string msg = "MarryInfoAddHandler.Fail";
				int needGold = 10000;
				if (needGold > player.PlayerCharacter.Gold)
				{
					eMsg = eMessageType.ERROR;
					msg = "MarryInfoAddHandler.Msg1";
				}
				else
				{
					player.SaveIntoDatabase();
					MarryInfo info = new MarryInfo();
					info.UserID = UserID;
					info.IsPublishEquip = IsPublishEquip;
					info.Introduction = Introduction;
					info.RegistTime = DateTime.Now;
					using (PlayerBussiness db = new PlayerBussiness())
					{
						if (db.AddMarryInfo(info))
						{
							player.RemoveGold(needGold);
							msg = "MarryInfoAddHandler.Msg2";
							player.PlayerCharacter.MarryInfoID = info.ID;
							player.Out.SendMarryInfoRefresh(info, info.ID, true);
						}
					}
				}
				player.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg, new object[0]));
				result = 0;
			}
			return result;
		}
	}
}
