using Game.Base.Packets;
using Game.Logic.Phy.Object;
using log4net;
using System;
using System.Reflection;
namespace Game.Logic.Cmd
{
	[GameCommand(133, "跳过剧情动画")]
	public class PassDramaCommand : ICommandHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game is PVEGame && game.GameState != eGameState.Playing)
			{
				PVEGame pveGame = game as PVEGame;
				pveGame.IsPassDrama = packet.ReadBoolean();
				pveGame.CheckState(0);
			}
		}
	}
}
