using Game.Base.Managers;
using System;
using System.IO;
using System.Reflection;
namespace Game.Base.Commands
{
	[Cmd("&sm", ePrivLevel.Player, "eg:    /sm -list              : List all assemblies in scripts array.", new string[]
	{
		"       /sm -add <assembly>    : Add assembly into the scripts array.",
		"       /sm -remove <assembly> : Remove assembly from the scripts array."
	})]
	public class ScriptManagerCommand : AbstractCommandHandler, ICommandHandler
	{
		public bool OnCommand(BaseClient client, string[] args)
		{
			bool result;
			if (args.Length > 1)
			{
				string text = args[1];
				if (text != null)
				{
					if (text == "-list")
					{
						Assembly[] scripts = ScriptMgr.Scripts;
						for (int i = 0; i < scripts.Length; i++)
						{
							Assembly ass = scripts[i];
							this.DisplayMessage(client, ass.FullName);
						}
						result = true;
						return result;
					}
					if (text == "-add")
					{
						if (args.Length > 2 && args[2] != null && File.Exists(args[2]))
						{
							try
							{
								Assembly ass = Assembly.LoadFile(args[2]);
								if (ScriptMgr.InsertAssembly(ass))
								{
									this.DisplayMessage(client, "Add assembly success!");
									result = true;
									return result;
								}
								this.DisplayMessage(client, "Assembly already exists in the scripts array!");
								result = false;
								return result;
							}
							catch (Exception ex)
							{
								this.DisplayMessage(client, "Add assembly error:", new object[]
								{
									ex.Message
								});
								result = false;
								return result;
							}
						}
						this.DisplayMessage(client, "Can't find add assembly!");
						result = false;
						return result;
					}
					if (text == "-remove")
					{
						if (args.Length > 2 && args[2] != null && File.Exists(args[2]))
						{
							try
							{
								Assembly ass = Assembly.LoadFile(args[2]);
								if (ScriptMgr.RemoveAssembly(ass))
								{
									this.DisplayMessage(client, "Remove assembly success!");
									result = true;
									return result;
								}
								this.DisplayMessage(client, "Assembly didn't exist in the scripts array!");
								result = false;
								return result;
							}
							catch (Exception ex)
							{
								this.DisplayMessage(client, "Remove assembly error:", new object[]
								{
									ex.Message
								});
								result = false;
								return result;
							}
						}
						this.DisplayMessage(client, "Can't find remove assembly!");
						result = false;
						return result;
					}
				}
				this.DisplayMessage(client, "Can't fine option:{0}", new object[]
				{
					args[1]
				});
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
