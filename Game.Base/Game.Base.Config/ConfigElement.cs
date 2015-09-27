using System;
using System.Collections;
using System.Threading;
namespace Game.Base.Config
{
	public class ConfigElement
	{
		protected ConfigElement m_parent = null;
		protected Hashtable m_children = new Hashtable();
		protected string m_value = null;
		public ConfigElement this[string key]
		{
			get
			{
				Hashtable children;
				Monitor.Enter(children = this.m_children);
				try
				{
					if (!this.m_children.Contains(key))
					{
						this.m_children.Add(key, this.GetNewConfigElement(this));
					}
				}
				finally
				{
					Monitor.Exit(children);
				}
				return (ConfigElement)this.m_children[key];
			}
			set
			{
				Hashtable children;
				Monitor.Enter(children = this.m_children);
				try
				{
					this.m_children[key] = value;
				}
				finally
				{
					Monitor.Exit(children);
				}
			}
		}
		public ConfigElement Parent
		{
			get
			{
				return this.m_parent;
			}
		}
		public bool HasChildren
		{
			get
			{
				return this.m_children.Count > 0;
			}
		}
		public Hashtable Children
		{
			get
			{
				return this.m_children;
			}
		}
		public ConfigElement(ConfigElement parent)
		{
			this.m_parent = parent;
		}
		protected virtual ConfigElement GetNewConfigElement(ConfigElement parent)
		{
			return new ConfigElement(parent);
		}
		public string GetString()
		{
			return this.m_value;
		}
		public string GetString(string defaultValue)
		{
			return (this.m_value != null) ? this.m_value : defaultValue;
		}
		public int GetInt()
		{
			return int.Parse(this.m_value);
		}
		public int GetInt(int defaultValue)
		{
			return (this.m_value != null) ? int.Parse(this.m_value) : defaultValue;
		}
		public long GetLong()
		{
			return long.Parse(this.m_value);
		}
		public long GetLong(long defaultValue)
		{
			return (this.m_value != null) ? long.Parse(this.m_value) : defaultValue;
		}
		public bool GetBoolean()
		{
			return bool.Parse(this.m_value);
		}
		public bool GetBoolean(bool defaultValue)
		{
			return (this.m_value != null) ? bool.Parse(this.m_value) : defaultValue;
		}
		public void Set(object value)
		{
			this.m_value = value.ToString();
		}
	}
}
