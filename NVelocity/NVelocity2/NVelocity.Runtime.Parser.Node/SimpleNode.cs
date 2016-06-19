using NVelocity.Context;
using NVelocity.Runtime.Exception;
using System;
using System.IO;
using System.Text;

namespace NVelocity.Runtime.Parser.Node
{
	public class SimpleNode : INode
	{
		protected internal IRuntimeServices rsvc = null;

		protected internal INode parent;

		protected internal INode[] children;

		protected internal int id;

		protected internal Parser parser;

		protected internal int info;

		public bool state;

		protected internal bool invalid = false;

		protected internal Token first;

		protected internal Token last;

		public Token FirstToken
		{
			get
			{
				return this.first;
			}
			set
			{
				this.first = value;
			}
		}

		public Token LastToken
		{
			get
			{
				return this.last;
			}
		}

		public int Type
		{
			get
			{
				return this.id;
			}
		}

		public int Info
		{
			get
			{
				return this.info;
			}
			set
			{
				this.info = value;
			}
		}

		public int Line
		{
			get
			{
				return this.first.BeginLine;
			}
		}

		public int Column
		{
			get
			{
				return this.first.BeginColumn;
			}
		}

		public INode Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public int ChildrenCount
		{
			get
			{
				return (this.children == null) ? 0 : this.children.Length;
			}
		}

		public virtual string Literal
		{
			get
			{
				Token next = this.first;
				StringBuilder stringBuilder = new StringBuilder(next.Image);
				while (next != this.last)
				{
					next = next.Next;
					stringBuilder.Append(next.Image);
				}
				return stringBuilder.ToString();
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.invalid;
			}
			set
			{
				this.invalid = true;
			}
		}

		public SimpleNode(int i)
		{
			this.id = i;
		}

		public SimpleNode(Parser p, int i) : this(i)
		{
			this.parser = p;
		}

		public void Open()
		{
			this.first = this.parser.GetToken(1);
		}

		public void Close()
		{
			this.last = this.parser.GetToken(0);
		}

		public void AddChild(INode n, int i)
		{
			if (this.children == null)
			{
				this.children = new INode[i + 1];
			}
			else if (i >= this.children.Length)
			{
				INode[] destinationArray = new INode[i + 1];
				Array.Copy(this.children, 0, destinationArray, 0, this.children.Length);
				this.children = destinationArray;
			}
			this.children[i] = n;
		}

		public INode GetChild(int i)
		{
			return this.children[i];
		}

		public virtual object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public object ChildrenAccept(IParserVisitor visitor, object data)
		{
			if (this.children != null)
			{
				for (int i = 0; i < this.children.Length; i++)
				{
					this.children[i].Accept(visitor, data);
				}
			}
			return data;
		}

		public string ToString(string prefix)
		{
			return prefix + this.ToString();
		}

		public void Dump(string prefix)
		{
			Console.Out.WriteLine(this.ToString(prefix));
			if (this.children != null)
			{
				for (int i = 0; i < this.children.Length; i++)
				{
					SimpleNode simpleNode = (SimpleNode)this.children[i];
					if (simpleNode != null)
					{
						simpleNode.Dump(prefix + " ");
					}
				}
			}
		}

		public virtual object Init(IInternalContextAdapter context, object data)
		{
			this.rsvc = (IRuntimeServices)data;
			int childrenCount = this.ChildrenCount;
			for (int i = 0; i < childrenCount; i++)
			{
				try
				{
					this.GetChild(i).Init(context, data);
				}
				catch (ReferenceException message)
				{
					this.rsvc.Error(message);
				}
			}
			return data;
		}

		public virtual bool Evaluate(IInternalContextAdapter context)
		{
			return false;
		}

		public virtual object Value(IInternalContextAdapter context)
		{
			return null;
		}

		public virtual bool Render(IInternalContextAdapter context, TextWriter writer)
		{
			int childrenCount = this.ChildrenCount;
			for (int i = 0; i < childrenCount; i++)
			{
				this.GetChild(i).Render(context, writer);
			}
			return true;
		}

		public virtual object Execute(object o, IInternalContextAdapter context)
		{
			return null;
		}
	}
}
