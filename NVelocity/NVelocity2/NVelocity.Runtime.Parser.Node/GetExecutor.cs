using NVelocity.Util.Introspection;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class GetExecutor : AbstractExecutor
	{
		private object[] args = new object[1];

		public GetExecutor(IRuntimeLogger r, Introspector i, Type c, string key)
		{
			this.rlog = r;
			this.args[0] = key;
			this.method = i.GetMethod(c, "get_Item", this.args);
			if (this.method == null)
			{
				this.method = i.GetMethod(c, "Get", this.args);
				if (this.method == null)
				{
					this.method = i.GetMethod(c, "get", this.args);
				}
			}
		}

		public override object Execute(object o)
		{
			object result;
			if (this.method == null)
			{
				result = null;
			}
			else
			{
				result = this.method.Invoke(o, this.args);
			}
			return result;
		}
	}
}
