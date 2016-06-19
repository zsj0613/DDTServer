using NVelocity.Runtime;
using System;

namespace NVelocity.Util.Introspection
{
	public interface UberspectLoggable
	{
		IRuntimeLogger RuntimeLogger
		{
			set;
		}
	}
}
