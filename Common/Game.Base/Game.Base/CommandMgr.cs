using Game.Base.Events;
using Game.Base.Managers;
using Lsj.Util.Logs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace Game.Base
{
	public class CommandMgr
	{
        private static LogProvider log => LogProvider.Default;
		private static Hashtable m_cmds = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		private static string[] m_disabledarray = new string[0];
		public static string[] DisableCommands
		{
			get
			{
				return CommandMgr.m_disabledarray;
			}
			set
			{
				CommandMgr.m_disabledarray = ((value == null) ? new string[0] : value);
			}
		}
		public static GameCommand GetCommand(string cmd)
		{
			return CommandMgr.m_cmds[cmd] as GameCommand;
		}
		public static GameCommand GuessCommand(string cmd)
		{
			GameCommand myCommand = CommandMgr.GetCommand(cmd);
			GameCommand result;
			if (myCommand != null)
			{
				result = myCommand;
			}
			else
			{
				string compareCmdStr = cmd.ToLower();
				IDictionaryEnumerator iter = CommandMgr.m_cmds.GetEnumerator();
				while (iter.MoveNext())
				{
					GameCommand currentCommand = iter.Value as GameCommand;
					string currentCommandStr = iter.Key as string;
					if (currentCommand != null)
					{
						if (currentCommandStr.ToLower().StartsWith(compareCmdStr))
						{
							myCommand = currentCommand;
							break;
						}
					}
				}
				result = myCommand;
			}
			return result;
		}
		public static string[] GetCommandList(ePrivLevel plvl, bool addDesc)
		{
			IDictionaryEnumerator iter = CommandMgr.m_cmds.GetEnumerator();
			ArrayList list = new ArrayList();
			while (iter.MoveNext())
			{
				GameCommand cmd = iter.Value as GameCommand;
				string cmdString = iter.Key as string;
				if (cmd != null && cmdString != null)
				{
					if (cmdString[0] == '&')
					{
						cmdString = '/' + cmdString.Remove(0, 1);
					}
					if (plvl >= (ePrivLevel)cmd.m_lvl)
					{
						if (addDesc)
						{
							list.Add(cmdString + " - " + cmd.m_desc);
						}
						else
						{
							list.Add(cmd.m_cmd);
						}
					}
				}
			}
			return (string[])list.ToArray(typeof(string));
		}
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			CommandMgr.LoadCommands();
		}
		public static bool LoadCommands()
		{
			CommandMgr.m_cmds.Clear();
			ArrayList asms = new ArrayList(ScriptMgr.Scripts);
			foreach (Assembly script in asms)
			{
				CommandMgr.log.Debug("ScriptMgr: Searching for commands in " + script.GetName());
				
				Type[] types = script.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					Type type = types[i];
					if (type.IsClass)
					{
						if (type.GetInterface("Game.Base.ICommandHandler") != null)
						{
							try
							{
								object[] objs = type.GetCustomAttributes(typeof(CmdAttribute), false);
								object[] array = objs;
								for (int j = 0; j < array.Length; j++)
								{
									CmdAttribute attrib = (CmdAttribute)array[j];
									bool disabled = false;
									string[] array2 = CommandMgr.m_disabledarray;
									for (int k = 0; k < array2.Length; k++)
									{
										string str = array2[k];
										if (attrib.Cmd.Replace('&', '/') == str)
										{
											disabled = true;
											CommandMgr.log.Info("Will not load command " + attrib.Cmd + " as it is disabled in game properties");
											break;
										}
									}
									if (!disabled)
									{
										if (CommandMgr.m_cmds.ContainsKey(attrib.Cmd))
										{
											CommandMgr.log.Info(string.Concat(new object[]
											{
												attrib.Cmd,
												" from ",
												script.GetName(),
												" has been suppressed, a command of that type already exists!"
											}));
										}
										else
										{
											CommandMgr.log.Debug("Load: " + attrib.Cmd + "," + attrib.Description);
											
											GameCommand cmd = new GameCommand();
											cmd.m_usage = attrib.Usage;
											cmd.m_cmd = attrib.Cmd;
											cmd.m_lvl = attrib.Level;
											cmd.m_desc = attrib.Description;
											cmd.m_cmdHandler = (ICommandHandler)Activator.CreateInstance(type);
											CommandMgr.m_cmds.Add(attrib.Cmd, cmd);
											if (attrib.Aliases != null)
											{
												array2 = attrib.Aliases;
												for (int k = 0; k < array2.Length; k++)
												{
													string alias = array2[k];
													CommandMgr.m_cmds.Add(alias, cmd);
												}
											}
										}
									}
								}
							}
							catch (Exception e)
							{
								CommandMgr.log.Error("LoadCommands", e);
							}
						}
					}
				}
			}
			CommandMgr.log.Info("CommandMger:Loaded " + CommandMgr.m_cmds.Count + " commands!");
			return true;
		}
		public static void DisplaySyntax(BaseClient client)
		{
			client.DisplayMessage("Commands list:");
			string[] commandList = CommandMgr.GetCommandList(ePrivLevel.Admin, true);
			for (int i = 0; i < commandList.Length; i++)
			{
				string str = commandList[i];
				client.DisplayMessage(" " + str);
			}
		}
		public static bool HandleCommandNoPlvl(BaseClient client, string cmdLine)
		{
			bool result;
			try
			{
				string[] pars = CommandMgr.ParseCmdLine(cmdLine);
				GameCommand myCommand = CommandMgr.GuessCommand(pars[0]);
				if (myCommand == null)
				{
					result = false;
					return result;
				}
				CommandMgr.ExecuteCommand(client, myCommand, pars);
			}
			catch (Exception e)
			{
				CommandMgr.log.Error("HandleCommandNoPlvl", e);
				
			}
			result = true;
			return result;
		}
		private static bool ExecuteCommand(BaseClient client, GameCommand myCommand, string[] pars)
		{
			pars[0] = myCommand.m_cmd;
			return myCommand.m_cmdHandler.OnCommand(client, pars);
		}
		private static string[] ParseCmdLine(string cmdLine)
		{
			if (cmdLine == null)
			{
				throw new ArgumentNullException("cmdLine");
			}
			List<string> args = new List<string>();
			int state = 0;
			StringBuilder arg = new StringBuilder(cmdLine.Length >> 1);
			int i = 0;
			while (i < cmdLine.Length)
			{
				char c = cmdLine[i];
				switch (state)
				{
				case 0:
					if (c != ' ')
					{
						arg.Length = 0;
						if (c == '"')
						{
							state = 2;
						}
						else
						{
							state = 1;
							i--;
						}
					}
					break;
				case 1:
					if (c == ' ')
					{
						args.Add(arg.ToString());
						state = 0;
					}
					arg.Append(c);
					break;
				case 2:
					if (c == '"')
					{
						args.Add(arg.ToString());
						state = 0;
					}
					arg.Append(c);
					break;
				}
				//IL_E9:
				i++;
				continue;
				//goto IL_E9;
			}
			if (state != 0)
			{
				args.Add(arg.ToString());
			}
			string[] pars = new string[args.Count];
			args.CopyTo(pars);
			return pars;
		}
	}
}
