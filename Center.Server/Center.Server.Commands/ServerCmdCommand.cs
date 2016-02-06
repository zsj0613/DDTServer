using Game.Base;
using System;
using System.Text;
namespace Center.Server.Commands
{
	[Cmd("&sc", ePrivLevel.Admin, "Manage server properties at runtime(include CenterServer and GameServer).", new string[]
	{
		"   /sc <option> [para1][para2] ..."
	})]
	public class ServerCmdCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				StringBuilder cmdLine = new StringBuilder(args[1]);
				for (int i = 2; i < args.Length; i++)
				{
					cmdLine.Append(" ");
					cmdLine.Append(args[i]);
				}
				ServerClient[] list = CenterServer.Instance.GetAllClients();
				if (list != null)
				{
					ServerClient[] array = list;
					for (int j = 0; j < array.Length; j++)
					{
						ServerClient c = array[j];
						c.SendCmd(client, cmdLine.ToString());
					}
				}
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
