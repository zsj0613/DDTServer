using NVelocity.App.Events;
using NVelocity.Runtime.Resource;
using NVelocity.Util.Introspection;
using System;
using System.Collections;

namespace NVelocity.Context
{
	[Serializable]
	public class InternalContextBase : IInternalHousekeepingContext, IInternalEventContext
	{
		private Hashtable introspectionCache;

		private Stack templateNameStack;

		private EventCartridge eventCartridge = null;

		private Resource currentResource = null;

		public string CurrentTemplateName
		{
			get
			{
				string result;
				if (this.templateNameStack.Count == 0)
				{
					result = "<undef>";
				}
				else
				{
					result = (string)this.templateNameStack.Peek();
				}
				return result;
			}
		}

		public object[] TemplateNameStack
		{
			get
			{
				return this.templateNameStack.ToArray();
			}
		}

		public Resource CurrentResource
		{
			get
			{
				return this.currentResource;
			}
			set
			{
				this.currentResource = value;
			}
		}

		public EventCartridge EventCartridge
		{
			get
			{
				return this.eventCartridge;
			}
		}

		public InternalContextBase()
		{
			this.InitBlock();
		}

		private void InitBlock()
		{
			this.introspectionCache = new Hashtable(33);
			this.templateNameStack = new Stack();
		}

		public void PushCurrentTemplateName(string s)
		{
			this.templateNameStack.Push(s);
		}

		public void PopCurrentTemplateName()
		{
			this.templateNameStack.Pop();
		}

		public IntrospectionCacheData ICacheGet(object key)
		{
			return (IntrospectionCacheData)this.introspectionCache[key];
		}

		public void ICachePut(object key, IntrospectionCacheData o)
		{
			this.introspectionCache[key] = o;
		}

		public EventCartridge AttachEventCartridge(EventCartridge ec)
		{
			EventCartridge result = this.eventCartridge;
			this.eventCartridge = ec;
			return result;
		}
	}
}
