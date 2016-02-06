using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(13, "日常奖励")]
	public class GameUserDailyAward : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int getWay = packet.ReadInt();
			if (this.AddDailyAward(player, getWay))
			{
				this.UpdatePlayerDailyAward(player, getWay);
			}
			else
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GameUserDailyAward.Fail1", new object[0]));
			}
			return 2;
		}
		private bool AddDailyAward(GamePlayer player, int getWay)
		{
			bool result;
			switch (getWay)
			{
			case 0:
				result = AwardMgr.AddLoginAward(player);
				break;
			case 1:
				result = AwardMgr.AddAuncherAward(player);
				break;
			default:
				result = AwardMgr.AddLoginAward(player);
				break;
			}
			return result;
		}
		private void UpdatePlayerDailyAward(GamePlayer player, int getWay)
		{
			using (PlayerBussiness db = new PlayerBussiness())
			{
				if (getWay == 1)
				{
					bool updateResult = db.UpdatePlayerLastAuncherAward(player.PlayerCharacter.ID);
				}
				else
				{
					bool updateResult = db.UpdatePlayerLastAward(player.PlayerCharacter.ID);
				}
			}
		}
	}
}
