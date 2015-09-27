using Game.Base;
using System;
namespace Fighting.Server.Commands.admin
{
	[Cmd("&client", ePrivLevel.Player, " Manager client ", new string[]
	{
		"       /client -stop  [index] : stop the index of client in clientlist"
	})]
	public class ClientMgrCommand : AbstractCommandHandler, ICommandHandler
	{
		private BaseClient client = null;
		public bool OnCommand(BaseClient client, string[] args)
		{
			this.client = client;
			if (args.Length > 1)
			{
				string text = args[1];
				if (text != null)
				{
					if (text == "-stop")
					{
						this.OptionClient("stop");
					}
				}
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
		private void OptionClient(string optionType)
		{
			int i = 0;
			this.DisplayMessage(this.client, "client list:");
			this.DisplayMessage(this.client, "--------------------------------");
			ServerClient[] clientlist = FightServer.Instance.GetAllClients();
			ServerClient[] array = clientlist;
			for (int j = 0; j < array.Length; j++)
			{
				ServerClient sclient = array[j];
				this.DisplayMessage(sclient, "index:{0} , {1}", new object[]
				{
					i,
					this.client.ToString()
				});
				i++;
			}
			this.DisplayMessage(this.client, "--------------------------------");
			this.DisplayMessage(this.client, " Please enter the index of client you want to " + optionType + ": ");
			int index = int.Parse(Console.ReadLine());
			if (index > clientlist.Length)
			{
				this.DisplayMessage(this.client, "Out of client max number!");
			}
			else
			{
				if (optionType == "stop")
				{
					clientlist[index].Disconnect();
					if (!clientlist[index].IsConnected)
					{
						this.DisplayMessage(this.client, "Disconnect success!");
					}
					else
					{
						this.DisplayMessage(this.client, "Disconnect fail!");
					}
				}
				else
				{
					this.DisplayMessage(this.client, "Unknow command!");
				}
			}
		}
	}
}
