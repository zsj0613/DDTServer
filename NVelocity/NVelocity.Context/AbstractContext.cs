using System;

namespace NVelocity.Context
{
	[Serializable]
	public abstract class AbstractContext : InternalContextBase, IContext
	{
		private IContext innerContext = null;

		public object[] Keys
		{
			get
			{
				return this.InternalGetKeys();
			}
		}

		public IContext ChainedContext
		{
			get
			{
				return this.innerContext;
			}
		}

		public abstract int Count
		{
			get;
		}

		public abstract object InternalGet(string key);

		public abstract object InternalPut(string key, object value_);

		public abstract bool InternalContainsKey(object key);

		public abstract object[] InternalGetKeys();

		public abstract object InternalRemove(object key);

		public AbstractContext()
		{
		}

		public AbstractContext(IContext inner)
		{
			this.innerContext = inner;
			if (this.innerContext is IInternalEventContext)
			{
				base.AttachEventCartridge(((IInternalEventContext)this.innerContext).EventCartridge);
			}
		}

		public object Put(string key, object value_)
		{
			object result;
			if (key == null)
			{
				result = null;
			}
			else
			{
				result = this.InternalPut(key, value_);
			}
			return result;
		}

		public object Get(string key)
		{
			object result;
			if (key == null)
			{
				result = null;
			}
			else
			{
				object obj = this.InternalGet(key);
				if (obj == null && this.innerContext != null)
				{
					obj = this.innerContext.Get(key);
				}
				result = obj;
			}
			return result;
		}

		public bool ContainsKey(object key)
		{
			return key != null && this.InternalContainsKey(key);
		}

		public object Remove(object key)
		{
			object result;
			if (key == null)
			{
				result = null;
			}
			else
			{
				result = this.InternalRemove(key);
			}
			return result;
		}
	}
}
