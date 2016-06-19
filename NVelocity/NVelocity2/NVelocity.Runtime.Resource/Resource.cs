using NVelocity.Runtime.Resource.Loader;
using System;

namespace NVelocity.Runtime.Resource
{
	public abstract class Resource
	{
		protected internal const long MILLIS_PER_SECOND = 1000L;

		protected internal object data = null;

		protected internal string encoding = "ISO-8859-1";

		protected internal long lastModified = 0L;

		protected internal long modificationCheckInterval = 0L;

		protected internal string name;

		protected internal long nextCheck = 0L;

		protected internal ResourceLoader resourceLoader;

		protected internal IRuntimeServices rsvc = null;

		public object Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public string Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public long LastModified
		{
			get
			{
				return this.lastModified;
			}
			set
			{
				this.lastModified = value;
			}
		}

		public long ModificationCheckInterval
		{
			set
			{
				this.modificationCheckInterval = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public ResourceLoader ResourceLoader
		{
			get
			{
				return this.resourceLoader;
			}
			set
			{
				this.resourceLoader = value;
			}
		}

		public IRuntimeServices RuntimeServices
		{
			set
			{
				this.rsvc = value;
			}
		}

		public bool IsSourceModified()
		{
			return this.resourceLoader.IsSourceModified(this);
		}

		public abstract bool Process();

		public bool RequiresChecking()
		{
			return this.modificationCheckInterval > 0L && (DateTime.Now.Ticks - 621355968000000000L) / 10000L >= this.nextCheck;
		}

		public void Touch()
		{
			this.nextCheck = (DateTime.Now.Ticks - 621355968000000000L) / 10000L + 1000L * this.modificationCheckInterval;
		}
	}
}
