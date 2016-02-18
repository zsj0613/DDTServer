using System;

namespace NVelocity.Runtime.Resource
{
	public class ResourceFactory
	{
		public static Resource GetResource(string resourceName, ResourceType resourceType)
		{
			Resource result = null;
			switch (resourceType)
			{
			case ResourceType.Template:
				result = new Template();
				break;
			case ResourceType.Content:
				result = new ContentResource();
				break;
			}
			return result;
		}
	}
}
