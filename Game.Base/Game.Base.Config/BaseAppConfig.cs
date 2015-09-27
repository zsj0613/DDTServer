using log4net;
using System;
using System.Configuration;
using System.Reflection;
using Lsj.Util.Config;
namespace Game.Base.Config
{
	public abstract class BaseAppConfig
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public BaseAppConfig()
		{
		}
		protected virtual void Load(Type type)
		{
			ConfigurationManager.RefreshSection("appSettings");
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo f = fields[i];
				object[] attribs = f.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
				if (attribs.Length != 0)
				{
					ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)attribs[0];
					f.SetValue(this, this.LoadConfigProperty(attrib));
				}
			}
		}
		public void Save(string fileName, Type type)
		{
			Configuration config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
			{
				ExeConfigFilename = fileName
			}, ConfigurationUserLevel.None);
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo f = fields[i];
				object[] attribs = f.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
				if (attribs.Length != 0)
				{
					ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)attribs[0];
					config.AppSettings.Settings[attrib.Key].Value = f.GetValue(this).ToString();
				}
			}
			config.Save();
		}
		private object LoadConfigProperty(ConfigPropertyAttribute attrib)
		{
			string key = attrib.Key;
			string value = AppConfig.AppSettings[key];
			if (value == null)
			{
				value = attrib.DefaultValue.ToString();
				BaseAppConfig.log.Warn("Loading " + key + " value is null,using default vaule:" + value);
			}
			else
			{
				BaseAppConfig.log.Debug("Loading " + key + " Value is " + value);
			}
			object result;
			try
			{
				result = Convert.ChangeType(value, attrib.DefaultValue.GetType());
			}
			catch (Exception e)
			{
				BaseAppConfig.log.Error("Exception in ServerProperties Load: ", e);
				result = null;
			}
			return result;
		}
	}
}
