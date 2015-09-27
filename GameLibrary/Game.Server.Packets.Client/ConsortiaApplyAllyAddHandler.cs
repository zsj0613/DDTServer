using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(144, "申请同盟")]
	public class ConsortiaApplyAllyAddHandler : AbstractPlayerPacketHandler
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
				bool isAlly = packet.ReadBoolean();
				bool result = false;
				string msg = "ConsortiaApplyAllyAddHandler.Add_Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.AddConsortiaApplyAlly(new ConsortiaApplyAllyInfo
					{
						Consortia1ID = player.PlayerCharacter.ConsortiaID,
						Consortia2ID = id,
						Date = DateTime.Now,
						State = 0,
						Remark = "",
						IsExist = true
					}, player.PlayerCharacter.ID, ref msg))
					{
						msg = "ConsortiaApplyAllyAddHandler.Add_Success";
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
