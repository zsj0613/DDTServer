using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTElseIfStatement : SimpleNode
	{
		public ASTElseIfStatement(int id) : base(id)
		{
		}

		public ASTElseIfStatement(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override bool Evaluate(IInternalContextAdapter context)
		{
			return base.GetChild(0).Evaluate(context);
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			return base.GetChild(1).Render(context, writer);
		}
	}
}
