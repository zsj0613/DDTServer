using System;
using System.IO;
using System.Text;

namespace Commons.Collections
{
	internal class PropertiesReader : StreamReader
	{
		public PropertiesReader(StreamReader reader) : base(reader.BaseStream)
		{
		}

		public string ReadProperty()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string result;
			try
			{
				string text;
				while (true)
				{
					text = this.ReadLine();
					if (text == null)
					{
						break;
					}
					text = text.Trim();
					if (text.Length != 0 && text[0] != '#')
					{
						if (!text.EndsWith("\\"))
						{
							goto IL_71;
						}
						text = text.Substring(0, text.Length - 1);
						stringBuilder.Append(text);
					}
				}
				result = null;
				return result;
				IL_71:
				stringBuilder.Append(text);
			}
			catch (NullReferenceException)
			{
				result = null;
				return result;
			}
			result = stringBuilder.ToString();
			return result;
		}
	}
}
