using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(130, "战胜关卡中Boss翻牌")]
	public class BossTakeCardCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game is PVEGame)
			{
				PVEGame pve = game as PVEGame;
				if (pve.CanTakeCard > 0)
				{
					int index = (int)packet.ReadByte();
					if (index < 0 || index > pve.BossCards.Length)
					{
						pve.TakeBossCard(player);
					}
					else
					{
						pve.TakeBossCard(player, index);
					}
				}
			}
		}
	}
}
