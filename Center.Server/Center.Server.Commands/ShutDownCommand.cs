using Game.Base;
using System;
using System.Threading;
namespace Center.Server.Commands
{
	[Cmd("&shutdown", ePrivLevel.Admin, "停止并退出服务器", new string[]
	{
		"eg:    /shutdown",
		"       /shutdown n"
	})]
	public class ShutDownCommand : AbstractCommandHandler, ICommandHandler
	{
		private Timer m_timer;
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (this.m_timer == null)
			{
				this.m_timer = new Timer(new TimerCallback(this.CheckServer), null, 0, 1000);
				this.DisplayMessage(client, "All server begin shutdown!");
			}
			if (args.Length > 1)
			{
				CenterServer.Instance.ClientsExecuteCmd("/shutdown " + args[1]);
			}
			else
			{
				CenterServer.Instance.ClientsExecuteCmd("/shutdown");
			}
			return true;
		}
		private void CheckServer(object state)
		{
			ServerClient[] list = CenterServer.Instance.GetAllClients();
			if (list == null || list.Length == 0)
			{
				this.m_timer.Dispose();
				Environment.Exit(0);
			}
		}
	}
}
