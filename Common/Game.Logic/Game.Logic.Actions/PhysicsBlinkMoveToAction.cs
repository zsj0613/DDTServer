using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic.Actions
{
	public class PhysicsBlinkMoveToAction : BaseAction
	{
		private Physics m_Physics;
		private List<Point> m_path;
		private string m_action;
		private bool m_isSent;
		private int m_index;
		public PhysicsBlinkMoveToAction(Physics living, List<Point> path, string action, int delay) : base(delay, 0)
		{
			this.m_Physics = living;
			this.m_path = path;
			this.m_action = action;
			this.m_isSent = false;
			this.m_index = 0;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			if (!this.m_isSent)
			{
				this.m_isSent = true;
			}
			this.m_index++;
			if (this.m_index >= this.m_path.Count)
			{
				this.m_Physics.SetXY(this.m_path[this.m_index - 1].X, this.m_path[this.m_index - 1].Y);
				base.Finish(tick);
			}
		}
	}
}
