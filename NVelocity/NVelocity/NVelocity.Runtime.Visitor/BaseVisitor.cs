using NVelocity.Context;
using NVelocity.Runtime.Parser.Node;
using System;
using System.IO;

namespace NVelocity.Runtime.Visitor
{
	public abstract class BaseVisitor : IParserVisitor
	{
		protected internal IInternalContextAdapter context;

		protected internal StreamWriter writer;

		public virtual object Visit(SimpleNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTprocess node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTExpression node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTAssignment node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTOrNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTAndNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTEQNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTNENode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTLTNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTGTNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTLENode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTGENode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTAddNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTSubtractNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTMulNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTDivNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTModNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTNotNode node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTNumberLiteral node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTStringLiteral node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTIdentifier node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTMethod node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTReference node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTTrue node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTFalse node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTBlock node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTText node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTIfStatement node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTElseStatement node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTElseIfStatement node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTComment node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTObjectArray node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTMap node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTWord node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTSetDirective node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual object Visit(ASTDirective node, object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}
	}
}
