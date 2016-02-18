using Commons.Collections;
using NVelocity.Exception;
using NVelocity.Util;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace NVelocity.Runtime.Resource.Loader
{
	public class AssemblyResourceLoader : ResourceLoader
	{
		private ArrayList assemblyNames;

		public override void Init(ExtendedProperties configuration)
		{
			this.assemblyNames = configuration.GetVector("assembly");
		}

		public override Stream GetResourceStream(string templateName)
		{

            if (templateName == null || templateName.Length == 0)
			{
				throw new ResourceNotFoundException("Need to specify a file name or file path!");
			}
			string text = StringUtils.NormalizePath(templateName);
			if (text.StartsWith("\\"))
			{
				text = text.Substring(1);
			}
			if (text == null || text.Length == 0)
			{
				string exceptionMessage = "File resource error : argument " + text + " contains .. and may be trying to access content outside of template root.  Rejected.";
				throw new ResourceNotFoundException(exceptionMessage);
			}
			if (text.StartsWith("/"))
			{
				text = text.Substring(1);
			}
			text = text.Replace('\\', '.');
			text = text.Replace('/', '.');
			foreach (string text2 in this.assemblyNames)
			{
                Assembly assembly = null;
				try
				{
					assembly = Assembly.LoadFrom(text2);
				}
				catch (System.Exception innerException)
				{              
                    throw new ResourceNotFoundException("Assembly could not be found " + text2, innerException);
				}
				Stream manifestResourceStream = assembly.GetManifestResourceStream(text);
				if (manifestResourceStream != null)
				{
					return manifestResourceStream;
				}
			}
			throw new ResourceNotFoundException("AssemblyResourceLoader Error: cannot locate resource " + text);
		}

		public override bool IsSourceModified(Resource resource)
		{
			return false;
		}

		public override long GetLastModified(Resource resource)
		{
			return resource.LastModified;
		}
	}
}
