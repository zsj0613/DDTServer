using NVelocity.Runtime.Parser.Node;
using System;
using System.Collections;

namespace NVelocity.Runtime.Visitor
{
	public class VMReferenceMungeVisitor : BaseVisitor
	{
		private Hashtable argmap = null;

		public VMReferenceMungeVisitor(Hashtable map)
		{
			this.argmap = map;
		}

		public override object Visit(ASTReference node, object data)
		{
			string text = (string)this.argmap[node.Literal.Substring(1)];
			if (text != null)
			{
				node.SetLiteral(text);
			}
			data = node.ChildrenAccept(this, data);
			return data;
		}
	}
}
