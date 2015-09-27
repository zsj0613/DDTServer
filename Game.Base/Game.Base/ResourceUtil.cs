using System;
using System.IO;
using System.Reflection;
namespace Game.Base
{
	public class ResourceUtil
	{
		public static Stream GetResourceStream(string fileName, Assembly assem)
		{
			fileName = fileName.ToLower();
			string[] manifestResourceNames = assem.GetManifestResourceNames();
			Stream result;
			for (int i = 0; i < manifestResourceNames.Length; i++)
			{
				string name = manifestResourceNames[i];
				if (name.ToLower().EndsWith(fileName))
				{
					result = assem.GetManifestResourceStream(name);
					return result;
				}
			}
			result = null;
			return result;
		}
		public static void ExtractResource(string fileName, Assembly assembly)
		{
			ResourceUtil.ExtractResource(fileName, fileName, assembly);
		}
		public static void ExtractResource(string resourceName, string fileName, Assembly assembly)
		{
			FileInfo finfo = new FileInfo(fileName);
			if (!finfo.Directory.Exists)
			{
				finfo.Directory.Create();
			}
			using (StreamReader reader = new StreamReader(ResourceUtil.GetResourceStream(resourceName, assembly)))
			{
				using (StreamWriter writer = new StreamWriter(File.Create(fileName)))
				{
					writer.Write(reader.ReadToEnd());
				}
			}

		}
        public static void ExtractResource(string resourceName, string fileName, Assembly assembly, bool IsReplace)
        {
            if (IsReplace)
            {
                FileInfo finfo = new FileInfo(fileName);
                if (finfo.Exists)
                {
                    finfo.Delete();
                }
                ExtractResource(resourceName, fileName, assembly);
            }
            else
            {
                ExtractResource(resourceName, fileName, assembly);
            }

        }
    }
}
