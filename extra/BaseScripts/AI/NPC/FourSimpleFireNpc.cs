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
    public class FourSimpleFireNpc : ABrain
    {
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
            int x = 0;
            int y = 0;
            int dir = 0;
            x = Game.Random.Next(300, 1350);
            y = Game.Random.Next(300, 450);
            dir = Game.Random.Next(0, 2);
            if (dir == 0)
                Body.ChangeDirection(-1, 100);
            else
                Body.ChangeDirection(1, 100);
            ((PVEGame)Body.Game).AddAction(new LivingBoltMoveAction(Body, x, y, "", 0, 0));
            Game.WaitTime(1000);
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
