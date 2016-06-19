using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;
using System;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public class Component : Directive
	{
		public override string Name
		{
			get
			{
				return "component";
			}
			set
			{
			}
		}

		public override DirectiveType Type
		{
			get
			{
				return DirectiveType.LINE;
			}
		}

		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			return true;
		}
	}
}
