using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTText : SimpleNode
	{
		private char[] ctext;

		public ASTText(int id) : base(id)
		{
		}

		public ASTText(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			Token firstToken = base.FirstToken;
			string text = NodeUtils.tokenLiteral(firstToken);
			this.ctext = text.ToCharArray();
			return data;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			writer.Write(this.ctext, 0, this.ctext.Length);
			return true;
		}
	}
}
