using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{
    public class FourSimpleBlowArmsNpc : ABrain
    {
        private Living m_target = null;

        private int m_targetDis = 0;

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            Body.CurrentDamagePlus = 1;
            Body.CurrentShootMinus = 1;
            if (Body.CanSay)
            {
                string msg = GetOneChat();
                int delay = Game.Random.Next(0, 5000);
                Body.Say(msg, 0, delay);
            }
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            int x = int.MaxValue;
            m_target = null;

            if (Body.X >= 900)
            {
                AttackDoor();
                return;
            }

            foreach (Living p in Game.FindAppointDeGreeNpc(2))
            {
                if (Math.Abs(p.X - Body.X) < x && p.X > Body.X - 20 )
                {
                    x = Math.Abs(p.X - Body.X);
                    m_target = p;
                }
            }
            if (m_target == null)
            {
                foreach (Living p in Game.FindAppointDeGreeNpc(1))
                {
                    m_target = p;
                }
            }
            m_targetDis = (int)m_target.Distance(Body.X, Body.Y);
            if (m_targetDis <= 50)
            {
                Body.PlayMovie("beatA", 100, 0);
                Body.RangeAttacking(Body.X - 50, Body.X + 50, "cry", 1500, null);
                Body.Die(1000);
            }
            else
            {
                MoveToPlayer(m_target);
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void AttackDoor()
        {
            int index = Game.Random.Next(0, attackChat.Length);
            Body.Say(attackChat[index], 1, 1000);
            foreach (Living p in Game.FindAppointDeGreeNpc(1))
            {
                Body.MoveTo(p.X - 10, Body.Y, "walk", 2000, new LivingCallBack(BeatDoor));
            } 
        }

        public void MoveToPlayer(Living livingObj)
        {
            int dis = Game.Random.Next(((SimpleBoss)Body).NpcInfo.MoveMin, ((SimpleBoss)Body).NpcInfo.MoveMax);
            if (livingObj.X > Body.X)
            {
                if (Body.X + dis >= livingObj.X)
                {
                    Body.MoveTo(livingObj.X - 10, Body.Y, "walk", 2000, new LivingCallBack(Beat));
                }
                else
                {
                    Body.MoveTo(Body.X + dis, Body.Y, "walk", 2000, new LivingCallBack(Beat));
                }
            }
            else
            {
                if (Body.X - dis <= livingObj.X)
                {
                    Body.MoveTo(livingObj.X + 10, Body.Y, "walk", 2000, new LivingCallBack(Beat));
                }
                else
                {
                    Body.MoveTo(Body.X - dis, Body.Y, "walk", 2000, new LivingCallBack(Beat));
                }
            }
        }

        public void Beat()
        {
            m_targetDis = (int)m_target.Distance(Body.X, Body.Y);
            if (m_targetDis <= 50)
            {
                Body.PlayMovie("beatA", 100, 0);
                Body.RangeAttacking(Body.X - 100, Body.X + 100, "cry", 1500, null);
                Body.Die(1000);
            }
        }

        public void BeatDoor()
        {
            Body.PlayMovie("beatA", 100, 0);
            Body.RangeAttacking(Body.X - 100, Body.X + 100, "cry", 1500, null);
            Body.Die(1000);
        }

        #region NPC 小炸弹人说话

        private static Random random = new Random();
        private static string[] bombNpcChat = new string[] { 
            "我就是董存瑞！",
        };
        private static string[] attackChat = new string[] { 
            "冲锋的时候到啦！兄弟们掩护我！！",
        };
        public static string GetOneChat()
        {
            int index = random.Next(0, bombNpcChat.Length);
            return bombNpcChat[index];
        }
        #endregion
    }
}
