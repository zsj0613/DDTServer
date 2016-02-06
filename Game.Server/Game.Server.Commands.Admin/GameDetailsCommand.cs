using Game.Base;
using Game.Logic;
using Game.Server.Games;
using System;
using System.Collections.Generic;
namespace Game.Server.Commands.Admin
{
	[Cmd("&game", ePrivLevel.Player, "   Game对象相关查询.", new string[]
	{
		"       /game -list           : 列举所有game对象",
		"       /game -gameid [gameid]: 根据gameid列举该对象所有信息"
	})]
	public class GameDetailsCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			if (args.Length > 1)
			{
				List<BaseGame> baseGameList = GameMgr.GetAllGame();
				string text = args[1];
				if (text != null)
				{
					if (!(text == "-list"))
					{
						if (text == "-gameid")
						{
							int id = 0;
							if (args.Length <= 2)
							{
								this.DisplaySyntax(client);
							}
							else
							{
								try
								{
									id = int.Parse(args[2]);
								}
								catch (Exception)
								{
									this.DisplayMessage(client, "You enter id type error!");
									goto IL_274;
								}
								string mapName = baseGameList[id].Map.Info.Name;
								string playerCount = baseGameList[id].PlayerCount.ToString();
								this.DisplayMessage(client, "-------------------------------------------------------");
								this.DisplayMessage(client, "Id:{0},PlayerCount:{1}", new object[]
								{
									id,
									playerCount
								});
								if (baseGameList[id] is PVEGame)
								{
									PVEGame pg = baseGameList[id] as PVEGame;
									this.DisplayMessage(client, this.ToString(pg));
								}
								else
								{
									this.DisplayMessage(client, baseGameList[id].ToString());
								}
								this.DisplayMessage(client, "-------------------------------------------------------");
							}
						}
					}
					else
					{
						int pveGameCount = 0;
						int pvpGameCount = 0;
						this.DisplayMessage(client, "game list:");
						this.DisplayMessage(client, "-------------------------------");
						foreach (BaseGame g in baseGameList)
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
							baseGameList.Count
						});
						this.DisplayMessage(client, "total:{0}", new object[]
						{
							baseGameList.Count
						});
						this.DisplayMessage(client, "pvecount:{0}", new object[]
						{
							pveGameCount
						});
						this.DisplayMessage(client, "pvpcount:{0}", new object[]
						{
							pvpGameCount
						});
					}
				}
				IL_274:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
		private string ToString(PVEGame pveGame)
		{
			return string.Format("Type:PVE ,Id:{0}, Name:{1},IncrementDelay:{2},Delay:{3},Script:{4}", new object[]
			{
				pveGame.MissionInfo.Id,
				pveGame.MissionInfo.Name,
				pveGame.MissionInfo.IncrementDelay,
				pveGame.MissionInfo.Delay,
				pveGame.MissionInfo.Script
			});
		}
	}
}
