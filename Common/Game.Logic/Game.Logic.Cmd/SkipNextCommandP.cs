using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(12, "跳过")]
	public class SkipNextCommandP : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (player.IsAttacking)
			{
				player.Skip((int)packet.ReadByte());
			}
		}
	}
}
