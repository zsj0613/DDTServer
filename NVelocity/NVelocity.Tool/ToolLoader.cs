using System;

namespace NVelocity.Tool
{
	public class ToolLoader
	{
		public object Load(string clazz)
		{
			object result;
			try
			{
				Type type = Type.GetType(clazz);
				object obj = Activator.CreateInstance(type);
				result = obj;
			}
			catch (System.Exception)
			{
				result = null;
			}
			return result;
		}
	}
}
