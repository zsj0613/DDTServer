using Commons.Collections;
using NVelocity.Exception;
using NVelocity.Util;
using System;
using System.Collections;
using System.IO;

namespace NVelocity.Runtime.Resource.Loader
{
	public class FileResourceLoader : ResourceLoader
	{
		protected ArrayList paths = null;

		protected Hashtable templatePaths = new Hashtable();

		public override void Init(ExtendedProperties configuration)
		{
			this.rsvc.Info("FileResourceLoader : initialization starting.");
			this.paths = configuration.GetVector("path");
		}

		public override Stream GetResourceStream(string templateName)
		{
			Stream result;
			lock (this)
			{
				int count = this.paths.Count;
				if (templateName == null || templateName.Length == 0)
				{
					throw new ResourceNotFoundException("Need to specify a file name or file path!");
				}
				string text = StringUtils.NormalizePath(templateName);
				if (text == null || text.Length == 0)
				{
					string exceptionMessage = "File resource error : argument " + text + " contains .. and may be trying to access content outside of template root.  Rejected.";
					throw new ResourceNotFoundException(exceptionMessage);
				}
				if (text.StartsWith("/") || text.StartsWith("\\"))
				{
					text = text.Substring(1);
				}
				for (int i = 0; i < count; i++)
				{
					string text2 = (string)this.paths[i];
					if (text2.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
					{
						text2 = text2.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
					}
					Stream stream = this.FindTemplate(text2, text);
					if (stream != null)
					{
						SupportClass.PutElement(this.templatePaths, templateName, text2);
						result = stream;
						return result;
					}
				}
				string exceptionMessage2 = "FileResourceLoader Error: cannot find resource " + text;
				throw new ResourceNotFoundException(exceptionMessage2);
			}
			return result;
		}

		private Stream FindTemplate(string path, string template)
		{
			Stream result;
			try
			{
				string text;
				if (path != null)
				{
					text = path + Path.DirectorySeparatorChar + template;
				}
				else
				{
					text = template;
				}
				this.rsvc.Debug("FileResourceLoader attempting to load " + text);
				FileInfo fileInfo = new FileInfo(text);
				result = new BufferedStream(fileInfo.OpenRead());
			}
			catch (System.Exception ex)
			{
				this.rsvc.Debug("FileResourceLoader : " + ex.Message);
				result = null;
			}
			return result;
		}

		public override bool IsSourceModified(Resource resource)
		{
			string arg = (string)this.templatePaths[resource.Name];
			FileInfo fileInfo = new FileInfo(arg + Path.AltDirectorySeparatorChar + resource.Name);
			return !fileInfo.Exists || fileInfo.LastWriteTime.Ticks != resource.LastModified;
		}

		public override long GetLastModified(Resource resource)
		{
			string arg = (string)this.templatePaths[resource.Name];
			FileInfo fileInfo = new FileInfo(arg + Path.AltDirectorySeparatorChar + resource.Name);
			long result;
			if (fileInfo.Exists)
			{
				result = fileInfo.LastWriteTime.Ticks;
			}
			else
			{
				result = 0L;
			}
			return result;
		}
	}
}
