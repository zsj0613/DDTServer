using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTprocess : SimpleNode
	{
		public ASTprocess(int id) : base(id)
		{
		}

		public ASTprocess(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
