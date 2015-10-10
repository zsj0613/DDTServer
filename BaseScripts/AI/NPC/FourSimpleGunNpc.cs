using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class FourSimpleGunNpc : ABrain
    {
        protected Living m_blow = null;
        protected Living attack = null;
        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            SimpleBoss npc = (SimpleBoss)Body;
            m_blow = null;
            foreach (Living p in Game.FindAppointDeGreeNpc(3))
            {
                m_blow = p;
            }

            if (m_blow == null)
            {
                return;
            }
            attack = m_blow;
            NextAttack();
        }

        private void NextAttack()
        {
            if (attack != null)
            {
                if (attack.X > Body.X)
                {
                    Body.ChangeDirection(1, 800);
                }
                else
                {
                    Body.ChangeDirection(-1, 800);
                }

                Body.CurrentDamagePlus = 0.8f;

                int mtX = Game.Random.Next(attack.X - 50, attack.X + 50);

                if (Body.ShootPoint(mtX, attack.Y, 48, 1000, 10000, 1, 2.0f, 2300))
                {
                    Body.PlayMovie("beatA", 1500, 0);
                }
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}
