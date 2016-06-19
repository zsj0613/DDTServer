using System;
using System.Collections;
using System.IO;
using System.Text;

namespace NVelocity.Util
{
	public class StringUtils
	{
		private static readonly string EOL = Environment.NewLine;

		public string Concat(IList list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(list[i].ToString());
			}
			return stringBuilder.ToString();
		}

		public static string GetPackageAsPath(string pckge)
		{
			return pckge.Replace('.', Path.DirectorySeparatorChar.ToString()[0]) + Path.DirectorySeparatorChar.ToString();
		}

		public static string RemoveUnderScores(string data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(data, "_");
			while (tokenizer.HasMoreTokens())
			{
				string data2 = tokenizer.NextToken();
				stringBuilder.Append(StringUtils.FirstLetterCaps(data2));
			}
			return stringBuilder.ToString();
		}

		public static string RemoveAndHump(string data)
		{
			return StringUtils.RemoveAndHump(data, "_");
		}

		public static string RemoveAndHump(string data, string replaceThis)
		{
			StringBuilder stringBuilder = new StringBuilder();
			SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(data, replaceThis);
			while (tokenizer.HasMoreTokens())
			{
				string data2 = tokenizer.NextToken();
				stringBuilder.Append(StringUtils.CapitalizeFirstLetter(data2));
			}
			return stringBuilder.ToString();
		}

		public static string FirstLetterCaps(string data)
		{
			string str = data.Substring(0, 1).ToUpper();
			string str2 = data.Substring(1).ToLower();
			return str + str2;
		}

		public static string CapitalizeFirstLetter(string data)
		{
			string str = data.Substring(0, 1).ToUpper();
			string str2 = data.Substring(1);
			return str + str2;
		}

		public static string[] Split(string line, string delim)
		{
			ArrayList arrayList = new ArrayList();
			SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(line, delim);
			while (tokenizer.HasMoreTokens())
			{
				arrayList.Add(tokenizer.NextToken());
			}
			return (string[])arrayList.ToArray(typeof(string));
		}

		public static string Chop(string s, int i)
		{
			return StringUtils.Chop(s, i, StringUtils.EOL);
		}

		public static string Chop(string s, int i, string eol)
		{
			string result;
			if (i == 0 || s == null || eol == null)
			{
				result = s;
			}
			else
			{
				int num = s.Length;
				if (eol.Length == 2 && s.EndsWith(eol))
				{
					num -= 2;
					i--;
				}
				if (i > 0)
				{
					num -= i;
				}
				if (num < 0)
				{
					num = 0;
				}
				result = s.Substring(0, num);
			}
			return result;
		}

		public static StringBuilder StringSubstitution(string argStr, Hashtable vars)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < argStr.Length)
			{
				char c = argStr[i];
				char c2 = c;
				if (c2 != '$')
				{
					stringBuilder.Append(c);
					i++;
				}
				else
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					for (i++; i < argStr.Length; i++)
					{
						c = argStr[i];
						if (c != '_' && !char.IsLetterOrDigit(c))
						{
							break;
						}
						stringBuilder2.Append(c);
					}
					if (stringBuilder2.Length > 0)
					{
						string text = (string)vars[stringBuilder2.ToString()];
						if (text != null)
						{
							stringBuilder.Append(text);
						}
					}
				}
			}
			return stringBuilder;
		}

		public static string FileContentsToString(string file)
		{
			string result = "";
			FileInfo fileInfo = new FileInfo(file);
			bool flag = File.Exists(fileInfo.FullName) || Directory.Exists(fileInfo.FullName);
			if (flag)
			{
				try
				{
					StreamReader streamReader = new StreamReader(fileInfo.FullName);
					char[] array = new char[(int)SupportClass.FileLength(fileInfo)];
					streamReader.Read(array, 0, array.Length);
					result = new string(array);
					streamReader.Close();
				}
				catch (System.Exception ex)
				{
					Console.Out.WriteLine(ex);
					SupportClass.WriteStackTrace(ex, Console.Error);
				}
			}
			return result;
		}

		public static string CollapseNewlines(string argStr)
		{
			char c = argStr[0];
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < argStr.Length; i++)
			{
				char c2 = argStr[i];
				if (c2 != '\n' || c != '\n')
				{
					stringBuilder.Append(c2);
					c = c2;
				}
			}
			return stringBuilder.ToString();
		}

		public static string CollapseSpaces(string argStr)
		{
			char c = argStr[0];
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < argStr.Length; i++)
			{
				char c2 = argStr[i];
				if (c2 != ' ' || c != ' ')
				{
					stringBuilder.Append(c2);
					c = c2;
				}
			}
			return stringBuilder.ToString();
		}

		public static string Sub(string line, string oldString, string newString)
		{
			int num = 0;
			string result;
			if ((num = line.IndexOf(oldString, num)) >= 0)
			{
				char[] array = line.ToCharArray();
				char[] value = newString.ToCharArray();
				int length = oldString.Length;
				StringBuilder stringBuilder = new StringBuilder(array.Length);
				stringBuilder.Append(array, 0, num).Append(value);
				num += length;
				int num2 = num;
				while ((num = line.IndexOf(oldString, num)) > 0)
				{
					stringBuilder.Append(array, num2, num - num2).Append(value);
					num += length;
					num2 = num;
				}
				stringBuilder.Append(array, num2, array.Length - num2);
				result = stringBuilder.ToString();
			}
			else
			{
				result = line;
			}
			return result;
		}

		public static string StackTrace(System.Exception e)
		{
			string result = null;
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				SupportClass.WriteStackTrace(e, new StreamWriter(memoryStream)
				{
					AutoFlush = true
				});
				byte[] buffer = memoryStream.GetBuffer();
				char[] array = new char[buffer.Length];
				buffer.CopyTo(array, 0);
				result = new string(array);
			}
			catch (System.Exception var_5_47)
			{
			}
			return result;
		}

		public static string NormalizePath(string path)
		{
			string text = path;
			if (text.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
			{
				text = text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}
			if (!text.StartsWith(Path.DirectorySeparatorChar.ToString()))
			{
				text = Path.DirectorySeparatorChar + text;
			}
			while (true)
			{
				int num = text.IndexOf("//");
				if (num < 0)
				{
					break;
				}
				text = text.Substring(0, num) + text.Substring(num + 1);
			}
			while (true)
			{
				int num = text.IndexOf("%20");
				if (num < 0)
				{
					break;
				}
				text = text.Substring(0, num) + " " + text.Substring(num + 3);
			}
			while (true)
			{
				int num = text.IndexOf("/./");
				if (num < 0)
				{
					break;
				}
				text = text.Substring(0, num) + text.Substring(num + 2);
			}
			while (true)
			{
				int num = text.IndexOf("/../");
				if (num < 0)
				{
					break;
				}
				if (num == 0)
				{
					goto Block_7;
				}
				int length = text.LastIndexOf('/', num - 1);
				text = text.Substring(0, length) + text.Substring(num + 3);
			}
			string result = text;
			return result;
			Block_7:
			result = null;
			return result;
		}

		public string Select(bool state, string trueString, string falseString)
		{
			string result;
			if (state)
			{
				result = trueString;
			}
			else
			{
				result = falseString;
			}
			return result;
		}

		public bool AllEmpty(IList list)
		{
			int count = list.Count;
			bool result;
			for (int i = 0; i < count; i++)
			{
				if (list[i] != null && list[i].ToString().Length > 0)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
	}
}
