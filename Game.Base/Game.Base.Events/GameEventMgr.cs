using log4net;
using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
namespace Game.Base.Events
{
	public sealed class GameEventMgr
	{
		private const int TIMEOUT = 3000;
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly HybridDictionary m_GameObjectEventCollections = new HybridDictionary();
		private static readonly ReaderWriterLock m_lock = new ReaderWriterLock();
		private static RoadEventHandlerCollection m_GlobalHandlerCollection = new RoadEventHandlerCollection();
		public static void RegisterGlobalEvents(Assembly asm, Type attribute, RoadEvent e)
		{
			if (asm == null)
			{
				throw new ArgumentNullException("asm", "No assembly given to search for global event handlers!");
			}
			if (attribute == null)
			{
				throw new ArgumentNullException("attribute", "No attribute given!");
			}
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			Type[] types = asm.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass)
				{
					MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
					for (int j = 0; j < methods.Length; j++)
					{
						MethodInfo mInfo = methods[j];
						object[] myAttribs = mInfo.GetCustomAttributes(attribute, false);
						if (myAttribs.Length != 0)
						{
							try
							{
								GameEventMgr.m_GlobalHandlerCollection.AddHandler(e, (RoadEventHandler)Delegate.CreateDelegate(typeof(RoadEventHandler), mInfo));
							}
							catch (Exception ex)
							{
								if (GameEventMgr.log.IsErrorEnabled)
								{
									GameEventMgr.log.Error("Error registering global event. Method: " + type.FullName + "." + mInfo.Name, ex);
								}
							}
						}
					}
				}
			}
		}
		public static void AddHandler(RoadEvent e, RoadEventHandler del)
		{
			GameEventMgr.AddHandler(e, del, false);
		}
		public static void AddHandlerUnique(RoadEvent e, RoadEventHandler del)
		{
			GameEventMgr.AddHandler(e, del, true);
		}
		private static void AddHandler(RoadEvent e, RoadEventHandler del, bool unique)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			if (del == null)
			{
				throw new ArgumentNullException("del", "No event handler given!");
			}
			if (unique)
			{
				GameEventMgr.m_GlobalHandlerCollection.AddHandlerUnique(e, del);
			}
			else
			{
				GameEventMgr.m_GlobalHandlerCollection.AddHandler(e, del);
			}
		}
		public static void AddHandler(object obj, RoadEvent e, RoadEventHandler del)
		{
			GameEventMgr.AddHandler(obj, e, del, false);
		}
		public static void AddHandlerUnique(object obj, RoadEvent e, RoadEventHandler del)
		{
			GameEventMgr.AddHandler(obj, e, del, true);
		}
		private static void AddHandler(object obj, RoadEvent e, RoadEventHandler del, bool unique)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "No object given!");
			}
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			if (del == null)
			{
				throw new ArgumentNullException("del", "No event handler given!");
			}
			if (!e.IsValidFor(obj))
			{
				throw new ArgumentException("Object is not valid for this event type", "obj");
			}
			try
			{
				GameEventMgr.m_lock.AcquireReaderLock(3000);
				try
				{
					RoadEventHandlerCollection col = (RoadEventHandlerCollection)GameEventMgr.m_GameObjectEventCollections[obj];
					if (col == null)
					{
						col = new RoadEventHandlerCollection();
						LockCookie lc = GameEventMgr.m_lock.UpgradeToWriterLock(3000);
						try
						{
							GameEventMgr.m_GameObjectEventCollections[obj] = col;
						}
						finally
						{
							GameEventMgr.m_lock.DowngradeFromWriterLock(ref lc);
						}
					}
					if (unique)
					{
						col.AddHandlerUnique(e, del);
					}
					else
					{
						col.AddHandler(e, del);
					}
				}
				finally
				{
					GameEventMgr.m_lock.ReleaseReaderLock();
				}
			}
			catch (ApplicationException ex)
			{
				if (GameEventMgr.log.IsErrorEnabled)
				{
					GameEventMgr.log.Error("Failed to add local event handler!", ex);
				}
			}
		}
		public static void RemoveHandler(RoadEvent e, RoadEventHandler del)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			if (del == null)
			{
				throw new ArgumentNullException("del", "No event handler given!");
			}
			GameEventMgr.m_GlobalHandlerCollection.RemoveHandler(e, del);
		}
		public static void RemoveHandler(object obj, RoadEvent e, RoadEventHandler del)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "No object given!");
			}
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			if (del == null)
			{
				throw new ArgumentNullException("del", "No event handler given!");
			}
			try
			{
				GameEventMgr.m_lock.AcquireReaderLock(3000);
				try
				{
					RoadEventHandlerCollection col = (RoadEventHandlerCollection)GameEventMgr.m_GameObjectEventCollections[obj];
					if (col != null)
					{
						col.RemoveHandler(e, del);
					}
				}
				finally
				{
					GameEventMgr.m_lock.ReleaseReaderLock();
				}
			}
			catch (ApplicationException ex)
			{
				if (GameEventMgr.log.IsErrorEnabled)
				{
					GameEventMgr.log.Error("Failed to remove local event handler!", ex);
				}
			}
		}
		public static void RemoveAllHandlers(RoadEvent e, bool deep)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			if (deep)
			{
				try
				{
					GameEventMgr.m_lock.AcquireReaderLock(3000);
					try
					{
						foreach (RoadEventHandlerCollection col in GameEventMgr.m_GameObjectEventCollections.Values)
						{
							col.RemoveAllHandlers(e);
						}
					}
					finally
					{
						GameEventMgr.m_lock.ReleaseReaderLock();
					}
				}
				catch (ApplicationException ex)
				{
					if (GameEventMgr.log.IsErrorEnabled)
					{
						GameEventMgr.log.Error("Failed to add local event handlers!", ex);
					}
				}
			}
			GameEventMgr.m_GlobalHandlerCollection.RemoveAllHandlers(e);
		}
		public static void RemoveAllHandlers(object obj, RoadEvent e)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "No object given!");
			}
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			try
			{
				GameEventMgr.m_lock.AcquireReaderLock(3000);
				try
				{
					RoadEventHandlerCollection col = (RoadEventHandlerCollection)GameEventMgr.m_GameObjectEventCollections[obj];
					if (col != null)
					{
						col.RemoveAllHandlers(e);
					}
				}
				finally
				{
					GameEventMgr.m_lock.ReleaseReaderLock();
				}
			}
			catch (ApplicationException ex)
			{
				if (GameEventMgr.log.IsErrorEnabled)
				{
					GameEventMgr.log.Error("Failed to remove local event handlers!", ex);
				}
			}
		}
		public static void RemoveAllHandlers(bool deep)
		{
			if (deep)
			{
				try
				{
					GameEventMgr.m_lock.AcquireWriterLock(3000);
					try
					{
						GameEventMgr.m_GameObjectEventCollections.Clear();
					}
					finally
					{
						GameEventMgr.m_lock.ReleaseWriterLock();
					}
				}
				catch (ApplicationException ex)
				{
					if (GameEventMgr.log.IsErrorEnabled)
					{
						GameEventMgr.log.Error("Failed to remove all local event handlers!", ex);
					}
				}
			}
			GameEventMgr.m_GlobalHandlerCollection.RemoveAllHandlers();
		}
		public static void Notify(RoadEvent e)
		{
			GameEventMgr.Notify(e, null, null);
		}
		public static void Notify(RoadEvent e, object sender)
		{
			GameEventMgr.Notify(e, sender, null);
		}
		public static void Notify(RoadEvent e, EventArgs args)
		{
			GameEventMgr.Notify(e, null, args);
		}
		public static void Notify(RoadEvent e, object sender, EventArgs eArgs)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e", "No event type given!");
			}
			if (sender != null)
			{
				try
				{
					RoadEventHandlerCollection col = null;
					GameEventMgr.m_lock.AcquireReaderLock(3000);
					try
					{
						col = (RoadEventHandlerCollection)GameEventMgr.m_GameObjectEventCollections[sender];
					}
					finally
					{
						GameEventMgr.m_lock.ReleaseReaderLock();
					}
					if (col != null)
					{
						col.Notify(e, sender, eArgs);
					}
				}
				catch (ApplicationException ex)
				{
					if (GameEventMgr.log.IsErrorEnabled)
					{
						GameEventMgr.log.Error("Failed to notify local event handler!", ex);
					}
				}
			}
			GameEventMgr.m_GlobalHandlerCollection.Notify(e, sender, eArgs);
		}
	}
}
