using Bussiness;
using Bussiness.Managers;
using CrossFighting.Server.Games;
using CrossFighting.Server.Rooms;
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

namespace CrossFighting.Server
{
	public class CrossFightServer : BaseServer
	{
		//private static bool KeepRunning = false;
		private CrossFightServerConfig m_config;
		private bool m_running;
		private static CrossFightServer m_instance;
		public CrossFightServerConfig Configuration
		{
			get
			{
				return this.m_config;
			}
		}
		public static CrossFightServer Instance
		{
			get
			{
				return CrossFightServer.m_instance;
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
                LogProvider.Default = new LogProvider(new LogConfig { FilePath = "./log/fight/", UseConsole = true, UseFile = true });
                this.m_running = true;
					Thread.CurrentThread.Priority = ThreadPriority.Normal;
					AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

                    CrossFightServer.log.Warn("正在初始化…………");
                    if (!Sql_DbObject.TryConnection())
                    {
                        result = false;
                        CrossFightServer.log.Error("数据库连接失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("数据库连接成功!");

                    if (!this.InitSocket(IPAddress.Parse(this.m_config.IP), this.m_config.Port))
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化监听端口失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化监听端口成功!");



                    if (!MapMgr.Init())
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化地图失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化地图成功!");

                    if (!ItemMgr.Init())
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化物品失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化物品成功!");

                    if (!PropItemMgr.Init())
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化道具失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化道具成功!");

                    if (!BallMgr.Init())
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化炸弹失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化炸弹成功!");

                    if (!DropMgr.Init())
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化掉落失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化掉落成功!");

                    if (!NPCInfoMgr.Init())
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化npc失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化npc成功!");

                    if (!LanguageMgr.Load())
                    {
                        result = false;
                        CrossFightServer.log.Error("初始化语言包失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("初始化语言包成功!");

             


                    if (!base.Start())
                    {
                        result = false;
                        CrossFightServer.log.Error("基础服务启动失败，请检查!");
                        return result;
                    }
                    CrossFightServer.log.Info("基础服务启动成功!");


                    if (!ProxyRoomMgr.Setup())
                    {
                        result = false;
                        CrossFightServer.log.Error("启动房间管理服务失败，请检查!");
                        return result;
                    }
                    ProxyRoomMgr.Start();
                    CrossFightServer.log.Info("启动房间管理服务成功!");

                    if (!GameMgr.Setup(0, 4))
                    {
                        result = false;
                        CrossFightServer.log.Error("启动游戏管理服务失败，请检查!");
                        return result;
                    }
                    GameMgr.Start();
                    StartScriptComponents();
                    GameEventMgr.Notify(ScriptEvent.Loaded);
                    CrossFightServer.log.Info("启动游戏管理服务成功!");
                    GC.Collect(GC.MaxGeneration);
                    CrossFightServer.log.Warn("战斗服务器已启动!");
                    result = true;

                }

                catch (Exception e)
                {
                    CrossFightServer.log.Error("Failed to start the server", e);
                    result = false;
                }

                return result;
            }
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			CrossFightServer.log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
		}
		protected bool InitComponent(bool componentInitState, string text)
		{
			CrossFightServer.log.Debug(text + ": " + componentInitState);
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
				ScriptMgr.InsertAssembly(typeof(CrossFightServer).Assembly);
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
				CrossFightServer.log.Debug("Registering global event handlers: true");
				result = true;
			}
			catch (Exception e)
			{
				CrossFightServer.log.Error("StartScriptComponents", e);
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
					CrossFightServer.log.Error("Server stopp error:", ex);
				}
				finally
				{
					base.Stop();
				}
				CrossFightServer.log.Warn("Fight server is stopped!");
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
		private CrossFightServer(CrossFightServerConfig config)
		{
			this.m_config = config;
		}
		public static void CreateInstance(CrossFightServerConfig config)
		{
			if (CrossFightServer.m_instance == null)
			{
				CrossFightServer.m_instance = new CrossFightServer(config);
			}
		}
	}
}
