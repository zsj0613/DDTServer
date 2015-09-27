using Game.Base;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Rooms;
using System;
using System.Collections.Generic;
using System.Text;
namespace Game.Server.Commands.Admin
{
	[Cmd("&list", ePrivLevel.Player, "   服务器信息大全.", new string[]
	{
		"       /list -g :  列举所有game对象",
		"       /list -c :  列举所有客户端",
		"       /list -p :  列举所有玩家对象",
		"       /list -r :  列举所有房间",
		"       /list -b :  列举所有 battle server",
		"       /list -rate:查询所有倍率",
		"       /list -permission [playerId]: 根据playerid查询该player的关卡信息"
	})]
	public class ListObjectsCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				string text = args[1];
				switch (text)
				{
				case "-c":
				{
					this.DisplayMessage(client, "client list:");
					this.DisplayMessage(client, "-------------------------------");
					GameClient[] cs = GameServer.Instance.GetAllClients();
					GameClient[] array = cs;
					for (int j = 0; j < array.Length; j++)
					{
						GameClient cl = array[j];
						this.DisplayMessage(client, cl.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						cs.Length
					});
					goto IL_4F7;
				}
				case "-p":
				{
					this.DisplayMessage(client, "player list:");
					this.DisplayMessage(client, "-------------------------------");
					GamePlayer[] ps = WorldMgr.GetAllPlayers();
					GamePlayer[] array2 = ps;
					for (int j = 0; j < array2.Length; j++)
					{
						GamePlayer player = array2[j];
						this.DisplayMessage(client, player.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						ps.Length
					});
					goto IL_4F7;
				}
				case "-r":
				{
					this.DisplayMessage(client, "room list:");
					this.DisplayMessage(client, "-------------------------------");
					List<BaseRoom> rs = RoomMgr.GetAllUsingRoom();
					foreach (BaseRoom room in rs)
					{
						this.DisplayMessage(client, room.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						rs.Count
					});
					goto IL_4F7;
				}
				case "-g":
				{
					int pveGameCount = 0;
					int pvpGameCount = 0;
					this.DisplayMessage(client, "game list:");
					this.DisplayMessage(client, "-------------------------------");
					List<BaseGame> gs = GameMgr.GetAllGame();
					foreach (BaseGame g in gs)
					{
						this.DisplayMessage(client, g.ToString());
						if (g is PVEGame)
						{
							pveGameCount++;
						}
						else
						{
							pvpGameCount++;
						}
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						gs.Count
					});
					this.DisplayMessage(client, "pvecount:{0}", new object[]
					{
						pveGameCount
					});
					this.DisplayMessage(client, "pvpcount:{0}", new object[]
					{
						pvpGameCount
					});
					goto IL_4F7;
				}
				case "-b":
				{
					this.DisplayMessage(client, "battle list:");
					this.DisplayMessage(client, "-------------------------------");
					List<BattleServer> bs = BattleMgr.GetAllBattles();
					foreach (BattleServer battleSvr in bs)
					{
						this.DisplayMessage(client, battleSvr.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						bs.Count
					});
					goto IL_4F7;
				}
				case "-rate":
					this.DisplayMessage(client, RateMgr.GetAllRate());
					goto IL_4F7;
				case "-permission":
				{
					int playerId = int.Parse(args[2]);
					StringBuilder sb = new StringBuilder();
					GamePlayer[] gamePlays = WorldMgr.GetAllPlayers();
					GamePlayer[] array2 = gamePlays;
					for (int j = 0; j < array2.Length; j++)
					{
						GamePlayer gamePlay = array2[j];
						if (gamePlay.PlayerId == playerId)
						{
							int[] mission = gamePlay.PvePermissionInt();
							int[] array3 = mission;
							for (int k = 0; k < array3.Length; k++)
							{
								int i = array3[k];
								sb.Append(i.ToString() + ",");
							}
							this.DisplayMessage(client, sb.ToString());
							break;
						}
					}
					goto IL_4F7;
				}
				}
				this.DisplaySyntax(client);
				IL_4F7:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
