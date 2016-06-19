using NVelocity.Util;
using System;

namespace NVelocity.Runtime.Resource.Loader
{
	public class ResourceLoaderFactory
	{
		public static ResourceLoader getLoader(IRuntimeServices rs, string loaderClassName)
		{
			ResourceLoader result;
			try
			{
				loaderClassName = loaderClassName.Replace(';', ',');
				Type type = Type.GetType(loaderClassName);
				object obj = Activator.CreateInstance(type);
				ResourceLoader resourceLoader = (ResourceLoader)obj;
				rs.Info("Resource Loader Instantiated: " + resourceLoader.GetType().FullName);
				result = resourceLoader;
			}
			catch (System.Exception e)
			{
				rs.Error("Problem instantiating the template loader.\nLook at your properties file and make sure the\nname of the template loader is correct. Here is the\nerror: " + StringUtils.StackTrace(e));
				throw new System.Exception("Problem initializing template loader: " + loaderClassName + "\nError is: " + StringUtils.StackTrace(e));
			}
			return result;
		}
	}
}
