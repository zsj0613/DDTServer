using Game.Base;
using Game.Base.Managers;
using Game.Base.Config;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using SqlDataProvider.BaseClass;
using Lsj.Util.Net.Web;
using Lsj.Util.Config;
using System.Net;
using Lsj.Util;
using System.Collections;
using Game.Base.Events;

namespace Web.Server
{
    public class WebServer
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly string Edition = "10000";
        private static WebServer m_instance;
        private Server server;
        
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
            FileInfo logConfig = new FileInfo("Weblogconfig.xml");
            ResourceUtil.ExtractResource(logConfig.Name, logConfig.FullName, Assembly.GetAssembly(typeof(WebServer)), true);
            XmlConfigurator.ConfigureAndWatch(logConfig);
            m_instance = new WebServer();
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WebServer.log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
            if (e.IsTerminating)
            {
                LogManager.Shutdown();
            }
        }
        public bool Start()
        {
            bool result = true;
            try
            {
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
                if (!StartScriptComponents())
                {
                    result = false;
                    WebServer.log.Error("Fail to Load Modules!");
                    return result;
                }
                WebServer.log.Info("Succeed to Load Modules!");
                
                GameEventMgr.Notify(ScriptEvent.Loaded);
                server = new Server(IPAddress.Parse(AppConfig.AppSettings["IP"]),AppConfig.AppSettings["Port"].ConvertToInt(88));
                server.Path = @"web\";
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
                server.Stop();
            }
            catch
            {
            }
        }
    }
}
