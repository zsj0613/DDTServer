using System;
namespace Game.Base.Commands
{
	[Cmd("&cmd", ePrivLevel.Admin, "eg:    /cmd -reload           :重新加载命令行系统.", new string[]
	{

	})]
	public class CommandMgrSetupCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string text = args[1];
				if (text != null)
				{
					if (text == "-reload")
					{
						CommandMgr.LoadCommands();
						goto IL_37;
					}
				}
				this.DisplaySyntax(client);
				IL_37:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
