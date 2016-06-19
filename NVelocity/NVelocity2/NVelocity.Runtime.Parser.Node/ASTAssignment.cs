using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTAssignment : SimpleNode
	{
		public ASTAssignment(int id) : base(id)
		{
		}

		public ASTAssignment(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
