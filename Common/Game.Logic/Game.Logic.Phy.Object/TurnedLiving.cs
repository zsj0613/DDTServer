using System;
namespace Game.Logic.Phy.Object
{
	public class TurnedLiving : Living
	{
		public int DefaultDelay;
		protected int m_delay;
		private int m_dander;
		public int Delay
		{
			get
			{
				return this.m_delay;
			}
			set
			{
				this.m_delay = value;
			}
		}
		public int Dander
		{
			get
			{
				return this.m_dander;
			}
			set
			{
				this.m_dander = value;
			}
		}
		public TurnedLiving(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction, int maxBeatDis) : base(id, game, team, name, modelId, maxBlood, immunity, direction, maxBeatDis)
		{
		}
		public override void Reset()
		{
			base.Reset();
		}
		public void AddDelay(int value)
		{
			this.m_delay += value;
		}
		public override void PrepareSelfTurn()
		{
			base.PrepareSelfTurn();
		}
		public void AddDander(int value)
		{
			if (value > 0)
			{
				if (base.IsLiving)
				{
					this.SetDander(this.m_dander + value);
				}
			}
		}
		public void SetDander(int value)
		{
			this.m_dander = Math.Min(value, 200);
			if (base.SyncAtTime)
			{
				this.m_game.SendGameUpdateDander(this);
			}
		}
		public virtual void StartGame()
		{
		}
		public virtual void Skip(int spendTime)
		{
			if (base.IsAttacking)
			{
				this.StopAttacking();
				this.m_game.CheckState(0);
			}
		}
	}
}
