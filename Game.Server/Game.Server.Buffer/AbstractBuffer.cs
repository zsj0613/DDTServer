using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Buffer
{
	public class AbstractBuffer
	{
		protected BufferInfo m_info;
		protected GamePlayer m_player;
		public BufferInfo Info
		{
			get
			{
				return this.m_info;
			}
		}
		public AbstractBuffer(BufferInfo info)
		{
			this.m_info = info;
		}
		public virtual void Start(GamePlayer player)
		{
			this.m_info.UserID = player.PlayerId;
			this.m_info.IsExist = true;
			this.m_player = player;
			this.m_player.BufferList.AddBuffer(this);
		}
		public virtual void Restore(GamePlayer player)
		{
			this.Start(player);
		}
		public virtual void Stop()
		{
			this.m_info.IsExist = false;
			this.m_player.BufferList.RemoveBuffer(this);
			this.m_player = null;
		}
		public bool Check()
		{
			return !(this.m_info.BeginDate.AddMinutes((double)this.m_info.ValidDate) < DateTime.Now);
		}
	}
}
