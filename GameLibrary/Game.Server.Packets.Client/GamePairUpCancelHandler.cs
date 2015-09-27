using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(210, "撮合取消")]
	public class GamePairUpCancelHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null && player.CurrentRoom.BattleServer != null)
			{
				player.CurrentRoom.BattleServer.RemoveRoom(player.CurrentRoom);
				if (player != player.CurrentRoom.Host)
				{
					player.CurrentRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed", new object[0]));
					RoomMgr.UpdatePlayerState(player, 0);
				}
				else
				{
					RoomMgr.UpdatePlayerState(player, 2);
				}
			}
			return 0;
		}
	}
}
