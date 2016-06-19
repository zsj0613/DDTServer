using System;

namespace NVelocity.Util.Introspection
{
	public interface IVelPropertySet
	{
		bool Cacheable
		{
			get;
		}

		string MethodName
		{
			get;
		}

		object Invoke(object o, object arg);
	}
}
