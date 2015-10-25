using Game.Base;
using Game.Base.Managers;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using SqlDataProvider.BaseClass;
using Lsj.Util.Config;
using System.Net;
using Lsj.Util;
using System.Collections;
using Game.Base.Events;
using Lsj.Util.Net.Web.Modules;
using Lsj.Util.Log;

namespace Web.Server
{
    public class WebServer
    {
        public static Log log = new Log(new LogConfig { FilePath= "/log/web/"});
        public static readonly string Edition = "10000";
        private static WebServer m_instance;
        private Server server;
        private CenterServerConnector m_centerServer;
        public bool IsOpen = false;
        public RunMgr runmgr
        {
            get
            {
                return m_runmgr;
            }
        }

        public static WebServer Instance
        {
            get
            {
                return WebServer.m_instance;
            }
        }

        public static void CreateInstance()
        {
            if (WebServer.m_instance != null)
            {
                throw new Exception("Can't create more than one WebServer!");
            }
            m_instance = new WebServer();
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WebServer.log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
        }
        public bool Start()
        {
            bool result = true;
            try
            {
                AllocatePacketBuffers();
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
                WebServer.log.Warn("Initiating…………");

                if (!Sql_DbObject.TryConnection())
                {
                    result = false;
                    WebServer.log.Error("Fail to connect to SQL!");
                    return result;
                }
                WebServer.log.Info("Succeed to connect to SQL!");
                server = new Server(IPAddress.Parse(AppConfig.AppSettings["IP"]), AppConfig.AppSettings["Port"].ConvertToInt(88));
                if (!StartScriptComponents())
                {
                    result = false;
                    WebServer.log.Error("Fail to Load Modules!");
                    return result;
                }
                WebServer.log.Info("Succeed to Load Modules!");
                if (!this.ConnecteToCenterServer())
                {                    
                    WebServer.log.Error("Fail to Connect to Center Server");
                }
                else
                {
                    this.IsOpen = true;
                    WebServer.log.Info("Succeed to Connect to Center Server!");
                }

                this.m_runmgr = new RunMgr();
                GameEventMgr.Notify(ScriptEvent.Loaded);
                
                FileModule.Path = @"web\";
                server.Start();
                WebServer.log.Warn("Web Service Started!");




            }
            catch (Exception e)
            {
                WebServer.log.Error("Fail to start the server", e);
                result = false;
            }
            return result;
        }

        private bool ConnecteToCenterServer()
        {
            this.m_centerServer = new CenterServerConnector(AppConfig.AppSettings["CenterIP"], AppConfig.AppSettings["CenterPort"].ConvertToInt(25001),this.AcquirePacketBuffer(), this.AcquirePacketBuffer());
            this.m_centerServer.Disconnected += new ClientEventHandle(this.centerServer_Disconnected);
            return this.m_centerServer.Connect();
        }
        private Queue m_packetBufPool;
        private RunMgr m_runmgr;

        private bool AllocatePacketBuffers()
        {
            int count = 1000;
            this.m_packetBufPool = new Queue(count);
            for (int i = 0; i < count; i++)
            {
                this.m_packetBufPool.Enqueue(new byte[2048]);
            }
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
            WebServer.log.Warn("packet buffer pool is empty!");
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
        private void centerServer_Disconnected(BaseClient client)
        {
            this.IsOpen = false;
            WebServer.log.Warn("Disconnected From Center Server");
        }
        public bool StartScriptComponents()
        {
            bool result;
            try
            {
                ScriptMgr.InsertAssembly(typeof(WebServer).Assembly);
                string[] a = Directory.GetFiles(@".\Web\Modules\", "*.dll");
                foreach (var b in a)
                {
                    WebServer.log.Debug("Loading" + b);
                    if (ScriptMgr.InsertAssembly(Assembly.LoadFile(System.Environment.CurrentDirectory + b.Remove(0, 1))))
                        WebServer.log.Debug("Succeed to load" + b);
                    else
                        WebServer.log.Error("Fail to load" + b);
                }
                ArrayList scripts = new ArrayList(ScriptMgr.Scripts);
                foreach (Assembly asm in scripts)
                {
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
                    GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
                }
                WebServer.log.Debug("Registering global event handlers: true");
            }
            catch (Exception e)
            {
                WebServer.log.Error("StartScriptComponents", e);
                result = false;
                return result;
            }
            result = true;
            return result;
        }
        public void Stop()
        {
            try
            {
                if(m_centerServer!=null&&m_centerServer.IsConnected)
                m_centerServer.Disconnect();
                server.Stop();
            }
            catch
            {
            }
        }

        public bool Reconnect()
        {
            if (this.ConnecteToCenterServer())
            {
                this.IsOpen = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
