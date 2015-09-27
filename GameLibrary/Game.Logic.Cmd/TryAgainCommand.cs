using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(119, "关卡失败再试一次")]
	public class TryAgainCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game is PVEGame)
			{
				PVEGame pve = game as PVEGame;
				bool tryAgain = packet.ReadBoolean();
				bool isHost = packet.ReadBoolean();
				if (isHost)
				{
					if (tryAgain)
					{
						if (player.PlayerDetail.RemoveMoney(500, LogMoneyType.Game, LogMoneyType.Game_TryAgain) > 0)
						{
							pve.WantTryAgain = 1;
							game.SendToAll(packet);
						}
						else
						{
							player.PlayerDetail.SendInsufficientMoney(2);
						}
					}
					else
					{
						pve.WantTryAgain = 0;
						game.SendToAll(packet);
					}
					pve.CheckState(0);
				}
			}
		}
	}
}
