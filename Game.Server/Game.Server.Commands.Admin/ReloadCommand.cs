using Bussiness.Managers;
using Game.Base;
using Game.Base.Events;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.Managers;
using System;
using System.Linq;
using System.Text;
namespace Game.Server.Commands.Admin
{
	[Cmd("&load", ePrivLevel.Player, "   相关文件重新加载.", new string[]
	{
		"       /load /config     :加载配置文件.",
		"       /load /shop       :加载商城文件.",
		"       /load /rate       :加载倍率.",
		"       /load /item       :加载模板.",
		"       /load /property   :刷新游戏道具."
	})]
	public class ReloadCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			bool result;
			if (args.Length > 1)
			{
				StringBuilder success = new StringBuilder();
				StringBuilder failed = new StringBuilder();
				if (args.Contains("/cmd"))
				{
					CommandMgr.LoadCommands();
					this.DisplayMessage(client, "Command load success!");
					success.Append("/cmd,");
				}
				if (args.Contains("/config"))
				{
					GameServer.Instance.Configuration.Refresh();
					this.DisplayMessage(client, "Application config file load success!");
					success.Append("/config,");
				}
				if (args.Contains("/property"))
				{
					GameServer.Instance.RefreshGameProperties();
					this.DisplayMessage(client, "Game properties load success!");
					success.Append("/property,");
				}
				if (args.Contains("/item"))
				{
					if (ItemMgr.ReLoad())
					{
						this.DisplayMessage(client, "Items load success!");
						success.Append("/item,");
					}
					else
					{
						this.DisplayMessage(client, "Items load failed!");
						failed.Append("/item,");
					}
				}
				if (args.Contains("/shop"))
				{
					if (ShopMgr.ReLoad())
					{
						this.DisplayMessage(client, "Shops load success!");
						success.Append("/shop,");
					}
					else
					{
						this.DisplayMessage(client, "Shops load failed!");
						failed.Append("/shop,");
					}
				}
				if (args.Contains("/rate"))
				{
					if (RateMgr.ReLoad())
					{
						this.DisplayMessage(client, "Rates load success!");
						success.Append("/rate,");
					}
					else
					{
						this.DisplayMessage(client, "Rates load failed!");
						failed.Append("/rate,");
					}
				}
                if (args.Contains("/quest"))
                {
                    if (QuestMgr.ReLoad())
                    {
                        this.DisplayMessage(client, "Quest load success!");
                        success.Append("/quest,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "Quest load failed!");
                        failed.Append("/quest,");
                    }
                }
                if (args.Contains("/fb"))
                {
                    if (GameServer.Instance.StartScriptComponents()&&MapMgr.Init()&&BallMgr.ReLoad()&&NPCInfoMgr.ReLoad()&&DropMgr.ReLoad())
                    {
                        GameEventMgr.Notify(ScriptEvent.Loaded);
                        this.DisplayMessage(client, "FB load success!");
                        success.Append("/fb,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "FB load failed!");
                        failed.Append("/fb,");
                    }
                    if (PveInfoMgr.ReLoad())
                    {
                        this.DisplayMessage(client, "PveInfo load success!");
                        success.Append("/pve,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "PveInfo load failed!");
                        failed.Append("/pve,");
                    }
                    if (NPCInfoMgr.ReLoad())
                    {
                        this.DisplayMessage(client, "NPCInfo load success!");
                        success.Append("/npc,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "NPCInfo load failed!");
                        failed.Append("/npc,");
                    }
                    if (MissionInfoMgr.Reload())
                    {
                        this.DisplayMessage(client, "MissionInfo load success!");
                        success.Append("/mission,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "MissionInfo load failed!");
                        failed.Append("/mission,");
                    }
                }
                if (args.Contains("/fusion"))
                {
                    if (FusionMgr.ReLoad())
                    {
                        this.DisplayMessage(client, "Fusion load success!");
                        success.Append("/Fusion,");
                    }
                    else
                    {
                        this.DisplayMessage(client, "Quest load failed!");
                        failed.Append("/Fusion,");
                    }
                }
                if (success.Length == 0 && failed.Length == 0)
				{
					this.DisplayMessage(client, "Nothing executed!");
					this.DisplaySyntax(client);
				}
				else
				{
					this.DisplayMessage(client, "Success Options:    " + success.ToString());
					if (failed.Length > 0)
					{
						this.DisplayMessage(client, "Faile Options:      " + failed.ToString());
						result = false;
						return result;
					}
				}
				result = true;
			}
			else
			{
				this.DisplaySyntax(client);
				result = true;
			}
			return result;
		}
	}
}
