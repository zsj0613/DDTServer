using Game.Logic.Phy.Maths;
using Game.Logic.Phy.Object;
using System;
using System.Drawing;
namespace Game.Logic.Actions
{
	public class GhostMoveAction : BaseAction
	{
		private Point m_target;
		private Player m_player;
		private PointF m_v;
		private bool m_isSend;
		private PointF m_current;
		private int m_steps;
		public GhostMoveAction(Player player, Point target) : base(0, 1000)
		{
			this.m_player = player;
			this.m_target = target;
			this.m_v = new PointF((float)(target.X - this.m_player.X), (float)(target.Y - this.m_player.Y));
			this.m_steps = (int)Math.Floor(this.m_v.Length() / 2.0);
			this.m_current = new PointF((float)player.X, (float)player.Y);
			this.m_v = this.m_v.Normalize(2f);
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!this.m_isSend)
			{
				this.m_isSend = true;
				game.SendPlayerMove(this.m_player, 2, this.m_target.X, this.m_target.Y, (byte)((this.m_v.X > 0f) ? 1 : -1), false, null);
			}
			if (this.m_steps > 0)
			{
				this.m_current.X = this.m_current.X + this.m_v.X;
				this.m_current.Y = this.m_current.Y + this.m_v.Y;
				this.m_steps--;
				this.m_player.SetXY((int)this.m_current.X, (int)this.m_current.Y);
			}
			else
			{
				this.m_player.SetXY(this.m_target.X, this.m_target.Y);
				base.Finish(tick);
			}
		}
	}
}
