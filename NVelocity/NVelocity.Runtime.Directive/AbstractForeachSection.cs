using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;
using System;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public abstract class AbstractForeachSection : Directive, IForeachSection
	{
		public override string Name
		{
			get
			{
				return this.Section.ToString().ToLower();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override bool AcceptParams
		{
			get
			{
				return false;
			}
		}

		public override DirectiveType Type
		{
			get
			{
				return DirectiveType.LINE;
			}
		}

		public abstract ForeachSectionEnum Section
		{
			get;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			return true;
		}
	}
}
