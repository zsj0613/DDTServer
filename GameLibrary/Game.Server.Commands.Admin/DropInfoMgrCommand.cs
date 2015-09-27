using Game.Base;
using Game.Logic;
using System;
namespace Game.Server.Commands.Admin
{
	[Cmd("&drop", ePrivLevel.Player, "   物品掉落相关查询", new string[]
	{
		"       /drop -list             :   列举宏观掉落信息.",
		"       /drop -dropid [dropid]  :   根据ID查询宏观掉落详细信息."
	})]
	public class DropInfoMgrCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string text = args[1];
				if (text != null)
				{
					if (!(text == "-list"))
					{
						if (text == "-dropid")
						{
							int dropId = 0;
							if (args.Length <= 2)
							{
								this.DisplaySyntax(client);
								goto IL_14E;
							}
							try
							{
								dropId = int.Parse(args[2]);
							}
							catch
							{
								this.DisplayMessage(client, " You Enter DropId Type Error,Please Enter Int Type！");
								goto IL_14E;
							}
							if (!DropInfoMgr.DropInfo.ContainsKey(dropId))
							{
								this.DisplayMessage(client, " You Enter DropId Not Exist！");
								goto IL_14E;
							}
							this.DisplayMessage(client, " " + DropInfoMgr.DropInfo[dropId].ToString());
							goto IL_14E;
						}
					}
					else
					{
						if (DropInfoMgr.DropInfo.Count == 0)
						{
							this.DisplayMessage(client, "There is no data!");
							goto IL_14E;
						}
						foreach (int key in DropInfoMgr.DropInfo.Keys)
						{
							this.DisplayMessage(client, " " + DropInfoMgr.DropInfo[key].ToString());
						}
						goto IL_14E;
					}
				}
				this.DisplaySyntax(client);
				IL_14E:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
