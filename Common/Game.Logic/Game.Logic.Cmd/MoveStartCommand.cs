using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(9, "开始移动")]
	public class MoveStartCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (player.IsAttacking)
			{
				GSPacketIn pkg = packet.Clone();
				pkg.ClientID = player.PlayerDetail.PlayerCharacter.ID;
				pkg.Parameter1 = player.Id;
				game.SendToAll(pkg, player.PlayerDetail);
				byte type = packet.ReadByte();
				int tx = packet.ReadInt();
				int ty = packet.ReadInt();
				byte dir = packet.ReadByte();
				bool isLiving = packet.ReadBoolean();
				switch (type)
				{
				case 0:
				case 1:
					player.SetXY(tx, ty);
					player.StartFalling(true);
					if (player.Y - ty > 1 || player.IsLiving != isLiving)
					{
						game.SendPlayerMove(player, 3, player.X, player.Y, 0, player.IsLiving, null);
					}
					break;
				}
			}
		}
	}
}
