using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	public class MarryCommandMgr
	{
		private Dictionary<int, IMarryCommandHandler> handles = new Dictionary<int, IMarryCommandHandler>();
		public IMarryCommandHandler LoadCommandHandler(int code)
		{
			return this.handles[code];
		}
		public MarryCommandMgr()
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
					if (type.GetInterface("Game.Server.SceneMarryRooms.TankHandle.IMarryCommandHandler") != null)
					{
						MarryCommandAttbute[] attr = (MarryCommandAttbute[])type.GetCustomAttributes(typeof(MarryCommandAttbute), true);
						if (attr.Length > 0)
						{
							count++;
							this.RegisterCommandHandler((int)attr[0].Code, Activator.CreateInstance(type) as IMarryCommandHandler);
						}
					}
				}
			}
			return count;
		}
		protected void RegisterCommandHandler(int code, IMarryCommandHandler handle)
		{
			this.handles.Add(code, handle);
		}
	}
}
