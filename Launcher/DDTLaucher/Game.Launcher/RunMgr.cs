using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using Lsj.Util;
using Center.Server;
using Web.Server.Manager;
using Fighting.Server;
using Game.Server;
using Web.Server;
using System.Threading;

namespace Game.Launcher
{
    public class RunMgr : DisposableClass, IDisposable,IRunMgr
    {
        public RunMgr()
        {
            UpdateStatus();
            var aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(UpdateStatus);
            aTimer.Interval = 1 * 1000;    //1s
            aTimer.Enabled = true;
        }


        private bool m_CenterStatus = false;
        private bool m_FightStatus = false;
        private bool m_GameStatus = false;
        private bool m_WebStatus = false;

        public bool CenterStatus
        {
            get
            {
                return m_CenterStatus;
            }
        }
        public bool FightStatus
        {
            get
            {
                return m_FightStatus;
            }
        }
        public bool GameStatus
        {
            get
            {
                return m_GameStatus;
            }
        }
        public bool WebStatus
        {
            get
            {
                return m_WebStatus;
            }
        }
        public bool IsAllRun
        {
            get
            {
                return m_CenterStatus && m_FightStatus && m_GameStatus && m_WebStatus;
            }
        }



        public bool UpdateStatus()
        {
            if (CenterServer.IsRun)
                m_CenterStatus = true;
            else
                m_CenterStatus = false;

            if (FightServer.IsRun)
                m_FightStatus = true;
            else
                m_FightStatus = false;

            if (GameServer.IsRun)
                m_GameStatus = true;
            else
                m_GameStatus = false;
            if (WebServer.IsRun)
                m_WebStatus = true;
            else
                m_WebStatus = false;
            return true;
        }

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private void UpdateStatus(object source, ElapsedEventArgs e)
        {
            UpdateStatus();
        }

        public bool StartCenter()
        {
            if (!CenterStatus)
            {
                new Thread(()=> { CenterServer.StartServer(); }).Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool StopCenter()
        {
            if (CenterStatus)
            {
                CenterServer.StopServer();
                return true;
            }
            else
            {       
                return false;
            }
        }
        public bool StartFight()
        {
            if (!FightStatus)
            {
                new Thread(() => { FightServer.StartServer(); }).Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool StopFight()
        {
            if (FightStatus)
            {
                FightServer.StopServer();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool StartGame()
        {
            if (!GameStatus)
            {
                new Thread(() => { GameServer.StartServer(); }).Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool StopGame()
        {
            if (GameStatus)
            {
                GameServer.StopServer();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool StartWeb()
        {

            if (!WebStatus)
            {
                new Thread(() => { WebServer.StartServer(); }).Start();
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool StopWeb()
        {
            if (WebStatus)
            {
                WebServer.StopServer();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
