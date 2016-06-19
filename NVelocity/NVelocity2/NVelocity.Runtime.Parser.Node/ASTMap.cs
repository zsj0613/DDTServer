using NVelocity.Context;
using System;
using System.Collections;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTMap : SimpleNode
	{
		public ASTMap(int id) : base(id)
		{
		}

		public ASTMap(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Value(IInternalContextAdapter context)
		{
			int childrenCount = base.ChildrenCount;
			IDictionary dictionary = new Hashtable();
			for (int i = 0; i < childrenCount; i += 2)
			{
				SimpleNode simpleNode = (SimpleNode)base.GetChild(i);
				SimpleNode simpleNode2 = (SimpleNode)base.GetChild(i + 1);
				object key = (simpleNode == null) ? null : simpleNode.Value(context);
				object value = (simpleNode2 == null) ? null : simpleNode2.Value(context);
				dictionary.Add(key, value);
			}
			return dictionary;
		}
	}
}
