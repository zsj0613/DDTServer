using Game.Base.Config;
using System;
namespace Fighting.Server
{
	public class FightServerConfig : BaseAppConfig
	{
		[ConfigProperty("Logconfig", "日志配置文件", "logconfig.xml")]
		public string LogConfigFile = "logconfig.xml";
		[ConfigProperty("Id", "服务器ID,用于从数据库加载地图", 1)]
		public int Id = 1;
		[ConfigProperty("Ip", "IP地址", "127.0.0.1")]
		public string Ip = "127.0.0.1";
		[ConfigProperty("Port", "监听的端口", 9208)]
		public int Port = 9208;
		[ConfigProperty("ServerType", "服务器类型", 0)]
		public int ServerType = 0;
		public FightServerConfig()
		{
			this.Load(typeof(FightServerConfig));
		}
	}
}
