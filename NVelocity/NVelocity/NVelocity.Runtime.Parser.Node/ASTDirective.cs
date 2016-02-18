using NVelocity.Context;
using NVelocity.Runtime.Directive;
using System;
using System.IO;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTDirective : SimpleNode
	{
		private Directive.Directive directive;

		private string directiveName = "";

		public string DirectiveName
		{
			get
			{
				return this.directiveName;
			}
			set
			{
				this.directiveName = value;
			}
		}

		public Directive.Directive Directive
		{
			get
			{
				return this.directive;
			}
			set
			{
				this.directive = value;
			}
		}

		public ASTDirective(int id) : base(id)
		{
		}

		public ASTDirective(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			base.Init(context, data);
			if (this.directive == null && this.rsvc.IsVelocimacro(this.directiveName, context.CurrentTemplateName))
			{
				this.directive = this.rsvc.GetVelocimacro(this.directiveName, context.CurrentTemplateName);
			}
			if (this.directive != null)
			{
				this.directive.Init(this.rsvc, context, this);
				this.directive.SetLocation(base.Line, base.Column);
			}
			return data;
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			if (this.directive != null)
			{
				this.directive.Render(context, writer, this);
			}
			else
			{
				writer.Write("#");
				writer.Write(this.directiveName);
			}
			return true;
		}
	}
}
