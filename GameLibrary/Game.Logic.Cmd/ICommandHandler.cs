using Game.Base.Packets;
using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Cmd
{
	public interface ICommandHandler
	{
		void HandleCommand(BaseGame game, Player player, GSPacketIn packet);
	}
}
