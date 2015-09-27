using Game.Base;
using System;
namespace Fighting.Server.Commands.admin
{
	[Cmd("&?", ePrivLevel.Admin, "查询所有帮助命令", new string[]
	{

	})]
	internal class ConsoleStartCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			CommandMgr.DisplaySyntax(client);
			return true;
		}
	}
}
