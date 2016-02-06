using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Buffer
{
	public class BufferList
	{
		private static LogProvider log => LogProvider.Default;
		private object m_lock;
		protected List<AbstractBuffer> m_buffers;
		protected ArrayList m_clearList;
		protected volatile sbyte m_changesCount;
		private GamePlayer m_player;
		protected ArrayList m_changedBuffers = new ArrayList();
		private int m_changeCount;
		public BufferList(GamePlayer player)
		{
			this.m_player = player;
			this.m_lock = new object();
			this.m_buffers = new List<AbstractBuffer>();
			this.m_clearList = new ArrayList();
		}
		public void LoadFromDatabase(int playerId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					BufferInfo[] infos = db.GetUserBuffer(playerId);
					this.BeginChanges();
					BufferInfo[] array = infos;
					for (int i = 0; i < array.Length; i++)
					{
						BufferInfo info = array[i];
						AbstractBuffer buffer = BufferList.CreateBuffer(info);
						if (buffer != null)
						{
							buffer.Start(this.m_player);
						}
					}
					this.CommitChanges();
				}
				this.Update();
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public void SaveToDatabase()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					foreach (AbstractBuffer buffer in this.m_buffers)
					{
						pb.SaveBuffer(buffer.Info);
					}
					foreach (BufferInfo info in this.m_clearList)
					{
						pb.SaveBuffer(info);
					}
					this.m_clearList.Clear();
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public bool AddBuffer(AbstractBuffer buffer)
		{
			List<AbstractBuffer> buffers;
			Monitor.Enter(buffers = this.m_buffers);
			try
			{
				this.m_buffers.Add(buffer);
			}
			finally
			{
				Monitor.Exit(buffers);
			}
			this.OnBuffersChanged(buffer);
			return true;
		}
		public bool RemoveBuffer(AbstractBuffer buffer)
		{
			List<AbstractBuffer> buffers;
			Monitor.Enter(buffers = this.m_buffers);
			try
			{
				if (this.m_buffers.Remove(buffer))
				{
					this.m_clearList.Add(buffer.Info);
				}
			}
			finally
			{
				Monitor.Exit(buffers);
			}
			this.OnBuffersChanged(buffer);
			return true;
		}
		public void UpdateBuffer(AbstractBuffer buffer)
		{
			this.OnBuffersChanged(buffer);
		}
		protected void OnBuffersChanged(AbstractBuffer buffer)
		{
			if (!this.m_changedBuffers.Contains(buffer))
			{
				this.m_changedBuffers.Add(buffer);
			}
			if (this.m_changeCount <= 0 && this.m_changedBuffers.Count > 0)
			{
				this.UpdateChangedBuffers();
			}
		}
		public void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changeCount);
		}
		public void CommitChanges()
		{
			int changes = Interlocked.Decrement(ref this.m_changeCount);
			if (changes < 0)
			{
				//if (BufferList.log.IsErrorEnabled)
				{
					BufferList.log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref this.m_changeCount, 0);
			}
			if (changes <= 0 && this.m_changedBuffers.Count > 0)
			{
				this.UpdateChangedBuffers();
			}
		}
		public void UpdateChangedBuffers()
		{
			List<BufferInfo> m_changbuffers = new List<BufferInfo>();
			foreach (AbstractBuffer buffer in this.m_changedBuffers)
			{
				m_changbuffers.Add(buffer.Info);
			}
			BufferInfo[] changeBuffers = m_changbuffers.ToArray();
			GSPacketIn pkg = this.m_player.Out.SendUpdateBuffer(this.m_player, changeBuffers);
			if (this.m_player.CurrentRoom != null)
			{
				this.m_player.CurrentRoom.SendToAll(pkg, this.m_player);
			}
			this.m_changedBuffers.Clear();
		}
		public virtual AbstractBuffer GetOfType(Type bufferType)
		{
			List<AbstractBuffer> buffers;
			Monitor.Enter(buffers = this.m_buffers);
			AbstractBuffer result;
			try
			{
				foreach (AbstractBuffer buffer in this.m_buffers)
				{
					if (buffer.GetType().Equals(bufferType))
					{
						result = buffer;
						return result;
					}
				}
			}
			finally
			{
				Monitor.Exit(buffers);
			}
			result = null;
			return result;
		}
		public List<AbstractBuffer> GetAllBuffer()
		{
			List<AbstractBuffer> list = new List<AbstractBuffer>();
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				foreach (AbstractBuffer buffer in this.m_buffers)
				{
					list.Add(buffer);
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
			return list;
		}
		public void Update()
		{
			List<AbstractBuffer> buffers = this.GetAllBuffer();
			foreach (AbstractBuffer buffer in buffers)
			{
				try
				{
					if (!buffer.Check())
					{
						if (this.m_player.CurrentRoom != null)
						{
							if (!this.m_player.CurrentRoom.IsPlaying)
							{
								buffer.Stop();
							}
						}
						else
						{
							buffer.Stop();
						}
					}
				}
				catch (Exception ex)
				{
					BufferList.log.Error(ex);
				}
			}
		}
		public static AbstractBuffer CreateBuffer(ItemTemplateInfo template, int ValidDate)
		{
			return BufferList.CreateBuffer(new BufferInfo
			{
				BeginDate = DateTime.Now,
				ValidDate = ValidDate * 24 * 60,
				Value = template.Property2,
				Type = template.Property1,
				IsExist = true
			});
		}
		public static AbstractBuffer CreateBufferHour(ItemTemplateInfo template, int ValidHour)
		{
			return BufferList.CreateBuffer(new BufferInfo
			{
				BeginDate = DateTime.Now,
				ValidDate = ValidHour * 60,
				Value = template.Property2,
				Type = template.Property1,
				IsExist = true
			});
		}
		public static AbstractBuffer CreateBufferMinute(ItemTemplateInfo template, int ValidMinute)
		{
			return BufferList.CreateBuffer(new BufferInfo
			{
				BeginDate = DateTime.Now,
				ValidDate = ValidMinute,
				Value = template.Property2,
				Type = template.Property1,
				IsExist = true
			});
		}
		public static AbstractBuffer CreateBuffer(BufferInfo info)
		{
			AbstractBuffer buffer = null;
			switch (info.Type)
			{
			case 11:
				buffer = new KickProtectBuffer(info);
				break;
			case 12:
				buffer = new OfferMultipleBuffer(info);
				break;
			case 13:
				buffer = new GPMultipleBuffer(info);
				break;
			case 15:
				buffer = new PropsBuffer(info);
				break;
			}
			return buffer;
		}
	}
}
