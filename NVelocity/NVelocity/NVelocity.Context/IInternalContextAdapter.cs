using System;
using System.Collections;

namespace NVelocity.Context
{
	public interface IInternalContextAdapter : IInternalHousekeepingContext, IContext, IInternalWrapperContext, IInternalEventContext, IDictionary, ICollection, IEnumerable
	{
		new object Remove(object key);
	}
}
