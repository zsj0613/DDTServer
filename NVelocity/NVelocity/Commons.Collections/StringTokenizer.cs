using System;
using System.Collections;

namespace Commons.Collections
{
	public class StringTokenizer
	{
		private ArrayList elements;

		private string source;

		private string delimiters = " \t\n\r";

		public int Count
		{
			get
			{
				return this.elements.Count;
			}
		}

		public StringTokenizer(string source)
		{
			this.elements = new ArrayList();
			this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
			this.RemoveEmptyStrings();
			this.source = source;
		}

		public StringTokenizer(string source, string delimiters)
		{
			this.elements = new ArrayList();
			this.delimiters = delimiters;
			this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
			this.RemoveEmptyStrings();
			this.source = source;
		}

		public virtual bool HasMoreTokens()
		{
			return this.elements.Count > 0;
		}

		public virtual string NextToken()
		{
			if (this.source == "")
			{
				throw new Exception();
			}
			this.elements = new ArrayList();
			this.elements.AddRange(this.source.Split(this.delimiters.ToCharArray()));
			this.RemoveEmptyStrings();
			string text = (string)this.elements[0];
			this.elements.RemoveAt(0);
			this.source = this.source.Replace(text, "");
			this.source = this.source.TrimStart(this.delimiters.ToCharArray());
			return text;
		}

		public string NextToken(string delimiters)
		{
			this.delimiters = delimiters;
			return this.NextToken();
		}

		private void RemoveEmptyStrings()
		{
			for (int i = 0; i < this.elements.Count; i++)
			{
				if ((string)this.elements[i] == "")
				{
					this.elements.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
