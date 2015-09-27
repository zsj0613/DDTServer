using System;
namespace Game.Base.Config
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ConfigPropertyAttribute : Attribute
	{
		private string m_key;
		private string m_description;
		private object m_defaultValue;
		public string Key
		{
			get
			{
				return this.m_key;
			}
		}
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}
		public object DefaultValue
		{
			get
			{
				return this.m_defaultValue;
			}
		}
		public ConfigPropertyAttribute(string key, string description, object defaultValue)
		{
			this.m_key = key;
			this.m_description = description;
			this.m_defaultValue = defaultValue;
		}
	}
}
