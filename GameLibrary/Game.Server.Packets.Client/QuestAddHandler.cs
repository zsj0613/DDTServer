using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(176, "添加任务")]
	public class QuestAddHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int count = packet.ReadInt();
			for (int i = 0; i < count; i++)
			{
				int id = packet.ReadInt();
				QuestInfo info = QuestMgr.GetSingleQuest(id);
				string msg;
				player.QuestInventory.AddQuest(info, out msg);
			}
			return 0;
		}
	}
}
