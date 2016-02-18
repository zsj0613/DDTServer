using Game.Base;
using Game.Server.Battle;
using System;
namespace Game.Server.Commands.Admin
{
	[Cmd("&fs", ePrivLevel.Player, "   战斗服务器相关操作", new string[]
	{
		"       /fs -list                       : 列举所有FightServer信息",
		"       /fs -connect id ip port key     : 连接战斗服务器",
		"       /fs -disconnect id              : 断开战斗服务器连接",
		"       /fs -remove id                  : 移除战斗服务器",
		"       /fs -open  id  bool             : 开关战斗服务器",
		"       /fs -reconnect                  : 重连battle.xml内战斗服务器"
	})]
	public class BattleMgrCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			bool result;
			if (args.Length > 1)
			{
				string text = args[1];
				switch (text)
				{
				case "-list":
					foreach (BattleServer battleServer in BattleMgr.GetAllBattles())
					{
						this.DisplayMessage(client, battleServer.ToString());
					}
					this.DisplayMessage(client, "---------------------------");
					this.DisplayMessage(client, string.Format("TotalFightServerCount : {0}", BattleMgr.GetAllBattles().Count));
					result = true;
					return result;
				case "-disconnect":
					if (args.Length == 3)
					{
						BattleMgr.Disconnet(int.Parse(args[2]));
						this.DisplayMessage(client, "server disconnected!");
						result = true;
						return result;
					}
					break;
				case "-remove":
					if (args.Length == 3)
					{
						BattleServer server = BattleMgr.GetServer(int.Parse(args[2]));
						if (server.IsActive)
						{
							this.DisplayMessage(client, "Server is active,please disconnect it first!");
						}
						else
						{
							BattleMgr.RemoveServer(server);
						}
						result = true;
						return result;
					}
					break;
				case "-open":
					if (args.Length == 4)
					{
						BattleServer server = BattleMgr.GetServer(int.Parse(args[2]));
						if (server != null && server.IsActive)
						{
							server.IsOpen = bool.Parse(args[3]);
							this.DisplayMessage(client, string.Format("FightServer id:{0} is {1}", server.ServerId, server.IsOpen));
							result = true;
							return result;
						}
					}
					break;
				case "-reconnect":
					if (args.Length == 2)
					{
						BattleMgr.ReconnectAllBattle();
						foreach (BattleServer server in BattleMgr.GetAllBattles())
						{
							this.DisplayMessage(client, string.Format("{0},{1} connect completed!", server.Ip, server.Port));
						}
						result = true;
						return result;
					}
					break;
				}
			}
			this.DisplaySyntax(client);
			result = true;
			return result;
		}
	}
}
