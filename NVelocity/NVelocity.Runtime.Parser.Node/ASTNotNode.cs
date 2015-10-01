using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTNotNode : SimpleNode
	{
		public ASTNotNode(int id) : base(id)
		{
		}

		public ASTNotNode(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			return !base.GetChild(0).Evaluate(context);
		}

		public override object Value(IInternalContextAdapter context)
		{
			return !this.Evaluate(context);
		}
	}
}
