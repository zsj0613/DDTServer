using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(158, "公会商城升级")]
	public class ConsortiaShopUpGradeHandler : AbstractPlayerPacketHandler
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
				string msg = "ConsortiaShopUpGradeHandler.Failed";
				ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(player.PlayerCharacter.ConsortiaID);
				if (info == null)
				{
					msg = "ConsortiaShopUpGradeHandler.NoConsortia";
				}
				else
				{
					using (ConsortiaBussiness cb = new ConsortiaBussiness())
					{
						if (cb.UpGradeShopConsortia(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, ref msg))
						{
							info.ShopLevel++;
							GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(info);
							msg = "ConsortiaShopUpGradeHandler.Success";
							result = true;
						}
					}
				}
				if (info.ShopLevel >= 2 && info.Level <= 10)
				{
					string msg2 = LanguageMgr.GetTranslation("ConsortiaShopUpGradeHandler.Notice", new object[]
					{
						player.PlayerCharacter.ConsortiaName,
						info.ShopLevel
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
