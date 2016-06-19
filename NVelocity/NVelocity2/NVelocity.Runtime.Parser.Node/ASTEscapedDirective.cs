using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTEscapedDirective : SimpleNode
	{
		public ASTEscapedDirective(int id) : base(id)
		{
		}

		public ASTEscapedDirective(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			writer.Write(base.FirstToken.Image);
			return true;
		}
	}
}
