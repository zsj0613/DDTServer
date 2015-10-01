using System;
using System.Collections;

namespace NVelocity.Runtime.Resource
{
	public interface ResourceCache
	{
		void initialize(IRuntimeServices rs);

		Resource get(object resourceKey);

		Resource put(object resourceKey, Resource resource);

		Resource remove(object resourceKey);

		IEnumerator enumerateKeys();
	}
}
