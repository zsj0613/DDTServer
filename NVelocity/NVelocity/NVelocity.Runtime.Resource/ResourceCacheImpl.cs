using Commons.Collections;
using System;
using System.Collections;

namespace NVelocity.Runtime.Resource
{
	public class ResourceCacheImpl : ResourceCache
	{
		protected internal IDictionary cache = Hashtable.Synchronized(new Hashtable());

		protected internal IRuntimeServices rsvc = null;

		public void initialize(IRuntimeServices rs)
		{
			this.rsvc = rs;
			int @int = this.rsvc.GetInt("resource.manager.defaultcache.size", 89);
			if (@int > 0)
			{
				LRUMap lRUMap = LRUMap.Synchronized(new LRUMap(@int));
				lRUMap.AddAll(this.cache);
				this.cache = lRUMap;
			}
			this.rsvc.Info("ResourceCache : initialized. (" + base.GetType() + ")");
		}

		public Resource get(object key)
		{
			return (Resource)this.cache[key];
		}

		public Resource put(object key, Resource value)
		{
			object obj = this.cache[key];
			this.cache[key] = value;
			return (Resource)obj;
		}

		public Resource remove(object key)
		{
			object obj = this.cache[key];
			this.cache.Remove(key);
			return (Resource)obj;
		}

		public IEnumerator enumerateKeys()
		{
			return this.cache.Keys.GetEnumerator();
		}
	}
}
