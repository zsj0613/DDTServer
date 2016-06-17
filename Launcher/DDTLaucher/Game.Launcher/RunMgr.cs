using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using Lsj.Util;
using Center.Server;

namespace Game.Launcher
{
    public class RunMgr : DisposableClass, IDisposable
    {
        public RunMgr()
        {
            UpdateStatus();
            GetIntptr();
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(GetIntptr);
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


        private IntPtr Centerintptr = IntPtr.Zero;
        private IntPtr Fightintptr = IntPtr.Zero;
        private IntPtr Gameintptr = IntPtr.Zero;
        private IntPtr Webintptr = IntPtr.Zero;


        public bool UpdateStatus()
        {
            if (CenterServer.IsRun)
                m_CenterStatus = true;
            else
                m_CenterStatus = false;

            if (Process.GetProcessesByName("Fight").Length > 0)
                m_FightStatus = true;
            else
                m_FightStatus = false;

            if (Process.GetProcessesByName("Game").Length > 0)
                m_GameStatus = true;
            else
                m_GameStatus = false;
            if (Process.GetProcessesByName("Web").Length > 0)
                m_WebStatus = true;
            else
                m_WebStatus = false;
            return true;
        }

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private void GetIntptr(object source, ElapsedEventArgs e)
        {
            UpdateStatus();
            GetIntptr();
        }

        private void GetIntptr()
        {
            Centerintptr = FindWindow("ConsoleWindowClass", "DDTank Center Service");
            Fightintptr = FindWindow("ConsoleWindowClass", "DDTank Fighting Service");
            Gameintptr = FindWindow("ConsoleWindowClass", "DDTank Game Service");
            Webintptr = FindWindow("ConsoleWindowClass", "DDTank Web Service");
        }

        public bool StartCenter()
        {
            return true;
        }
        public bool StopCenter()
        {
            if (Centerintptr != IntPtr.Zero)
            {
                ShowWindow(Centerintptr, 1);
                Process.GetProcessesByName("Center")[0].CloseMainWindow();
                return true;
            }
            else
            {
                Process.GetProcessesByName("Center")[0].Close();
                return true;
            }
        }
        public bool StartFight()
        {
            return true;
        }
        public bool StopFight()
        {
            if (Fightintptr != IntPtr.Zero)
            {
                ShowWindow(Fightintptr, 1);
                Process.GetProcessesByName("Fight")[0].CloseMainWindow();
                return true;
            }
            else
            {
                Process.GetProcessesByName("Fight")[0].Close();
                return true;
            }
        }
        public bool StartGame()
        {          
                return true;
        }
        public bool StopGame()
        {
            if (Gameintptr != IntPtr.Zero)
            {
                ShowWindow(Gameintptr, 1);
                Process.GetProcessesByName("Game")[0].CloseMainWindow();
                return true;
            }
            else
            {
                Process.GetProcessesByName("Game")[0].Close();
                return true;
            }
        }
        public bool StartWeb()
        {
         
                return true;
        
        }
        public bool StopWeb()
        {
            if (Webintptr != IntPtr.Zero)
            {
                ShowWindow(Webintptr, 1);
                Process.GetProcessesByName("Web")[0].CloseMainWindow();
                return true;
            }
            else
            {
                Process.GetProcessesByName("Web")[0].Close();
                return true;
            }
        }
        public void ChangeSlient(bool IsSlient)
        {
            if (Centerintptr != IntPtr.Zero)
                ShowWindow(Centerintptr, (IsSlient ? (uint)0 : (uint)1));
            if (Fightintptr != IntPtr.Zero)
                ShowWindow(Fightintptr, (IsSlient ? (uint)0 : (uint)1));
            if (Gameintptr != IntPtr.Zero)
                ShowWindow(Gameintptr, (IsSlient ? (uint)0 : (uint)1));
            if (Webintptr != IntPtr.Zero)
                ShowWindow(Webintptr, (IsSlient ? (uint)0 : (uint)1));
        }
        protected override void CleanUpManagedResources()
        {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(Centerintptr);
            Centerintptr = IntPtr.Zero;
            System.Runtime.InteropServices.Marshal.FreeHGlobal(Fightintptr);
            Fightintptr = IntPtr.Zero;
            System.Runtime.InteropServices.Marshal.FreeHGlobal(Fightintptr);
            Fightintptr = IntPtr.Zero;
            System.Runtime.InteropServices.Marshal.FreeHGlobal(Webintptr);
            Webintptr = IntPtr.Zero;
            base.CleanUpManagedResources();
        }
    }
}
