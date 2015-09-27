using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(84, "副武器")]
	public class SecondWeaponCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			player.UseSecondWeapon();
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(84);
			pkg.WriteInt(player.AngelCount);
			player.PlayerDetail.SendTCP(pkg);
		}
	}
}
