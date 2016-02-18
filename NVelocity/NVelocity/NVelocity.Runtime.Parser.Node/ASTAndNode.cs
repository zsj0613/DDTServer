using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTAndNode : SimpleNode
	{
		public ASTAndNode(int id) : base(id)
		{
		}

		public ASTAndNode(Parser p, int id) : base(p, id)
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
			bool result;
			if (child == null || child2 == null)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					(child == null) ? "Left" : "Right",
					" side of '&&' operation is null. Operation not possible. ",
					context.CurrentTemplateName,
					" [line ",
					base.Line,
					", column ",
					base.Column,
					"]"
				}));
				result = false;
			}
			else
			{
				result = (child.Evaluate(context) && child2.Evaluate(context));
			}
			return result;
		}
	}
}
