using System;

namespace NVelocity.Util.Introspection
{
	public interface IUberspect
	{
		void Init();

		IVelMethod GetMethod(object obj, string method, object[] args, Info info);

		IVelPropertyGet GetPropertyGet(object obj, string identifier, Info info);

		IVelPropertySet GetPropertySet(object obj, string identifier, object arg, Info info);
	}
}
