using Bussiness;
using Game.Base;
using Game.Server.Packets;
using System;
using System.Threading;
using Game.Language;
namespace Game.Server.Commands.Admin
{
	[Cmd("&shutdown", ePrivLevel.Admin, "停止服务器的业务,并退出服务器进程", new string[]
	{
		"eg: /shutdown      在5分钟后停止服务器",
		"   /shutdown n     在n分钟后停止服务器"
	})]
	public class ShutDownCommand : AbstractCommandHandler, ICommandHandler
	{
		private BaseClient m_client;
		private Thread m_thread;
		private int m_count;
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (client != null)
			{
				this.m_client = client;
				this.m_count = GameServer.Instance.Configuration.SHUTDOWN_MINUTS;
				if (args.Length > 1)
				{
					int.TryParse(args[1], out this.m_count);
				}
				this.m_count = ((this.m_count > 0) ? this.m_count : 0);
				this.m_thread = new Thread(new ThreadStart(this.ShutDownThread));
				this.m_thread.Start();
				this.m_thread.Join((this.m_count + 1) * 60 * 1000);
				GameServer.Instance.Stop();
				this.m_client.DisplayMessage("Server has stopped!");
				GameServer.KeepRunning = false;
				Environment.Exit(0);
			}
			else
			{
				this.DisplayMessage(client, "Server is shutdowning,{0} mins left!", new object[]
				{
					this.m_count
				});
			}
			return true;
		}
		private void ShutDownThread()
		{
			for (int i = this.m_count; i > 0; i--)
			{
				this.m_client.DisplayMessage(string.Format("Server will shutdown after {0} mins!", i));
				string msg = string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1", new object[0]), i, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2", new object[0]));
				GameClient[] list = GameServer.Instance.GetAllClients();
				GameClient[] array = list;
				for (int j = 0; j < array.Length; j++)
				{
					GameClient c = array[j];
					if (c.Out != null)
					{
						c.Out.SendMessage(eMessageType.Normal, msg);
					}
				}
				Thread.Sleep(60000);
			}
		}
	}
}
