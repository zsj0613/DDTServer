using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTComment : SimpleNode
	{
		public ASTComment(int id) : base(id)
		{
		}

		public ASTComment(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
