using NVelocity.Context;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTNumberLiteral : SimpleNode
	{
		private int value__Field;

		public ASTNumberLiteral(int id) : base(id)
		{
		}

		public ASTNumberLiteral(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			base.Init(context, data);
			this.value__Field = int.Parse(base.FirstToken.Image);
			return data;
		}

		public override object Value(IInternalContextAdapter context)
		{
			return this.value__Field;
		}
	}
}
