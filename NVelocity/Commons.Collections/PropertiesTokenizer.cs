using System;
using System.Text;

namespace Commons.Collections
{
	internal class PropertiesTokenizer : StringTokenizer
	{
		internal const string DELIMITER = ",";

		public PropertiesTokenizer(string str) : base(str, ",")
		{
		}

		public override bool HasMoreTokens()
		{
			return base.HasMoreTokens();
		}

		public override string NextToken()
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (this.HasMoreTokens())
			{
				string text = base.NextToken();
				if (!text.EndsWith("\\"))
				{
					stringBuilder.Append(text);
					break;
				}
				stringBuilder.Append(text.Substring(0, text.Length - 1));
				stringBuilder.Append(",");
			}
			return stringBuilder.ToString().Trim();
		}
	}
}
