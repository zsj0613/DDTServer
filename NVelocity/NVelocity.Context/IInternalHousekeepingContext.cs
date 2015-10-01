using NVelocity.Runtime.Resource;
using NVelocity.Util.Introspection;
using System;

namespace NVelocity.Context
{
	public interface IInternalHousekeepingContext
	{
		string CurrentTemplateName
		{
			get;
		}

		object[] TemplateNameStack
		{
			get;
		}

		Resource CurrentResource
		{
			get;
			set;
		}

		void PushCurrentTemplateName(string s);

		void PopCurrentTemplateName();

		IntrospectionCacheData ICacheGet(object key);

		void ICachePut(object key, IntrospectionCacheData o);
	}
}
