using Bussiness;
using Bussiness.Managers;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Logic;
using Game.Base.Managers;
using Game.Server.Managers;
using Lsj.Util.Logs;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using SqlDataProvider.BaseClass;
using Fighting.Server.Managers;

namespace Fighting.Server
{
	public class FightServer : BaseServer
	{
		private static LogProvider log => LogProvider.Default;
		//private static bool KeepRunning = false;
		private FightServerConfig m_config;
		private bool m_running;
		private static FightServer m_instance;
		public FightServerConfig Configuration
		{
			get
			{
				return this.m_config;
			}
		}
		public static FightServer Instance
		{
			get
			{
				return FightServer.m_instance;
			}
		}
		protected override BaseClient GetNewClient()
		{
			return new ServerClient(this);
		}
		public override bool Start()
		{
			bool result;

				try
				{
                LogProvider.Default = new LogProvider(new LogConfig { FilePath = "./log/fight/" });
                this.m_running = true;
					Thread.CurrentThread.Priority = ThreadPriority.Normal;
					AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

                    FightServer.log.Warn("正在初始化…………");
                    if (!Sql_DbObject.TryConnection())
                    {
                        result = false;
                        FightServer.log.Error("数据库连接失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("数据库连接成功!");

                    if (!this.InitSocket(IPAddress.Parse(this.m_config.Ip), this.m_config.Port))
                    {
                        result = false;
                        FightServer.log.Error("初始化监听端口失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化监听端口成功!");



                    if (!MapMgr.Init())
                    {
                        result = false;
                        FightServer.log.Error("初始化地图失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化地图成功!");

                    if (!ItemMgr.Init())
                    {
                        result = false;
                        FightServer.log.Error("初始化物品失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化物品成功!");

                    if (!PropItemMgr.Init())
                    {
                        result = false;
                        FightServer.log.Error("初始化道具失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化道具成功!");

                    if (!BallMgr.Init())
                    {
                        result = false;
                        FightServer.log.Error("初始化炸弹失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化炸弹成功!");

                    if (!DropMgr.Init())
                    {
                        result = false;
                        FightServer.log.Error("初始化掉落失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化掉落成功!");

                    if (!NPCInfoMgr.Init())
                    {
                        result = false;
                        FightServer.log.Error("初始化npc失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化npc成功!");

                    if (!LanguageMgr.Load())
                    {
                        result = false;
                        FightServer.log.Error("初始化语言包失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化语言包成功!");

                    if (!RateMgr.Init(this.Configuration))
                    {
                        result = false;
                        FightServer.log.Error("初始化倍率失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("初始化倍率成功!");


                    if (!base.Start())
                    {
                        result = false;
                        FightServer.log.Error("基础服务启动失败，请检查!");
                        return result;
                    }
                    FightServer.log.Info("基础服务启动成功!");


                    if (!ProxyRoomMgr.Setup())
                    {
                        result = false;
                        FightServer.log.Error("启动房间管理服务失败，请检查!");
                        return result;
                    }
                    ProxyRoomMgr.Start();
                    FightServer.log.Info("启动房间管理服务成功!");

                    if (!GameMgr.Setup(0, 4))
                    {
                        result = false;
                        FightServer.log.Error("启动游戏管理服务失败，请检查!");
                        return result;
                    }
                    GameMgr.Start();
                    StartScriptComponents();
                    GameEventMgr.Notify(ScriptEvent.Loaded);
                    FightServer.log.Info("启动游戏管理服务成功!");
                    GC.Collect(GC.MaxGeneration);
                    FightServer.log.Warn("战斗服务器已启动!");
                    result = true;

                }

                catch (Exception e)
                {
                    FightServer.log.Error("Failed to start the server", e);
                    result = false;
                }

                return result;
            }
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			FightServer.log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
		}
		protected bool InitComponent(bool componentInitState, string text)
		{
			FightServer.log.Debug(text + ": " + componentInitState);
			if (!componentInitState)
			{
				this.Stop();
			}
			return componentInitState;
		}
		protected bool StartScriptComponents()
		{
			bool result;
			try
			{
				ScriptMgr.InsertAssembly(typeof(FightServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
				Assembly[] scripts = ScriptMgr.Scripts;
				Assembly[] array = scripts;
				for (int i = 0; i < array.Length; i++)
				{
					Assembly asm = array[i];
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
				}
				FightServer.log.Debug("Registering global event handlers: true");
				result = true;
			}
			catch (Exception e)
			{
				FightServer.log.Error("StartScriptComponents", e);
				result = false;
			}
			return result;
		}
		public override void Stop()
		{
			if (this.m_running)
			{
				try
				{
					this.m_running = false;
					GameMgr.Stop();
					ProxyRoomMgr.Stop();
				}
				catch (Exception ex)
				{
					FightServer.log.Error("Server stopp error:", ex);
				}
				finally
				{
					base.Stop();
				}
				FightServer.log.Warn("Fight server is stopped!");
			}
		}
		public new ServerClient[] GetAllClients()
		{
			ServerClient[] list = null;
			object syncRoot;
			Monitor.Enter(syncRoot = this._clients.SyncRoot);
			try
			{
				list = new ServerClient[this._clients.Count];
				this._clients.Keys.CopyTo(list, 0);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
			return list;
		}
		public void SendToALL(GSPacketIn pkg)
		{
			this.SendToALL(pkg, null);
		}
		public void SendToALL(GSPacketIn pkg, ServerClient except)
		{
			ServerClient[] list = this.GetAllClients();
			if (list != null)
			{
				ServerClient[] array = list;
				for (int i = 0; i < array.Length; i++)
				{
					ServerClient client = array[i];
					if (client != except)
					{
						client.SendTCP(pkg);
					}
				}
			}
		}
		private FightServer(FightServerConfig config)
		{
			this.m_config = config;
		}
		public static void CreateInstance(FightServerConfig config)
		{
			if (FightServer.m_instance == null)
			{
				FightServer.m_instance = new FightServer(config);
			}
		}
	}
}
