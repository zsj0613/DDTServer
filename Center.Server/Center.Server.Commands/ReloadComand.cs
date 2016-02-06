using Bussiness;
using Game.Base;
using System;
using System.Linq;
using System.Text;
namespace Center.Server.Commands
{
	[Cmd("&load", ePrivLevel.Admin, "从新加载内存中的相关数据.", new string[]
	{
		"eg:    /load /config     :加载配置文件.",
		"       /load /property   :刷新游戏道具.",
		"       /load /serverlist :加载Server List."
	})]
	public class ReloadComand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length >= 2)
			{
				StringBuilder cmdLine = new StringBuilder();
				if (args.Contains("/config"))
				{
					CenterServer.Instance.Config.Refresh();
					CenterServer.Instance.InitGlobalTimers();
					this.DisplayMessage(client, "Reload config success!");
				}
				if (args.Contains("/serverlist"))
				{
					ServerMgr.ReLoadServerList();
					this.DisplayMessage(client, "Reload server list success!");
				}
				//if (args.Contains("/property"))
				//{
					//GameProperties.Refresh();
				//	this.DisplayMessage(client, "Reload game properties success!");
				//}
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return false;
		}
	}
}
