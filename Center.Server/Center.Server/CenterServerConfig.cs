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
		[ConfigProperty("IP", "中心服务器监听IP", "127.0.0.1")]
		public string IP;
		[ConfigProperty("Port", "中心服务器监听端口", 9202)]
		public int Port;
		[ConfigProperty("LoginLapseInterval", "登陆超时时间,分钟为单位", 1)]
		public int LoginLapseInterval;
		[ConfigProperty("SaveInterval", "数据保存周期,分钟为单位", 1)]
		public int SaveIntervalInterval;
		[ConfigProperty("SaveRecordInterval", "日志保存周期,分钟为单位", 1)]
		public int SaveRecordInterval;
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
