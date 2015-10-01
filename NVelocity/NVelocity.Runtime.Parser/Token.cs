using System;

namespace NVelocity.Runtime.Parser
{
	public class Token
	{
		public int Kind;

		public int BeginLine;

		public int BeginColumn;

		public int EndLine;

		public int EndColumn;

		public string Image;

		public Token Next;

		public Token SpecialToken;

		public override string ToString()
		{
			return this.Image;
		}

		public static Token NewToken(int ofKind)
		{
			return new Token();
		}
	}
}
