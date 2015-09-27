using Game.Base.Config;
using System;
using System.IO;
using System.Net;
namespace Game.Base
{
	public class BaseServerConfiguration
	{
		protected ushort _port;
		protected IPAddress _ip;
		public ushort Port
		{
			get
			{
				return this._port;
			}
			set
			{
				this._port = value;
			}
		}
		public IPAddress Ip
		{
			get
			{
				return this._ip;
			}
			set
			{
				this._ip = value;
			}
		}
		protected virtual void LoadFromConfig(ConfigElement root)
		{
			string ip = root["Server"]["IP"].GetString("any");
			if (ip == "any")
			{
				this._ip = IPAddress.Any;
			}
			else
			{
				this._ip = IPAddress.Parse(ip);
			}
			this._port = (ushort)root["Server"]["Port"].GetInt((int)this._port);
		}
		public void LoadFromXMLFile(FileInfo configFile)
		{
			XMLConfigFile xmlConfig = XMLConfigFile.ParseXMLFile(configFile);
			this.LoadFromConfig(xmlConfig);
		}
		protected virtual void SaveToConfig(ConfigElement root)
		{
			root["Server"]["Port"].Set(this._port);
			root["Server"]["IP"].Set(this._ip);
		}
		public void SaveToXMLFile(FileInfo configFile)
		{
			if (configFile == null)
			{
				throw new ArgumentNullException("configFile");
			}
			XMLConfigFile config = new XMLConfigFile();
			this.SaveToConfig(config);
			config.Save(configFile);
		}
		public BaseServerConfiguration()
		{
			this._port = 7000;
			this._ip = IPAddress.Any;
		}
	}
}
