using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(237, "更新征婚信息")]
	public class MarryInfoUpdateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.MarryInfoID == 0)
			{
				result = 1;
			}
			else
			{
				bool IsPublishEquip = packet.ReadBoolean();
				string Introduction = packet.ReadString();
				int id = player.PlayerCharacter.MarryInfoID;
				string msg = "MarryInfoUpdateHandler.Fail";
				using (PlayerBussiness db = new PlayerBussiness())
				{
					MarryInfo info = db.GetMarryInfoSingle(id);
					if (info == null)
					{
						msg = "MarryInfoUpdateHandler.Msg1";
					}
					else
					{
						info.IsPublishEquip = IsPublishEquip;
						info.Introduction = Introduction;
						info.RegistTime = DateTime.Now;
						if (db.UpdateMarryInfo(info))
						{
							msg = "MarryInfoUpdateHandler.Succeed";
						}
					}
					player.Out.SendMarryInfoRefresh(info, id, info != null);
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg, new object[0]));
				}
				result = 0;
			}
			return result;
		}
	}
}
