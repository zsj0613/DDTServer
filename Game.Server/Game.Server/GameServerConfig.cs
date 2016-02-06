using Bussiness;
using Game.Base.Config;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.IO;
using System.Reflection;
namespace Game.Server
{
	public class GameServerConfig : BaseAppConfig
	{
		private static LogProvider log => LogProvider.Default;
		public string RootDirectory;
		public int MaxRoomCount = 1000;
		public int MaxPlayerCount = 2000;
		[ConfigProperty("MaxClientCount", "最大连接数", 8000)]
		public int MaxClientCount = 2000;
		[ConfigProperty("AppID", "代理商编号", 1)]
		public int AppID;
		[ConfigProperty("AppName", "代理商名称", "7Road")]
		public string AppName;
		[ConfigProperty("AreaID", "区编号", 1)]
		public int AreaID;
		[ConfigProperty("AreaName", "区名称", "7Road")]
		public string AreaName;
		[ConfigProperty("ServerID", "服务器编号", 1)]
		public int ServerID;
		[ConfigProperty("ServerName", "频道的名称", "7Road")]
		public string ServerName;
		[ConfigProperty("IP", "频道的IP", "127.0.0.1")]
		public string Ip;
		[ConfigProperty("Port", "频道开放端口", 9200)]
		public int Port;
		[ConfigProperty("LoginServerIp", "中心服务器的IP", "192.168.0.2")]
		public string LoginServerIp;
		[ConfigProperty("LoginServerPort", "中心服务器的端口", 9202)]
		public int LoginServerPort;
		[ConfigProperty("ScriptAssemblies", "脚本编译引用库", "")]
		public string ScriptAssemblies;
		[ConfigProperty("ScriptCompilationTarget", "脚本编译目标名称", "")]
		public string ScriptCompilationTarget;
		[ConfigProperty("LogConfigFile", "日志配置文件", "logconfig.xml")]
		public string LogConfigFile;
		[ConfigProperty("LogPath", "日志路径", "RecordLog")]
		public string LogPath;
		[ConfigProperty("CountRecord", "是否记录日志", true)]
		public bool CountRecord;
		[ConfigProperty("TxtRecord", "是否记录统计信息", true)]
		public bool TxtRecord;
		[ConfigProperty("SaveRecordInterval", "统计信息保存的时间间隔,分钟为单位", 5)]
		public int SaveRecordInterval;
		[ConfigProperty("PrivateKey", "RSA的私钥", "")]
		public string PrivateKey;
		[ConfigProperty("DBAutosaveInterval", "数据库自动保存的时间间隔,分钟为单位", 5)]
		public int DBSaveInterval;
		[ConfigProperty("PingCheckInterval", "PING检查时间间隔,分钟为单位", 5)]
		public int PingCheckInterval;
		[ConfigProperty("GameType", "游戏类型", 0)]
		public int GAME_TYPE;
		[ConfigProperty("InterName", "接口类型", "sevenroad")]
		public string INTERFACE_NAME;
		[ConfigProperty("ShutDonwMinitus", "停服提示分钟数", 5)]
		public int SHUTDOWN_MINUTS;
        [ConfigProperty("VIPMoneys", "VIP充值金额", "1,2,3,4,5,6,7,8,9,10,2147483647")]
        public string VIP_MONEYS;
        [ConfigProperty("Expericence", "经验", "0,2147483647")]
        public string Levels;
        public GameServerConfig()
		{
			this.Load(typeof(GameServerConfig));
		}
		public bool Refresh()
		{
			this.Load(typeof(GameServerConfig));
			return GameServer.Instance.InitGlobalTimer();
		}
		protected override void Load(Type type)
		{
			if (Assembly.GetEntryAssembly() != null)
			{
				this.RootDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
			}
			else
			{
				this.RootDirectory = new FileInfo(Assembly.GetAssembly(typeof(GameServer)).Location).DirectoryName;
			}
			base.Load(type);
			using (ServiceBussiness sb = new ServiceBussiness())
			{
				ServerInfo info = sb.GetServiceSingle(this.ServerID);
				if (info == null)
				{
					GameServerConfig.log.ErrorFormat("Can't find server config,server id {0}", this.ServerID);
				}
				else
				{
					this.ServerName = info.Name;
					this.MaxRoomCount = info.Room;
					this.MaxPlayerCount = info.Total;
				}
			}
		}
	}
}
