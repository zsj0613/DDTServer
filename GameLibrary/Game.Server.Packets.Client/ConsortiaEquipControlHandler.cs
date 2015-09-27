using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(170, "财富控制")]
	public class ConsortiaEquipControlHandler : AbstractPlayerPacketHandler
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
				string msg = "ConsortiaEquipControlHandler.Fail";
				ConsortiaEquipControlInfo info = new ConsortiaEquipControlInfo();
				info.ConsortiaID = player.PlayerCharacter.ConsortiaID;
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					for (int i = 0; i < 5; i++)
					{
						info.Riches = packet.ReadInt();
						info.Type = 1;
						info.Level = i + 1;
						db.AddAndUpdateConsortiaEuqipControl(info, player.PlayerCharacter.ID, ref msg);
					}
					info.Riches = packet.ReadInt();
					info.Type = 2;
					info.Level = 0;
					db.AddAndUpdateConsortiaEuqipControl(info, player.PlayerCharacter.ID, ref msg);
					msg = "ConsortiaEquipControlHandler.Success";
					result = true;
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
