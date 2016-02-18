using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTGTNode : SimpleNode
	{
		public ASTGTNode(int id) : base(id)
		{
		}

		public ASTGTNode(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			object obj = base.GetChild(0).Value(context);
			object obj2 = base.GetChild(1).Value(context);
			bool result;
			if (obj == null || obj2 == null)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					(obj == null) ? "Left" : "Right",
					" side (",
					base.GetChild((obj == null) ? 0 : 1).Literal,
					") of '>' operation has null value. Operation not possible. ",
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
				try
				{
					result = (ObjectComparer.CompareObjects(obj, obj2) == 1);
				}
				catch (ArgumentException ex)
				{
					this.rsvc.Error(ex.Message);
					result = false;
				}
			}
			return result;
		}
	}
}
