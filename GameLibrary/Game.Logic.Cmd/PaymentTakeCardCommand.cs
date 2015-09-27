using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(114, "付费翻牌")]
	public class PaymentTakeCardCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if ((game.GameState == eGameState.GameOver || game.GameState == eGameState.ALLSessionStopped) && player.FinishTakeCard && player.CanTakeOut == 0 && !player.HasPaymentTakeCard)
			{
				if (player.PlayerDetail.RemoveMoney(486, LogMoneyType.Game, LogMoneyType.Game_PaymentTakeCard) > 0)
				{
					int index = (int)packet.ReadByte();
					player.CanTakeOut++;
					player.FinishTakeCard = false;
					player.HasPaymentTakeCard = true;
					if (index < 0 || index > game.Cards.Length)
					{
						game.TakeCard(player, false);
					}
					else
					{
						game.TakeCard(player, index, false);
					}
				}
				else
				{
					player.PlayerDetail.SendInsufficientMoney(1);
				}
			}
		}
	}
}
