using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using System;
using System.Collections;

namespace NVelocityTemplateEngine.BaseClasses
{
	public abstract class NVelocityEngineBase : VelocityEngine
	{
		protected NVelocityEngineBase(bool cacheTemplate)
		{
			base.SetProperty("assembly.resource.loader.cache", cacheTemplate.ToString().ToLower());
		}

		protected static IContext CreateContext(IDictionary context)
		{
			return new VelocityContext(new Hashtable(context));
		}
	}
}
