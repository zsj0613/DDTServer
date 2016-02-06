using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Server.SpaRooms.CommandHandle
{
	public class SpaCommandMgr
	{
		private Dictionary<int, ISpaCommandHandler> handles = new Dictionary<int, ISpaCommandHandler>();
		public ISpaCommandHandler LoadCommandHandler(int code)
		{
			return this.handles[code];
		}
		public SpaCommandMgr()
		{
			this.handles.Clear();
			this.SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
		}
		protected int SearchCommandHandlers(Assembly assembly)
		{
			int count = 0;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass)
				{
					if (type.GetInterface("Game.Server.SpaRooms.CommandHandle.ISpaCommandHandler") != null)
					{
						SpaCommandAttbute[] attr = (SpaCommandAttbute[])type.GetCustomAttributes(typeof(SpaCommandAttbute), true);
						if (attr.Length > 0)
						{
							count++;
							this.RegisterCommandHandler((int)attr[0].Code, Activator.CreateInstance(type) as ISpaCommandHandler);
						}
					}
				}
			}
			return count;
		}
		protected void RegisterCommandHandler(int code, ISpaCommandHandler handle)
		{
			this.handles.Add(code, handle);
		}
	}
}
