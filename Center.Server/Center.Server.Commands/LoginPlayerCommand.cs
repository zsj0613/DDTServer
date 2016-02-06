using Game.Base;
using System;
using System.Linq;
namespace Center.Server.Commands
{
	[Cmd("&player", ePrivLevel.Admin, "获取当前登录玩家的相关数据.", new string[]
	{
		"eg:    /player -l     :获取当前玩家信息列表.",
		"       /player -n [nickname]    :通过昵称获取当前玩家信息.",
		"       /player -i [userid]      :通过玩家id获取当前玩家信息."
	})]
	public class LoginPlayerCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string text = args[1];
				if (text != null)
				{
					if (text == "-l")
					{
						this.DisplayMessage(client, "-------------------------------");
						this.DisplayMessage(client, "loginplayer infomation list : ");
						Player[] playerLoginList = LoginMgr.GetAllPlayer();
						Player[] array = playerLoginList;
						for (int i = 0; i < array.Length; i++)
						{
							Player playerTemp = array[i];
							this.DisplayMessage(client, string.Concat(new string[]
							{
								"    id : ",
								playerTemp.Id.ToString(),
								" name : ",
								playerTemp.Name,
								" password : ",
								playerTemp.Password,
								" state : ",
								playerTemp.State.ToString()
							}));
						}
						this.DisplayMessage(client, "-------------------------------");
						this.DisplayMessage(client, "total count is " + playerLoginList.Count<Player>().ToString());
						goto IL_379;
					}
					if (text == "-n")
					{
						if (args.Count<string>() == 3)
						{
							this.DisplayMessage(client, "-------------------------------");
							this.DisplayMessage(client, "get loginplayer infomation from the nickname");
							Player playerTemp = LoginMgr.GetPlayerByName(string.Format(args[2], new object[0]));
							if (playerTemp != null)
							{
								this.DisplayMessage(client, string.Concat(new string[]
								{
									"    id : ",
									playerTemp.Id.ToString(),
									" name : ",
									playerTemp.Name,
									" password : ",
									playerTemp.Password,
									" state : ",
									playerTemp.State.ToString()
								}));
							}
							else
							{
								this.DisplayMessage(client, "-------------------------------");
								this.DisplayMessage(client, "cannot find the player ! ");
							}
						}
						else
						{
							this.DisplayMessage(client, "-------------------------------");
							this.DisplayMessage(client, "input in the wrong way ! ");
						}
						goto IL_379;
					}
					if (text == "-i")
					{
						if (args.Count<string>() == 3)
						{
							this.DisplayMessage(client, "-------------------------------");
							this.DisplayMessage(client, "get loginplayer infomation from the userid");
							Player playerTemp = null;
							try
							{
								playerTemp = LoginMgr.GetPlayer(Convert.ToInt32(args[2]));
							}
							catch (Exception ex)
							{
								this.DisplayMessage(client, "-------------------------------");
								this.DisplayMessage(client, ex.ToString());
								goto IL_379;
							}
							if (playerTemp != null)
							{
								this.DisplayMessage(client, string.Concat(new string[]
								{
									"    id : ",
									playerTemp.Id.ToString(),
									" name : ",
									playerTemp.Name,
									" password : ",
									playerTemp.Password,
									" state : ",
									playerTemp.State.ToString()
								}));
							}
							else
							{
								this.DisplayMessage(client, "-------------------------------");
								this.DisplayMessage(client, "cannot find the player ! ");
							}
						}
						else
						{
							this.DisplayMessage(client, "-------------------------------");
							this.DisplayMessage(client, "input in the wrong way ! ");
						}
						goto IL_379;
					}
				}
				this.DisplaySyntax(client);
				IL_379:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
