using log4net;
using System;
using System.Configuration;
using System.Reflection;
namespace Bussiness
{
	public class StaticFunction
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static bool UpdateConfig(string fileName, string name, string value)
		{
			bool result;
			try
			{
				Configuration config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = fileName
				}, ConfigurationUserLevel.None);
				config.AppSettings.Settings[name].Value = value;
				config.Save();
				ConfigurationManager.RefreshSection("appSettings");
				result = true;
				return result;
			}
			catch (Exception e)
			{
				if (StaticFunction.log.IsErrorEnabled)
				{
					StaticFunction.log.Error("UpdateConfig", e);
				}
			}
			result = false;
			return result;
		}
	}
}
