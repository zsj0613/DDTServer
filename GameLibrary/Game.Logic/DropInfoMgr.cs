using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class DropInfoMgr
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected static ReaderWriterLock m_lock = new ReaderWriterLock();
		public static Dictionary<int, MacroDropInfo> DropInfo;
		public static bool CanDrop(int templateId)
		{
			bool result;
			if (DropInfoMgr.DropInfo == null)
			{
				result = true;
			}
			else
			{
				DropInfoMgr.m_lock.AcquireWriterLock(-1);
				try
				{
					if (DropInfoMgr.DropInfo.ContainsKey(templateId))
					{
						MacroDropInfo mdi = DropInfoMgr.DropInfo[templateId];
						if (mdi.DropCount < mdi.MaxDropCount || mdi.SelfDropCount >= mdi.DropCount)
						{
							mdi.SelfDropCount++;
							mdi.DropCount++;
							result = true;
							return result;
						}
						result = false;
						return result;
					}
				}
				catch (Exception e)
				{
					if (DropInfoMgr.log.IsErrorEnabled)
					{
						DropInfoMgr.log.Error("DropInfoMgr CanDrop", e);
					}
				}
				finally
				{
					DropInfoMgr.m_lock.ReleaseWriterLock();
				}
				result = true;
			}
			return result;
		}
	}
}
