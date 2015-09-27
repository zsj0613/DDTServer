using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.AI
{
	public abstract class ABrain
	{
		protected Living m_body;
		protected BaseGame m_game;
		public Living Body
		{
			get
			{
				return this.m_body;
			}
			set
			{
				this.m_body = value;
			}
		}
		public BaseGame Game
		{
			get
			{
				return this.m_game;
			}
			set
			{
				this.m_game = value;
			}
		}
		public ABrain()
		{
		}
		public virtual void OnCreated()
		{
		}
		public virtual void OnBeginNewTurn()
		{
		}
		public virtual void OnBeginSelfTurn()
		{
		}
		public virtual void OnStartAttacking()
		{
		}
		public virtual void OnStopAttacking()
		{
		}
		public virtual void Dispose()
		{
		}
		public virtual void OnKillPlayerSay()
		{
		}
		public virtual void OnDiedSay()
		{
		}
		public virtual void OnDiedEvent()
		{
		}
		public virtual void OnShootedSay(int delay)
		{
		}
	}
}
