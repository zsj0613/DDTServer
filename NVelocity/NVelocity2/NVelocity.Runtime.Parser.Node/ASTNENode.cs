using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTNENode : SimpleNode
	{
		public ASTNENode(int id) : base(id)
		{
		}

		public ASTNENode(Parser p, int id) : base(p, id)
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
			try
			{
				result = (ObjectComparer.CompareObjects(obj, obj2) != 0);
				return result;
			}
			catch
			{
			}
			result = (obj != obj2);
			return result;
		}
	}
}
