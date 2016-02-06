using System;
namespace Game.Base.Commands
{
	[Cmd("&?", ePrivLevel.Admin, "/?     List all commands", new string[]
	{

	})]
	public class ListAllCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			CommandMgr.DisplaySyntax(client);
			return true;
		}
	}
}
