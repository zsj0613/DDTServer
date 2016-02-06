using System;
using System.Runtime.InteropServices;
using System.Text;
namespace Bussiness
{
	public class IniReader
	{
		private string FilePath;
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
		public IniReader(string _FilePath)
		{
			this.FilePath = _FilePath;
		}
		public string GetIniString(string Section, string Key)
		{
			StringBuilder temp = new StringBuilder(2550);
			IniReader.GetPrivateProfileString(Section, Key, "", temp, 2550, this.FilePath);
			return temp.ToString();
		}
	}
}
