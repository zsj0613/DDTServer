using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SpaRooms;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(187, "进入温泉模块")]
	public class UserEnterSpaSceneHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			if (WorldMgr.SpaScene.AddPlayer(player))
			{
				pkg.WriteBoolean(true);
			}
			else
			{
				pkg.WriteBoolean(false);
			}
			player.Out.SendTCP(pkg);
			DateTime lastTimeLeaveSpaRoom = player.PlayerCharacter.LastSpaDate;
			//bool flag = 1 == 0;
			if (lastTimeLeaveSpaRoom.Year != DateTime.Now.Year || lastTimeLeaveSpaRoom.Month != DateTime.Now.Month || lastTimeLeaveSpaRoom.Day != DateTime.Now.Day)
			{
				player.UpdateSpaPubGoldRoomLimit(SpaRoomMgr.pubGoldRoom_MinLimit);
				player.UpdateSpaPubMoneyRoomLimit(SpaRoomMgr.pubMoneyRoom_MinLimit);
				player.UpdateIsInSpaPubGoldToday(false);
				player.UpdateIsInSpaPubMoneyToday(false);
			}
			if (player.CurrentSpaRoom != null)
			{
				player.CurrentSpaRoom.RemovePlayer(player);
			}
			SpaRoom[] list = SpaRoomMgr.GetAllSpaRoom();
			player.Out.SendSpaRoomList(player, list);
			return 0;
		}
	}
}
