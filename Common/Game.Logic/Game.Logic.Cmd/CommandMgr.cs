using Game.Base.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using Lsj.Util.Logs;
namespace Game.Logic.Cmd
{
	public class CommandMgr
	{
        private static LogProvider log => LogProvider.Default;
        private static Dictionary<int, ICommandHandler> handles = new Dictionary<int, ICommandHandler>();
		public static ICommandHandler LoadCommandHandler(int code)
		{
			return CommandMgr.handles[code];
		}
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			CommandMgr.handles.Clear();
			int a = CommandMgr.SearchCommandHandlers(Assembly.GetAssembly(typeof(BaseGame)));
            CommandMgr.log.Debug("Loaded"+a.ToString()+"Command");
		}
		protected static int SearchCommandHandlers(Assembly assembly)
		{
			int count = 0;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass)
				{
					if (type.GetInterface("Game.Logic.Cmd.ICommandHandler") != null)
					{
						GameCommandAttribute[] attr = (GameCommandAttribute[])type.GetCustomAttributes(typeof(GameCommandAttribute), true);
						if (attr.Length > 0)
						{
							count++;
							CommandMgr.RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as ICommandHandler);
						}
					}
				}
			}
			return count;
		}
		protected static void RegisterCommandHandler(int code, ICommandHandler handle)
		{
			CommandMgr.handles.Add(code, handle);
		}
	}
}
