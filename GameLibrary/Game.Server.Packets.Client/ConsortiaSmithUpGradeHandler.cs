using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(157, "公会铁匠铺升级")]
	public class ConsortiaSmithUpGradeHandler : AbstractPlayerPacketHandler
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
				string msg = "ConsortiaSmithUpGradeHandler.Failed";
				ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
				if (info == null)
				{
					msg = "ConsortiaSmithUpGradeHandler.NoConsortia";
				}
				else
				{
					using (ConsortiaBussiness cb = new ConsortiaBussiness())
					{
						if (cb.UpGradeSmithConsortia(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, ref msg))
						{
							info.SmithLevel++;
							GameServer.Instance.LoginServer.SendConsortiaSmithUpGrade(info);
							msg = "ConsortiaSmithUpGradeHandler.Success";
							result = true;
						}
					}
				}
				if (info.SmithLevel >= 3 && info.Level <= 10)
				{
					string msg2 = LanguageMgr.GetTranslation("ConsortiaSmithUpGradeHandler.Notice", new object[]
					{
						player.PlayerCharacter.ConsortiaName,
						info.SmithLevel
					});
					GSPacketIn pkg = new GSPacketIn(10);
					pkg.WriteInt(2);
					pkg.WriteString(msg2);
					GameServer.Instance.LoginServer.SendPacket(pkg);
					GamePlayer[] players = WorldMgr.GetAllPlayers();
					GamePlayer[] array = players;
					for (int i = 0; i < array.Length; i++)
					{
						GamePlayer p = array[i];
						if (p != player)
						{
							p.Out.SendTCP(pkg);
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
