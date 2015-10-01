using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime.Parser;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Visitor;
using NVelocity.Util;
using System;
using System.Collections;
using System.IO;

namespace NVelocity.Runtime.Directive
{
	public class VelocimacroProxy : Directive
	{
		private string macroName = "";

		private string macroBody = "";

		private string[] argArray = null;

		private SimpleNode nodeTree = null;

		private int numMacroArgs = 0;

		private string ns = "";

		private bool init = false;

		private string[] callingArgs;

		private int[] callingArgTypes;

		private Hashtable proxyArgHash = new Hashtable();

		public override string Name
		{
			get
			{
				return this.macroName;
			}
			set
			{
				this.macroName = value;
			}
		}

		public override DirectiveType Type
		{
			get
			{
				return DirectiveType.LINE;
			}
		}

		public string[] ArgArray
		{
			set
			{
				this.argArray = value;
				this.numMacroArgs = this.argArray.Length - 1;
			}
		}

		public SimpleNode NodeTree
		{
			set
			{
				this.nodeTree = value;
			}
		}

		public int NumArgs
		{
			get
			{
				return this.numMacroArgs;
			}
		}

		public string Macrobody
		{
			set
			{
				this.macroBody = value;
			}
		}

		public string Namespace
		{
			set
			{
				this.ns = value;
			}
		}

		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			try
			{
				if (this.nodeTree != null)
				{
					if (!this.init)
					{
						this.nodeTree.Init(context, this.rsvc);
						this.init = true;
					}
					VMContext vMContext = new VMContext(context, this.rsvc);
					for (int i = 1; i < this.argArray.Length; i++)
					{
						VMProxyArg vmpa = (VMProxyArg)this.proxyArgHash[this.argArray[i]];
						vMContext.AddVMProxyArg(vmpa);
					}
					this.nodeTree.Render(vMContext, writer);
				}
				else
				{
					this.rsvc.Error("VM error : " + this.macroName + ". Null AST");
				}
			}
			catch (System.Exception ex)
			{
				if (ex is MethodInvocationException)
				{
					throw;
				}
				this.rsvc.Error("VelocimacroProxy.render() : exception VM = #" + this.macroName + "() : " + StringUtils.StackTrace(ex));
			}
			return true;
		}

		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);
			int childrenCount = node.ChildrenCount;
			if (this.NumArgs != childrenCount)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					"VM #",
					this.macroName,
					": error : too ",
					(this.NumArgs > childrenCount) ? "few" : "many",
					" arguments to macro. Wanted ",
					this.NumArgs,
					" got ",
					childrenCount
				}));
			}
			else
			{
				this.callingArgs = this.getArgArray(node);
				this.setupMacro(this.callingArgs, this.callingArgTypes);
			}
		}

		public bool setupMacro(string[] callArgs, int[] callArgTypes)
		{
			this.setupProxyArgs(callArgs, callArgTypes);
			this.parseTree(callArgs);
			return true;
		}

		private void parseTree(string[] callArgs)
		{
			try
			{
				TextReader reader = new StringReader(this.macroBody);
				this.nodeTree = this.rsvc.Parse(reader, this.ns, false);
				Hashtable hashtable = new Hashtable();
				for (int i = 1; i < this.argArray.Length; i++)
				{
					string text = callArgs[i - 1];
					if (text[0] == '$')
					{
						hashtable[this.argArray[i]] = text;
					}
				}
				VMReferenceMungeVisitor visitor = new VMReferenceMungeVisitor(hashtable);
				this.nodeTree.Accept(visitor, null);
			}
			catch (System.Exception e)
			{
				this.rsvc.Error("VelocimacroManager.parseTree() : exception " + this.macroName + " : " + StringUtils.StackTrace(e));
			}
		}

		private void setupProxyArgs(string[] callArgs, int[] callArgTypes)
		{
			for (int i = 1; i < this.argArray.Length; i++)
			{
				VMProxyArg value = new VMProxyArg(this.rsvc, this.argArray[i], callArgs[i - 1], callArgTypes[i - 1]);
				this.proxyArgHash[this.argArray[i]] = value;
			}
		}

		private string[] getArgArray(INode node)
		{
			int childrenCount = node.ChildrenCount;
			string[] array = new string[childrenCount];
			this.callingArgTypes = new int[childrenCount];
			for (int i = 0; i < childrenCount; i++)
			{
				array[i] = "";
				this.callingArgTypes[i] = node.GetChild(i).Type;
				Token token = node.GetChild(i).FirstToken;
				Token lastToken = node.GetChild(i).LastToken;
				string[] array2;
				IntPtr intPtr;
				while (token != lastToken)
				{
					(array2 = array)[(int)(intPtr = (IntPtr)i)] = array2[(int)intPtr] + token.Image;
					token = token.Next;
				}
				(array2 = array)[(int)(intPtr = (IntPtr)i)] = array2[(int)intPtr] + token.Image;
			}
			return array;
		}
	}
}
