using System;

namespace NVelocity.Util.Introspection
{
	public class Info
	{
		private int line;

		private int column;

		private string templateName;

		public string TemplateName
		{
			get
			{
				return this.templateName;
			}
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

		public Info(string source, int line, int column)
		{
			this.templateName = source;
			this.line = line;
			this.column = column;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.TemplateName,
				" [line ",
				this.Line,
				", column ",
				this.Column,
				']'
			});
		}
	}
}
