using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTVariable : SimpleNode
	{
		public ASTVariable(int id) : base(id)
		{
		}

		public ASTVariable(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
