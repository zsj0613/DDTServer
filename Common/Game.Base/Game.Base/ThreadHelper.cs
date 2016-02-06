using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
namespace Game.Base
{
	public class ThreadHelper
	{
		public static string GetThreadStackTrace(Thread thread)
		{
			return ThreadHelper.FormatStackTrace(ThreadHelper.GetThreadStack(thread));
		}
		public static StackTrace GetThreadStack(Thread thread)
		{
			thread.Suspend();
			StackTrace trace = new StackTrace(thread, true);
			thread.Resume();
			return trace;
		}
		public static string FormatStackTrace(StackTrace trace)
		{
			StringBuilder str = new StringBuilder(128);
			str.Append("\n");
			if (trace == null)
			{
				str.Append("(null)");
			}
			else
			{
				for (int i = 0; i < trace.FrameCount; i++)
				{
					StackFrame frame = trace.GetFrame(i);
					Type declType = frame.GetMethod().DeclaringType;
					str.Append("   at ").Append((declType == null) ? "(null)" : declType.FullName).Append('.').Append(frame.GetMethod().Name).Append(" in ").Append(frame.GetFileName()).Append("  line:").Append(frame.GetFileLineNumber()).Append(" col:").Append(frame.GetFileColumnNumber()).Append("\n");
				}
			}
			return str.ToString();
		}
		public static string FormatTime(long seconds)
		{
			StringBuilder str = new StringBuilder(10);
			long minutes = seconds / 60L;
			if (minutes > 0L)
			{
				str.Append(minutes).Append(":").Append((seconds - minutes * 60L).ToString("D2")).Append(" min");
			}
			else
			{
				str.Append(seconds).Append(" sec");
			}
			return str.ToString();
		}
	}
}
