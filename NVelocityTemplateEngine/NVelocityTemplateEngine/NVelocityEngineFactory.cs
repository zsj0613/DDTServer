using NVelocityTemplateEngine.Engines;
using NVelocityTemplateEngine.Interfaces;
using System;

namespace NVelocityTemplateEngine
{
	public class NVelocityEngineFactory
	{
		public static INVelocityEngine CreateNVelocityFileEngine(string templateDirectory, bool cacheTemplate)
		{
			return new NVelocityFileEngine(templateDirectory, cacheTemplate);
		}

		public static INVelocityEngine CreateNVelocityAssemblyEngine(string assemblyName, bool cacheTemplate)
		{
			return new NVelocityAssemblyEngine(assemblyName, cacheTemplate);
		}
	}
}
