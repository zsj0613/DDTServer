using NVelocity.App.Events;
using NVelocity.Runtime.Resource;
using NVelocity.Util.Introspection;
using System;
using System.Collections;

namespace NVelocity.Context
{
	public class InternalContextAdapterImpl : IInternalContextAdapter, IInternalHousekeepingContext, IContext, IInternalWrapperContext, IInternalEventContext, IDictionary, ICollection, IEnumerable
	{
		internal IContext context = null;

		internal IInternalHousekeepingContext icb = null;

		internal IInternalEventContext iec = null;

		public string CurrentTemplateName
		{
			get
			{
				return this.icb.CurrentTemplateName;
			}
		}

		public object[] TemplateNameStack
		{
			get
			{
				return this.icb.TemplateNameStack;
			}
		}

		public Resource CurrentResource
		{
			get
			{
				return this.icb.CurrentResource;
			}
			set
			{
				this.icb.CurrentResource = value;
			}
		}

		public object[] Keys
		{
			get
			{
				return this.context.Keys;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return this.context.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				object[] keys = this.Keys;
				object[] array = new object[keys.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.Get(keys[i].ToString());
				}
				return array;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public object this[object key]
		{
			get
			{
				return this.Get(key.ToString());
			}
			set
			{
				this.Put(key.ToString(), value);
			}
		}

		public IContext InternalUserContext
		{
			get
			{
				return this.context;
			}
		}

		public IInternalContextAdapter BaseContext
		{
			get
			{
				return this;
			}
		}

		public EventCartridge EventCartridge
		{
			get
			{
				EventCartridge result;
				if (this.iec != null)
				{
					result = this.iec.EventCartridge;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public int Count
		{
			get
			{
				return this.context.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.context;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public InternalContextAdapterImpl(IContext c)
		{
			this.context = c;
			if (!(c is IInternalHousekeepingContext))
			{
				this.icb = new InternalContextBase();
			}
			else
			{
				this.icb = (IInternalHousekeepingContext)this.context;
			}
			if (c is IInternalEventContext)
			{
				this.iec = (IInternalEventContext)this.context;
			}
		}

		public void PushCurrentTemplateName(string s)
		{
			this.icb.PushCurrentTemplateName(s);
		}

		public void PopCurrentTemplateName()
		{
			this.icb.PopCurrentTemplateName();
		}

		public IntrospectionCacheData ICacheGet(object key)
		{
			return this.icb.ICacheGet(key);
		}

		public void ICachePut(object key, IntrospectionCacheData o)
		{
			this.icb.ICachePut(key, o);
		}

		public object Put(string key, object value_)
		{
			return this.context.Put(key, value_);
		}

		public object Get(string key)
		{
			return this.context.Get(key);
		}

		public bool ContainsKey(object key)
		{
			return this.context.ContainsKey(key);
		}

		public object Remove(object key)
		{
			return this.context.Remove(key);
		}

		public EventCartridge AttachEventCartridge(EventCartridge ec)
		{
			EventCartridge result;
			if (this.iec != null)
			{
				result = this.iec.AttachEventCartridge(ec);
			}
			else
			{
				result = null;
			}
			return result;
		}

		void IDictionary.Remove(object key)
		{
			this.context.Remove(key);
		}

		public void CopyTo(Array array, int index)
		{
			foreach (object current in this.Values)
			{
				array.SetValue(current, index++);
			}
		}

		public bool Contains(object key)
		{
			return this.context.ContainsKey(key);
		}

		public void Add(object key, object value)
		{
			this.context.Put(key.ToString(), value);
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return this.CreateEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.CreateEnumerator();
		}

		private InternalContextAdapterImplEnumerator CreateEnumerator()
		{
			return new InternalContextAdapterImplEnumerator(this.context);
		}
	}
}
