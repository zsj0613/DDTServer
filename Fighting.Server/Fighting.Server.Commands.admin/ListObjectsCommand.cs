using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Logic;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Fighting.Server.Commands.Admin
{
	[Cmd("&list", ePrivLevel.Player, "   服务器信息大全.", new string[]
	{
		"       /list -g    :列举所有game对象",
		"       /list -c    :列举所有客户端",
		"       /list -r    :列举所有房间",
		"       /list -p    :列举所有战斗中玩家的信息",
		"       /list -b    :列举所有 battle server"
	})]
	public class ListObjectsCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			List<Player> LivingPlayerList = new List<Player>();
			List<Player> allPlayerList = new List<Player>();
			List<BaseGame> gs = GameMgr.GetGames();
			foreach (BaseGame g in gs)
			{
				foreach (Player playerTemp in g.GetAllLivingPlayers())
				{
					LivingPlayerList.Add(playerTemp);
				}
				Player[] allPlayers = g.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					Player playerTemp2 = allPlayers[i];
					allPlayerList.Add(playerTemp2);
				}
			}
			if (args.Length > 1)
			{
				string text = args[1];
				switch (text)
				{
				case "-c":
				{
					this.DisplayMessage(client, "client list:");
					this.DisplayMessage(client, "-------------------------------");
					ServerClient[] cs = FightServer.Instance.GetAllClients();
					ServerClient[] array = cs;
					for (int i = 0; i < array.Length; i++)
					{
						ServerClient cl = array[i];
						this.DisplayMessage(client, cl.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						cs.Length
					});
					goto IL_80E;
				}
				case "-r":
				{
					this.DisplayMessage(client, "room list:");
					this.DisplayMessage(client, "-------------------------------");
					ProxyRoom[] rs = ProxyRoomMgr.GetAllRoom().ToArray<ProxyRoom>();
					ProxyRoom[] array2 = rs;
					for (int i = 0; i < array2.Length; i++)
					{
						ProxyRoom room = array2[i];
						this.DisplayMessage(client, room.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						rs.Count<ProxyRoom>()
					});
					goto IL_80E;
				}
				case "-rw":
				{
					this.DisplayMessage(client, "wait room list:");
					this.DisplayMessage(client, "-------------------------------");
					List<ProxyRoom> wrs = ProxyRoomMgr.GetWaitMatchRoom();
					wrs.Sort();
					foreach (ProxyRoom room in wrs)
					{
						this.DisplayMessage(client, room.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						wrs.Count<ProxyRoom>()
					});
					goto IL_80E;
				}
				case "-g":
					this.DisplayMessage(client, "game list:");
					this.DisplayMessage(client, "-------------------------------");
					foreach (BaseGame gameTemp in gs)
					{
						this.DisplayMessage(client, gameTemp.ToString());
					}
					this.DisplayMessage(client, "-------------------------------");
					this.DisplayMessage(client, "total:{0}", new object[]
					{
						gs.Count
					});
					goto IL_80E;
				case "-p":
					if (args.Count<string>() < 3)
					{
						this.DisplayMessage(client, "player list:");
						this.DisplayMessage(client, "/list -p -n [nickname]");
						this.DisplayMessage(client, "/list -p -i [userid]");
						this.DisplayMessage(client, "-------------------------------");
						this.DisplayMessage(client, "all player list:");
						foreach (Player playerlistTemp in allPlayerList)
						{
							this.DisplayMessage(client, playerlistTemp.GetFsPlayerInfo().ToString());
						}
						this.DisplayMessage(client, "-------------------------------");
						this.DisplayMessage(client, "total:{0}", new object[]
						{
							allPlayerList.Count
						});
						goto IL_80E;
					}
					if (args.Count<string>() == 4)
					{
						Player playerTemp3 = null;
						string playerInfoShow = string.Empty;
						if (args[2] == "-n")
						{
							this.DisplayMessage(client, "-------------------------------");
							foreach (Player playerTempByN in allPlayerList)
							{
								if (playerTempByN.PlayerDetail.PlayerCharacter.NickName == args[3])
								{
									playerTemp3 = playerTempByN;
								}
							}
							if (playerTemp3 != null)
							{
								this.DisplayMessage(client, " the player information : ");
								this.DisplayMessage(client, playerTemp3.GetFsPlayerInfo());
								this.DisplayMessage(client, "-------------------------------");
								this.DisplayMessage(client, " Game infomation : ");
								this.DisplayMessage(client, "     " + playerTemp3.Game.ToString());
								this.DisplayMessage(client, "-------------------------------");
								this.DisplayMessage(client, " living player: ");
								foreach (Player livingPlayerPhyTemp in playerTemp3.Game.GetAllLivingPlayers())
								{
									if (livingPlayerPhyTemp != null)
									{
										this.DisplayMessage(client, livingPlayerPhyTemp.GetFsPlayerInfo());
									}
								}
							}
							else
							{
								this.DisplayMessage(client, "cannot find the player !");
							}
						}
						else
						{
							if (args[2] == "-u")
							{
								this.DisplayMessage(client, "-------------------------------");
								this.DisplayMessage(client, "select from username is fail ---- since the username if null! ");
							}
							else
							{
								if (args[2] == "-i")
								{
									this.DisplayMessage(client, "-------------------------------");
									foreach (Player playerTempByI in allPlayerList)
									{
										int iD = playerTempByI.PlayerDetail.PlayerCharacter.ID;
										if (iD.ToString() == args[3])
										{
											playerTemp3 = playerTempByI;
										}
									}
									if (playerTemp3 != null)
									{
										this.DisplayMessage(client, " the player information : ");
										this.DisplayMessage(client, playerTemp3.GetFsPlayerInfo());
										this.DisplayMessage(client, "-------------------------------");
										this.DisplayMessage(client, " Game infomation : ");
										this.DisplayMessage(client, "     " + playerTemp3.Game.ToString());
										this.DisplayMessage(client, "-------------------------------");
										this.DisplayMessage(client, " living player: ");
										foreach (Player livingPlayerPhyTemp in playerTemp3.Game.GetAllLivingPlayers())
										{
											if (livingPlayerPhyTemp != null)
											{
												this.DisplayMessage(client, livingPlayerPhyTemp.GetFsPlayerInfo());
											}
										}
									}
									else
									{
										this.DisplayMessage(client, "cannot find the player !");
									}
								}
							}
						}
					}
					goto IL_80E;
				case "-b":
					goto IL_80E;
				}
				this.DisplaySyntax(client);
				IL_80E:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
	}
}
