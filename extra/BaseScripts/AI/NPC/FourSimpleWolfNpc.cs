using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic.Effects;
using Game.Logic.Actions;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
    public class FourSimpleWolfNpc : ABrain
    {
        private int m_attackTurn = 0;

        private int m_area = 300;

        private Player m_lockPlayer = null;

        private static string[] BlindlyShootChat = new string[]{
             "妈妈的，人在哪里？？？~！",
        };
        private static string[] KillShootChat = new string[]{
             "哈哈~看到你了！摧花爆菊手！！",
        };

        private static string[] KillAttackChat = new string[]{
            "闻到人的味道了！哇呀呀呀看招！！"
        };

        public override void OnBeginSelfTurn()
        {
        }

        public override void OnBeginNewTurn()
        {
            Body.CurrentDamagePlus = 1;
        }

        public override void OnCreated()
        {
        }

        public override void OnStartAttacking()
        {
            bool result = false;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > Body.X - 100 && player.X < Body.X + 100)
                {
                    result = true;
                }
            }

            if (result)
            {
                KillAttack(Body.X - 100, Body.X + 100);
                return;
            }

            if (m_attackTurn == 0)
            {
                NextAttack();
                return;
            }
        }

        private void NextAttack()
        {
            int x = 0;
            bool IsLock = false;
            m_lockPlayer = null;
            x = Game.Random.Next(Body.X - 200, Body.X + 200);
            foreach (Living p in Game.FindAppointDeGreeNpc(3))
            {
                IsLock = true;
                m_lockPlayer = (Player)p;
            }
            if (IsLock)
            {
                Body.SetRelateDemagemRect(-1160, -1110, 157, 119);
                Body.CallFuction(new LivingCallBack(LockAttack), 0);
            }
            else
            {
                Body.SetRelateDemagemRect(-116, -111, 157, 119);
                Body.MoveTo(x, Body.Y, "walk", 0, new LivingCallBack(BlindlyAttack));
            }
        }

        private void LockAttack()
        {
            int index = Game.Random.Next(0, KillShootChat.Length);
            Body.Say(KillShootChat[index], 1, 0);
            ChangeDir();
            if (Math.Abs(m_lockPlayer.X - Body.X) < m_area)
            {
                MoveToPlayer();
                return;
            }
            int x = Game.Random.Next(m_lockPlayer.X - 100, m_lockPlayer.X + 100);
            Game.AddAction(new LivingBoltMoveAction(Body, x, m_lockPlayer.Y - 50, "", 3000, 0));
            Body.CallFuction(new LivingCallBack(Falling), 3100);
            Body.CallFuction(new LivingCallBack(AllAttack), 4000);
            Game.AddAction(new FocusAction(m_lockPlayer.X, m_lockPlayer.Y, 0, 2500, 0));
        }

        public void Falling()
        {
            Body.StartFalling(false);
            ChangeDir();
        }

        public void ChangeDir()
        {
            if (m_lockPlayer.X > Body.X)
            {
                Body.ChangeDirection(1, 500);
            }
            else
            {
                Body.ChangeDirection(-1, 500);
            }
        }

        public void MoveToPlayer()
        {
            int dis = (int)m_lockPlayer.Distance(Body.X, Body.Y);
            if (dis > 60)
            {
                dis -= 60;

                if (m_lockPlayer.X <= Body.X)
                {
                    Body.MoveTo(Body.X - dis, Body.Y, "walk", 800, new LivingCallBack(StartAttack));
                }
                else
                {
                    Body.MoveTo(Body.X + dis, Body.Y, "walk", 800, new LivingCallBack(StartAttack));
                }
            }
            else
                StartAttack();
        }

        private void StartAttack()
        {
            Body.PlayMovie("beatA", 0, 0);
            Body.Beat(m_lockPlayer, "beatA", 800);
        }

        private void AllAttack()
        {
            Body.CurrentDamagePlus = 0.8f;
            Body.PlayMovie("beatA", 0, 2000);
            Body.RangeAttacking(Body.X - 200, Body.X + 200, "beatA", 800, null);
        }

        private void KillAttack(int fx, int tx)
        {
            Body.CurrentDamagePlus = 10;
            int index = Game.Random.Next(0, KillAttackChat.Length);
            Body.Say(KillAttackChat[index], 1, 1000);
            Body.PlayMovie("beatA", 3000, 0);
            Game.AddAction(new PlaySoundAction("078", 0));
            Body.RangeAttacking(fx, tx, "beatA", 4000, null);
        }

        private void BlindlyAttack()
        {
            ChangeDirection(3);
            int index = Game.Random.Next(0, BlindlyShootChat.Length);
            Body.Say(BlindlyShootChat[index], 1, 0);
            Body.PlayMovie("beatA", 1000, 2000);
        }

        private void ChangeDirection(int count)
        {
            int direction = Body.Direction;
            for (int i = 0; i < count; i++)
            {
                Body.ChangeDirection(-direction, i * 200 + 100);
                Body.ChangeDirection(direction, (i + 1) * 100 + i * 200);
            }
        }

        private void PersonalAttack()
        {

        }

        public override void OnStopAttacking()
        {
        }

        public override void OnKillPlayerSay()
        {
        }

        public override void OnDiedSay()
        {
        }

        public override void OnShootedSay(int delay)
        {
        }
    }
}
