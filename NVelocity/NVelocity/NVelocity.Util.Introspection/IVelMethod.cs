using System;

namespace NVelocity.Util.Introspection
{
	public interface IVelMethod
	{
		bool Cacheable
		{
			get;
		}

		string MethodName
		{
			get;
		}

		Type ReturnType
		{
			get;
		}

		object Invoke(object o, object[] paramsRenamed);
	}
}
