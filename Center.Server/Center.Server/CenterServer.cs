using Bussiness;
using Game.Base;
using Game.Base.Packets;
using System;
using System.Net;
using System.Threading;
using SqlDataProvider.BaseClass;
using Lsj.Util.Logs;
using Lsj.Util.Config;
using Lsj.Util;
using Center.Server.Managers;
using Lsj.Util.Text;

namespace Center.Server
{
    public class CenterServer : BaseServer
    {
        public new static LogProvider log = LogProvider.Default;
        public static readonly string Edition = "11000";
        private CenterServerConfig m_config;
        private Timer m_loginLapseTimer;
        private Timer m_saveDBTimer;
        private Timer m_scanAuction;
        private Timer m_scanMail;
        private Timer m_scanConsortia;
        private Timer m_ChargeTimer;
        private Timer m_RenameCheckTimer;
        private int IsRunning = -1;
        public CrossServerConnector connector
        {
            get;
            private set;
        }

        public CenterServerConfig Config
        {
            get
            {
                return this.m_config;
            }
        }



        private static CenterServer m_instance;
        public static CenterServer Instance
        {
            get
            {
                return CenterServer.m_instance;
            }
        }



        private CenterServer(CenterServerConfig config)
        {
            this.m_config = config;
        }
        protected override BaseClient GetNewClient()
        {
            return new ServerClient(this);
        }
        public override bool Start()
        {
            this.IsRunning = 0;
            bool result = true;
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
            

                if (!this.InitSocket(IPAddress.Parse(this.m_config.CenterIP), this.m_config.CenterPort))
                {
                    result = false;
                    CenterServer.log.Error("初始化监听端口失败，请检查!");
                    return result;
                }
                CenterServer.log.Info("初始化监听端口成功!");

                if (!CenterService.Start())
                {
                    result = false;
                    CenterServer.log.Error("启动服务失败，请检查!");
                    return result;
                }
                CenterServer.log.Info("启动服务成功!");
         

                if (!this.InitGlobalTimers())
                {
                    result = false;
                    CenterServer.log.Error("初始化全局Timer失败，请检查!");
                    return result;
                }
                CenterServer.log.Info("初始化全局Timer成功!");


                if (!this.InitRenameCheckTimer())
                {
                    result = false;
                    CenterServer.log.Error("初始化改名卡失败，请检查!");
                    return result;
                }
                CenterServer.log.Info("初始化改名卡成功!");
                if (!this.InitChargeTimer())
                {
                    result = false;
                    CenterServer.log.Error("初始化充值服务失败，请检查!");
                    return result;
                }
                CenterServer.log.Info("初始化充值服务成功!");

                if (!this.ConnectToCrossServer())
                {

                    result = false;
                    CenterServer.log.Error("Failed to Connect to CrossServer");
                    return result;
                }

                CenterServer.log.Info("Succeed to Connect to CrossServer");

                if (!base.Start())
                {

                    result = false;
                    CenterServer.log.Error("监听连接错误，请检查!");
                    return result;
                }
                GC.Collect(GC.MaxGeneration);
                CenterServer.log.Warn("中心服务器已启动!");
                this.IsRunning = 1;


            }
            catch (Exception e)
            {
                CenterServer.log.Error("Failed to start the server", e);
                result = false;
            }
            return result;
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CenterServer.log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
        }
        protected bool InitComponent(bool componentInitState, string text)
        {
            CenterServer.log.Debug(text + ": " + componentInitState);
            if (!componentInitState)
            {
                this.Stop();
            }
            return componentInitState;
        }
        private bool ConnectToCrossServer()
        {
            var result = false;
            try
            {
                connector = new CrossServerConnector(AppConfig.AppSettings["CrossServerIP"], AppConfig.AppSettings["CrossServerPort"].ConvertToInt(30000), AppConfig.AppSettings["CrossServerKey"], new byte[2048], new byte[2048]);
                result = connector.Connect();
            }
            catch(Exception e)
            {
                log.Error(e);
            }
            return result;
        }

        private bool InitChargeTimer()
        {
            int interval = 30 * 1000;//三十秒
            this.m_ChargeTimer = new Timer(new TimerCallback(this.CheckCharge), null, interval, interval);

            return true;
        }
        private bool InitRenameCheckTimer()
        {
            int interval = 60 * 1000;//三十秒
            this.m_RenameCheckTimer = new Timer(new TimerCallback(this.CheckRename), null, interval, interval);

            return true;
        }

        private void CheckRename(object state)
        {
            CenterServer.log.Debug("检查改名中...");
            ThreadPriority oldprio = Thread.CurrentThread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            RenameMgr.Do();
        }

        public bool InitGlobalTimers()
        {
            int interval = this.m_config.SaveIntervalInterval * 60 * 1000;
            if (this.m_saveDBTimer == null)
            {
                this.m_saveDBTimer = new Timer(new TimerCallback(this.SaveTimerProc), null, interval, interval);
            }
            else
            {
                this.m_saveDBTimer.Change(interval, interval);
            }
            interval = 60000;
            if (this.m_loginLapseTimer == null)
            {
                this.m_loginLapseTimer = new Timer(new TimerCallback(this.LoginLapseTimerProc), null, interval, interval);
            }
            else
            {
                this.m_loginLapseTimer.Change(interval, interval);
            }
            interval = this.m_config.ScanAuctionInterval * 60 * 1000;
            if (this.m_scanAuction == null)
            {
                this.m_scanAuction = new Timer(new TimerCallback(this.ScanAuctionProc), null, interval, interval);
            }
            else
            {
                this.m_scanAuction.Change(interval, interval);
            }
            interval = this.m_config.ScanMailInterval * 60 * 1000;
            if (this.m_scanMail == null)
            {
                this.m_scanMail = new Timer(new TimerCallback(this.ScanMailProc), null, interval, interval);
            }
            else
            {
                this.m_scanMail.Change(interval, interval);
            }
            interval = this.m_config.ScanConsortiaInterval * 60 * 1000;
            if (this.m_scanConsortia == null)
            {
                this.m_scanConsortia = new Timer(new TimerCallback(this.ScanConsortiaProc), null, interval, interval);
            }
            else
            {
                this.m_scanConsortia.Change(interval, interval);
            }
            return true;
        }
        public void DisposeGlobalTimers()
        {
            if (this.m_saveDBTimer != null)
            {
                this.m_saveDBTimer.Dispose();
            }
            if (this.m_loginLapseTimer != null)
            {
                this.m_loginLapseTimer.Dispose();
            }
            if (this.m_scanAuction != null)
            {
                this.m_scanAuction.Dispose();
            }
            if (this.m_scanMail != null)
            {
                this.m_scanMail.Dispose();
            }
            if (this.m_scanConsortia != null)
            {
                this.m_scanConsortia.Dispose();
            }
            if (this.m_ChargeTimer != null)
            {
                this.m_ChargeTimer.Dispose();
            }
        }
        protected void CheckCharge(object sender)
        {
            CenterServer.log.Debug("检查充值中...");
            ThreadPriority oldprio = Thread.CurrentThread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            ChargeMgr.Do();
        }
        protected void SaveTimerProc(object state)
        {
            try
            {
                int startTick = Environment.TickCount;
                CenterServer.log.Debug("Saving database...");
                CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                Thread.CurrentThread.Priority = oldprio;
                startTick = Environment.TickCount - startTick;
                CenterServer.log.Debug("Saving database complete!");
                CenterServer.log.Debug("Saved all databases " + startTick + "ms");
            }
            catch (Exception e)
            {

                CenterServer.log.Error("SaveTimerProc", e);

            }
        }
        protected void LoginLapseTimerProc(object sender)
        {
            try
            {
                Player[] list = LoginMgr.GetAllPlayer();
                long now = DateTime.Now.Ticks;
                long interval = (long)this.m_config.LoginLapseInterval * 60L * 1000L * 10L * 1000L;
                Player[] array = list;
                for (int i = 0; i < array.Length; i++)
                {
                    Player player = array[i];
                    if (player.State == ePlayerState.NotLogin)
                    {
                        if (player.LastTime + interval < now)
                        {
                            LoginMgr.RemovePlayer(player.Id);
                        }
                    }
                    else
                    {
                        player.LastTime = DateTime.Now.Ticks;
                    }
                }
            }
            catch (Exception ex)
            {

                CenterServer.log.Error("LoginLapseTimer callback", ex);

            }
        }
        protected void ScanAuctionProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                CenterServer.log.Debug("Saving Record...");
                CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                string noticeUserID = "";
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    db.ScanAuction(ref noticeUserID);
                }
                string[] userIDs = noticeUserID.Split(new char[]
                {
                    ','
                });
                string[] array = userIDs;
                for (int i = 0; i < array.Length; i++)
                {
                    string s = array[i];
                    if (!string.IsNullOrEmpty(s))
                    {
                        GSPacketIn pkg = new GSPacketIn(117);
                        pkg.WriteInt(int.Parse(s));
                        pkg.WriteInt(1);
                        this.SendToALL(pkg);
                    }
                }
                Thread.CurrentThread.Priority = oldprio;
                startTick = Environment.TickCount - startTick;
                CenterServer.log.Debug("Scan Auction complete!");
                if (startTick > 120000)
                {
                    CenterServer.log.WarnFormat("Scan all Auction  in {0} ms", startTick);
                }
            }
            catch (Exception e)
            {

                CenterServer.log.Error("ScanAuctionProc", e);

            }
        }
        protected void ScanMailProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                CenterServer.log.Debug("Saving Record...");
                CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                string noticeUserID = "";
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    db.ScanMail(ref noticeUserID);
                }
                string[] userIDs = noticeUserID.Split(new char[]
                {
                    ','
                });
                string[] array = userIDs;
                for (int i = 0; i < array.Length; i++)
                {
                    string s = array[i];
                    if (!string.IsNullOrEmpty(s))
                    {
                        GSPacketIn pkg = new GSPacketIn(117);
                        pkg.WriteInt(int.Parse(s));
                        pkg.WriteInt(1);
                        this.SendToALL(pkg);
                    }
                }
                Thread.CurrentThread.Priority = oldprio;
                startTick = Environment.TickCount - startTick;
                CenterServer.log.Debug("Scan Mail complete!");
                if (startTick > 120000)
                {
                    CenterServer.log.WarnFormat("Scan all Mail in {0} ms", startTick);
                }
            }
            catch (Exception e)
            {

                CenterServer.log.Error("ScanMailProc", e);

            }
        }
        protected void ScanConsortiaProc(object sender)
        {
            try
            {
                int startTick = Environment.TickCount;
                CenterServer.log.Debug("Saving Record...");
                CenterServer.log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
                ThreadPriority oldprio = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                string noticeID = "";
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    db.ScanConsortia(ref noticeID);
                }
                string[] noticeIDs = noticeID.Split(new char[]
                {
                    ','
                });
                string[] array = noticeIDs;
                for (int i = 0; i < array.Length; i++)
                {
                    string s = array[i];
                    if (!string.IsNullOrEmpty(s))
                    {
                        GSPacketIn pkg = new GSPacketIn(128);
                        pkg.WriteByte(2);
                        pkg.WriteInt(int.Parse(s));
                        this.SendToALL(pkg);
                    }
                }
                Thread.CurrentThread.Priority = oldprio;
                startTick = Environment.TickCount - startTick;
                CenterServer.log.Debug("Scan Consortia complete!");
                if (startTick > 120000)
                {
                    CenterServer.log.WarnFormat("Scan all Consortia in {0} ms", startTick);
                }
            }
            catch (Exception e)
            {

                CenterServer.log.Error("ScanConsortiaProc", e);

            }
        }
        public override void Stop()
        {
            try
            {
                this.IsRunning = -1;
                this.DisposeGlobalTimers();
                this.SaveTimerProc(null);
                CenterService.Stop();
                base.Stop();

            }
            catch (Exception ex)
            {
                CenterServer.log.Error("Center service stopp error:", ex);
            }
            CenterServer.log.Warn("Center Server Stopped!");
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
        public void SendConsortiaDelete(int consortiaID)
        {
            GSPacketIn pkg = new GSPacketIn(128);
            pkg.WriteByte(5);
            pkg.WriteInt(consortiaID);
            this.SendToALL(pkg);
        }
        public void SendSystemNotice(string msg)
        {
            GSPacketIn pkg = new GSPacketIn(10);
            pkg.WriteInt(0);
            pkg.WriteString(msg);
            this.SendToALL(pkg, null);
        }
        public int RateUpdate(int serverId)
        {
            ServerClient[] list = this.GetAllClients();
            int result;
            if (list != null)
            {
                ServerClient[] array = list;
                for (int i = 0; i < array.Length; i++)
                {
                    ServerClient client = array[i];
                    if (client.Info.ID == serverId)
                    {
                        GSPacketIn pkg = new GSPacketIn(177);
                        pkg.WriteInt(serverId);
                        client.SendTCP(pkg);
                        result = 0;
                        return result;
                    }
                }
            }
            result = 1;
            return result;
        }

        public bool ClientsExecuteCmd(string cmdLine)
        {
            bool result;
            try
            {
                LogClient client = new LogClient();
                ServerClient[] list = CenterServer.Instance.GetAllClients();
                if (list != null)
                {
                    ServerClient[] array = list;
                    for (int i = 0; i < array.Length; i++)
                    {
                        ServerClient c = array[i];
                        c.SendCmd(client, cmdLine);
                    }
                }
                result = true;
                return result;
            }
            catch (Exception ex)
            {
                CenterServer.log.Error(ex.Message, ex);
            }
            result = false;
            return result;
        }



        public static bool IsRun => Instance?.IsRunning == 1;
        public static void StartServer()
        {
            if(Instance?.IsRunning>=0)
            {
                return;
            }
            m_instance = new CenterServer(new CenterServerConfig());
            if (Instance.Start() == false)
            {
                Instance.IsRunning = -1;
            }
            return;
        }
        public static void StopServer() => Instance?.Stop();


    }
}