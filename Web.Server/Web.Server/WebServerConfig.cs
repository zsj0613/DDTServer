using Game.Base.Config;
using System;
namespace Web.Server
{
	public class WebServerConfig : BaseAppConfig
	{
		[ConfigProperty("WebIP", "IP地址", "127.0.0.1")]
		public string WebIP = "127.0.0.1";
        [ConfigProperty("WebPort", "端口", 8080)]
        public int WebPort = 8080;
        [ConfigProperty("Domain", "主域名", "")]
        public string Domain = "";
        [ConfigProperty("APIDomain", "API域名", "")]
        public string APIDomain = "";
        [ConfigProperty("GameDomain", "Game域名", "")]
        public string GameDomain = "";
        [ConfigProperty("CDNDomain", "CDN域名", "")]
        public string CDNDomain = "";
        public WebServerConfig()
		{
			this.Load(typeof(WebServerConfig));
		}
	}
}
