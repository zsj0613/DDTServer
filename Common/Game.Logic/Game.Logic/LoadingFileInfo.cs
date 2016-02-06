using System;
namespace Game.Logic
{
	public class LoadingFileInfo
	{
		public int Type;
		public string Path;
		public string ClassName;
		public LoadingFileInfo(int type, string path, string className)
		{
			this.Type = type;
			this.Path = path;
			this.ClassName = className;
		}
	}
}
