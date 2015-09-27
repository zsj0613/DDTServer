using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using log4net;
using System;
using System.Reflection;
namespace Fighting.Server.Commands.admin
{
	[Cmd("&trace", ePrivLevel.Admin, "显示服务的线程堆栈信息", new string[]
	{
		"eg: /trace -r      ProxyRoomMgr的线程",
		"   /trace  -g      GameMgr的线程"
	})]
	public class StackTraceCommand : AbstractCommandHandler, ICommandHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string text = args[1];
				if (text != null)
				{
					if (text == "-r")
					{
						StackTraceCommand.log.Error(ThreadHelper.GetThreadStackTrace(ProxyRoomMgr.Thread));
						goto IL_6A;
					}
					if (text == "-g")
					{
						StackTraceCommand.log.Error(ThreadHelper.GetThreadStackTrace(GameMgr.Thread));
						goto IL_6A;
					}
				}
				this.DisplaySyntax(client);
				IL_6A:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
