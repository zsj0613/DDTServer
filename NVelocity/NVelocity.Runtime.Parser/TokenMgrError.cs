using System;
using System.Text;

namespace NVelocity.Runtime.Parser
{
	public class TokenMgrError : ApplicationException
	{
		internal const int LEXICAL_ERROR = 0;

		internal const int STATIC_LEXER_ERROR = 1;

		internal const int INVALID_LEXICAL_STATE = 2;

		internal const int LOOP_DETECTED = 3;

		internal int errorCode;

		public override string Message
		{
			get
			{
				return base.Message;
			}
		}

		protected internal static string AddEscapes(string str)
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

		private static string LexicalError(bool EOFSeen, int lexState, int errorLine, int errorColumn, string errorAfter, char curChar)
		{
			return string.Concat(new object[]
			{
				"Lexical error at line ",
				errorLine,
				", column ",
				errorColumn,
				".  Encountered: ",
				EOFSeen ? "<EOF> " : string.Concat(new object[]
				{
					"\"",
					TokenMgrError.AddEscapes(curChar.ToString()),
					"\" (",
					(int)curChar,
					"), "
				}),
				"after : \"",
				TokenMgrError.AddEscapes(errorAfter),
				"\""
			});
		}

		public TokenMgrError()
		{
		}

		public TokenMgrError(string message, int reason) : base(message)
		{
			this.errorCode = reason;
		}

		public TokenMgrError(bool EOFSeen, int lexState, int errorLine, int errorColumn, string errorAfter, char curChar, int reason) : this(TokenMgrError.LexicalError(EOFSeen, lexState, errorLine, errorColumn, errorAfter, curChar), reason)
		{
		}
	}
}
