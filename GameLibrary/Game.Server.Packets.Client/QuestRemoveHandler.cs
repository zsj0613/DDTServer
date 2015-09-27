using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Quests;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(177, "删除任务")]
	public class QuestRemoveHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			BaseQuest quest = player.QuestInventory.FindQuest(id);
			if (quest != null)
			{
				player.QuestInventory.RemoveQuest(quest);
			}
			return 0;
		}
	}
}
