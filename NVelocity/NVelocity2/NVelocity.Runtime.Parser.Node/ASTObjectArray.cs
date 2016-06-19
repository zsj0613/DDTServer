using NVelocity.Context;
using System;
using System.Collections;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTObjectArray : SimpleNode
	{
		public ASTObjectArray(int id) : base(id)
		{
		}

		public ASTObjectArray(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Value(IInternalContextAdapter context)
		{
			int childrenCount = base.ChildrenCount;
			ArrayList arrayList = new ArrayList(childrenCount);
			for (int i = 0; i < childrenCount; i++)
			{
				arrayList.Add(base.GetChild(i).Value(context));
			}
			return arrayList;
		}
	}
}
