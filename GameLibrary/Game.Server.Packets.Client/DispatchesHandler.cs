using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(123, "战报")]
	public class DispatchesHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			if (player.RemoveMoney(500, LogMoneyType.Game, LogMoneyType.Game_Dispatches) > 0)
			{
				player.Out.SendDispatchesMsg(packet);
				player.OnPlayerDispatches();
			}
			else
			{
				player.SendInsufficientMoney(3);
			}
			return 0;
		}
	}
}
