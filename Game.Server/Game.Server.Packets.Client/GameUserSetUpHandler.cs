using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(107, "房间设置")]
	public class GameUserSetUpHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentRoom != null && player == player.CurrentRoom.Host && !player.CurrentRoom.IsPlaying)
			{
				int mapId = packet.ReadInt();
				eRoomType roomType = (eRoomType)packet.ReadByte();
				byte timeType = packet.ReadByte();
				byte hardLevel = packet.ReadByte();
				int levelLimits = packet.ReadInt();
				bool IsArea = packet.ReadBoolean();
				if (!IsArea && BattleMgr.IsOpenAreaFight && roomType == eRoomType.Match)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GameUserSetUpHandler.required", new object[0]));
				}
				else
				{
					RoomMgr.UpdateRoomGameType(player.CurrentRoom, packet, roomType, timeType, (eHardLevel)hardLevel, levelLimits, mapId, IsArea);
				}
			}
			return 0;
		}
	}
}
