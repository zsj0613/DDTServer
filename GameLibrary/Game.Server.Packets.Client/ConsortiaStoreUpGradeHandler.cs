using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(156, "银行升级")]
	public class ConsortiaStoreUpGradeHandler : AbstractPlayerPacketHandler
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
				bool result = false;
				string msg = "ConsortiaStoreUpGradeHandler.Failed";
				ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
				if (info == null)
				{
					msg = "ConsortiaStoreUpGradeHandler.NoConsortia";
				}
				else
				{
					using (ConsortiaBussiness cb = new ConsortiaBussiness())
					{
						if (cb.UpGradeStoreConsortia(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, ref msg))
						{
							info.StoreLevel++;
							GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(info);
							msg = "ConsortiaStoreUpGradeHandler.Success";
							result = true;
						}
					}
				}
				packet.WriteBoolean(result);
				packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
				player.Out.SendTCP(packet);
				result2 = 1;
			}
			return result2;
		}
	}
}
