using Game.Base.Config;
using System;
namespace Fighting.Server
{
	public class FightServerConfig : BaseAppConfig
	{
		[ConfigProperty("FightIP", "IP地址", "127.0.0.1")]
		public string FightIP = "127.0.0.1";
		[ConfigProperty("FightPort", "监听的端口", 9208)]
		public int FightPort = 9208;
		[ConfigProperty("ServerType", "服务器类型", 0)]
		public int ServerType = 0;

        [ConfigProperty("FightKey", "服务器Key", "")]
        public string FightKey = "ouiwyqw5o82q793548$$";
        public FightServerConfig()
		{
			this.Load(typeof(FightServerConfig));
		}
	}
}
