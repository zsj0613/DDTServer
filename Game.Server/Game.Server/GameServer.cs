using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Events;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using Game.Base.Managers;
using Lsj.Util.Logs;

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

using SqlDataProvider.BaseClass;
using Game.Base.Packets;

namespace Game.Server
{
	public class GameServer : BaseServer
	{
		private const int BUF_SIZE = 2048;
		public static LogProvider log => LogProvider.Default;
		public static readonly string Edition = "10000";
		public static bool KeepRunning = false;
		public static bool ManagerLogined = false;
		private static GameServer m_instance = null;
		private bool m_isRunning;
		private GameServerConfig m_config;
		private LoginServerConnector _loginServer;
		private Queue m_packetBufPool;
		private bool m_debugMenory = false;
		private static int m_tryCount = 4;
		private static bool m_compiled = false;
		private Timer _shutdownTimer;
		private int _shutdownCount = 6;
		protected Timer m_saveDbTimer;
		protected Timer m_pingCheckTimer;
		protected Timer m_saveRecordTimer;
		protected Timer m_buffScanTimer;
		protected Timer m_fightServerTimer;
		protected Timer m_messageClearTimer;
		protected Timer m_limitItemRefreshTimer;
		public static GameServer Instance
		{
			get
			{
				return GameServer.m_instance;
			}
		}
		public bool IsRunning
		{
			get
			{
				return this.m_isRunning;
			}
		}
		public GameServerConfig Configuration
		{
			get
			{
				return this.m_config;
			}
		}
		public LoginServerConnector LoginServer
		{
			get
			{
				return this._loginServer;
			}
		}
		public int PacketPoolSize
		{
			get
			{
				return this.m_packetBufPool.Count;
			}
		}
		public static void CreateInstance(GameServerConfig config)
		{
			if (GameServer.m_instance == null)
			{
				GameServer.m_instance = new GameServer(config);
			}
		}
		protected GameServer(GameServerConfig config)
		{
			this.m_config = config;
			//if (GameServer.log.IsDebugEnabled)
			//{
				GameServer.log.Debug("Current directory is: " + Directory.GetCurrentDirectory());
				GameServer.log.Debug("Gameserver root directory is: " + this.Configuration.RootDirectory);
				GameServer.log.Debug("Changing directory to root directory");
			//}
			Directory.SetCurrentDirectory(this.Configuration.RootDirectory);
		}
		public bool RefreshGameProperties()
		{
			//GameProperties.Refresh();
			//AwardMgr.DailyAwardState = GameProperties.DAILY_AWARD_STATE;
			//AntiAddictionMgr.SetASSState(GameProperties.ASS_STATE);
			return true;
		}
		private bool AllocatePacketBuffers()
		{
			int count = this.Configuration.MaxClientCount * 3;
			this.m_packetBufPool = new Queue(count);
			for (int i = 0; i < count; i++)
			{
				this.m_packetBufPool.Enqueue(new byte[2048]);
			}
			//if (GameServer.log.IsDebugEnabled)
			//{
				GameServer.log.DebugFormat("allocated packet buffers: {0}", count.ToString());
			//}
			return true;
		}
		public byte[] AcquirePacketBuffer()
		{
			object syncRoot;
			Monitor.Enter(syncRoot = this.m_packetBufPool.SyncRoot);
			byte[] result;
			try
			{
				if (this.m_packetBufPool.Count > 0)
				{
					result = (byte[])this.m_packetBufPool.Dequeue();
					return result;
				}
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			GameServer.log.Warn("packet buffer pool is empty!");
			result = new byte[2048];
			return result;
		}
		public void ReleasePacketBuffer(byte[] buf)
		{
			if (buf != null && GC.GetGeneration(buf) >= GC.MaxGeneration)
			{
				object syncRoot;
				Monitor.Enter(syncRoot = this.m_packetBufPool.SyncRoot);
				try
				{
					this.m_packetBufPool.Enqueue(buf);
				}
				finally
				{
					Monitor.Exit(syncRoot);
				}
			}
		}
		protected override BaseClient GetNewClient()
		{
			return new GameClient(this, this.AcquirePacketBuffer(), this.AcquirePacketBuffer());
		}
		public new GameClient[] GetAllClients()
		{
			GameClient[] list = null;
			object syncRoot;
			Monitor.Enter(syncRoot = this._clients.SyncRoot);
			try
			{
				list = new GameClient[this._clients.Count];
				this._clients.Keys.CopyTo(list, 0);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return list;
		}
		public override bool Start()
		{

            bool result = true;
            try
            {
                LogProvider.Default = new LogProvider(new LogConfig { FilePath = "./log/game/" });
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

                GameServer.log.Warn("正在初始化…………");


               
                if (!Sql_DbObject.TryConnection())
                {
                    result = false;
                    GameServer.log.Error("数据库连接失败，请检查!");
                    return result;
                }
                GameServer.log.Info("数据库连接成功!");
                RefreshGameProperties();

                if (!this.StartScriptComponents())
                {
                    result = false;
                    GameServer.log.Error("初始化脚本失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化脚本成功!");

                if (!this.InitSocket(IPAddress.Parse(this.Configuration.Ip), this.Configuration.Port))
                {
                    result = false;
                    GameServer.log.Error("初始化监听端口失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化监听端口成功!");

                if (!this.AllocatePacketBuffers())
                {
                    result = false;
                    GameServer.log.Error("分配数据包缓冲区失败，请检查!");
                    return result;
                }
                GameServer.log.Info("分配数据包缓冲区成功!");

                //LogMgr.Setup

                if (!WorldMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化世界场景失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化世界场景成功!");

                if (!MapMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化地图失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化地图成功!");

                if (!ItemMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化物品失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化物品成功!");

                if (!ItemBoxMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化宝箱失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化宝箱成功!");

                if (!BallMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化炸弹失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化炸弹成功!");

                if (!FusionMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化熔炼失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化熔炼成功!");

                if (!AwardMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化奖励失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化奖励成功!");

                if (!NPCInfoMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化npc信息失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化npc信息成功!");

                if (!MissionInfoMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化关卡失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化关卡成功!");

                if (!PveInfoMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化pve失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化pve成功!");

                if (!DropMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化掉落失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化掉落成功!");

                if (!FightRateMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化战斗倍率失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化战斗倍率成功!");

                if (!ConsortiaLevelMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化公会等级失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化公会等级成功!");

                if (!StrengthenMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化强化失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化强化成功!");

                if (!PropItemMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化道具失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化道具成功!");

                if (!ShopMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化商店失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化商店成功!");

                if (!BoxMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化时间宝箱失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化时间宝箱成功!");

                if (!QuestMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化任务失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化任务成功!");

                if (!AchievementMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化成就失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化成就成功!");

                if (!AchievementMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化成就失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化成就成功!");

                if (!RoomMgr.Setup(this.Configuration.MaxRoomCount))
                {
                    result = false;
                    GameServer.log.Error("初始化房间管理服务失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化房间管理服务成功!");

                if (!GameMgr.Setup(1, 4))
                {
                    result = false;
                    GameServer.log.Error("初始化游戏管理服务失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化游戏管理服务成功!");

                if (!ConsortiaMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化公会失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化公会成功!");

                if (!LanguageMgr.Load())
                {
                    result = false;
                    GameServer.log.Error("初始化语言包失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化语言包成功!");

                if (!Game.Server.Managers.RateMgr.Init(this.Configuration))
                {
                    result = false;
                    GameServer.log.Error("初始化经验倍率管理服务失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化经验倍率管理服务成功!");

                if (!MacroDropMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化宏观掉落失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化宏观掉落成功!");


                if (!BattleMgr.Setup())
                {
                    result = false;
                    GameServer.log.Error("加载战斗管理服务失败，请检查!");
                    return result;
                }
                GameServer.log.Info("加载战斗管理服务成功!");

                if (!this.InitGlobalTimer())
                {
                    result = false;
                    GameServer.log.Error("初始化全局Timers失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化全局Timers成功!");

                if (!this.InitLoginServer())
                {
                    result = false;
                    GameServer.log.Error("登陆到中心服务器失败，请检查!");
                    return result;
                }
                GameServer.log.Info("登陆到中心服务器成功!");

                if (!MarryRoomMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化礼堂失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化礼堂成功!");

                if (!SpaRoomMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化温泉失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化温泉成功!");

                if (!ActiveMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化活动失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化活动成功!");
                if (!VIPMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化VIP失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化VIP成功!");
                if (!LevelMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化等级失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化等级成功!");
                /*if (!WorldBossMgr.Init())
                {
                    result = false;
                    GameServer.log.Error("初始化世界Boss失败，请检查!");
                    return result;
                }
                GameServer.log.Info("初始化世界Boss成功!");*/

                RoomMgr.Start();
                GameMgr.Start();
                BattleMgr.Start();
                MacroDropMgr.Start();

                if (!base.Start())
                {
                    result = false;
                    GameServer.log.Error("启动基础服务失败，请检查!");
                    return result;
                }
                GameServer.log.Info("启动基础服务成功!");
                this.m_isRunning = true;

                GameEventMgr.Notify(ScriptEvent.Loaded);
                GC.Collect(GC.MaxGeneration);
                //LogMgr.Setup(1, 1, 1);
                GameServer.log.Warn("游戏服务器启动成功!");



            }
            catch (Exception e)
            {
                GameServer.log.Error("Failed to start the server", e);
               // throw e;
                result = false;
            }
            return result;
        }
		private bool InitLoginServer()
		{
			this._loginServer = new LoginServerConnector(this.m_config.LoginServerIp, this.m_config.LoginServerPort, this.m_config.ServerID, this.m_config.ServerName, this.AcquirePacketBuffer(), this.AcquirePacketBuffer());
			this._loginServer.Disconnected += new ClientEventHandle(this.loginServer_Disconnected);
			return this._loginServer.Connect();
		}
		private void loginServer_Disconnected(BaseClient client)
		{
			bool running = this.m_isRunning;
			this.Stop();
			if (running && GameServer.m_tryCount > 0)
			{
				GameServer.m_tryCount--;
				GameServer.log.Error("Center Server Disconnect! Stopping Server");
				GameServer.log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", GameServer.m_tryCount);
				Thread.Sleep(1000);
				if (this.Start())
				{
					GameServer.log.Error("Restart the game server success!");
				}
			}
			else
			{
				if (GameServer.m_tryCount == 0)
				{
					GameServer.log.ErrorFormat("Restart the game server failed after {0} times.", 4);
					GameServer.log.Error("Server Stopped!");
				}
			}
		}
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				GameServer.log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
			}
			catch
			{				
			}
		}

		public bool StartScriptComponents()
		{
			bool result;
			try
			{
				ScriptMgr.InsertAssembly(typeof(GameServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
                string[] a = Directory.GetFiles(@".\Scripts\", "*.dll");
                foreach (var b in a)
                {
                    GameServer.log.Debug("加载" + b);
                    if(ScriptMgr.InsertAssembly(Assembly.LoadFile(System.Environment.CurrentDirectory + b.Remove(0,1))))
                    GameServer.log.Debug("加载"+b+"成功！");
                    else
                    GameServer.log.Error("加载" + b + "失败！");
                }
                ArrayList scripts = new ArrayList(ScriptMgr.Scripts);
				foreach (Assembly asm in scripts)
				{
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
				}
				GameServer.log.Debug("Registering global event handlers: true");
			}
			catch (Exception e)
			{
				GameServer.log.Error("StartScriptComponents", e);
				result = false;
				return result;
			}
			result = true;
			return result;
		}
		protected bool InitComponent(bool componentInitState, string text)
		{
			if (this.m_debugMenory)
			{
				GameServer.log.Debug(string.Concat(new object[]
				{
					"Start Memory ",
					text,
					": ",
					GC.GetTotalMemory(false) / 1024L / 1024L
				}));
			}
			//if (GameServer.log.IsDebugEnabled)
			{
				GameServer.log.Debug(text + ": " + componentInitState);
			}
			if (!componentInitState)
			{
				GameServer.log.Error(text + ": " + componentInitState);
				this.Stop();
			}
			if (this.m_debugMenory)
			{
				GameServer.log.Debug(string.Concat(new object[]
				{
					"Finish Memory ",
					text,
					": ",
					GC.GetTotalMemory(false) / 1024L / 1024L
				}));
			}
			return componentInitState;
		}
		public override void Stop()
		{
			if (this.m_isRunning)
			{
				this.m_isRunning = false;
				if (!MarryRoomMgr.UpdateBreakTimeWhereServerStop())
				{
					Console.WriteLine("Update Marry BreakTime failed");
				}
				if (!SpaRoomMgr.UpdateBreakTimeWhereSpaServerStop())
				{
					Console.WriteLine("Update Spa BreakTime failed");
				}
				RoomMgr.Stop();
				GameMgr.Stop();
				BattleMgr.Stop();
				if (this._loginServer != null)
				{
					this._loginServer.Disconnected -= new ClientEventHandle(this.loginServer_Disconnected);
					this._loginServer.Disconnect();
				}
				if (this.m_pingCheckTimer != null)
				{
					this.m_pingCheckTimer.Change(-1, -1);
					this.m_pingCheckTimer.Dispose();
					this.m_pingCheckTimer = null;
				}
				if (this.m_saveDbTimer != null)
				{
					this.m_saveDbTimer.Change(-1, -1);
					this.m_saveDbTimer.Dispose();
					this.m_saveDbTimer = null;
				}
				if (this.m_saveRecordTimer != null)
				{
					this.m_saveRecordTimer.Change(-1, -1);
					this.m_saveRecordTimer.Dispose();
					this.m_saveRecordTimer = null;
					this.SaveRecordProc(null);
				}
				if (this.m_buffScanTimer != null)
				{
					this.m_buffScanTimer.Change(-1, -1);
					this.m_buffScanTimer.Dispose();
					this.m_buffScanTimer = null;
				}
				if (this.m_limitItemRefreshTimer != null)
				{
					this.m_limitItemRefreshTimer.Change(-1, -1);
					this.m_limitItemRefreshTimer.Dispose();
					this.m_limitItemRefreshTimer = null;
				}
				this.SaveTimerProc(null);
				base.Stop();
				Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
				GameServer.log.Warn("Server Stopped!");
			}
		}
		public void Shutdown()
		{
			GameServer.Instance.LoginServer.DisplayMessage(string.Format(GameServer.Instance.Configuration.ServerID + "Server begin stoping !", new object[0]));
			this._shutdownTimer = new Timer(new TimerCallback(this.ShutDownCallBack), null, 0, 60000);
		}
		private void ShutDownCallBack(object state)
		{
			try
			{
				this._shutdownCount--;
				Console.WriteLine(string.Format("Server will shutdown after {0} mins!", this._shutdownCount));
				GameClient[] list = GameServer.Instance.GetAllClients();
				GameClient[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					GameClient c = array[i];
					if (c.Out != null)
					{
						c.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1", new object[0]), this._shutdownCount, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2", new object[0])));
					}
				}
				if (this._shutdownCount == 0)
				{
					Console.WriteLine("Server has stopped!");
					GameServer.Instance.LoginServer.DisplayMessage(string.Format(GameServer.Instance.Configuration.ServerID + "Server has stopped!", new object[0]));
					this._shutdownTimer.Dispose();
					this._shutdownTimer = null;
					GameServer.Instance.Stop();
				}
			}
			catch (Exception ex)
			{
				GameServer.log.Error(ex);
			}
		}
		public bool InitGlobalTimer()
		{
			int interval = this.Configuration.DBSaveInterval * 60 * 1000;
			if (this.m_saveDbTimer == null)
			{
				this.m_saveDbTimer = new Timer(new TimerCallback(this.SaveTimerProc), null, interval, interval);
			}
			else
			{
				this.m_saveDbTimer.Change(interval, interval);
			}
			interval = this.Configuration.PingCheckInterval * 60 * 1000;
			if (this.m_pingCheckTimer == null)
			{
				this.m_pingCheckTimer = new Timer(new TimerCallback(this.PingCheck), null, interval, interval);
			}
			else
			{
				this.m_pingCheckTimer.Change(interval, interval);
			}
			interval = this.Configuration.SaveRecordInterval * 60 * 1000;
			if (this.m_saveRecordTimer == null)
			{
				this.m_saveRecordTimer = new Timer(new TimerCallback(this.SaveRecordProc), null, interval, interval);
			}
			else
			{
				this.m_saveRecordTimer.Change(interval, interval);
			}
			interval = 60000;
			if (this.m_buffScanTimer == null)
			{
				this.m_buffScanTimer = new Timer(new TimerCallback(this.BuffScanTimerProc), null, interval, interval);
			}
			else
			{
				this.m_buffScanTimer.Change(interval, interval);
			}
			interval = 60000;
			if (this.m_fightServerTimer == null)
			{
				this.m_fightServerTimer = new Timer(new TimerCallback(this.FightServerScanTimerProc), null, interval, interval);
			}
			else
			{
				this.m_fightServerTimer.Change(interval, interval);
			}
			if (this.m_messageClearTimer == null)
			{
				this.m_messageClearTimer = new Timer(new TimerCallback(this.MessageClearScanTimerProc), null, interval, interval);
			}
			else
			{
				this.m_messageClearTimer.Change(interval, interval);
			}
			if (this.m_limitItemRefreshTimer == null)
			{
				this.m_limitItemRefreshTimer = new Timer(new TimerCallback(this.LoadShopLimitCount), null, interval, interval);
			}
			else
			{
				this.m_limitItemRefreshTimer.Change(interval, interval);
			}
			return true;
		}
		protected void PingCheck(object sender)
		{
			try
			{
				GameServer.log.Info("Begin ping check....");
				long interval = (long)this.Configuration.PingCheckInterval * 60L * 1000L * 1000L * 10L;
				GameClient[] list = this.GetAllClients();
				if (list != null)
				{
					GameClient[] array = list;
					for (int i = 0; i < array.Length; i++)
					{
						GameClient client = array[i];
						if (client.IsConnected)
						{
							if (client.Player != null)
							{
								client.Out.SendPingTime(client.Player);
								if (AntiAddictionMgr.ISASSon && client.Player.ASSonSend)
								{
									if (client.Player.PlayerCharacter.IsFirst == 0 && (DateTime.Now - client.Player.PlayerCharacter.AntiDate).TotalMinutes >= 30.0)
									{
										client.Player.Out.SendAASState(true);
									}
									client.Player.ASSonSend = false;
								}
							}
							else
							{
								if (client.PingTime + interval < DateTime.Now.Ticks)
								{
									client.Disconnect();
								}
							}
						}
						else
						{
							client.Disconnect();
						}
					}
				}
				GameServer.log.Info("End ping check....");
			}
			catch (Exception e)
			{
				//if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("PingCheck callback", e);
				}
			}
			try
			{
				GameServer.log.Info("Begin ping center check....");
				GameServer.Instance.LoginServer.SendPingCenter();
				GameServer.log.Info("End ping center check....");
			}
			catch (Exception e)
			{
				//if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("PingCheck center callback", e);
				}
			}
		}
		protected void LoadShopLimitCount(object sender)
		{
			try
			{
				GameServer.log.Info("Begin loading ShopLimitCount....");
				if ( DateTime.Now.ToString("hh:mm") == "20:00")
				{
					ShopMgr.RefreshLimitCount();
				}
				GameServer.log.Info("End loading ShopLimitCount....");
			}
			catch (Exception e)
			{
				//if (GameServer.log.IsErrorEnabled)
				//{
					GameServer.log.Error("LoadShopLimitCountcallback", e);
				//}
			}
		}
		protected void SaveTimerProc(object sender)
		{
			try
			{
				int startTick = Environment.TickCount;
				//if (GameServer.log.IsInfoEnabled)
				//{
					GameServer.log.Info("Saving database...");
					GameServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				//}
				int saveCount = 0;
				ThreadPriority oldprio = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				GamePlayer[] list = WorldMgr.GetAllPlayers();
				GamePlayer[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					p.SaveIntoDatabase();
					saveCount++;
				}
				Thread.CurrentThread.Priority = oldprio;
				startTick = Environment.TickCount - startTick;
				//if (GameServer.log.IsInfoEnabled)
				//{
					GameServer.log.Info("Saving database complete!");
					GameServer.log.Info(string.Concat(new object[]
					{
						"Saved all databases and ",
						saveCount,
						" players in ",
						startTick,
						"ms"
					}));
				//}
				if (startTick > 120000)
				{
					GameServer.log.WarnFormat("Saved all databases and {0} players in {1} ms", saveCount, startTick);
				}
			}
			catch (Exception e)
			{
				//if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("SaveTimerProc", e);
				}
			}
			finally
			{
				GameEventMgr.Notify(GameServerEvent.WorldSave);
			}
		}
		protected void SaveRecordProc(object sender)
		{
			/*try
			{
				//int startTick = Environment.TickCount;
				//if (GameServer.log.IsInfoEnabled)
				//{
				//	GameServer.log.Info("Saving Record...");
				//	GameServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				//}
				//ThreadPriority oldprio = Thread.CurrentThread.Priority;
				//Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				LogMgr.Save();
				Thread.CurrentThread.Priority = oldprio;
				startTick = Environment.TickCount - startTick;
				if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Saving Record complete!");
				}
				if (startTick > 120000)
				{
					GameServer.log.WarnFormat("Saved all Record  in {0} ms", startTick);
				}
			}
			catch (Exception e)
			{
				if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("SaveRecordProc", e);
				}
			}*/
		}
		protected void BuffScanTimerProc(object sender)
		{
			try
			{
				int startTick = Environment.TickCount;
				//if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Buff Scaning ...");
					GameServer.log.Debug("BuffScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				int saveCount = 0;
				ThreadPriority oldprio = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				GamePlayer[] list = WorldMgr.GetAllPlayers();
				GamePlayer[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p.BufferList != null)
					{
						p.BufferList.Update();
						saveCount++;
					}
				}
				Thread.CurrentThread.Priority = oldprio;
				startTick = Environment.TickCount - startTick;
				//if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("Buff Scan complete!");
					GameServer.log.Info(string.Concat(new object[]
					{
						"Buff all ",
						saveCount,
						" players in ",
						startTick,
						"ms"
					}));
				}
				if (startTick > 120000)
				{
					GameServer.log.WarnFormat("Scan all Buff and {0} players in {1} ms", saveCount, startTick);
				}
			}
			catch (Exception e)
			{
				//if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("BuffScanTimerProc", e);
				}
			}
		}
		protected void FightServerScanTimerProc(object sender)
		{
			try
			{
				int startTick = Environment.TickCount;
				//if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Debug("FightServerScan Scaning ...");
					GameServer.log.Debug("FightServerScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority oldprio = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				BattleMgr.Update();
				Thread.CurrentThread.Priority = oldprio;
				startTick = Environment.TickCount - startTick;
				//if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Debug("FightServerScan  complete!");
				}
				if (startTick > 120000)
				{
				}
			}
			catch (Exception e)
			{
				//if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("FightServerScanTimerProc", e);
				}
			}
		}
		protected void MessageClearScanTimerProc(object sender)
		{
			try
			{
				int startTick = Environment.TickCount;
				//if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("MessageClearScan Scaning ...");
					GameServer.log.Debug("MessageClearScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority oldprio = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				MessageMgr.RemoveMessageRecord();
				Thread.CurrentThread.Priority = oldprio;
				startTick = Environment.TickCount - startTick;
				//if (GameServer.log.IsInfoEnabled)
				{
					GameServer.log.Info("MessageClearScan  complete!");
				}
				if (startTick > 120000)
				{
				}
			}
			catch (Exception e)
			{
				//if (GameServer.log.IsErrorEnabled)
				{
					GameServer.log.Error("MessageClearScanTimerProc", e);
				}
			}
		}
	}
}
