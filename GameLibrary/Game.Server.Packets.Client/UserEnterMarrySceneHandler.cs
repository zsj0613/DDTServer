using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(240, "进入结婚场景")]
	public class UserEnterMarrySceneHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			if (WorldMgr.MarryScene.AddPlayer(player))
			{
				pkg.WriteBoolean(true);
			}
			else
			{
				pkg.WriteBoolean(false);
			}
			player.Out.SendTCP(pkg);
			if (player.CurrentMarryRoom == null)
			{
				MarryRoom[] list = MarryRoomMgr.GetAllMarryRoom();
				MarryRoom[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					MarryRoom g = array[i];
					player.Out.SendMarryRoomInfo(player, g);
				}
			}
			return 0;
		}
	}
}
