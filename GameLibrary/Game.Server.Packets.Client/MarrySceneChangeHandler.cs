using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(233, "结婚场景切换")]
	internal class MarrySceneChangeHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.CurrentMarryRoom == null || player.MarryMap == 0)
			{
				result = 1;
			}
			else
			{
				int sceneID = packet.ReadInt();
				if (sceneID == player.MarryMap)
				{
					result = 1;
				}
				else
				{
					GSPacketIn pkg_leave = new GSPacketIn(244, player.PlayerCharacter.ID);
					player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(pkg_leave, player);
					player.MarryMap = sceneID;
					if (sceneID == 1)
					{
						player.X = 514;
						player.Y = 637;
					}
					else
					{
						if (sceneID == 2)
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
					result = 0;
				}
			}
			return result;
		}
	}
}
