using NVelocity.Context;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTEscape : SimpleNode
	{
		private string text = "";

		public ASTEscape(int id) : base(id)
		{
		}

		public ASTEscape(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			this.text = base.FirstToken.Image;
			return data;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			char[] array = this.text.ToCharArray();
			writer.Write(array, 0, array.Length);
			return true;
		}
	}
}
