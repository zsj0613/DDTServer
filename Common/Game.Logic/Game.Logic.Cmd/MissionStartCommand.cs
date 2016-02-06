using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(117, "关卡开始")]
	public class MissionStartCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState == eGameState.SessionPrepared || game.GameState == eGameState.GameOver)
			{
				bool isReady = packet.ReadBoolean();
				if (isReady)
				{
					player.Ready = true;
					game.SendSyncLifeTime();
					game.CheckState(0);
				}
			}
		}
	}
}
