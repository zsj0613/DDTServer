using System;
using System.Diagnostics;
namespace SqlDataProvider.BaseClass
{
	public static class ApplicationLog
	{
		public static void WriteError(string message)
		{
			ApplicationLog.WriteLog(TraceLevel.Error, message);
		}
		private static void WriteLog(TraceLevel level, string messageText)
		{
			try
			{
				EventLogEntryType LogEntryType;
				if (level != TraceLevel.Error)
				{
					LogEntryType = EventLogEntryType.Error;
				}
				else
				{
					LogEntryType = EventLogEntryType.Error;
				}
				string LogName = "Application";
				if (!EventLog.SourceExists(LogName))
				{
					EventLog.CreateEventSource(LogName, "BIZ");
				}
				EventLog eventLog = new EventLog(LogName, ".", LogName);
				eventLog.WriteEntry(messageText, LogEntryType);
			}
			catch
			{
			}
		}
	}
}
