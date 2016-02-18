using NVelocity.Context;
using System;
using System.Collections;

namespace NVelocity
{
	public class VelocityContext : AbstractContext
	{
		private Hashtable context = null;

		public override int Count
		{
			get
			{
				return this.context.Count;
			}
		}

		public VelocityContext() : this(null, null)
		{
		}

		public VelocityContext(Hashtable context) : this(context, null)
		{
		}

		public VelocityContext(IContext innerContext) : this(null, innerContext)
		{
		}

		public VelocityContext(Hashtable context, IContext innerContext) : base(innerContext)
		{
			this.context = ((context == null) ? new Hashtable() : context);
		}

		public override object InternalGet(string key)
		{
			return this.context[key];
		}

		public override object InternalPut(string key, object value)
		{
			this.context[key] = value;
			return value;
		}

		public override bool InternalContainsKey(object key)
		{
			return this.context.ContainsKey(key);
		}

		public override object[] InternalGetKeys()
		{
			object[] array = new object[this.context.Count];
			this.context.Keys.CopyTo(array, 0);
			return array;
		}

		public override object InternalRemove(object key)
		{
			object result = this.context[key];
			this.context.Remove(key);
			return result;
		}

		public object Clone()
		{
			VelocityContext velocityContext = null;
			try
			{
				velocityContext = (VelocityContext)base.MemberwiseClone();
				velocityContext.context = new Hashtable(this.context);
			}
			catch (System.Exception)
			{
			}
			return velocityContext;
		}
	}
}
