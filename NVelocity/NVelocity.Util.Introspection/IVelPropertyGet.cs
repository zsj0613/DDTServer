using System;

namespace NVelocity.Util.Introspection
{
	public interface IVelPropertyGet
	{
		bool Cacheable
		{
			get;
		}

		string MethodName
		{
			get;
		}

		object Invoke(object o);
	}
}
