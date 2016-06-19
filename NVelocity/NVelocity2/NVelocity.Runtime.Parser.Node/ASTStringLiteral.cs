using NVelocity.Context;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTStringLiteral : SimpleNode
	{
		private static readonly string dictStart = "%{";

		private static readonly string dictEnd = "}";

		private static readonly Regex dictPairRegex = new Regex(" (\\w+) \\s* = \\s* '(.*?)(?<!\\\\)' ", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		private static readonly Regex dictBetweenPairRegex = new Regex("^\\s*,\\s*$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		private static readonly Regex dictEdgePairRegex = new Regex("^\\s*$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		private bool interpolate = true;

		private SimpleNode nodeTree = null;

		private string image = "";

		private string interpolateimage = "";

		public ASTStringLiteral(int id) : base(id)
		{
		}

		public ASTStringLiteral(Parser p, int id) : base(p, id)
		{
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			base.Init(context, data);
			this.interpolate = (this.rsvc.GetBoolean("runtime.interpolate.string.literals", true) && base.FirstToken.Image.StartsWith("\"") && (base.FirstToken.Image.IndexOf('$') != -1 || base.FirstToken.Image.IndexOf('#') != -1));
			this.image = base.FirstToken.Image.Substring(1, base.FirstToken.Image.Length - 1 - 1);
			this.interpolateimage = this.image + " ";
			if (this.interpolate)
			{
				TextReader reader = new StringReader(this.interpolateimage);
				this.nodeTree = this.rsvc.Parse(reader, (context != null) ? context.CurrentTemplateName : "StringLiteral", false);
				this.nodeTree.Init(context, this.rsvc);
			}
			return data;
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Value(IInternalContextAdapter context)
		{
			string text = this.image;
			if (this.interpolate)
			{
				try
				{
					TextWriter textWriter = new StringWriter();
					this.nodeTree.Render(context, textWriter);
					string text2 = textWriter.ToString();
					text = text2.Substring(0, text2.Length - 1);
				}
				catch (System.Exception arg)
				{
					this.rsvc.Error("Error in interpolating string literal : " + arg);
					text = this.image;
				}
			}
			object result;
			if (this.IsDictionaryString(text))
			{
				result = this.InterpolateDictionaryString(text);
			}
			else
			{
				result = text;
			}
			return result;
		}

		private object InterpolateDictionaryString(string str)
		{
			MatchCollection matchCollection = ASTStringLiteral.dictPairRegex.Matches(str);
			HybridDictionary hybridDictionary = new HybridDictionary(matchCollection.Count, true);
			int num = ASTStringLiteral.dictStart.Length;
			Match match;
			object result;
			while ((match = ASTStringLiteral.dictPairRegex.Match(str, num)) != Match.Empty)
			{
				if (!this.AssertDictionaryString(str.Substring(num, match.Index - num), hybridDictionary.Count > 0))
				{
					result = str;
					return result;
				}
				string value = match.Groups[1].Value;
				string text = match.Groups[2].Value;
				text = text.Replace("\\'", "'");
				hybridDictionary[value] = text;
				num = match.Index + match.Length;
			}
			int num2 = str.Length - ASTStringLiteral.dictEnd.Length;
			if (num < num2 && !this.AssertDictionaryString(str.Substring(num, num2 - num), false))
			{
				result = str;
				return result;
			}
			result = hybridDictionary;
			return result;
		}

		private bool AssertDictionaryString(string str, bool isBetweenPairStr)
		{
			Regex regex = isBetweenPairStr ? ASTStringLiteral.dictBetweenPairRegex : ASTStringLiteral.dictEdgePairRegex;
			bool result;
			if (!regex.IsMatch(str))
			{
				this.rsvc.Error(string.Format("Error in interpolating dictionary string, cannot contain str <{0}> {1} dictionary params", str, isBetweenPairStr ? "between" : "before"));
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		private bool IsDictionaryString(string str)
		{
			return str.StartsWith(ASTStringLiteral.dictStart) && str.EndsWith(ASTStringLiteral.dictEnd);
		}
	}
}
