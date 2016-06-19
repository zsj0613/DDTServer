using System;

namespace NVelocity.Runtime.Resource
{
	public interface IResourceManager
	{
		void Initialize(IRuntimeServices rs);

		Resource GetResource(string resourceName, ResourceType resourceType, string encoding);

		string GetLoaderNameForResource(string resourceName);
	}
}
