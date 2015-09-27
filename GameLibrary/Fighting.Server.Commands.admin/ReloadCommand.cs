using Bussiness.Managers;
using Game.Base;
using System;
using System.Linq;
using System.Text;
namespace Fighting.Server.Commands.admin
{
	[Cmd("&load", ePrivLevel.Player, "Load the metedata.", new string[]
	{
		"       /config     :Application config file.",
		"       /shop       :ShopMgr.ReLoad().",
		"       /item       :ItemMgr.Reload().",
		"       /property   :Fight properties."
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
					this.DisplayMessage(client, "Application config file load success!");
					success.Append("/config,");
				}
				if (args.Contains("/property"))
				{
					this.DisplayMessage(client, "Fight properties load success!");
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
					if (ItemMgr.ReLoad())
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
