using System;

namespace NVelocity.Util.Introspection
{
	public class IntrospectionCacheData
	{
		public object Thingy;

		public Type ContextData;

		public IntrospectionCacheData()
		{
		}

		public IntrospectionCacheData(Type contextData, object thingy)
		{
			this.Thingy = thingy;
			this.ContextData = contextData;
		}
	}
}
