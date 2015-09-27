using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Text;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(139, "更新职务")]
	public class ConsortiaDutyUpdateHandler : AbstractPlayerPacketHandler
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
				int dutyID = packet.ReadInt();
				int updateType = (int)packet.ReadByte();
				bool result = false;
				string msg = "ConsortiaDutyUpdateHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					ConsortiaDutyInfo info = new ConsortiaDutyInfo();
					info.ConsortiaID = player.PlayerCharacter.ConsortiaID;
					info.DutyID = dutyID;
					info.IsExist = true;
					info.DutyName = "";
					switch (updateType)
					{
					case 1:
						result2 = 1;
						return result2;
					case 2:
						info.DutyName = packet.ReadString();
						if (string.IsNullOrEmpty(info.DutyName) || Encoding.Default.GetByteCount(info.DutyName) > 10)
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDutyUpdateHandler.Long", new object[0]));
							result2 = 1;
							return result2;
						}
						info.Right = packet.ReadInt();
						break;
					}
					if (db.UpdateConsortiaDuty(info, player.PlayerCharacter.ID, updateType, ref msg))
					{
						dutyID = info.DutyID;
						msg = "ConsortiaDutyUpdateHandler.Success";
						result = true;
						GameServer.Instance.LoginServer.SendConsortiaDuty(info, updateType, player.PlayerCharacter.ConsortiaID);
					}
				}
				packet.WriteBoolean(result);
				packet.WriteInt(dutyID);
				packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
				player.Out.SendTCP(packet);
				result2 = 0;
			}
			return result2;
		}
	}
}
