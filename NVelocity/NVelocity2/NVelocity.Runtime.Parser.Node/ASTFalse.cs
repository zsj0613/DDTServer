using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTFalse : SimpleNode
	{
		private const bool val = false;

		public ASTFalse(int id) : base(id)
		{
		}

		public ASTFalse(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			return false;
		}

		public override object Value(IInternalContextAdapter context)
		{
			return false;
		}
	}
}
