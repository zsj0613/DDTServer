using System;
using System.Collections;

namespace NVelocity.Runtime.Directive
{
	[Serializable]
	public class ParseDirectiveException : System.Exception
	{
		private Stack filenameStack;

		private string msg = "";

		private int depthCount = 0;

		public override string Message
		{
			get
			{
				string text = string.Concat(new object[]
				{
					"#parse() exception : depth = ",
					this.depthCount,
					" -> ",
					this.msg
				});
				text += " File stack : ";
				try
				{
					while (this.filenameStack.Count != 0)
					{
						text += (string)this.filenameStack.Pop();
						text += " -> ";
					}
				}
				catch (System.Exception var_1_84)
				{
				}
				return text;
			}
		}

		private void InitBlock()
		{
			this.filenameStack = new Stack();
		}

		internal ParseDirectiveException(string m, int i)
		{
			this.InitBlock();
			this.msg = m;
			this.depthCount = i;
		}

		public void addFile(string s)
		{
			this.filenameStack.Push(s);
		}
	}
}
