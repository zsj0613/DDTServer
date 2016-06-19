using NVelocity.Context;
using System;
using System.Collections;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTIntegerRange : SimpleNode
	{
		public ASTIntegerRange(int id) : base(id)
		{
		}

		public ASTIntegerRange(Parser p, int id) : base(p, id)
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
					" side of range operator [n..m] has null value. Operation not possible. ",
					context.CurrentTemplateName,
					" [line ",
					base.Line,
					", column ",
					base.Column,
					"]"
				}));
				result = null;
			}
			else if (!(obj is int) || !(obj2 is int))
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					(!(obj is int)) ? "Left" : "Right",
					" side of range operator is not a valid type. Currently only integers (1,2,3...) and Integer type is supported. ",
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
				int num = (int)obj;
				int num2 = (int)obj2;
				int num3 = Math.Abs(num - num2);
				num3++;
				int num4 = (num >= num2) ? -1 : 1;
				ArrayList arrayList = new ArrayList(num3);
				int num5 = num;
				for (int i = 0; i < num3; i++)
				{
					arrayList.Add(num5);
					num5 += num4;
				}
				result = arrayList;
			}
			return result;
		}
	}
}
