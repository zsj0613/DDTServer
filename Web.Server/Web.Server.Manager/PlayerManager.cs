using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;

namespace Web.Server.Manager
{
    public class PlayerManager
    {
        private class PlayerData
        {
            public string Name;
            public string Pass;
            public DateTime Date;
            public int Count;
        }

        private static Dictionary<string, PlayerManager.PlayerData> m_players = new Dictionary<string, PlayerManager.PlayerData>();
        private static object sys_obj = new object();
        private static Timer m_timer;
        private static int m_timeout = 30;
        public static void Setup()
        {
            PlayerManager.m_timer = new Timer(new TimerCallback(PlayerManager.CheckTimerCallback), null, 0, 60000);
        }
        protected static bool CheckTimeOut(DateTime dt)
        {
            return (DateTime.Now - dt).TotalMinutes > (double)PlayerManager.m_timeout;
        }
        private static void CheckTimerCallback(object state)
        {
            object obj;
            Monitor.Enter(obj = PlayerManager.sys_obj);
            try
            {
                List<string> list = new List<string>();
                foreach (PlayerManager.PlayerData p in PlayerManager.m_players.Values)
                {
                    if (PlayerManager.CheckTimeOut(p.Date))
                    {
                        list.Add(p.Name);
                    }
                }
                foreach (string name in list)
                {
                    PlayerManager.m_players.Remove(name);
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
        }



        public static void Add(string name, string pass)
        {
            object obj;
            Monitor.Enter(obj = PlayerManager.sys_obj);
            try
            {
                if (PlayerManager.m_players.ContainsKey(name))
                {
                    PlayerManager.m_players[name].Name = name;
                    PlayerManager.m_players[name].Pass = pass;
                    PlayerManager.m_players[name].Date = DateTime.Now;
                    PlayerManager.m_players[name].Count = 0;
                }
                else
                {
                    PlayerManager.PlayerData data = new PlayerManager.PlayerData();
                    data.Name = name;
                    data.Pass = pass;
                    data.Date = DateTime.Now;
                    PlayerManager.m_players.Add(name, data);
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
        }
        public static bool Login(string name, string pass)
        {
            object obj;
            Monitor.Enter(obj = PlayerManager.sys_obj);
            bool result;
            try
            {
                if (PlayerManager.m_players.ContainsKey(name) && PlayerManager.m_players[name].Pass == pass)
                {
                    PlayerManager.PlayerData p = PlayerManager.m_players[name];
                    if (p.Pass == pass && !PlayerManager.CheckTimeOut(p.Date))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
            return result;
        }
        public static bool Update(string name, string pass)
        {
            object obj;
            Monitor.Enter(obj = PlayerManager.sys_obj);
            bool result;
            try
            {
                if (PlayerManager.m_players.ContainsKey(name))
                {
                    PlayerManager.m_players[name].Pass = pass;
                    PlayerManager.m_players[name].Count++;
                    result = true;
                    return result;
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
            result = false;
            return result;
        }
        public static bool Remove(string name)
        {
            object obj;
            Monitor.Enter(obj = PlayerManager.sys_obj);
            bool result;
            try
            {
                result = PlayerManager.m_players.Remove(name);
            }
            finally
            {
                Monitor.Exit(obj);
            }
            return result;
        }
        public static bool GetByUserIsFirst(string name)
        {
            object obj;
            Monitor.Enter(obj = PlayerManager.sys_obj);
            bool result;
            try
            {
                if (PlayerManager.m_players.ContainsKey(name))
                {
                    result = (PlayerManager.m_players[name].Count == 0);
                    return result;
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
            result = false;
            return result;
        }
    }
}