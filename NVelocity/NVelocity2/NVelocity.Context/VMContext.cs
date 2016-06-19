using NVelocity.App.Events;
using NVelocity.Runtime;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime.Resource;
using NVelocity.Util.Introspection;
using System;
using System.Collections;

namespace NVelocity.Context
{
	public class VMContext : IInternalContextAdapter, IInternalHousekeepingContext, IContext, IInternalWrapperContext, IInternalEventContext, IDictionary, ICollection, IEnumerable
	{
		internal Hashtable vmproxyhash;

		internal Hashtable localcontext;

		internal IInternalContextAdapter innerContext = null;

		internal IInternalContextAdapter wrappedContext = null;

		private bool localcontextscope = false;

		public IContext InternalUserContext
		{
			get
			{
				return this.innerContext.InternalUserContext;
			}
		}

		public IInternalContextAdapter BaseContext
		{
			get
			{
				return this.innerContext.BaseContext;
			}
		}

		public object[] Keys
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Count
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ICollection Values
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsFixedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public object this[object key]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string CurrentTemplateName
		{
			get
			{
				return this.innerContext.CurrentTemplateName;
			}
		}

		public object[] TemplateNameStack
		{
			get
			{
				return this.innerContext.TemplateNameStack;
			}
		}

		public EventCartridge EventCartridge
		{
			get
			{
				return this.innerContext.EventCartridge;
			}
		}

		public Resource CurrentResource
		{
			get
			{
				return this.innerContext.CurrentResource;
			}
			set
			{
				this.innerContext.CurrentResource = value;
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private void InitBlock()
		{
			this.vmproxyhash = new Hashtable();
			this.localcontext = new Hashtable();
		}

		public VMContext(IInternalContextAdapter inner, IRuntimeServices rsvc)
		{
			this.InitBlock();
			this.localcontextscope = rsvc.GetBoolean("velocimacro.context.localscope", false);
			this.wrappedContext = inner;
			this.innerContext = inner.BaseContext;
		}

		public void AddVMProxyArg(VMProxyArg vmpa)
		{
			string contextReference = vmpa.ContextReference;
			if (vmpa.isConstant())
			{
				this.localcontext[contextReference] = vmpa.getObject(this.wrappedContext);
			}
			else
			{
				this.vmproxyhash[contextReference] = vmpa;
			}
		}

		public object Put(string key, object value_)
		{
			VMProxyArg vMProxyArg = (VMProxyArg)this.vmproxyhash[key];
			object result;
			if (vMProxyArg != null)
			{
				result = vMProxyArg.setObject(this.wrappedContext, value_);
			}
			else if (this.localcontextscope)
			{
				this.localcontext[key] = value_;
				result = value_;
			}
			else if (this.localcontext.ContainsKey(key))
			{
				this.localcontext[key] = value_;
				result = value_;
			}
			else
			{
				result = this.innerContext.Put(key, value_);
			}
			return result;
		}

		public object Get(string key)
		{
			VMProxyArg vMProxyArg = (VMProxyArg)this.vmproxyhash[key];
			object obj;
			if (vMProxyArg != null)
			{
				obj = vMProxyArg.getObject(this.wrappedContext);
			}
			else if (this.localcontextscope)
			{
				obj = this.localcontext[key];
			}
			else
			{
				obj = this.localcontext[key];
				if (obj == null)
				{
					obj = this.innerContext.Get(key);
				}
			}
			return obj;
		}

		public bool ContainsKey(object key)
		{
			return false;
		}

		public object Remove(object key)
		{
			object result = this.vmproxyhash[key];
			this.vmproxyhash.Remove(key);
			return result;
		}

		void IDictionary.Remove(object key)
		{
			this.Remove(key);
		}

		public void PushCurrentTemplateName(string s)
		{
			this.innerContext.PushCurrentTemplateName(s);
		}

		public void PopCurrentTemplateName()
		{
			this.innerContext.PopCurrentTemplateName();
		}

		public IntrospectionCacheData ICacheGet(object key)
		{
			return this.innerContext.ICacheGet(key);
		}

		public void ICachePut(object key, IntrospectionCacheData o)
		{
			this.innerContext.ICachePut(key, o);
		}

		public EventCartridge AttachEventCartridge(EventCartridge ec)
		{
			return this.innerContext.AttachEventCartridge(ec);
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public bool Contains(object key)
		{
			throw new NotImplementedException();
		}

		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
