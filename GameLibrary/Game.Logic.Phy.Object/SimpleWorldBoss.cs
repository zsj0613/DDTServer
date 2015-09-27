using Game.Logic.Effects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;

namespace Game.Logic.Phy.Object
{
    public class SimpleWorldBoss : SimpleBoss
    {
        public override int Blood
        {
            get
            {
                return WorldBossMgr.Blood;
            }
        }
        public override void Reset()
        {
            this.m_blood = WorldBossMgr.Blood;
            this.m_isFrost = false;
            this.m_isHide = false;
            this.m_isNoHole = false;
            this.m_isLiving = true;
            this.m_createAction = null;
            this.TurnNum = 0;
            this.TotalHurt = 0;
            this.TotalKill = 0;
            this.TotalShootCount = 0;
            this.TotalHitTargetCount = 0;
            this.FlyingPartical = 0;
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public SimpleWorldBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo, direction, type)
        {
        }
        public override Point StartFalling(bool direct, int delay, int speed)
        {
            return new Point
            {
                X = this.X,
                Y = this.Y
            };
        }
        public override bool IsFrost
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public override int AddBlood(int value,int type)
        {
            SimpleWorldBoss.log.Error("Error On WorldBoss Add Blood");
            return 0;
        }
        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, int type, int delay)
        {
            bool result = false;
            bool result2;
            if (this.Blood > 0)
            {
                if (type == 0)
                {
                    this.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount, delay);
                    this.StartAttacked();
                }
                WorldBossMgr.TakeDamage(damageAmount + criticalAmount,Game.FindRandomPlayer().PlayerDetail.GamePlayerId);
                if (this.m_syncAtTime)
                {
                    {
                        this.m_game.SendGameUpdateHealth(this, 1, damageAmount + criticalAmount);
                    }
                }
                this.OnAfterTakedDamage(source, damageAmount, criticalAmount, delay);
                if (this.Blood <= 0)
                {
                    this.Die();
                }
                source.OnAfterKillingLiving(this, damageAmount, criticalAmount, delay);
                result = true;
            }
            else
            {
                this.Die();
            }
            result2 = result;
            return result2;
        }


        public override void Die()
        {
            if (base.IsLiving)
            {
                this.StopMoving();
                this.m_isLiving = false;
                if (this.IsAttacking)
                {
                    this.StopAttacking();
                }
                this.OnDied();
                this.m_game.CheckState(0);
            }
        }
    }
}
