using Game.Base;
using System;
namespace Game.Server.Commands.Admin
{
	[Cmd("&clear", ePrivLevel.Player, "清除屏幕内容", new string[]
	{

	})]
	public class ClearCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			Console.Clear();
			return true;
		}
	}
}
