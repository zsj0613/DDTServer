using Game.Base.Config;
using System;
namespace CrossFighting.Server
{
	public class CrossFightServerConfig : BaseAppConfig
	{

		[ConfigProperty("IP", "IP地址", "127.0.0.1")]
		public string IP = "127.0.0.1";
		[ConfigProperty("Port", "监听的端口", 9208)]
		public int Port = 9208;
		[ConfigProperty("ServerType", "服务器类型", 0)]
		public int ServerType = 0;

        [ConfigProperty("Key", "服务器Key", "")]
        public string Key = "ouiwyqw5o82q793548$$";
        public CrossFightServerConfig()
		{
			this.Load(typeof(CrossFightServerConfig));
		}
	}
}
