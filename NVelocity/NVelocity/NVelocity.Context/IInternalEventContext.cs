using NVelocity.App.Events;
using System;

namespace NVelocity.Context
{
	public interface IInternalEventContext
	{
		EventCartridge EventCartridge
		{
			get;
		}

		EventCartridge AttachEventCartridge(EventCartridge ec);
	}
}
