using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using System;
using System.Text;

namespace NVelocity.Runtime.Visitor
{
	public class NodeViewMode : BaseVisitor
	{
		private int indent = 0;

		private bool showTokens = true;

		private string IndentString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.indent; i++)
			{
				stringBuilder.Append("  ");
			}
			return stringBuilder.ToString();
		}

		private object ShowNode(INode node, object data)
		{
			string arg = "";
			string str = "";
			if (this.showTokens)
			{
				Token firstToken = node.FirstToken;
				if (firstToken.SpecialToken != null && !firstToken.SpecialToken.Image.StartsWith("##"))
				{
					str = firstToken.SpecialToken.Image;
				}
				arg = " -> " + str + firstToken.Image;
			}
			Console.Out.WriteLine(this.IndentString() + node + arg);
			this.indent++;
			data = node.ChildrenAccept(this, data);
			this.indent--;
			return data;
		}

		public override object Visit(SimpleNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTprocess node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTExpression node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTAssignment node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTOrNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTAndNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTEQNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTNENode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTLTNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTGTNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTLENode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTGENode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTAddNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTSubtractNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTMulNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTDivNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTModNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTNotNode node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTNumberLiteral node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTStringLiteral node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTIdentifier node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTMethod node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTReference node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTTrue node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTFalse node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTBlock node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTText node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTIfStatement node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTElseStatement node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTElseIfStatement node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTObjectArray node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTDirective node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTWord node, object data)
		{
			return this.ShowNode(node, data);
		}

		public override object Visit(ASTSetDirective node, object data)
		{
			return this.ShowNode(node, data);
		}
	}
}
