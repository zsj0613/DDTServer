using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTTrue : SimpleNode
	{
		private const bool val = true;

		public ASTTrue(int id) : base(id)
		{
		}

		public ASTTrue(Parser p, int id) : base(p, id)
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

		public override object Value(IInternalContextAdapter context)
		{
			return true;
		}
	}
}
