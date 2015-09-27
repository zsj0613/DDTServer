using log4net;
using System;
using System.Reflection;
namespace Game.Base
{
	public class LogClient : BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public LogClient() : base(null, null)
		{
		}
		public override void DisplayMessage(string msg)
		{
			LogClient.log.Info(msg);
		}
	}
}
