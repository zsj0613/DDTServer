using NVelocity.Runtime.Parser.Node;
using System;
using System.Collections;

namespace NVelocity.Runtime.Parser
{
	internal class ParserState
	{
		private Stack nodes;

		private Stack marks;

		private int sp;

		private int mk;

		private bool node_created;

		internal INode RootNode
		{
			get
			{
				return (INode)this.nodes.ToArray()[this.nodes.Count - 1];
			}
		}

		internal ParserState()
		{
			this.nodes = new Stack();
			this.marks = new Stack();
			this.sp = 0;
			this.mk = 0;
		}

		internal bool NodeCreated()
		{
			return this.node_created;
		}

		internal void Reset()
		{
			this.nodes.Clear();
			this.marks.Clear();
			this.sp = 0;
			this.mk = 0;
		}

		internal void PushNode(INode n)
		{
			this.nodes.Push(n);
			this.sp++;
		}

		internal INode PopNode()
		{
			if (--this.sp < this.mk)
			{
				this.mk = (int)this.marks.Pop();
			}
			return (INode)this.nodes.Pop();
		}

		internal INode PeekNode()
		{
			return (INode)this.nodes.Peek();
		}

		internal int NodeArity()
		{
			return this.sp - this.mk;
		}

		internal void ClearNodeScope(INode n)
		{
			while (this.sp > this.mk)
			{
				this.PopNode();
			}
			this.mk = (int)this.marks.Pop();
		}

		internal void OpenNodeScope(INode n)
		{
			object obj = this.mk;
			this.marks.Push(obj);
			this.mk = this.sp;
			n.Open();
		}

		internal void CloseNodeScope(INode n, int num)
		{
			this.mk = (int)this.marks.Pop();
			while (num-- > 0)
			{
				INode node = this.PopNode();
				node.Parent = n;
				n.AddChild(node, num);
			}
			n.Close();
			this.PushNode(n);
			this.node_created = true;
		}

		internal void CloseNodeScope(INode n, bool condition)
		{
			if (condition)
			{
				int i = this.NodeArity();
				this.mk = (int)this.marks.Pop();
				while (i-- > 0)
				{
					INode node = this.PopNode();
					node.Parent = n;
					n.AddChild(node, i);
				}
				n.Close();
				this.PushNode(n);
				this.node_created = true;
			}
			else
			{
				this.mk = (int)this.marks.Pop();
				this.node_created = false;
			}
		}
	}
}
