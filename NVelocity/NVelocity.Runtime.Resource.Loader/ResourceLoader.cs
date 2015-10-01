using Commons.Collections;
using System;
using System.IO;

namespace NVelocity.Runtime.Resource.Loader
{
	public abstract class ResourceLoader
	{
		protected internal bool isCachingOn = false;

		protected internal long modificationCheckInterval = 2L;

		protected internal string className = null;

		protected internal IRuntimeServices rsvc = null;

		public string ClassName
		{
			get
			{
				return this.className;
			}
		}

		public bool CachingOn
		{
			get
			{
				return this.isCachingOn;
			}
			set
			{
				this.isCachingOn = value;
			}
		}

		public long ModificationCheckInterval
		{
			get
			{
				return this.modificationCheckInterval;
			}
			set
			{
				this.modificationCheckInterval = value;
			}
		}

		public void CommonInit(IRuntimeServices rs, ExtendedProperties configuration)
		{
			this.rsvc = rs;
			this.isCachingOn = configuration.GetBoolean("cache", false);
			this.modificationCheckInterval = configuration.GetLong("modificationCheckInterval", 0L);
			this.className = configuration.GetString("class");
		}

		public abstract void Init(ExtendedProperties configuration);

		public abstract Stream GetResourceStream(string source);

		public abstract bool IsSourceModified(Resource resource);

		public abstract long GetLastModified(Resource resource);
	}
}
