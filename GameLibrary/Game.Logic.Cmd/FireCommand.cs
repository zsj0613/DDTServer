using Game.Base.Packets;
using Game.Logic.Phy.Object;
using log4net;
using System;
using System.Reflection;
namespace Game.Logic.Cmd
{
	[GameCommand(2, "用户开炮")]
	public class FireCommand : ICommandHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (player.IsAttacking)
			{
				int x = packet.ReadInt();
				int y = packet.ReadInt();
				if (player.CheckShootPoint(x, y))
				{
					int force = packet.ReadInt();
					int angle = packet.ReadInt();
					player.Shoot(x, y, force, angle);
				}
			}
		}
	}
}
