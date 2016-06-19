using Game.Base.Config;
using System;
namespace Web.Server
{
	public class WebServerConfig : BaseAppConfig
	{
		[ConfigProperty("WebIP", "IP地址", "127.0.0.1")]
		public string WebIP = "127.0.0.1";
		
        public WebServerConfig()
		{
			this.Load(typeof(WebServerConfig));
		}
	}
}
