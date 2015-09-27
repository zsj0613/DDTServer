using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Quests;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(181, "客服端任务检查")]
	public class QuestCheckHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int questId = packet.ReadInt();
			int conditionId = packet.ReadInt();
			int value = packet.ReadInt();
			BaseQuest quest = player.QuestInventory.FindQuest(questId);
			if (quest != null)
			{
				ClientModifyCondition cd = quest.GetConditionById(conditionId) as ClientModifyCondition;
				if (cd != null)
				{
					cd.Value = value;
				}
			}
			return 0;
		}
	}
}
