using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(147, "添加敌对")]
	public class ConsortiaAllyAddHandler : AbstractPlayerPacketHandler
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
				bool isFight = packet.ReadBoolean();
				bool result = false;
				string msg = "ConsortiaAllyAddHandler.Add_Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					ConsortiaAllyInfo info = new ConsortiaAllyInfo();
					info.Consortia1ID = player.PlayerCharacter.ConsortiaID;
					info.Consortia2ID = id;
					info.Date = DateTime.Now;
					info.IsExist = true;
					info.State = 2;
					info.ValidDate = 0;
					if (db.AddConsortiaAlly(info, player.PlayerCharacter.ID, ref msg))
					{
						msg = (isFight ? "ConsortiaAllyAddHandler.Add_Success2" : "ConsortiaAllyAddHandler.Add_Success1");
						result = true;
						GameServer.Instance.LoginServer.SendConsortiaAlly(info.Consortia1ID, info.Consortia2ID, info.State);
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
