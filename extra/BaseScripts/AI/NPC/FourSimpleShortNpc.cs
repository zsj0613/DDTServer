using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class FourSimpleShortNpc : ABrain
    {

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

            if (m_body.CanSay)
            {
                string msg = GetOneChat(((SimpleNpc)Body));
                int delay = Game.Random.Next(0, 5000);
                m_body.Say(msg, 0, delay);
            }
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }


        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
            Move();
        }

        public void Move()
        {
            int dis = Game.Random.Next(610, 700);
            int ramdis = Game.Random.Next(((SimpleNpc)Body).NpcInfo.MoveMin, ((SimpleNpc)Body).NpcInfo.MoveMax);
            if (dis > 97)
            {
                if (dis > ((SimpleNpc)Body).NpcInfo.MoveMax)
                {
                    dis = ramdis;
                }
                else
                {
                    dis = dis - 90;
                }

                if (Body.X - dis > dis)
                {
                    Body.MoveTo(Body.X - dis, Body.Y, "walk", 0, null);
                }
                else
                {
                    Body.MoveTo(Body.X + dis, Body.Y, "walk", 0, null);
                }
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        #region NPC 小怪说话

        private static Random random = new Random();
        private static string[] BoguChat = new string[] { 
            "想炸城门~？没门！",
            "不会让你们过去~！~",
        };

        private static string[] AntChat = new string[] {
            "东西抢了！人给我！",
            "想跑？",
        };
        public static string GetOneChat(SimpleNpc Body)
        {
            int index = 0;
            string chat = "";

            switch (Body.NpcInfo.ID)
            {
                case 2001:
                case 2002:
                case 2004:
                case 2101:
                case 2102:
                case 2104:
                    index = random.Next(0, AntChat.Length);
                    chat = AntChat[index];
                    break;
                default:
                    index = random.Next(0, BoguChat.Length);
                    chat = BoguChat[index];
                    break;
            }

            return chat;
        }
        #endregion
    }
}
