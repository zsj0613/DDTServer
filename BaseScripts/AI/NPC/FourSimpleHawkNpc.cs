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
    public class FourSimpleHawkNpc : ABrain
    {
        private int m_attackTurn = 0;

        private int m_tagTurn = 0;

        private static string[] ShootChat = new string[]{
             "吐你一口痰~！",
        };
        private static string[] KillShootChat = new string[]{
             "看我必杀！飞鹰展翅~！",
        };

        private static string[] TagChat = new string[]{
             "听说你很喜欢被爆菊！我给你一个机会~~~",
        };

        private static string[] CancelTagChat = new string[]{
             "累死了,这招太消耗精力了。休息下....",
        };

        public override void OnBeginSelfTurn()
        {
        }

        public override void OnBeginNewTurn()
        {
        }

        public override void OnCreated()
        {
        }

        public override void OnStartAttacking()
        {
            foreach (Living p in Game.FindAppointDeGreeNpc(2))
            {
                if (!p.IsLiving)
                {
                    NextAttack( false );
                    return;
                }
            }

            if (m_attackTurn == 0)
            {
                NextAttack( true );
                m_attackTurn++;
                return;
            }
            if (m_attackTurn == 1)
            {
                LockPlayer();
                m_attackTurn++;
                return;
            }
            if (m_attackTurn == 2)
            {
                if ( LoafMove() )
                    m_attackTurn++;
                return;
            }
            if (m_attackTurn == 3)
            {
                CancelLock();
                m_attackTurn = 0;
                return;
            }
        }

        private void CancelLock()
        {
            int x = 0;
            int y = 0;
            x = Game.Random.Next(300, 1350);
            y = Game.Random.Next(300, 450);
            int index = Game.Random.Next(0, CancelTagChat.Length);
            Body.Say(CancelTagChat[index], 1, 1000);
            Body.PlayMovie("beatA", 2500, 2000);
            ((PVEGame)Body.Game).AddAction(new LivingBoltMoveAction(Body, x, y, "", 0, 0));
            Body.CallFuction(new LivingCallBack(StopTag), 3000);
        }

        private void StopTag()
        {
            foreach (Living p in Game.FindAppointDeGreeNpc(3))
            {
                p.Degree = 0;
                p.EffectList.StopEffect(typeof(ContinueReduceBloodEffect));
            }
        }


        private bool LoafMove()
        {
            if (m_tagTurn == 3)
            {
                m_tagTurn = 0;
                return true;
            }
            else
                return false;
        }

        private void LockPlayer()
        {
            int x = 0;
            int y = 0;
            x = Game.Random.Next(300, 1350);
            y = Game.Random.Next(300, 450);
            Body.SetRelateDemagemRect(-30, -75, 60, 70);
            int index = Game.Random.Next(0, TagChat.Length);
            Body.Say(TagChat[index], 1, 1000);
            Body.PlayMovie("beatA", 3000, 2000);
            ((PVEGame)Body.Game).AddAction(new LivingBoltMoveAction(Body, x, y, "", 0, 0));
            Body.CallFuction(new LivingCallBack(LockAttack), 3500);
        }
        
        private void LockAttack()
        {
            Player target = Game.FindRandomPlayer();
            if (target != null)
            {
                target.Degree = 3;
                target.AddEffect(new ContinueReduceBloodEffect(0, 100, null), 1000);
                Game.AddAction(new FocusAction(target.X, target.Y, 0, 0, 2000));
            }
        }

        private void NextAttack( bool invincible )
        {
            int x = 0;
            int y = 0;
            x = Game.Random.Next(300, 1350);
            y = Game.Random.Next(300, 450);
            if ( invincible )
                Body.SetRelateDemagemRect(-3000, -7500, 60, 70);
            else
                Body.SetRelateDemagemRect(-30, -75, 60, 70);
            ((PVEGame)Body.Game).AddAction(new LivingBoltMoveAction(Body, x, y, "", 0, 0));
            Body.CallFuction(new LivingCallBack(PersonalAttack), 1000);
        }

        private void PersonalAttack()
        {
            Player target = Game.FindNearestPlayer(Body.X, Body.Y);
            if (target != null)
            {
                int isKill = Game.Random.Next(0, 2);
                int index = Game.Random.Next(0, ShootChat.Length);
                if (isKill == 0)
                {
                    Body.Say(ShootChat[index], 1, 0);
                    AttackPlayer(target, 2300, 1500, false);
                }
                else
                {
                    Body.Say(KillShootChat[index], 1, 0);
                    AttackPlayer(target, 2300, 1500, true);
                }
            }
        }

        private void AttackPlayer(Living player, int ShootDelay, int MovieDelay, bool Range)
        {
            if (player.X > Body.X)
            {
                Body.ChangeDirection(1, 800 + MovieDelay - 1500);
            }
            else
            {
                Body.ChangeDirection(-1, 800 + MovieDelay - 1500);
            }

            int mtX = Game.Random.Next(player.X - 30, player.X + 30);

            if (!Range)
            {
                if (Body.ShootPoint(mtX, player.Y, 48, 1000, 10000, 1, 2, ShootDelay))
                {
                    Body.PlayMovie("beatA", MovieDelay, 0);
                }
            }
            else
            {
                if (Body.ShootPoint(mtX, player.Y, 48, 1000, 10000, 3, 2, ShootDelay))
                {
                    Body.PlayMovie("beatA", MovieDelay, 0);
                }
            }
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
            m_tagTurn++;
        }
    }
}
