using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTExpression : SimpleNode
	{
		public ASTExpression(int id) : base(id)
		{
		}

		public ASTExpression(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			return base.GetChild(0).Evaluate(context);
		}

		public override object Value(IInternalContextAdapter context)
		{
			return base.GetChild(0).Value(context);
		}
	}
}
