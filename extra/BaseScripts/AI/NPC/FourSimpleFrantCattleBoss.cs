using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
    public class FourSimpleFrantCattleBoss : ABrain
    {
        private Player m_attackPlayer = null;

        private int m_attackTurn = 0;

        private int m_beginMoveX = 0;

        #region NPC 说话内容
        private static string[] AllBakeChat = new string[]{
             "<span class='red'>气死我了！！我要你们陪葬~~！</span>",
        };
        private static string[] JumpAttackChat = new string[]{
             "<span class='red'>踩死你们！！！</span>",
        };
        private static string[] BumpAttackChat = new string[]{
             "<span class='red'>撞死你们！！！</span>",
        };

        #endregion

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            Body.CurrentDamagePlus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();

            if (m_attackTurn == 0)
            {
                Chat();
                m_attackTurn++;
                return;
            }

            if (m_attackTurn == 1)
            {
                JumpAttack();
                m_attackTurn++;
                return;
            }

            if (m_attackTurn == 2)
            {
                BumpAttack();
                m_attackTurn=1;
                return;
            }
        }

        private void Chat()
        {
            int index = Game.Random.Next(0, AllBakeChat.Length);
            Body.Say(AllBakeChat[index], 1, 1000,3000);
            int delay = GetMinDelay();
            ((SimpleBoss)Body).Delay = delay - 1;
        }

        public int GetMinDelay()
        {
            List<Player> players = Game.GetAllFightPlayers();
            Player RandomPlayer = Game.FindRandomPlayer();

            int minDelay = 0;

            if (RandomPlayer != null)
            {
                minDelay = RandomPlayer.Delay;
            }

            foreach (Player player in players)
            {
                if (player.Delay < minDelay)
                {
                    minDelay = player.Delay;
                }
            }
            return minDelay;
        }

        private void JumpAttack()
        {
            int index = Game.Random.Next(0, JumpAttackChat.Length);
            Body.Say(JumpAttackChat[index], 1, 0);
            m_attackPlayer = Game.FindRandomPlayer();
            ChangeDir();
            int x = Game.Random.Next(m_attackPlayer.X - 100, m_attackPlayer.X + 100);
            Game.AddAction(new FocusAction(m_attackPlayer.X, m_attackPlayer.Y, 0, 2500, 0));
            Game.AddAction(new LivingBoltMoveAction(Body, x, m_attackPlayer.Y - 100, "", 3000, 0));
            Body.CallFuction(new LivingCallBack(Falling), 3100);
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving)
                {
                    Body.Beat(player, "", 3200);
                }
            }
        }

        private void BumpAttack()
        {
            int index = Game.Random.Next(0, BumpAttackChat.Length);
            Body.Say(BumpAttackChat[index], 1, 0);
            m_attackPlayer = Game.FindRandomPlayer();
            m_beginMoveX = Body.X;
            Body.MoveTo(m_attackPlayer.X, m_attackPlayer.Y, "walk", 0, new LivingCallBack(BeginAttack));
        }

        public void Falling()
        {
            Body.StartFalling(false);
            ChangeDir();
        }

        public void ChangeDir()
        {
            if (m_attackPlayer.X > Body.X)
            {
                Body.ChangeDirection(1, 500);
            }
            else
            {
                Body.ChangeDirection(-1, 500);
            }
        }

        private void BeginAttack()
        {
            Body.Beat(m_attackPlayer, "", 0);
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving)
                if (Body.Direction == -1)
                {
                    if (player.IsLiving && player.X > Body.X - 100 && player.X < m_beginMoveX && player.Id != m_attackPlayer.Id)
                    {
                        Body.Beat(player, "", 0);
                    }
                }
                else
                {
                    if (player.IsLiving && player.X < Body.X + 100 && player.X > m_beginMoveX && player.Id != m_attackPlayer.Id)
                    {
                        Body.Beat(player, "", 0);
                    }
                }

            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public override void OnKillPlayerSay()
        {
            base.OnKillPlayerSay();
        }

        public override void OnDiedSay()
        {
        }

        public override void OnDiedEvent()
        {
        }

        public override void OnShootedSay(int delay)
        {
        }
    }
}
