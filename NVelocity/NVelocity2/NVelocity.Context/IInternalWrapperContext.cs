using System;

namespace NVelocity.Context
{
	public interface IInternalWrapperContext
	{
		IContext InternalUserContext
		{
			get;
		}

		IInternalContextAdapter BaseContext
		{
			get;
		}
	}
}
