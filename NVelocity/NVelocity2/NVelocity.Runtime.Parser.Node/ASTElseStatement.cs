using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTElseStatement : SimpleNode
	{
		public ASTElseStatement(int id) : base(id)
		{
		}

		public ASTElseStatement(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			return true;
		}
	}
}
