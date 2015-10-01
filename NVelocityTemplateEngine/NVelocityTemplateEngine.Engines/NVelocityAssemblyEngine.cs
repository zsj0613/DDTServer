using NVelocity;
using NVelocity.Exception;
using NVelocityTemplateEngine.BaseClasses;
using NVelocityTemplateEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NVelocityTemplateEngine.Engines
{
	public sealed class NVelocityAssemblyEngine : NVelocityEngineBase, INVelocityEngine
	{
        ArrayList listAssembly = new ArrayList();

		internal NVelocityAssemblyEngine(string assemblyName, bool cacheTamplate) : base(cacheTamplate)
		{
            listAssembly.Add(assemblyName);
			base.SetProperty("resource.loader", "assembly");
			base.SetProperty("assembly.resource.loader.class", "NVelocity.Runtime.Resource.Loader.AssemblyResourceLoader;NVelocity");
            base.SetProperty("assembly.resource.loader.assembly", listAssembly);
			base.Init();
		}

		public string Process(IDictionary context, string templateName)
		{
			StringWriter writer = new StringWriter();
			string result;
			try
			{
				Template template = base.GetTemplate(templateName);
				template.Merge(NVelocityEngineBase.CreateContext(context), writer);
			}
			catch (ResourceNotFoundException rnf)
			{
				result = rnf.Message;
				return result;
			}
			catch (ParseErrorException pe)
			{
				result = pe.Message;
				return result;
			}
			result = writer.ToString();
			return result;
		}

		public void Process(IDictionary context, TextWriter writer, string templateName)
		{
			try
			{
				Template template = base.GetTemplate(templateName);
				template.Merge(NVelocityEngineBase.CreateContext(context), writer);
			}
			catch (ResourceNotFoundException rnf)
			{
				writer.Write(rnf.Message);
			}
			catch (ParseErrorException pe)
			{
				writer.Write(pe.Message);
			}
		}
	}
}
