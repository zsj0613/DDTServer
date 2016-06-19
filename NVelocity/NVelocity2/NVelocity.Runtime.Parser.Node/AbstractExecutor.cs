using System;
using System.Reflection;

namespace NVelocity.Runtime.Parser.Node
{
	public abstract class AbstractExecutor
	{
		protected internal IRuntimeLogger rlog = null;

		protected internal MethodInfo method = null;

		protected internal PropertyInfo property = null;

		public bool IsAlive
		{
			get
			{
				return this.method != null || this.property != null;
			}
		}

		public MethodInfo Method
		{
			get
			{
				return this.method;
			}
		}

		public PropertyInfo Property
		{
			get
			{
				return this.property;
			}
		}

		public abstract object Execute(object o);
	}
}
