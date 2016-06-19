using NVelocity.Context;
using System;
using System.Text;

namespace NVelocity.Runtime.Parser.Node
{
	public class NodeUtils
	{
		public static string specialText(Token t)
		{
			string text = "";
			string result;
			if (t.SpecialToken == null || t.SpecialToken.Image.StartsWith("##"))
			{
				result = text;
			}
			else
			{
				Token token = t.SpecialToken;
				while (token.SpecialToken != null)
				{
					token = token.SpecialToken;
				}
				while (token != null)
				{
					string image = token.Image;
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < image.Length; i++)
					{
						char c = image[i];
						if (c == '#' || c == '$')
						{
							stringBuilder.Append(c);
						}
						if (c == '\\')
						{
							bool flag = false;
							int num = i;
							bool flag2 = true;
							while (flag2 && num < image.Length)
							{
								char c2 = image[num];
								if (c2 != '\\')
								{
									if (c2 == '$')
									{
										flag = true;
										flag2 = false;
									}
									else
									{
										flag2 = false;
									}
								}
								num++;
							}
							if (flag)
							{
								string value = image.Substring(i, num - i);
								stringBuilder.Append(value);
								i = num;
							}
						}
					}
					text += stringBuilder.ToString();
					token = token.Next;
				}
				result = text;
			}
			return result;
		}

		public static string tokenLiteral(Token t)
		{
			return NodeUtils.specialText(t) + t.Image;
		}

		public static string interpolate(string argStr, IContext vars)
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
						if (c == '_' || c == '-' || char.IsLetterOrDigit(c))
						{
							stringBuilder2.Append(c);
						}
						else if (c != '{' && c != '}')
						{
							break;
						}
					}
					if (stringBuilder2.Length > 0)
					{
						object obj = vars.Get(stringBuilder2.ToString());
						if (obj == null)
						{
							stringBuilder.Append("$").Append(stringBuilder2.ToString());
						}
						else
						{
							stringBuilder.Append(obj.ToString());
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
