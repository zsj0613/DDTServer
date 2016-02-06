
using Lsj.Util.Logs;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
namespace Game.Base.Events
{
	public class RoadEventHandlerCollection
	{
		protected const int TIMEOUT = 3000;

		protected readonly ReaderWriterLock m_lock = null;
		protected readonly HybridDictionary m_events = null;
		public RoadEventHandlerCollection()
		{
			this.m_lock = new ReaderWriterLock();
			this.m_events = new HybridDictionary();
		}
		public void AddHandler(RoadEvent e, RoadEventHandler del)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					WeakMulticastDelegate deleg = (WeakMulticastDelegate)this.m_events[e];
					if (deleg == null)
					{
						this.m_events[e] = new WeakMulticastDelegate(del);
					}
					else
					{
						this.m_events[e] = WeakMulticastDelegate.Combine(deleg, del);
					}
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException ex)
			{
                LogProvider.Default.Error("Failed to add event handler!", ex);
			}
		}
		public void AddHandlerUnique(RoadEvent e, RoadEventHandler del)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					WeakMulticastDelegate deleg = (WeakMulticastDelegate)this.m_events[e];
					if (deleg == null)
					{
						this.m_events[e] = new WeakMulticastDelegate(del);
					}
					else
					{
						this.m_events[e] = WeakMulticastDelegate.CombineUnique(deleg, del);
					}
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException ex)
			{
                LogProvider.Default.Error("Failed to add event handler!", ex);
			}
		}
		public void RemoveHandler(RoadEvent e, RoadEventHandler del)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					WeakMulticastDelegate deleg = (WeakMulticastDelegate)this.m_events[e];
					if (deleg != null)
					{
						deleg = WeakMulticastDelegate.Remove(deleg, del);
						if (deleg == null)
						{
							this.m_events.Remove(e);
						}
						else
						{
							this.m_events[e] = deleg;
						}
					}
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException ex)
			{
                LogProvider.Default.Error("Failed to remove event handler!", ex);
			}
		}
		public void RemoveAllHandlers(RoadEvent e)
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					this.m_events.Remove(e);
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException ex)
			{
                LogProvider.Default.Error("Failed to remove event handlers!", ex);
			}
		}
		public void RemoveAllHandlers()
		{
			try
			{
				this.m_lock.AcquireWriterLock(3000);
				try
				{
					this.m_events.Clear();
				}
				finally
				{
					this.m_lock.ReleaseWriterLock();
				}
			}
			catch (ApplicationException ex)
			{
                LogProvider.Default.Error("Failed to remove all event handlers!", ex);
			}
		}
		public void Notify(RoadEvent e)
		{
			this.Notify(e, null, null);
		}
		public void Notify(RoadEvent e, object sender)
		{
			this.Notify(e, sender, null);
		}
		public void Notify(RoadEvent e, EventArgs args)
		{
			this.Notify(e, null, args);
		}
		public void Notify(RoadEvent e, object sender, EventArgs eArgs)
		{
			try
			{
				this.m_lock.AcquireReaderLock(3000);
				WeakMulticastDelegate eventDelegate;
				try
				{
					eventDelegate = (WeakMulticastDelegate)this.m_events[e];
				}
				finally
				{
					this.m_lock.ReleaseReaderLock();
				}
				if (eventDelegate != null)
				{
					eventDelegate.InvokeSafe(new object[]
					{
						e,
						sender,
						eArgs
					});
				}
			}
			catch (ApplicationException ex)
			{
                LogProvider.Default.Error("Failed to notify event handler!", ex);
			}
		}
	}
}
