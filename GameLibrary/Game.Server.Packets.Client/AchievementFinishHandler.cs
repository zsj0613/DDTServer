using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(230, "成就完成")]
	public class AchievementFinishHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			AchievementInfo achievement = player.AchievementInventory.FindAchievement(id);
			return 0;
		}
	}
}
