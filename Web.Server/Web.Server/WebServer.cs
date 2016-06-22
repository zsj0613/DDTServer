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
using Game.Base.Config;
using Lsj.Util.Logs;
using Web.Server.Manager;
using Lsj.Util.Text;

namespace Web.Server
{
    public class WebServer
    {
        public static LogProvider log = LogProvider.Default;
        public static readonly string Edition = "10000";
        private static WebServer m_instance;
       // private Server server;
        private CenterServerConnector m_centerServer;
        public bool IsOpen
        {
            get;
            private set;
        }=false;
        public static IRunMgr Runmgr
        {
            get;
            set;
        }

        public static WebServer Instance
        {
            get
            {
                return WebServer.m_instance;
            }
        }
        public WebServerConfig Config
        {
            get
            {
                return this.m_config;
            }
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WebServer.log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
        }

        public WebServer(WebServerConfig config)
        {
            this.m_config = config;
        }
        public bool Start()
        {
            bool result = true;
            try
            {
                IsRunning = 0;
                AllocatePacketBuffers();
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
                
              //  server = new Server(IPAddress.Parse(config.WebIP), config.WebPort,  "./WebPath/");
        
                if (!this.ConnecteToCenterServer())
                {                    
                    WebServer.log.Error("Fail to Connect to Center Server");
                }
                else
                {
                    this.IsOpen = true;
                    WebServer.log.Info("Succeed to Connect to Center Server!");
                }
               
                GameEventMgr.Notify(ScriptEvent.Loaded);

               
                //server.Start();
                WebServer.log.Warn("WebHelper Service Started!");
                IsRunning = 1;




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
                this.IsRunning = -1;
                    // server.Stop();
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
        private int IsRunning = -1;
        private WebServerConfig m_config;

        public static bool IsRun => Instance?.IsRunning == 1;
        public static void StartServer()
        {
            if (Instance?.IsRunning >= 0)
            {
                return;
            }
            m_instance = new WebServer(new WebServerConfig());
            if (Instance.Start() == false)
            {
                Instance.IsRunning = -1;
            }
        }
        public static void StopServer() => Instance?.Stop();
    }
}
