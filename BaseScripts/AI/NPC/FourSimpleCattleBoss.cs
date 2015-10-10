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
    public class FourSimpleCattleBoss : ABrain
    {
        private List<SimpleWingNpc> fireNpc = new List<SimpleWingNpc>();

        private Player m_attackPlayer = null;

        private int m_attackTurn = 0;

        private int m_giddyCount = 0;

        private int m_damagePlus = 100;

        private bool m_isRunAttack = false;

        private int npcID = 4007;

        #region NPC 说话内容
        private static string[] KillPlayerChat = new string[] { 
            "你就是杯具...！",
            "你还不够格...！",
        };
        private static string[] ShootChat = new string[]{
             "给你一牛鞭~",
        };
        private static string[] SeriesShootChat = new string[]{
             "你乘得起几下？",
        };
        private static string[] KillAttackChat = new string[]{
            "敢在老牛屁股上打苍蝇？"
        };
        private static string[] AllAttackChat = new string[] { 
            "全部给我去死~~！",
        };
        private static string[] CallChat = new string[]{
            "幽冥火焰！~"
        };
        private static string[] AbsorbChat = new string[]{
             "给我力量吧！吸星大法！！！",
        };
        private static string[] AbsorbOKChat = new string[]{
             "哈哈,我感觉我的力量增强了~~！",
        };
        private static string[] AbsorbERRChat = new string[]{
             "我草！没吸收到？杯具了杯具了...",
        };
        private static string[] JumpAttackChat = new string[]{
             "看我的泰山压顶！...",
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
            Body.Direction = Game.FindlivingbyDir(Body);
            bool result = false;
            int maxdis = 0;

            if (m_giddyCount != 0)
            {
                m_giddyCount--;
                return;
            }

            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > Body.X - 200 && player.X < Body.X + 200)
                {
                    int dis = (int)Body.Distance(player.X, player.Y);
                    if (dis > maxdis)
                    {
                        maxdis = dis;
                    }
                    result = true;
                }
            }

            if (result)
            {
                KillAttack(Body.X - 200, Body.X + 200);
                return;
  
            }

            if (m_attackTurn == 0)
            {
                Summon();
                m_attackTurn++;
                return;
            }

            if (m_attackTurn == 1)
            {
                NextAttack();
                m_attackTurn++;
                return;
            }

            if (m_attackTurn == 2)
            {
                AllAttack();
                m_attackTurn++;
                return;
            }

            if (m_attackTurn == 3)
            {
                Absorb();
                m_attackTurn++;
                return;
            }

            if (m_attackTurn == 4)
            {
                JumpAttack();
                if (m_isRunAttack)
                    m_attackTurn++;
                else
                    m_attackTurn = 0;
                return;
            }

            if (m_attackTurn == 5)
            {
                BedAttack();
                m_attackTurn = 0;
                return;
            }
        }

        private void BedAttack()
        {
            int index = Game.Random.Next(0, 3);
            if (index == 0)
            {
                NextAttack();
                return;
            }
            if (index == 1)
            {
                AllAttack();
                return;
            }
            if (index == 2)
            {
                JumpAttack();
                return;
            }
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
            if (m_isRunAttack)
            {
                int delay = GetMinDelay();
                ((SimpleBoss)Body).Delay = delay - 1;
                m_isRunAttack = false;
            }
        }

        public void Falling()
        {
            Body.StartFalling(false);
            ChangeDir();
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

        private void Absorb()
        {
            int index = Game.Random.Next(0, AbsorbChat.Length);
            Body.Say(AbsorbChat[index], 1, 0);
            Body.PlayMovie("beatB", 0, 0);
            Body.CallFuction(new LivingCallBack(AbsorbResult), 2500);
        }

        private void AbsorbResult()
        {
            int npcCount = 0;
            foreach (Living p in Game.FindAppointDeGreeNpc(1))
            {
                p.Die();
                Game.RemoveLiving(p.Id);
                npcCount++;
            }
            if (npcCount == 0)
            {
                int index = Game.Random.Next(0, AbsorbERRChat.Length);
                Body.Say(AbsorbERRChat[index], 1, 1000);
                m_giddyCount = 2;
            }
            else
            {
                int index = Game.Random.Next(0, AbsorbOKChat.Length);
                Body.Say(AbsorbOKChat[index], 1, 1000);
                Body.AddBlood(npcCount * 3000 + npcCount * 1000);
                m_damagePlus += m_damagePlus / 2;
                //Body.ChangeAttack(Body.NpcInfo.Attack / 2);
                if (npcCount == 3)
                {
                    m_isRunAttack = true;
                }
            }
        }

        private void Summon()
        {
            int index = Game.Random.Next(0, CallChat.Length);
            Body.Say(CallChat[index], 1, 0);
            Body.PlayMovie("callB", 0, 1500, new LivingCallBack(CreateChild));
        }

        private void CreateChild()
        {
            int x = 0;
            int y = 0;
            x = Game.Random.Next(200, 1350);
            y = Game.Random.Next(200, 450);
            SimpleWingNpc npc1 = ((PVEGame)Game).CreateWingNpc(npcID, x, y, 1);
            npc1.Degree = 1;
            fireNpc.Add(npc1);
            x = Game.Random.Next(200, 1350);
            y = Game.Random.Next(200, 450);
            SimpleWingNpc npc2 = ((PVEGame)Game).CreateWingNpc(npcID, x, y, 1);
            npc2.Degree = 1;
            fireNpc.Add(npc2);
            x = Game.Random.Next(200, 1350);
            y = Game.Random.Next(200, 450);
            SimpleWingNpc npc3 = ((PVEGame)Game).CreateWingNpc(npcID, x, y, 1);
            npc3.Degree = 1;
            fireNpc.Add(npc3);
            Game.WaitTime(1000);
        }

        private void NextAttack()
        {
            int index = Game.Random.Next(Body.X - 100, Body.X + 100);
            Body.MoveTo(index, Body.Y, "walk", 2000, new LivingCallBack(SeriesAttack));
        }

        private void AllAttack()
        {
            Body.CurrentDamagePlus = 0.8f;
            int index = Game.Random.Next(0, AllAttackChat.Length);
            Body.Say(AllAttackChat[index], 1, 0);
            Body.PlayMovie("beatC", 2000, 0);
            Body.RangeAttacking(-1, Game.Map.Info.ForegroundWidth + 1, "cry", 4000, null);
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving)
                {
                    index = Game.Random.Next(0, 3);
                    if( index == 0 )
                        player.AddEffect(new ContinueReduceBloodEffect( 500, 2, null), 4000);     //灼烧
                    if (index == 1)
                        player.AddEffect(new LockDirectionEquipEffect(2,100), 4000);              //枷锁
                    if (index == 2)
                        player.AddEffect(new ReduceStrengthEffect(2), 4000);                      //疲劳
                }
            }
        }

        private void KillAttack(int fx, int tx)
        {
            Body.CurrentDamagePlus = 10;
            int index = Game.Random.Next(0, KillAttackChat.Length);
            Body.Say(KillAttackChat[index], 1, 1000);
            Body.PlayMovie("beatC", 3000, 0);
            Game.AddAction(new PlaySoundAction("078", 0));
            Body.RangeAttacking(fx, tx, "cry", 4000, null);
        }

        private void SeriesAttack()
        {
            Player target = Game.FindNearestPlayer(Body.X,Body.Y);
            int index = Game.Random.Next(0, SeriesShootChat.Length);
            Body.Say(SeriesShootChat[index], 1, 0);
            if (target.IsLiving)
            {
                if (target.X > Body.X)
                {
                    Body.ChangeDirection(1, 800);
                }
                else
                {
                    Body.ChangeDirection(-1, 800);
                }

                Body.PlayMovie("beatB", 2300, 0);
                Body.PlayMovie("beatB", 4100, 0);
                Body.PlayMovie("beatB", 5900, 0);
                Body.Beat(target, "", 7000);
                Body.Beat(target, "", 7500);
                Body.Beat(target, "", 8000);
                Game.AddAction(new FocusAction(target.X, target.Y, 0, 6500, 1000));
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
            foreach (Living p in Game.FindAppointDeGreeNpc(1))
            {
                p.Die();
                Game.RemoveLiving(p.Id);
            }
        }

        public override void OnShootedSay(int delay)
        {
        }
    }
}
