using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTAddNode : SimpleNode
	{
		public ASTAddNode(int id) : base(id)
		{
		}

		public ASTAddNode(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Value(IInternalContextAdapter context)
		{
			object obj = base.GetChild(0).Value(context);
			object obj2 = base.GetChild(1).Value(context);
			object result;
			if (obj == null || obj2 == null)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					(obj == null) ? "Left" : "Right",
					" side (",
					base.GetChild((obj == null) ? 0 : 1).Literal,
					") of addition operation has null value. Operation not possible. ",
					context.CurrentTemplateName,
					" [line ",
					base.Line,
					", column ",
					base.Column,
					"]"
				}));
				result = null;
			}
			else
			{
				Type type = MathUtil.ToMaxType(obj.GetType(), obj2.GetType());
				if (type == null)
				{
					result = null;
				}
				else
				{
					result = MathUtil.Add(type, obj, obj2);
				}
			}
			return result;
		}
	}
}
