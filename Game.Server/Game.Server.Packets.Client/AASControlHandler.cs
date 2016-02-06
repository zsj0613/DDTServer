using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(227, "防沉迷系统开关")]
	internal class AASControlHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			player.Out.SendAASControl(AntiAddictionMgr.ISASSon, player.IsAASInfo, player.IsMinor);
			return 0;
		}
	}
}
