using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(116, "关卡准备")]
	public class MissionPrepareCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState == eGameState.SessionPrepared || game.GameState == eGameState.GameOver)
			{
				bool isReady = packet.ReadBoolean();
				if (player.Ready != isReady)
				{
					player.Ready = isReady;
					game.SendSyncLifeTime();
					game.SendToAll(packet);
				}
			}
		}
	}
}
