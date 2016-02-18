using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;
using System;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public abstract class Directive
	{
		protected internal IRuntimeServices rsvc = null;

		private int line = 0;

		private int column = 0;

		public abstract string Name
		{
			get;
			set;
		}

		public abstract DirectiveType Type
		{
			get;
		}

		public int Line
		{
			get
			{
				return this.line;
			}
		}

		public int Column
		{
			get
			{
				return this.column;
			}
		}

		public virtual bool AcceptParams
		{
			get
			{
				return true;
			}
		}

		public virtual void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			this.rsvc = rs;
		}

		public virtual bool SupportsNestedDirective(string name)
		{
			return false;
		}

		public virtual Directive CreateNestedDirective(string name)
		{
			return null;
		}

		public void SetLocation(int line, int column)
		{
			this.line = line;
			this.column = column;
		}

		public abstract bool Render(IInternalContextAdapter context, TextWriter writer, INode node);
	}
}
