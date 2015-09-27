using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(129, "申请进入")]
	public class ConsortiaApplyLoginHandler : AbstractPlayerPacketHandler
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
				string msg = "ConsortiaApplyLoginHandler.ADD_Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.AddConsortiaApplyUsers(new ConsortiaApplyUserInfo
					{
						ApplyDate = DateTime.Now,
						ConsortiaID = id,
						ConsortiaName = "",
						IsExist = true,
						Remark = "",
						UserID = player.PlayerCharacter.ID,
						UserName = player.PlayerCharacter.NickName
					}, ref msg))
					{
						msg = ((id != 0) ? "ConsortiaApplyLoginHandler.ADD_Success" : "ConsortiaApplyLoginHandler.DELETE_Success");
						result = true;
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
