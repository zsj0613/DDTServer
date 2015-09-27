using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(159, "公会升级")]
	public class ConsortiaUpGradeHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int result2;
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				result2 = 0;
			}
			else
			{
				int bagType = (int)packet.ReadByte();
				int place = packet.ReadInt();
				bool result = false;
				string msg = "ConsortiaUpGradeHandler.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					ConsortiaInfo info = db.GetConsortiaSingle(client.Player.PlayerCharacter.ConsortiaID);
					if (info == null)
					{
						msg = "ConsortiaUpGradeHandler.NoConsortia";
					}
					else
					{
						ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(info.Level + 1);
						if (levelInfo == null)
						{
							msg = "ConsortiaUpGradeHandler.NoUpGrade";
						}
						else
						{
							if (levelInfo.NeedGold > client.Player.PlayerCharacter.Gold)
							{
								msg = "ConsortiaUpGradeHandler.NoGold";
							}
							else
							{
								using (ConsortiaBussiness cb = new ConsortiaBussiness())
								{
									if (cb.UpGradeConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
									{
										info.Level++;
										client.Player.RemoveGold(levelInfo.NeedGold);
										GameServer.Instance.LoginServer.SendConsortiaUpGrade(info);
										msg = "ConsortiaUpGradeHandler.Success";
										result = true;
									}
								}
							}
						}
					}
					if (info.Level >= 5 && info.Level <= 10)
					{
						string msg2 = LanguageMgr.GetTranslation("ConsortiaUpGradeHandler.Notice", new object[]
						{
							info.ConsortiaName,
							info.Level
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
							if (p != client.Player && p.PlayerCharacter.ConsortiaID != client.Player.PlayerCharacter.ConsortiaID)
							{
								p.Out.SendTCP(pkg);
							}
						}
					}
				}
				packet.WriteBoolean(result);
				packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
				client.Out.SendTCP(packet);
				result2 = 1;
			}
			return result2;
		}
	}
}
