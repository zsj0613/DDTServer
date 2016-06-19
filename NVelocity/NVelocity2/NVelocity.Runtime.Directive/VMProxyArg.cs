using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Util;
using System;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public class VMProxyArg
	{
		private const int GENERALSTATIC = -1;

		private int type = 0;

		private SimpleNode nodeTree = null;

		private object staticObject = null;

		private IInternalContextAdapter usercontext = null;

		private int numTreeChildren = 0;

		private string contextReference = null;

		private string callerReference = null;

		private string singleLevelRef = null;

		private bool constant = false;

		private IRuntimeServices rsvc = null;

		public string CallerReference
		{
			get
			{
				return this.callerReference;
			}
		}

		public string ContextReference
		{
			get
			{
				return this.contextReference;
			}
		}

		public SimpleNode NodeTree
		{
			get
			{
				return this.nodeTree;
			}
		}

		public object StaticObject
		{
			get
			{
				return this.staticObject;
			}
		}

		public int Type
		{
			get
			{
				return this.type;
			}
		}

		public VMProxyArg(IRuntimeServices rs, string contextRef, string callerRef, int t)
		{
			this.rsvc = rs;
			this.contextReference = contextRef;
			this.callerReference = callerRef;
			this.type = t;
			this.setup();
			if (this.nodeTree != null)
			{
				this.numTreeChildren = this.nodeTree.ChildrenCount;
			}
			if (this.type == 14)
			{
				if (this.numTreeChildren == 0)
				{
					this.singleLevelRef = ((ASTReference)this.nodeTree).RootString;
				}
			}
		}

		public bool isConstant()
		{
			return this.constant;
		}

		public object setObject(IInternalContextAdapter context, object o)
		{
			if (this.type == 14)
			{
				if (this.numTreeChildren > 0)
				{
					try
					{
						((ASTReference)this.nodeTree).SetValue(context, o);
					}
					catch (MethodInvocationException arg)
					{
						this.rsvc.Error("VMProxyArg.getObject() : method invocation error setting value : " + arg);
					}
				}
				else
				{
					context.Put(this.singleLevelRef, o);
				}
			}
			else
			{
				this.type = -1;
				this.staticObject = o;
				this.rsvc.Error("VMProxyArg.setObject() : Programmer error : I am a constant!  No setting! : " + this.contextReference + " / " + this.callerReference);
			}
			return null;
		}

		public object getObject(IInternalContextAdapter context)
		{
			object result;
			try
			{
				object obj = null;
				if (this.type == 14)
				{
					if (this.numTreeChildren == 0)
					{
						obj = context.Get(this.singleLevelRef);
					}
					else
					{
						obj = this.nodeTree.Execute(null, context);
					}
				}
				else if (this.type == 11)
				{
					obj = this.nodeTree.Value(context);
				}
				else if (this.type == 12)
				{
					obj = this.nodeTree.Value(context);
				}
				else if (this.type == 15)
				{
					obj = this.staticObject;
				}
				else if (this.type == 16)
				{
					obj = this.staticObject;
				}
				else if (this.type == 6)
				{
					obj = this.nodeTree.Value(context);
				}
				else if (this.type == 5)
				{
					obj = this.staticObject;
				}
				else if (this.type == 17)
				{
					try
					{
						StringWriter stringWriter = new StringWriter();
						this.nodeTree.Render(context, stringWriter);
						obj = stringWriter;
					}
					catch (System.Exception arg)
					{
						this.rsvc.Error("VMProxyArg.getObject() : error rendering reference : " + arg);
					}
				}
				else if (this.type == -1)
				{
					obj = this.staticObject;
				}
				else
				{
					this.rsvc.Error(string.Concat(new object[]
					{
						"Unsupported VM arg type : VM arg = ",
						this.callerReference,
						" type = ",
						this.type,
						"( VMProxyArg.getObject() )"
					}));
				}
				result = obj;
			}
			catch (MethodInvocationException arg2)
			{
				this.rsvc.Error("VMProxyArg.getObject() : method invocation error getting value : " + arg2);
				result = null;
			}
			return result;
		}

		private void setup()
		{
			switch (this.type)
			{
			case 5:
				this.constant = true;
				this.staticObject = int.Parse(this.callerReference);
				return;
			case 6:
			case 11:
			case 12:
			case 14:
			case 17:
				this.constant = false;
				try
				{
					string s = "#include(" + this.callerReference + " ) ";
					TextReader reader = new StringReader(s);
					this.nodeTree = this.rsvc.Parse(reader, "VMProxyArg:" + this.callerReference, true);
					this.nodeTree = (SimpleNode)this.nodeTree.GetChild(0).GetChild(0);
					if (this.nodeTree != null && this.nodeTree.Type != this.type)
					{
						this.rsvc.Error("VMProxyArg.setup() : programmer error : type doesn't match node type.");
					}
					this.nodeTree.Init(null, this.rsvc);
				}
				catch (System.Exception e)
				{
					this.rsvc.Error("VMProxyArg.setup() : exception " + this.callerReference + " : " + StringUtils.StackTrace(e));
				}
				return;
			case 8:
				this.rsvc.Error("Unsupported arg type : " + this.callerReference + "  You most likely intended to call a VM with a string literal, so enclose with ' or \" characters. (VMProxyArg.setup())");
				this.constant = true;
				this.staticObject = new string(this.callerReference.ToCharArray());
				return;
			case 15:
				this.constant = true;
				this.staticObject = true;
				return;
			case 16:
				this.constant = true;
				this.staticObject = false;
				return;
			}
			this.rsvc.Error(" VMProxyArg.setup() : unsupported type : " + this.callerReference);
		}

		public VMProxyArg(VMProxyArg model, IInternalContextAdapter c)
		{
			this.usercontext = c;
			this.contextReference = model.ContextReference;
			this.callerReference = model.CallerReference;
			this.nodeTree = model.NodeTree;
			this.staticObject = model.StaticObject;
			this.type = model.Type;
			if (this.nodeTree != null)
			{
				this.numTreeChildren = this.nodeTree.ChildrenCount;
			}
			if (this.type == 14)
			{
				if (this.numTreeChildren == 0)
				{
					this.singleLevelRef = ((ASTReference)this.nodeTree).RootString;
				}
			}
		}
	}
}
