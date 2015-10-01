using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTIncludeStatement : SimpleNode
	{
		public ASTIncludeStatement(int id) : base(id)
		{
		}

		public ASTIncludeStatement(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
