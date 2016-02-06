using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Quests;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(179, "任务完成")]
	public class QuestFinishHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			int rewardItemID = packet.ReadInt();
			BaseQuest _baseQuest = player.QuestInventory.FindQuest(id);
			int i = 0;
			if (_baseQuest != null)
			{
				i = (player.QuestInventory.Finish(_baseQuest, rewardItemID) ? 1 : 0);
			}
			if (i == 1)
			{
				packet.WriteInt(id);
				player.Out.SendTCP(packet);
			}
			return i;
		}
	}
}
