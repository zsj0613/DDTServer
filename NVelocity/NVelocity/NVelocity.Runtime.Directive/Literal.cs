using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;
using System;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public class Literal : Directive
	{
		internal string literalText;

		public override string Name
		{
			get
			{
				return "literal";
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override DirectiveType Type
		{
			get
			{
				return DirectiveType.BLOCK;
			}
		}

		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);
			this.literalText = node.GetChild(0).Literal;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			writer.Write(this.literalText);
			return true;
		}
	}
}
