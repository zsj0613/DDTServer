using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTIfStatement : SimpleNode
	{
		public ASTIfStatement(int id) : base(id)
		{
		}

		public ASTIfStatement(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			bool result;
			if (base.GetChild(0).Evaluate(context))
			{
				base.GetChild(1).Render(context, writer);
				result = true;
			}
			else
			{
				int childrenCount = base.ChildrenCount;
				for (int i = 2; i < childrenCount; i++)
				{
					if (base.GetChild(i).Evaluate(context))
					{
						base.GetChild(i).Render(context, writer);
						result = true;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		public void Process(IInternalContextAdapter context, IParserVisitor visitor)
		{
		}
	}
}
