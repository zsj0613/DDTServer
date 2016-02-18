using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTBlock : SimpleNode
	{
		public ASTBlock(int id) : base(id)
		{
		}

		public ASTBlock(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			int childrenCount = base.ChildrenCount;
			for (int i = 0; i < childrenCount; i++)
			{
				base.GetChild(i).Render(context, writer);
			}
			return true;
		}
	}
}
