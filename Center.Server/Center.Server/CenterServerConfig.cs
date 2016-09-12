using Game.Base.Config;
using Lsj.Util.Logs;
using System;
using System.IO;
using System.Reflection;
namespace Center.Server
{
	public class CenterServerConfig : BaseAppConfig
	{
		private static LogProvider log => CenterServer.log;
		[ConfigProperty("CenterIP", "中心服务器监听IP", "127.0.0.1")]
		public string CenterIP;
		[ConfigProperty("CenterPort", "中心服务器监听端口", 9202)]
		public int CenterPort;
		[ConfigProperty("LoginLapseInterval", "登陆超时时间,分钟为单位", 1)]
		public int LoginLapseInterval;
		[ConfigProperty("ScanAuctionInterval", "排名行扫描周期,分钟为单位", 60)]
		public int ScanAuctionInterval;
		[ConfigProperty("ScanMailInterval", "邮件扫描周期,分钟为单位", 60)]
		public int ScanMailInterval;
		[ConfigProperty("ScanConsortiaInterval", "工会扫描周期,以分钟为单位", 60)]
		public int ScanConsortiaInterval;
		public CenterServerConfig()
		{
			this.Load(typeof(CenterServerConfig));
		}
		public void Refresh()
		{
			this.Load(typeof(CenterServerConfig));
		}
		protected override void Load(Type type)
        {
			base.Load(type);
		}
	}
}
