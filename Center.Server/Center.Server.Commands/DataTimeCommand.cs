using Game.Base;
using System;
namespace Center.Server.Commands
{
	[Cmd("&time", ePrivLevel.Admin, "   显示当前系统时间", new string[]
	{
		"       /time :  显示当前系统时间",
		"                   "
	})]
	public class DataTimeCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			this.DisplayMessage(client, "data time:");
			this.DisplayMessage(client, "-------------------------------");
			this.DisplayMessage(client, DateTime.Now.ToString());
			return true;
		}
	}
}
