using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Text;
using Game.Language;

namespace Game.Server.Packets.Client
{
	[PacketHandler(152, "修改成员备注")]
	public class ConsortiaUserRemarkHandler : AbstractPlayerPacketHandler
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
				string remark = packet.ReadString();
				if (string.IsNullOrEmpty(remark) || Encoding.Default.GetByteCount(remark) > 100)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaUserRemarkHandler.Long", new object[0]));
					result2 = 1;
				}
				else
				{
					bool result = false;
					string msg = "ConsortiaUserRemarkHandler.Failed";
					using (ConsortiaBussiness db = new ConsortiaBussiness())
					{
						if (db.UpdateConsortiaUserRemark(id, player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, remark, ref msg))
						{
							msg = "ConsortiaUserRemarkHandler.Success";
							result = true;
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
