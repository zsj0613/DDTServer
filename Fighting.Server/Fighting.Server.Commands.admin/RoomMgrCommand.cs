using Fighting.Server.Rooms;
using Game.Base;
using System;
namespace Fighting.Server.Commands.admin
{
	[Cmd("&rm", ePrivLevel.Player, "Room manger", new string[]
	{
		"eg:    /rm -showtip    Show pickup info...",
		"       /rm -closetip   Close pickup info..."
	})]
	public class RoomMgrCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string text = args[1];
				if (text != null)
				{
					if (!(text == "-showtip"))
					{
						if (text == "-closetip")
						{
							ProxyRoomMgr.ShowTick = false;
						}
					}
					else
					{
						ProxyRoomMgr.ShowTick = true;
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
