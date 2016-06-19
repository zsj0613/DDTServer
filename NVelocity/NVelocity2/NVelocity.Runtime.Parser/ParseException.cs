using System;
using System.Text;


namespace NVelocity.Runtime.Parser
{
	public class ParseException : System.Exception
	{
		protected internal string eol = Environment.NewLine;

		protected internal bool specialConstructor;

		public Token currentToken;

		public int[][] expectedTokenSequences;

		public string[] tokenImage;

		public override string Message
		{
			get
			{
				string result;
				if (!this.specialConstructor)
				{
					result = base.Message;
				}
				else
				{
					string text = "";
					int num = 0;
					for (int i = 0; i < this.expectedTokenSequences.Length; i++)
					{
						if (num < this.expectedTokenSequences[i].Length)
						{
							num = this.expectedTokenSequences[i].Length;
						}
						for (int j = 0; j < this.expectedTokenSequences[i].Length; j++)
						{
							text = text + this.tokenImage[this.expectedTokenSequences[i][j]] + " ";
						}
						if (this.expectedTokenSequences[i][this.expectedTokenSequences[i].Length - 1] != 0)
						{
							text += "...";
						}
						text = text + this.eol + "    ";
					}
					string text2 = "Encountered \"";
					Token next = this.currentToken.Next;
					for (int i = 0; i < num; i++)
					{
						if (i != 0)
						{
							text2 += " ";
						}
						if (next.Kind == 0)
						{
							text2 += this.tokenImage[0];
							break;
						}
						text2 += this.add_escapes(next.Image);
						next = next.Next;
					}
					object obj = text2;
					text2 = string.Concat(new object[]
					{
						obj,
						"\" at line ",
						this.currentToken.Next.BeginLine,
						", column ",
						this.currentToken.Next.BeginColumn
					});
					text2 = text2 + "." + this.eol;
					if (this.expectedTokenSequences.Length == 1)
					{
						text2 = text2 + "Was expecting:" + this.eol + "    ";
					}
					else
					{
						text2 = text2 + "Was expecting one of:" + this.eol + "    ";
					}
					text2 += text;
					result = text2;
				}
				return result;
			}
		}

		public ParseException(Token currentTokenVal, int[][] expectedTokenSequencesVal, string[] tokenImageVal) : base("")
		{
			this.specialConstructor = true;
			this.currentToken = currentTokenVal;
			this.expectedTokenSequences = expectedTokenSequencesVal;
			this.tokenImage = tokenImageVal;
		}

		public ParseException()
		{
			this.specialConstructor = false;
		}

		public ParseException(string message) : base(message)
		{
			this.specialConstructor = false;
		}

		protected internal string add_escapes(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < str.Length)
			{
				char c = str[i];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\b':
							stringBuilder.Append("\\b");
							break;
						case '\t':
							stringBuilder.Append("\\t");
							break;
						case '\n':
							stringBuilder.Append("\\n");
							break;
						case '\v':
							goto IL_E7;
						case '\f':
							stringBuilder.Append("\\f");
							break;
						case '\r':
							stringBuilder.Append("\\r");
							break;
						default:
							goto IL_E7;
						}
					}
				}
				else if (c != '"')
				{
					if (c != '\'')
					{
						if (c != '\\')
						{
							goto IL_E7;
						}
						stringBuilder.Append("\\\\");
					}
					else
					{
						stringBuilder.Append("\\'");
					}
				}
				else
				{
					stringBuilder.Append("\\\"");
				}
				IL_156:
				i++;
				continue;
				IL_E7:
				char c2;
				if ((c2 = str[i]) < ' ' || c2 > '~')
				{
					string text = "0000" + Convert.ToString((int)c2, 16);
					stringBuilder.Append("\\u" + text.Substring(text.Length - 4, text.Length - (text.Length - 4)));
				}
				else
				{
					stringBuilder.Append(c2);
				}
				goto IL_156;
			}
			return stringBuilder.ToString();
		}
	}
}
