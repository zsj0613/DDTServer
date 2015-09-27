using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(136, "公会申请状态")]
	public class ConsotiaApplyStateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerCharacter.ConsortiaID == 0)
			{
				result2 = 1;
			}
			else
			{
				bool state = packet.ReadBoolean();
				bool result = false;
				string msg = "CONSORTIA_APPLY_STATE.Failed";
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (db.UpdateConsotiaApplyState(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, state, ref msg))
					{
						msg = "CONSORTIA_APPLY_STATE.Success";
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
