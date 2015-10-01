using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTOrNode : SimpleNode
	{
		public ASTOrNode(int id) : base(id)
		{
		}

		public ASTOrNode(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Value(IInternalContextAdapter context)
		{
			return this.Evaluate(context);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			INode child = base.GetChild(0);
			INode child2 = base.GetChild(1);
			return (child != null && child.Evaluate(context)) || (child2 != null && child2.Evaluate(context));
		}
	}
}
