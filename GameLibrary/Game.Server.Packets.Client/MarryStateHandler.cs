using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(251, "当前场景状态")]
	internal class MarryStateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			switch (packet.ReadInt())
			{
			case 0:
				if (player.IsInMarryRoom)
				{
					if (player.MarryMap == 1)
					{
						player.X = 646;
						player.Y = 1241;
					}
					else
					{
						if (player.MarryMap == 2)
						{
							player.X = 800;
							player.Y = 763;
						}
					}
					GamePlayer[] list = player.CurrentMarryRoom.GetAllPlayers();
					GamePlayer[] array = list;
					for (int i = 0; i < array.Length; i++)
					{
						GamePlayer p = array[i];
						if (p != player && p.MarryMap == player.MarryMap)
						{
							p.Out.SendPlayerEnterMarryRoom(player);
							player.Out.SendPlayerEnterMarryRoom(p);
						}
					}
				}
				break;
			}
			return 0;
		}
	}
}
