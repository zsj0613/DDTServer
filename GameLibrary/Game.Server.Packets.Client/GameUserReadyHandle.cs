using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(87, "用户状态改变")]
	public class GameUserReadyHandle : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.MainWeapon == null)
			{
				player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
				result = 0;
			}
			else
			{
				if (player.CurrentRoom != null)
				{
					RoomMgr.UpdatePlayerState(player, packet.ReadByte());
				}
				result = 0;
			}
			return result;
		}
	}
}
