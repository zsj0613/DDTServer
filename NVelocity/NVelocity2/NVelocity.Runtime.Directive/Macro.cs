using NVelocity.Context;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace NVelocity.Runtime.Directive
{
	public class Macro : Directive
	{
		public override string Name
		{
			get
			{
				return "macro";
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

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			return true;
		}

		public static void processAndRegister(IRuntimeServices rs, INode node, string sourceTemplate)
		{
			int childrenCount = node.ChildrenCount;
			if (childrenCount < 2)
			{
				rs.Error("#macro error : Velocimacro must have name as 1st argument to #macro()");
			}
			else
			{
				string[] argArray = Macro.getArgArray(node);
				IList aSTAsStringArray = Macro.getASTAsStringArray(node.GetChild(childrenCount - 1));
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < aSTAsStringArray.Count; i++)
				{
					stringBuilder.Append(aSTAsStringArray[i]);
				}
				string macro = stringBuilder.ToString();
				rs.AddVelocimacro(argArray[0], macro, argArray, sourceTemplate);
			}
		}

		private static string[] getArgArray(INode node)
		{
			int num = node.ChildrenCount;
			num--;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = node.GetChild(i).FirstToken.Image;
				if (i > 0)
				{
					if (array[i].StartsWith("$"))
					{
						array[i] = array[i].Substring(1, array[i].Length - 1);
					}
				}
			}
			return array;
		}

		private static IList getASTAsStringArray(INode rootNode)
		{
			Token lastToken = rootNode.LastToken;
			ArrayList arrayList = new ArrayList();
			Token token;
			for (token = rootNode.FirstToken; token != lastToken; token = token.Next)
			{
				arrayList.Add(NodeUtils.tokenLiteral(token));
			}
			arrayList.Add(NodeUtils.tokenLiteral(token));
			return arrayList;
		}
	}
}
