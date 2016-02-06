using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Game.Server.Managers
{
    public class WorldBossMgr
    {
        public static int Blood
        {
            get
            {
                return m_Blood;
            }
        }

        private static int m_Blood;


        private static Dictionary<int, int> Damages = new Dictionary<int, int>();
        private static int KillerID;

        public static LogProvider log => LogProvider.Default;
        private static Timer m_TenTimer = null;
        private static Timer m_FourteenTimer = null;
        private static Timer m_TwentyTimer = null;
        private static Timer m_CheckTimer = null;
        private static bool m_CanJoin = false;
        public static bool CanJoin
        {
            get
            {
                return m_CanJoin;
            }
        }

        public static bool Init()
        {
            InitTimer();
            return true;
        }
        private static void InitTimer()
        {
            InitBlood();
            m_CanJoin = true; //just for test;
            var Twenty = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0, 0, DateTimeKind.Local);
            var Fourteen = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0, 0, DateTimeKind.Local);
            var Ten = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0, 0, DateTimeKind.Local);
            var Zero = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 0, 0, 0, 0, DateTimeKind.Local);

            var span = new TimeSpan(1, 0, 0, 0, 0);

            m_TenTimer = new Timer(new TimerCallback(OnTime), null, Ten > DateTime.Now ? Ten - DateTime.Now : Zero - DateTime.Now + new TimeSpan(10, 0, 0), span);
            m_FourteenTimer = new Timer(new TimerCallback(OnTime), null, Fourteen > DateTime.Now ? Fourteen - DateTime.Now : Zero - DateTime.Now + new TimeSpan(14, 0, 0), span);
            m_TwentyTimer = new Timer(new TimerCallback(OnTime), null, Twenty > DateTime.Now ? Twenty - DateTime.Now : Zero - DateTime.Now + new TimeSpan(20, 0, 0), span);


            GC.KeepAlive(m_TenTimer);
            GC.KeepAlive(m_FourteenTimer);
            GC.KeepAlive(m_TwentyTimer);
        }
        private static void OnTime(object state)
        {
            m_CheckTimer = new Timer(new TimerCallback(OnOverTime), null, 0, 60 * 60 * 1000);
            WorldBossMgr.log.Warn(DateTime.Now.Hour.ToString() + "时世界Boss开始");
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (var player in players)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, "世界Boss已经开启，点击主界面的世界Boss参加活动！");
            }
            m_CanJoin = true;

        }

        private static void OnOverTime(object state)
        {
            WorldBossMgr.log.Warn(DateTime.Now.Hour.ToString() + "时世界Boss超时");
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (var player in players)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, "超过1个小时世界Boss未能击杀，活动已结束，开始结算奖励！");
            }
            OnOver();
        }
        private static void OnOver()
        {
            m_CheckTimer = null;
            //结算奖励逻辑。。待添加

            Damages = new Dictionary<int, int>();
            KillerID = 0;
            m_CanJoin = false;
            InitBlood();
        }


        private static void InitBlood()
        {
            m_Blood = new ProduceBussiness().GetAllNPCInfo().ToList().Find((b) =>  b.ID == 70002 ).Blood;
        }


        static object @lock;

        public static void TakeDamage(int damage,int PlayerID)
        {
            Monitor.Enter(@lock);
            {
                if (m_Blood > 0)
                {
                    m_Blood -= damage;
                    Damages[PlayerID] += damage;
                }
                if (m_Blood <= 0)
                {
                    KillerID = PlayerID;
                    OnKilled();
                }
            }
            Monitor.Exit(@lock);
        }


        private static void OnKilled()
        {
            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (var player in players)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, $"世界Boss已被{new PlayerBussiness().GetUserSingleByUserID(KillerID).NickName}击杀，开始结算奖励！");
            }
            OnOver();
        }

    }
}
