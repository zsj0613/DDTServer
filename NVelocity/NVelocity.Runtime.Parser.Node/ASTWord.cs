using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTWord : SimpleNode
	{
		public ASTWord(int id) : base(id)
		{
		}

		public ASTWord(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
